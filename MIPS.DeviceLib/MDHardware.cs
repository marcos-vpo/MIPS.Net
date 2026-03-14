using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MIPS.DeviceLib
{
    public class MDHardware
    {


        #region
        int devBufferSize = 0;
        //  private bool busyLockAcquired = false;
        //  bool busy = false;

        public delegate void ConnectionPing();
        public event ConnectionPing OnPing;

        public delegate void DataReceive(byte[] data, int readed);
        public event DataReceive OnDataReceived;

        public delegate void ReceiveFailed(string msg, bool connected);
        public event ReceiveFailed OnReceiveFailed;

        public delegate void DataSend();
        public event DataSend OnDataSend;

        public delegate void DataSendFail(string msg, bool connected);
        public event DataSendFail OnDataSendFailed;
        #endregion

        Socket INPUT = null;
        Socket OUTPUT = null;
        TcpListener listener = null;
        private void HandShake(int port, byte device_type)
        {
            try
            {


                List<byte> ident_handshake = new List<byte>();
                ident_handshake.Add(device_type);
                ident_handshake.AddRange(BitConverter.GetBytes(port + 1));

                byte[] devSize = new byte[4];
                OUTPUT.Send(ident_handshake.ToArray());
                OUTPUT.Receive(devSize);
                devBufferSize = BitConverter.ToInt32(devSize);

                INPUT = listener.AcceptSocket();
                allowInput = true;
            }
            catch (Exception e)
            {
                stop = true;
            }
        }

        private bool ConnectInternal(int port, byte device_type)
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                listener = new TcpListener(ip, port + 1);
                listener.Start();


                OUTPUT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                OUTPUT.Connect(IPAddress.Parse("127.0.0.1"), port);

                HandShake(port, device_type);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        Func<byte[], int> pendingInputCallback = null;
        private bool allowInput;
        byte[] buffer = null;
        public bool IsConnected { get; private set; }
        public void Connect(int port, byte device_type)
        {
            IsConnected = ConnectInternal(port, device_type);
            if (!IsConnected)
            {
                INPUT?.Dispose();
                INPUT = null;
                OUTPUT?.Dispose();
                OUTPUT = null;
                throw new Exception("Failed to connect");
            }
            buffer = new byte[10 + devBufferSize];
            int ct = 0;
            Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                while (true)
                {
                    try
                    {
                        //    INPUT.ReceiveTimeout = 1000;
                        if (stop) break;
                        if (allowInput == false) continue;

                        int readed = INPUT.Receive(buffer);
                        if (!INPUT.Connected) throw new Exception("disconnected");

                        if (readed > 0)
                        {


                            try
                            {
                                OnDataReceived?.Invoke(buffer, readed);
                            }
                            catch { }
                            try { OUTPUT.Send(new byte[1] { 0x1 }); } catch { }

                            if (pendingInputCallback != null)
                            {
                                byte[] data_raw = new byte[readed];
                                Array.Copy(buffer, 0, data_raw, 0, data_raw.Length);

                                var inpt = pendingInputCallback;
                                pendingInputCallback = null;
                                inpt(data_raw);
                            }
                            //  else SleepPrecise(0.2);


                        }

                    }
                    catch (Exception ex)
                    {
                        OnReceiveFailed?.Invoke(
                            ex.Message,
                            INPUT == null ? false : INPUT.Connected &&
                            OUTPUT == null ? false : OUTPUT.Connected);
                        break;
                    }
                }
            });
        }

        public static void SleepPrecise(double milliseconds)
        {
            if (milliseconds <= 0) return; // Não precisa esperar

            Stopwatch sw = Stopwatch.StartNew();
            long targetTicks = (long)(milliseconds / (1000.0 / Stopwatch.Frequency));

            while (sw.ElapsedTicks < targetTicks)
            {
                // Spin-wait: Não faz nada além de esperar
                // Pode adicionar Thread.Yield() aqui para ser um pouco mais gentil com outros threads,
                // mas a precisão pode diminuir.
            }
        }

        public void Disconnect()
        {
            IsConnected = false;
            try
            {
                if (listener != null)
                    listener.Stop();
            }
            catch { }
            try
            {
                if (INPUT != null) INPUT.Disconnect(false);
            }
            catch
            { }
            try
            {
                if (OUTPUT != null) OUTPUT.Disconnect(false);
            }
            catch { }
            try
            {

                listener?.Dispose();
                listener = null;
                INPUT?.Dispose();
                INPUT = null;
                OUTPUT?.Dispose();
                OUTPUT = null;
            }
            catch { }

        }

        private void WaitForInput(Func<byte[], int> predicate)
        {
            pendingInputCallback = predicate;
        }

        public void SendResponse(byte[] data)
        {
            try
            {
                byte[] buffer = new byte[devBufferSize];
                Array.Copy(data, 0, buffer, 0, data.Length);

                OUTPUT.Send(buffer);
            }
            catch (Exception ex)
            {
                OnDataSendFailed?.Invoke(ex.Message, OUTPUT.Connected);
            }
        }

        public float SEND_DATA_MS = 0;
        private bool stop;

        public string RECEIVE_DATA_RATE { get; private set; }

        public void SendData(byte[] data, Func<int>? callback = null)
        {
            Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                try
                {
                    byte[] requestInterrupt = new byte[5] { 0x2b, 0, 0, 0, 0 };
                    byte[] dataLen = BitConverter.GetBytes(data.Length);
                    Array.Copy(dataLen, 0, requestInterrupt, 1, 4);

                    byte[] result = new byte[1];
                    AutoResetEvent ar = new AutoResetEvent(false);
                    WaitForInput((resAccept) =>
                    {
                        result = resAccept;
                        ar.Set();
                        return 0;
                    });


                    int sended = 0; // request interrupt [ 0x2b + data len ] (bytes)
                    while ((sended = OUTPUT.Send(requestInterrupt)) == 0)
                        Thread.Sleep(10);



                    ar.WaitOne();

                    if (result[0] == 0x7b) // interruption accepted ?
                    {
                        while (OUTPUT.Send(data) != data.Length)
                            Thread.Sleep(10); // send effective data

                        WaitForInput((resFinish) =>  // wait for interruption finish
                        {
                            if (resFinish[0] == 0x9b)
                            {// operation successfull?
                                OnDataSend?.Invoke();
                                callback?.Invoke();
                                ar.Set();

                                sw.Stop();
                                SEND_DATA_MS = sw.ElapsedMilliseconds;
                            }
                            else
                            {
                                /*
                                if (resFinish[0] == 0x9c)
                                    while (OUTPUT.Send(data) != data.Length) Thread.Sleep(10);

                                WaitForInput((resFinish) =>
                                {
                                    OnDataSend?.Invoke();
                                    callback?.Invoke();
                                    ar.Set();

                                    sw.Stop();
                                    SEND_DATA_MS = sw.ElapsedMilliseconds;

                                    return 0;
                                });
                                */
                            }


                            return 0;
                        });
                    }

                    ar.WaitOne();
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    SEND_DATA_MS = sw.ElapsedMilliseconds;
                    OnDataSendFailed?.Invoke(ex.Message, OUTPUT.Connected);
                    //  ar.Set();
                    callback?.Invoke();
                }
            });

        }
    }
}
