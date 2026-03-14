using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MemoryBUS
    {

        // Pinos do MIPS
        public int AddressBus { get; set; }
        public byte[] DataBus { get; set; }
        public bool ReadSignal { get; set; }
        public bool WriteSignal { get; set; }
        public bool InterruptionSignal { get; set; }

        public delegate void BusSignal(Func<KeyValuePair<bool, byte[]>, int> callBack = null);
        public event BusSignal OnSignalReceived;


        public delegate void StatusChange(MemoryBUS bus);
        public event StatusChange OnStatusChanged;



        private static MemoryBUS _instance;

        public MemoryBUS()
        {
            _instance = this;
        }


        internal void Reset()
        {
            DataBus = new byte[0];
            AddressBus = 0;
            ReadSignal = false;
            WriteSignal = false;
            InterruptionSignal = false;
        }

        public static void SEND(char operation, int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            _instance.SendBus(operation, address, data, callBack);
        }

        public static void State(int address, byte[] data)
        {
            if (notifications == false) return;
            _instance.AddressBus = address;
            _instance.DataBus = data;
            _instance.OnSignalReceived?.Invoke();
        }

        // Método para escrever dados no barramento
        public void SendBus(char operation, int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            // Ativa o sinal de escrita
            if (operation == 'W') { WriteSignal = true; ReadSignal = false; }
            else if (operation == 'R') { WriteSignal = false; ReadSignal = true; }
            else if (operation == 'I')
            {
                InterruptionSignal = true;
                AddressBus = address;

                if (notifications) OnStatusChanged?.Invoke(this);

                OnSignalReceived?.Invoke((res) =>
                {

                    InterruptionSignal = false;
                    AddressBus = 0;
                    if (notifications) OnStatusChanged?.Invoke(this);

                    if (callBack != null)
                        return callBack(res);
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

            if (notifications)
                OnStatusChanged?.Invoke(this);

            if (operation == 'W')
                HandleWriteRequest(address, data, callBack);
            if (operation == 'R')
                HandleReadRequest(address, callBack);
        }

        private void HandleReadRequest(int address, Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            if (IOBUS.DEVICE_MAPPED(address))
                IOBUS.SEND('R', address, new byte[0], (dev_res) =>
                {
                    // alternar para escrita
                    WriteSignal = true;
                    ReadSignal = false;
                    DataBus = dev_res.Value; // dados do dispositivo
                    AddressBus += 10; // endereço do free-buffer



                    callBack(new KeyValuePair<bool, byte[]>(true, dev_res.Value));
                    return 0;
                });
            else
                OnSignalReceived?.Invoke((res) =>
                {
                    DataBus = res.Value;
                    OnStatusChanged?.Invoke(this);

                    WriteSignal = false;
                    ReadSignal = false;
                    InterruptionSignal = false;
                    AddressBus = 0;
                    DataBus = new byte[0];

                    if (callBack != null) callBack(res);
                    return 0;
                });
        }

        private void HandleWriteRequest(int address, byte[] data, Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            // dispara um evento a ser capturado pela 
            // MotherBoard; ela ira acionar a memoria
            OnSignalReceived?.Invoke((res) =>
            {
                DataBus = res.Value;
                OnStatusChanged?.Invoke(this);

                WriteSignal = false;
                ReadSignal = false;
                InterruptionSignal = false;
                AddressBus = 0;
                DataBus = new byte[0];

                if (IOBUS.DEVICE_MAPPED(address))
                    IOBUS.SEND('W', address, data, (dev_res) =>
                    {
                        if (callBack != null) callBack(dev_res);
                        return 0;
                    });
                else
                    if (callBack != null) callBack(res);

                return 0;
            });
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

                byte[] dt = new byte[4];
                byte[] bus = _instance.DataBus[0] == 0
                   ? BitConverter.GetBytes(_instance.AddressBus)
                   : _instance.DataBus;

                if (bus.Length >= 4)
                    Array.Copy(bus, 0, dt, 0, 4);
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

        private static bool notifications = true;
        internal static void SetNotifications(bool enabled)
        {
            notifications = enabled;
        }
    }
}
