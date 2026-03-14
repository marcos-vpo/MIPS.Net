using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.DeviceLib;

namespace NetPC.Controllers
{
    internal interface IKeyboardAdapter
    {
        void KeyboardStatus(bool connected);
    }
    internal class KeyBoardController
    {
        private static MDHardware PORT;
        private static IKeyboardAdapter ADAPTER;

        public static string PingSpeed { get; internal set; }

        internal static bool IsConnected()
        {
            if (PORT == null) return false;
            return PORT.IsConnected;
        }

        public static void INIT(int port, IKeyboardAdapter adapter)
        {
            ADAPTER = adapter;
            if (PORT == null) PORT = new MDHardware();
            else
            {
                PORT.Disconnect();
                PORT = new MDHardware();
            }

            Connect(port);
        }


        public static void SendKey(bool control, bool shift, bool alt, int keyCode, Func<int> callback)
        {
            byte bkey = Convert.ToByte(keyCode);

            byte[] keyboardSignal = new byte[8];
            keyboardSignal[0] = Convert.ToByte(control);
            keyboardSignal[1] = Convert.ToByte(shift);
            keyboardSignal[2] = Convert.ToByte(alt);
            keyboardSignal[3] = bkey;

            PORT.SendData(keyboardSignal, () =>
            {
                PingSpeed = $"~{PORT.SEND_DATA_MS:N1}ms";
                callback();
                return 0;
            });
        }

        private static void Connect(int port)
        {
            try
            {
                PORT.OnDataReceived += Hw_OnDataReceived;
                PORT.OnReceiveFailed += Hw_OnReceiveFailed;

                PORT.Connect(port, Convert.ToByte("0x7D", 16));
                ADAPTER.KeyboardStatus(connected: true);

            }
            catch (Exception ex)
            {
                ADAPTER.KeyboardStatus(connected: false);
            }
        }

        private static void Hw_OnReceiveFailed(string msg, bool connected)
        {
            if (!connected)
                return;
        }

        private static void Hw_OnDataReceived(byte[] data, int readed)
        {

        }

        internal static void TURN_OFF()
        {
            if (PORT != null)
            {
                PORT.OnDataReceived -= Hw_OnDataReceived;
                PORT.OnReceiveFailed -= Hw_OnReceiveFailed;
                PORT.Disconnect();
            }
        }

        
    }
}
