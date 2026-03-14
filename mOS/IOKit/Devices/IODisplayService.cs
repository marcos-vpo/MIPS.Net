using System;
using System.Threading;
using MIPS.Abi;
using mOS.IOKit.Devices.mOS.IOKit.Devices;

namespace mOS.IOKit.Devices
{
    internal class IODisplayService : ABIManaged, IDisposable
    {
        private readonly IOService io_service;

        private readonly AutoResetEvent waitHandle = new(false);
        private readonly object sync = new();

        private static int resolution_x;
        private static int resolution_y;

        public int CursorX { get; private set; }
        public int CursorY { get; private set; }

        public int Columns => resolution_x;
        public int Rows => resolution_y;

        public int BytesPerCell => 2;

        public int RequiredBufferSize =>
            resolution_x * resolution_y * BytesPerCell;


        public IODisplayService()
        {
            io_service = IORegistry.GetService(IODeviceClass.DISPLAY)
                ?? throw new InvalidOperationException("Display IOService not found");

            if (resolution_x == 0)
            {
                io_service.IOMessage += IOService_OnMessage;
                io_service.Open(IODeviceOperation.READ, IODisplayCommand.TEXT_REQUEST_RESOLUTION);

                // Request resolution
                io_service.PrepareBuffer(new byte[1] { 0x99 });
                io_service.Fire();

                waitHandle.WaitOne();

                io_service.ClearBuffer();

                io_service.Close();
                io_service.Open(IODeviceOperation.WRITE, IODisplayCommand.TEXT_PRINT_CHAR);
            }
            else
            {
                io_service.Open(IODeviceOperation.WRITE, IODisplayCommand.TEXT_PRINT_CHAR);
            }

            CursorX = 0;
            CursorY = 0;

            if (RequiredBufferSize > io_service.BufferLength())
            {
                throw new InvalidOperationException($"Required buffer ({RequiredBufferSize} bytes) is more than memory-buffer capacity allocated for device class '{IODeviceClass.DISPLAY}'; Allocated buffer-length for device is: {io_service.BufferLength()} bytes");
            }
        }

        public void SetCursor(int x, int y)
        {
            lock (sync)
            {
                CursorX = x;// Math.Clamp(x, 0, resolution_x - 1);
                CursorY = y;//Math.Clamp(y, 0, resolution_y - 1);

                EnsureCursorVisible(0);
            }
        }

        public void PrintChar(char c, mOSColor foreground = mOSColor.White,
            mOSColor background = mOSColor.Black,
            bool bright = false)
        {
            byte attr = ComposeAttr(foreground, background, bright);
            InternalPrintChar(c, attr);
          
        }

        private void InternalPrintChar(char c, byte attr)
        {
            lock (sync)
            {
                switch (c)
                {
                    case '\n':
                        CursorX = 0;
                        CursorY++;
                        EnsureCursorVisible(attr);
                        return;

                    case '\r':
                        CursorX = 0;
                        return;

                    case '\b':
                        HandleBackspace(attr);
                        return;
                }

                // ⬇️ garante que a escrita nunca ocorre fora da tela
                EnsureCursorVisible(attr);

                WriteCell(CursorX, CursorY, c, attr);
                CursorX++;

                // wrapping horizontal pós-escrita
                if (CursorX >= Columns)
                {
                    CursorX = 0;
                    CursorY++;
                    EnsureCursorVisible(attr);
                }
            }
        }

        private void HandleBackspace(byte attr)
        {
            if (CursorX > 0)
            {
                CursorX--;
            }
            else if (CursorY > 0)
            {
                CursorY--;
                CursorX = Columns - 1;
            }
            else
            {
                return;
            }

            WriteCell(CursorX, CursorY, ' ', attr);
        }


        private void EnsureCursorVisible(byte clearAttr)
        {
            if (CursorY < Rows)
                return;

            ScrollUp(clearAttr);
        }


