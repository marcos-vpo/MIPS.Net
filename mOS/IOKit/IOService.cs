using MIPS.Abi;
using mOS.kernel;

namespace mOS.IOKit
{
    internal class IOService : ABIManaged
    {
        private readonly IODeviceMemory device_mem;
        private byte[] clean_buffer;

        public bool IsBusy { get; private set; }

        public IOService(IODeviceMemory dm)
        {
            device_mem = dm;
            clean_buffer = new byte[dm.BufferSize];
        }

        public bool Open(byte operation, byte mode)
        {
            if (IsBusy)
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(100);
                    if (IsBusy == false) break;
                }

                if (IsBusy) return false;
            }

            IsBusy = true;
            byte[] args = new byte[2]
            {
               operation,
               mode
            };

            m_write(device_mem.BaseDeviceAddr + 4, ref args);
            device_mem.Operation = operation;
            device_mem.Mode = mode;

            return true;
        }

        /// <summary>
        /// Prapares device buffer copying data from specified address
        /// </summary>
        /// <param name="physSourceAddr">source physical address of data to transfer to device buffer</param>
        /// <param name="len">source data length</param>
        public void PrepareBuffer(int physSourceAddr, int len)
        {
            byte[] buffer = new byte[len];

            m_read(physSourceAddr, ref buffer);
            m_write(device_mem.BufferAddr, ref buffer);
        }

        public void PrepareBuffer(byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                m_write(device_mem.BufferAddr, ref clean_buffer);
                m_write(device_mem.BufferAddr, ref buffer);
            }
            canFire = true;
        }

        private bool canFire = false;
        public void Fire()
        {
            if (canFire == false) throw new Exception("Buffer is not prepared. Call PrepareBuffer() before this.");

            if (device_mem.Operation == IODeviceOperation.WRITE) device_mem.LastWrite = DateTime.Now.Ticks;
            else device_mem.LastRead = DateTime.Now.Ticks;

            byte[] fire = new byte[1] { 0x01 };

            if (device_mem.Operation == IODeviceOperation.WRITE) m_write(device_mem.BaseDeviceAddr, ref fire);
            else if (device_mem.Operation == IODeviceOperation.READ) m_read(device_mem.BaseDeviceAddr, ref fire);

            canFire = false;
        }


        public IOResult? Result()
        {
            byte[] resLengthB = new byte[4];
            m_read(device_mem.BufferAddr, ref resLengthB);

            int responseLength = BitConverter.ToInt32(resLengthB);
            if (responseLength == 0) return null;
            else
            {
                byte[] responseBuffer = new byte[responseLength];
                m_read(device_mem.BufferAddr + 4, ref responseBuffer);

                IOResult requestResult = new IOResult
                {
                    // "-4" = [bufferStart + [res_code(4b)] + [res_len(4b) ~~~ data ~~~]
                    IsComplete = responseLength == 0 || responseLength < device_mem.BufferSize - 4,
                    Data = responseBuffer
                };

                ClearBuffer();
                return requestResult;
            }
        }


        public void Close(bool bufferClear = true)
        {
            if (bufferClear)
                m_write(device_mem.BufferAddr, ref clean_buffer);
            IsBusy = false;
        }

        public void ClearBuffer()
        {
            m_write(device_mem.BufferAddr, ref clean_buffer);
        }


        internal bool IsDeviceClass(byte deviceClass)
        {
            return device_mem.DeviceClass == deviceClass;
        }

        internal bool IsAddressOwnerOf(int device_addr)
        {
            return device_mem.BaseDeviceAddr == device_addr;
        }

        public event Action<IOEvent>? IOMessage;

        internal void DataReceived() => IOMessage?.Invoke(IOEvent.DATA_RECEIVED);
        internal void DeviceConnected() => IOMessage?.Invoke(IOEvent.DEVICE_CONNECTED);
        internal void DeviceDisconnected() => IOMessage?.Invoke(IOEvent.DEVICE_DISCONNECTED);
        internal void SendDataFailed() => IOMessage?.Invoke(IOEvent.DATA_SEND_FAILED);
        internal void ReceiveDataFailed() => IOMessage?.Invoke(IOEvent.DATA_RECEIVE_FAILED);

        internal int DeviceBaseAddr() => device_mem.BaseDeviceAddr;
        internal int DeviceBufferAddr() => device_mem.BufferAddr;
        internal int BufferLength() => device_mem.BufferSize;

        internal void SetMode(byte mode)
        {
            device_mem.Mode = mode;
            byte[] mode_b = new byte[1] { mode };
            m_write(device_mem.BaseDeviceAddr + 5, ref mode_b);
        }

        internal bool IsClosed()
        {
            return IsBusy == false;
        }

    }
}