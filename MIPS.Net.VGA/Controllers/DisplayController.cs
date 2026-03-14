using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.DeviceLib;

namespace MIPS.Net.VGA.Controllers
{
    public interface IDisplayAdapter
    {
        void DisplayStatus(bool connected);
        void PrintTextLine(string line, int x, int y, Color color, object font);
        void ClearScreen();
        void PrintChar(char b, int x, int y, Color white, object value);
        Tuple<int, int> GetDimensions();
    }

    internal class DisplayController
    {
        private static MDHardware PORT;
        private static IDisplayAdapter ADAPTER;
        public static void INIT(int port, IDisplayAdapter adapter)
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
        public static string ReceiveRate => PORT?.RECEIVE_DATA_RATE;

        private static void Connect(int port)
        {
            try
            {
                PORT.OnDataReceived += Hw_OnDataReceived;
                PORT.OnReceiveFailed += Hw_OnReceiveFailed;

                PORT.Connect(port, Convert.ToByte("0x3D", 16));
                ADAPTER.DisplayStatus(connected: true);

            }
            catch (Exception ex)
            {
                ADAPTER.DisplayStatus(connected: false);
            }
        }

        private static void Hw_OnReceiveFailed(string msg, bool connected)
        {
            if (!connected)
                return;

        }

        static int x = 0;
        static int y = 0;
        static int bufferPos = 0;
        private static void Hw_OnDataReceived(byte[] data, int readed)
        {
            byte[] header = new byte[10];
            byte[] raw = new byte[readed - 10];

            Array.Copy(data, 0, header, 0, 10);
            Array.Copy(data, 10, raw, 0, raw.Length);

            byte oper_code = header[6];

            Tuple<int, int> screenDimension = ADAPTER.GetDimensions();

            if (oper_code == 0x1A) //  text mode
            {
                int writed = 0;
                for (int i = bufferPos; i < raw.Length; i++)
                {
                    byte b = raw[i];
                    if ((char)b == '\r' || (char)b == '\n') { x = 0; y += 10; }
                    if ((char)b == '\0') break;
                    if (char.IsAsciiLetter((char)b) || char.IsDigit((char)b) || char.IsNumber((char)b))
                    {
                     
                        if ((x + 8) > screenDimension.Item1) { x = 0; y += 8; }

                        ADAPTER.PrintChar((char)b, x, y, Color.White, null);

                        x += 8;
                        writed += 1;
                    }
                }
                bufferPos += writed;

            }
            if (oper_code == 0x2A) // graphical mode
            {

            }
        }

        internal static void TURN_OFF()
        {
            PORT.Disconnect();
        }
    }
}
