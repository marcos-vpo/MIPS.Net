using System.Text;
using MIPS.Abi;
using mOS.IOKit.Devices.mOS.IOKit.Devices;


namespace mOS.IOKit.Devices
{
    internal sealed class IOKeyboardService : ABIManaged, IDisposable
    {
        private readonly IOService io_service;

        private readonly Queue<IOKeyPressedEvent> queue = new();
        private readonly AutoResetEvent waitHandle = new(false);
        private readonly object sync = new();

        private byte[] kbd_data = new byte[4];

        public IOKeyboardService()
        {
            io_service = IORegistry.GetService(IODeviceClass.KEYBOARD)
                ?? throw new InvalidOperationException("Keyboard IOService not found");

            io_service.IOMessage += IOService_OnMessage;
            io_service.Open(IODeviceOperation.READ, IODeviceMode.UNUSED);
        }

        /// <summary>
        /// Blocks until a key is pressed (raw mode)
        /// </summary>
        public IOKeyPressedEvent ReadKey()
        {
            while (true)
            {

                if (last_event == IOEvent.DEVICE_DISCONNECTED) return null;
                lock (sync)
                {
                    if (queue.Count > 0)
                        return queue.Dequeue();
                }

                waitHandle.WaitOne();
            }
        }


        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                var key = ReadKey();

                if (key.KeyCode == (byte)KeyboardControlKey.Enter)
                {
                    break;
                }

                if (key.KeyCode == (byte)KeyboardControlKey.Back)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                        //  HandleBackspaceEcho();
                    }
                    continue;
                }

                if (key.KeyCode == (byte)KeyboardControlKey.ShiftKey)
                    continue;
                if (key.KeyCode == (byte)KeyboardControlKey.ControlKey)
                    continue;
                //     if (key.KeyCode == (byte)KeyboardControlKey.) continue;

                if (key.KeyCode == 0) continue;

                sb.Append((char)key.KeyCode);
            }

            return sb.ToString();
        }

        private IOEvent last_event;
        private void IOService_OnMessage(IOEvent e)
        {
            last_event = e;
            if (e == IOEvent.DATA_RECEIVED)
            {

                m_read(io_service.DeviceBufferAddr(), ref kbd_data);

                var evt = new IOKeyPressedEvent
                {
                    Ctrl = kbd_data[0] == 1,
                    Shift = kbd_data[1] == 1,
                    Alt = kbd_data[2] == 1,
                    KeyCode = kbd_data[3],
                    ControlKey = (KeyboardControlKey)(int)kbd_data[3]
                };

                lock (sync)
                {
                    queue.Enqueue(evt);
                }
            }

            if (e == IOEvent.DEVICE_DISCONNECTED)
            {
                queue.Clear();
                io_service.Close();
            }

            waitHandle.Set();
        }



        public void Dispose()
        {
            io_service.IOMessage -= IOService_OnMessage;
            io_service.Close();
        }
    }
}
