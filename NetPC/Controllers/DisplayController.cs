using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.DeviceLib;
using MIPS.Net.InstructionSet;

namespace NetPC.Controllers
{
    public class DisplayCmd
    {
        public const byte TEXT_PRINT_CHAR = 0x1A;
        public const byte TEXT_FLUSH = 0x1C;
        public const byte TEXT_REQUEST_RESOLUTION = 0x99;
        public const byte TEXT_SCROLL_UP = 0x1E;
        public const byte TEXT_CLEAR = 0x2F;
    }

    public interface IDisplayAdapter
    {
        void DisplayStatus(bool connected);
        void ClearScreen();
        void PrintChar(byte[] raw);
        Tuple<int, int> GetDimensions();
        void Flush();
        void ScrollUp();
    }

    internal class DisplayController
    {
        public static MDHardware PORT;
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

            PORT.OnDataReceived += PORT_OnDataReceived;
        }

        private static bool resolution_defined = false;
        private static object lck = new object();

        public static int DisplayWidth { get; set; }
        public static int DisplayHeight { get; set; }

        private static void PORT_OnDataReceived(byte[] data, int readed)
        {
            if (data[5] == 0x99) // CMD REQUEST RESOLUTION
            {
                lock (lck)
                {
                    var dim = ADAPTER.GetDimensions();
                    DisplayWidth = dim.Item1;
                    DisplayHeight = dim.Item2;

                    //   DisplayWidth = 1500;
                    //   DisplayHeight = 600;
                    resolution_defined = true;
                    byte[] resolution_x_y = new byte[9];
                    using (MemoryStream ms = new MemoryStream(resolution_x_y))
                    {
                        ms.Write([0x99]);
                        ms.Write(BitConverter.GetBytes((int)DisplayWidth));
                        ms.Write(BitConverter.GetBytes((int)DisplayHeight));
                    }

                    PORT.SendResponse(resolution_x_y);
                    /*
                    PORT.SendData(resolution_x_y, () =>
                    {


                        return 0;
                    });
                    */
                }
            }
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
            if (resolution_defined == false) return;
            if (readed == 1) return;
            byte[] header = new byte[10];
            byte[] raw = new byte[readed - 10];

            Array.Copy(data, 0, header, 0, 10);
            Array.Copy(data, 10, raw, 0, raw.Length);

            byte mode = header[5];

            //        Tuple<int, int> screenDimension = ADAPTER.GetDimensions();

            if (mode == DisplayCmd.TEXT_PRINT_CHAR) //  text mode, print char
            {
                ADAPTER.PrintChar(raw);
                PORT.SendResponse(new byte[1]);
            }
            else if (mode == DisplayCmd.TEXT_FLUSH) // text mode, flush
            {
                ADAPTER.Flush();
                PORT.SendResponse(new byte[1]);
            }
            else if(mode == DisplayCmd.TEXT_SCROLL_UP) // text mode, scroll up
            {
                ADAPTER.ScrollUp();
                PORT.SendResponse(new byte[1]);
            }
            else if(mode  == DisplayCmd.TEXT_CLEAR)
            {
                ADAPTER.ClearScreen();
                PORT.SendResponse(new byte[1]);
            }
            else if (mode == 0x2A) // graphical mode
            {

            }

        }


        internal static void TURN_OFF()
        {
            if (PORT == null) return;

            PORT.OnDataReceived -= Hw_OnDataReceived;
            PORT.OnReceiveFailed -= Hw_OnReceiveFailed;

            PORT.Disconnect();
            bufferPos = 0;
            x = 0;
            y = 0;
            resolution_defined = false;
        }
    }
}
