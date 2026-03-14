using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using mOS.IOKit.Devices.mOS.IOKit.Devices;

namespace mOS.IOKit.Devices
{
    internal class IOConsoleService : IDisposable
    {
        private readonly IOKeyboardService keyboard;
        private readonly IODisplayService display;
        private bool autoFlush;

        public IOConsoleService(bool autoFlush = true)
        {
            this.autoFlush = autoFlush;
            keyboard = new IOKeyboardService();
            display = new IODisplayService();

        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            byte keyCode = 0x0;


            while (true)
            {
                var key = ReadKey();
                if (key == null) break;
                if (key.KeyCode == (int)Keys.Enter)
                    return sb.ToString();
                else if (key.KeyCode == (int)Keys.Escape)
                    return "";
                else if (key.Ctrl || key.Alt || key.Shift) continue;
                else
                {
                    sb.Append((char)key.KeyCode);
                    display.PrintChar((char)key.KeyCode);
          //          Thread.Sleep(1);
                    display.SendFlush();
                }
            }

            return sb.ToString();
        }

        public IOKeyPressedEvent ReadKey()
        {
            var key = keyboard.ReadKey();
            if (key == null) return null;

            if (key.Ctrl || key.Alt || key.Shift) return key;

            if (key.KeyCode == (int)Keys.Enter)
            {
                display.SetCursor(0, display.CursorY + 1);
            }

            if (key.KeyCode == (int)Keys.Back)
            {
                if (display.CursorX == 0)
                {
                    display.SetCursor(display.Columns, display.CursorY - 1);
                    display.PrintChar(' ');
                    display.SetCursor(display.Columns, display.CursorY - 1);
                }
                else
                {
                    display.SetCursor(display.CursorX - 1, display.CursorY);
                    display.PrintChar(' ');
                    display.SetCursor(display.CursorX - 1, display.CursorY);
                }
            }

         //   Thread.Sleep(1);
            display.SendFlush();
            return key;
        }

        public void PrintLine(string line, mOSColor foreground = mOSColor.White,
            mOSColor background = mOSColor.Black,
            bool bright = false)
        {
            foreach (char c in line)
            {
                display.PrintChar(c, foreground, background, bright);
             //   Thread.Sleep(1);
            }

            display.PrintChar('\n');

            if (autoFlush)
                display.SendFlush();
        }

        public void Print(string text, mOSColor foreground = mOSColor.White,
            mOSColor background = mOSColor.Black,
            bool bright = false)
        {
            foreach (char c in text)
            {
                display.PrintChar(c, foreground, background, bright);
              //  Thread.Sleep(1);
            }

            if (autoFlush)
                display.SendFlush();
        }

        public void PrintChar(char c, mOSColor foreground = mOSColor.White,
            mOSColor background = mOSColor.Black,
            bool bright = false)
        {
            display.PrintChar(c, foreground, background, bright);
           // Thread.Sleep(1);
            if (autoFlush)
                display.SendFlush();
        }

        public void Dispose()
        {
            keyboard.Dispose();
            display.Dispose();

        }

        internal void Flush()
        {
            if (autoFlush) return;
            display.SendFlush();
        }

        internal void Clear()
        {
            display.Clear();
        }
    }
}
