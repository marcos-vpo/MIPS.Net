using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MIPS.Net.IO
{
    public class USBPort : IOPort
    {
        public USBPort()
        {
            Class = 0x20;
        }
        public byte ID { get; set; }
        public byte Class { get; set; }
        public int DevicePort { get; set; }
        public int INTR_DATA_RECEIVED { get; set; }
        public int INTR_DEV_CONNECT { get; set; }
        public int INTR_DEV_DISCONNECT { get; set; }
        public int INTR_SEND_FAIL { get; set; }
        public int INTR_REC_FAIL { get; set; }

        public bool Connected => OUTPUT != null && INPUT != null;

        private int _buffer_len;
        public int DataBufferLen
        {
            get
            {
                return _buffer_len;
            }
            set
            {
                _buffer_len = value;
                _port_buffer = new byte[value + 10];
            }
        }
        private int _address;

        private byte[] _port_buffer;
        private IOBUS _io_bus = null;

        public int GetMemAddress()
        {
            return _address;
        }


        bool isOn = false;

        public delegate void ConnectionChange(bool connected);
        public event ConnectionChange ConnectionChanged;

        public delegate void DeviceTypeDetection(string dev_type);
        public event DeviceTypeDetection DeviceTypeDetected;

        public delegate void OnOffStateChange(bool is_on);
        public event OnOffStateChange OnOffChanged;

        TcpListener port = null;
        private Socket INPUT = null;
        private Socket OUTPUT = null;



        public void TurnOn()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            port = new TcpListener(ip, DevicePort);
            port.Start();

            isOn = true;
            bool handshaked = false;
            AutoResetEvent ar = new AutoResetEvent(false);

            if (INPUT == null)
            {
                OnOffChanged.Invoke(true);
                Task.Run(() =>
                {
                    int resHs = HandShakeConnect();
                    if (resHs > 0)
                    {
                        byte[] statusOn = new byte[1] { 0x01 };
                        DMA.StoreData(_address + 3, ref statusOn);
                    }
                    handshaked = resHs > 0;

                });

            }
            Task.Run(() =>
            {

                ar.Set();
                while (isOn)
                {
                    if (!handshaked)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    try
                    {
                        if (INPUT == null) break;
                        //  else
                        {
                            //    if (INPUT.Available == 0) continue;
                            int rec = INPUT.Receive(_port_buffer);
                            if (rec != 5)
                            {

                            }
                            if (rec == 5 && _port_buffer[0] == 0x2b) // interrupt request
                            {
                                int sended = 0;
                                while ((sended = OUTPUT.Send(new byte[1] { 0x7b })) == 0) // send interrupt accept
                                    Thread.Sleep(10);

                                int data_len = BitConverter.ToInt32(_port_buffer[1..5]);
                                byte[] data_raw = new byte[data_len];

                                INPUT.ReceiveBufferSize = data_len;
                                //    Thread.Sleep(100);

                                rec = INPUT.Receive(data_raw, 0, data_len, SocketFlags.None);

                                while (rec != data_len)
                                {
                                    rec = INPUT.Receive(data_raw, 0, data_len, SocketFlags.None);
                                    //         while ((sended = OUTPUT.Send(new byte[1] { 0x9c })) == 0)
                                    //          Thread.Sleep(10);
                                    //      rec = INPUT.Receive(data_raw, 0, data_len, SocketFlags.None);
                                }



                                IOBUS.SEND('W', _address + 10, data_raw, (r) =>
                                {
                                    SendDataReceived((a) =>
                                    {

                                        try
                                        {
                                            OUTPUT.Send(new byte[1] { 0x9b }); // send finish
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        return 0;
                                    });
                                    return 0;
                                });
                            }
                            else if (rec > 0 && pendingInputCallback != null)
                            {
                                byte[] rec_raw = new byte[rec];
                                Array.Copy(_port_buffer, 0, rec_raw, 0, rec_raw.Length);
                                pendingInputCallback(rec_raw);
                                pendingInputCallback = null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandlerInputError();
                    }
                }


                if (port != null)
                {
                    port.Stop();
                    port.Dispose();
                    port = null;
                }
                if (INPUT != null)
                {
                    try { INPUT.Close(); } catch { }
                    try { INPUT.Dispose(); } catch { }
                    try { INPUT = null; } catch { }
                    try { OUTPUT.Close(); } catch { }
                    try { OUTPUT.Dispose(); } catch { }
                    try { OUTPUT = null; } catch { }
                }

            });

            ar.WaitOne();
        }

        private void HandlerInputError()
        {
            if (INPUT?.Connected == false)
            {
                byte[] statusOff = new byte[1] { 0x00 };
                DMA.StoreData(_address + 3, ref statusOff);

                INPUT?.Dispose();
                INPUT = null;

                OUTPUT?.Dispose();
                OUTPUT = null;

                ConnectionChanged?.Invoke(false);

                // falha de recebimento de dados
                // envia um sinal de interrupção
                // par que seja acionada a rotina de tratamento
                IOBUS.SEND(
                    operation: 'I', // request interruption
                    address: _address,
                    data: BitConverter.GetBytes(INTR_DEV_DISCONNECT),
                    callBack: (res) =>
                    {
                        return 0;
                    });
            }
        }

        private int HandShakeConnect()
        {
            int bytesReceived;
            _port_buffer = new byte[DataBufferLen + 10];
            //      var newClient = port.AcceptSocket();

            try
            {
                INPUT = port.AcceptSocket();
            }
            catch
            {
                return 0;
            }

            ConnectionChanged?.Invoke(true);

            bytesReceived = INPUT.Receive(_port_buffer);
            if (_port_buffer[0] == 0x2D) DeviceTypeDetected?.Invoke("storage");
            if (_port_buffer[0] == 0x3D) DeviceTypeDetected?.Invoke("display");
            if (_port_buffer[0] == 0x4D) DeviceTypeDetected?.Invoke("printer");
            if (_port_buffer[0] == 0x5D) DeviceTypeDetected?.Invoke("debugger");
            if (_port_buffer[0] == 0x6D) DeviceTypeDetected?.Invoke("network");
            if (_port_buffer[0] == 0x7D) DeviceTypeDetected?.Invoke("keyboard");
            if (_port_buffer[0] == 0x8D) DeviceTypeDetected?.Invoke("mouse");

            byte[] portByte = _port_buffer[1..5];
            int outPort = BitConverter.ToInt32(portByte);

            OUTPUT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            OUTPUT.Connect(IPAddress.Parse("127.0.0.1"), outPort);
            INPUT.Send(BitConverter.GetBytes(DataBufferLen));

            _io_bus.SendBus(
             operation: 'I',
             address: _address,
             data: BitConverter.GetBytes(INTR_DEV_CONNECT),
             callBack: (res) =>
             {
                 return 0;
             });
            return bytesReceived;
        }
        public void TurnOff()
        {
            OnOffChanged.Invoke(false);
            if (INPUT != null)
            {
                try { INPUT?.Close(); } catch { }
                try { INPUT?.Dispose(); } catch { }
                try { INPUT = null; } catch { }
            }
            if (OUTPUT != null)
            {
                try { OUTPUT?.Close(); } catch { }
                try { OUTPUT?.Dispose(); } catch { }
                try { OUTPUT = null; } catch { }
            }
            if (port != null)
            {
                port.Stop();
                port.Dispose();
                port = null;
            }
            isOn = false;
        }

        Func<byte[], int> pendingInputCallback = null;

        public void ReadRequest(byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            try
            {
                if (INPUT == null || OUTPUT == null)
                {
                    callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
                    return;
                }

                //          ConnectionChanged?.Invoke(true);
                var mem = deviceMemory;


                AutoResetEvent ar = new AutoResetEvent(false);
                bool cbCalled = false;
                pendingInputCallback = (byte[] input) =>
                {   
                    cbCalled = true;
                    var dt = new byte[DataBufferLen];
                    Array.Copy(input, 0, dt, 0, dt.Length);

                    DMA.StoreData(_address + 10, ref dt);
                    callBack(new KeyValuePair<bool, byte[]>(true, dt));
                    ar.Set();
                    SendDataReceived();

                
                    return 0;
                };

                OUTPUT.Send(mem);

                ar.WaitOne();
                if (!cbCalled) 
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));

            }
            catch (SocketException ex)
            {
                // falha de recebimento de dados
                // envia um sinal de interrupção
                // par que seja acionada a rotina de tratamento
                SendReceiveFail();
                SendDisconnect();

                callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
            }
        }

        public void WriteRequest(byte[] data, byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            //   Task.Run(() =>
            //  {
            try
            {
                if (OUTPUT == null)
                {
                    Thread.Sleep(100);
                    callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));

                    return;
                }

                var mem = deviceMemory;

                AutoResetEvent ar = new AutoResetEvent(false);
                bool cbCalled = false;
                pendingInputCallback = (byte[] input) =>
                {
                    //    ConnectionChanged?.Invoke(true);
                    cbCalled = true;
                    ar.Set();
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
   
                  
                    return 0;
                };

                OUTPUT.Send(mem);

                ar.WaitOne();
                if(!cbCalled) 
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
            }
            catch (SocketException ex)
            {
                // falha de recebimento de dados
                // envia um sinal de interrupção
                // par que seja acionada a rotina de tratamento
                SendReceiveFail();
                SendDisconnect();

                callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
            }

            //  });

        }

        public object Tag { get; set; }

        private InterruptionEntry[] _interruptions;

        public void BusConnected(int addr, IOBUS bus, InterruptionEntry[] port_interruptions)
        {
            this._address = addr;
            this._io_bus = bus;
            _interruptions = port_interruptions;

            byte[] device_header = new byte[10];
            DMA.RequestData(addr, ref device_header);
            DataBufferLen = BitConverter.ToInt32(device_header, 6);
            if (DataBufferLen == 0)
            {
                MIPS_CPU.Instance.RequestHalt();
                ConnectionChanged?.Invoke(false);
                return;
            }

            INTR_DATA_RECEIVED = port_interruptions[0].Code;
            INTR_DEV_CONNECT = port_interruptions[1].Code;
            INTR_DEV_DISCONNECT = port_interruptions[2].Code;
            INTR_SEND_FAIL = port_interruptions[3].Code;
            INTR_REC_FAIL = port_interruptions[4].Code;
            ConnectionChanged?.Invoke(false);
        }

        private void SendDataReceived(Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            IOBUS.SEND(
                operation: 'I', // request interruption
                address: _address,
                data: BitConverter.GetBytes(INTR_DATA_RECEIVED),
                callBack: (a) =>
                {
                    if (callBack != null)
                        callBack(a);
                    return 0;
                });
        }

        private void SendDisconnect()
        {
            IOBUS.SEND(
                            operation: 'I', // request interruption
                            address: _address,
                            data: BitConverter.GetBytes(INTR_DEV_DISCONNECT),
                            callBack: (res) =>
                            {
                                return 0;
                            });
        }

        private void SendReceiveFail()
        {
            IOBUS.SEND(
                operation: 'I', // request interruption
                address: _address,
                data: BitConverter.GetBytes(INTR_REC_FAIL),
                callBack: (res) =>
                {
                    return 0;
                });
            ConnectionChanged?.Invoke(false);
        }

        public void ForceHalt()
        {
            SendDisconnect();
            isOn = false;
        }
    }
}