        private void ScrollUp(byte clearAttr)
        {

            io_service.SetMode(IODisplayCommand.TEXT_SCROLL_UP);
            io_service.PrepareBuffer(new byte[0]);
            io_service.Fire();
            io_service.SetMode(IODisplayCommand.TEXT_PRINT_CHAR);
            CursorY = resolution_y - 1;
            CursorX = 0;

            return;
            int textBufferBase = io_service.DeviceBufferAddr() + 8;
            int lineSize = resolution_x * BytesPerCell;
            int totalLines = resolution_y;

            // Move linhas 1..N-1 para 0..N-2
            byte[] temp = new byte[lineSize];

            for (int y = 1; y < totalLines; y++)
            {
                int src = textBufferBase + (y * lineSize);
                int dst = textBufferBase + ((y - 1) * lineSize);

                m_read(src, ref temp);
                m_write(dst, ref temp);
            }

            // Limpa última linha
            int lastLineAddr = textBufferBase + ((totalLines - 1) * lineSize);
            byte[] clearLine = new byte[lineSize];

            for (int i = 0; i < resolution_x; i++)
            {
                clearLine[i * 2] = (byte)' ';
                clearLine[i * 2 + 1] = clearAttr;
            }

            m_write(lastLineAddr, ref clearLine);

            CursorY = resolution_y - 1;
            CursorX = 0;
        }


        private void WriteCell(int x, int y, char c, byte attr)
        {
            int textBufferBase = io_service.DeviceBufferAddr() + 8;
            int cellIndex = y * resolution_x + x;
            int addr = textBufferBase + (cellIndex * BytesPerCell);

            byte[] data = new byte[2] { (byte)c, attr };

            m_write(addr, ref data);

            byte[] xyb = new byte[8];
            Array.Copy(BitConverter.GetBytes(CursorX), 0, xyb, 0, 4);
            Array.Copy(BitConverter.GetBytes(CursorY), 0, xyb, 4, 4);

            m_write(io_service.DeviceBufferAddr(), ref xyb);

            io_service.PrepareBuffer(Array.Empty<byte>());
            io_service.Fire();
        }

     

        private void IOService_OnMessage(IOEvent e)
        {
            if (e == IOEvent.DATA_RECEIVED)
            {
                byte[] data_type = new byte[1];
                m_read(io_service.DeviceBufferAddr(), ref data_type);

                if (data_type[0] == 0x99)
                {
                    byte[] resolution = new byte[8];
                    m_read(io_service.DeviceBufferAddr() + 1, ref resolution);

                    resolution_x = BitConverter.ToInt32(resolution, 0);
                    resolution_y = BitConverter.ToInt32(resolution, 4);
                }
            }


            if (e == IOEvent.DEVICE_DISCONNECTED)
            {
                resolution_x = 0;
                resolution_y = 0;
                CursorX = 0;
                CursorY = 0;
                io_service.Close();
            }

            waitHandle.Set();
        }

        public void Dispose()
        {
            io_service.IOMessage -= IOService_OnMessage;
            io_service.Close(bufferClear: false);
        }

        private static byte ComposeAttr(
            mOSColor foreground,
            mOSColor background,
            bool bright = false)
        {
            byte fg = (byte)((byte)foreground & 0x0F);
            byte bg = (byte)(((byte)background & 0x07) << 4);
            byte br = bright ? (byte)0x80 : (byte)0x00;

            return (byte)(fg | bg | br);
        }

        internal void SendFlush()
        {
            io_service.SetMode(IODisplayCommand.TEXT_FLUSH);
            io_service.PrepareBuffer(new byte[0]);
            io_service.Fire();
            io_service.SetMode(IODisplayCommand.TEXT_PRINT_CHAR);
        }

        internal void Clear()
        {
            CursorX = 0;
            CursorY = 0;
            io_service.SetMode(IODisplayCommand.TEXT_CLEAR);
            io_service.PrepareBuffer(new byte[0]);
            io_service.Fire();
            io_service.SetMode(IODisplayCommand.TEXT_PRINT_CHAR);
            io_service.ClearBuffer();
        }
    }
}
