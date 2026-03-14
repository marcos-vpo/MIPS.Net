using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    public class IOBUS
    {
        // Mapa de endereços para dispositivos
        private Dictionary<int, IOPort> devices = new Dictionary<int, IOPort>();

        // Pinos do MIPS
        public int AddressBus { get; set; }
        public byte[] DataBus { get; set; }
        public bool ReadSignal { get; set; }
        public bool WriteSignal { get; set; }
        public bool InterruptionSignal { get; set; }

        public delegate void BusSignal(Func<KeyValuePair<bool, byte[]>, int> callBack = null);
        public event BusSignal OnSignalReceived;

        public delegate void StatusChange(IOBUS bus);
        public event StatusChange OnStatusChanged;

        private static IOBUS _instance;

        public IOBUS()
        {
            _instance = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DEVICE_MAPPED(int address)
        {
            return _instance.devices.TryGetValue(address, out _);
        }

        public static void SEND(char operation, int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            lock (lck)
            {
                _instance.SendBus(operation, address, data, callBack);
            }
        }

        // Método para escrever dados no barramento
        public void SendBus(char operation, int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            // Ativa o sinal de escrita
            if (operation == 'W') { WriteSignal = true; ReadSignal = false; }
            else if (operation == 'R') { WriteSignal = false; ReadSignal = true; }
            else if (operation == 'I')
            {
                DataBus = data;
                AddressBus = address;
                ReadSignal = false;
                WriteSignal = false;
                InterruptionSignal = true;
                OnStatusChanged?.Invoke(this);
                OnSignalReceived?.Invoke((res) =>
                {
                    callBack(res);
                    if (res.Key == true)
                        Clear();
                    return 0;
                });
                return;
            }
            else throw new Exception($"Unknown bus operation '{operation}'");

            // Define o endereço no barramento
            AddressBus = address;
            // Define os dados no barramento
            if (data != null)
                DataBus = data;

            OnStatusChanged?.Invoke(this);

            if (operation == 'W')
                HandleWriteRequest(address, data, callBack);
            if (operation == 'R')
                HandleReadRequest(address, callBack);
            if (operation == 'I')
                OnSignalReceived?.Invoke(callBack);
        }

        private void HandleReadRequest(int address, Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            IOPort? dev = devices.FirstOrDefault(d => address == d.Key).Value;

            if (dev == null)
            {
                OnStatusChanged?.Invoke(this);

                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[1]));
                //     Clear();
            }
            else
            {
                // obter toda a memoria do dispositivo
                byte[] deviceMemory = new byte[dev.DataBufferLen + 10];
                int deviceMemAddr = dev.GetMemAddress();

                DMA.RequestData(deviceMemAddr, ref deviceMemory);

                DataBus = deviceMemory;
                OnStatusChanged?.Invoke(this);
                // Solicita os dados no dispositivo e aguarda resposta no dev_res (callback)
                devices[address].ReadRequest(deviceMemory, (dev_res) =>
                {
                    DataBus = dev_res.Value;
                    OnStatusChanged?.Invoke(this);
                    Clear();
                    callBack(dev_res);
                    return 0;
                });
            }
        }

        private void Clear()
        {
            DataBus = new byte[0];
            InterruptionSignal = false;
            WriteSignal = false;
            ReadSignal = false;
            AddressBus = 0;
            OnStatusChanged?.Invoke(this);
        }

        private void HandleWriteRequest(int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            IOPort? dev = devices.FirstOrDefault(d => address == d.Key).Value;

            if (dev == null)
            {

                DMA.StoreData(address, ref data);

                if (callBack != null)
                    callBack(new KeyValuePair<bool, byte[]>(true, new byte[0]));
                //  Clear();
            }
            else
            {
                // obter toda a memorida do dispositivo
                byte[] deviceMemory = new byte[dev.DataBufferLen + 10];
                int deviceMemAddr = dev.GetMemAddress();

                DMA.RequestData(deviceMemAddr, ref deviceMemory);


                // Escreve os dados no dispositivo e aguarda resposta no dev_res (callback)
                devices[address].WriteRequest(data, deviceMemory, (dev_res) =>
                {
                    DataBus = dev_res.Value;
                    OnStatusChanged?.Invoke(this);
                    //       Clear();

                    callBack(dev_res);

                    return 0;
                });

            }
        }


        public static short[] AddressPins()
        {
            byte[] addrb = BitConverter.GetBytes(_instance.AddressBus);
            short[] shortArray = new short[32];

            for (int i = 0; i < addrb.Length; i++)
                for (int bit = 0; bit < 8; bit++)
                    shortArray[i * 8 + bit] = (short)((addrb[i] >> bit) & 1);

            return shortArray;
        }

        public static short[] DataPins()
        {
            short[] shortArray = new short[32];
            try
            {

                if (_instance.DataBus == null) return shortArray;

                bool first10zero = true;

                int len = _instance.DataBus.Length <= 10 ? _instance.DataBus.Length : 10;
                for (int i = 0; i < len; i++)
                {
                    if (_instance.DataBus[i] != 0)
                    {
                        first10zero = false;
                        break;
                    }
                }

                byte[] dt = new byte[4];
                byte[] bus = first10zero
                   ? BitConverter.GetBytes(_instance.AddressBus)
                   : _instance.DataBus;

                if (bus.Length >= 4)
                    Array.Copy(bus, (bus.Length > 10 ? 10 : 0), dt, 0, 4);
                else if (bus.Length < 4)
                    Array.Copy(bus, 0, dt, 0, bus.Length);

                for (int i = 0; i < dt.Length; i++)
                    for (int bit = 0; bit < 8; bit++)
                        shortArray[i * 8 + bit] = (short)((dt[i] >> bit) & 1);

                return shortArray;
            }
            catch
            {
                return shortArray;
            }
        }

        internal void MapPort(IOPort io_port, int deviceAddr)
        {
            devices.Add(deviceAddr, io_port);
        }


        internal void Reset()
        {
            DataBus = new byte[0];
            AddressBus = 0;
            ReadSignal = false;
            WriteSignal = false;
            InterruptionSignal = false;
            devices.Clear();
        }



        private static object lck = new object();
    }
}
