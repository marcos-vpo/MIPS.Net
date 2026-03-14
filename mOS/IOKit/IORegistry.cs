
using MIPS.Abi;
using MIPS.Net.abi;
using MIPS.Net.IO;

namespace mOS.IOKit
{
    internal sealed class IORegistry : ABIManaged
    {
        private static readonly List<IOService> services = new();

        public void Register(int deviceAddress)
        {
            int deviceAddr = deviceAddress;
            byte[] dev_header = new byte[10];
            m_read(deviceAddr, ref dev_header);

            IODeviceMemory dm = new IODeviceMemory
            {
                BaseDeviceAddr = deviceAddr,
                Action = dev_header[0],
                DeviceId = dev_header[1],
                DeviceClass = dev_header[2],
                Status = dev_header[3],
                Operation = dev_header[4],
                Mode = dev_header[5],
                BufferSize = BitConverter.ToInt32(dev_header[6..10]),
                BufferAddr = deviceAddr + 10
            };

            if (dm.BufferSize == 0) return;
            if (IODeviceClass.IsValid(dm.DeviceClass) == false) return;

            IOService service = new IOService(dm);
            services.Add(service);
        }

        public static IOService? GetService(byte deviceClass)
        {
            IOService selected = null;
            foreach (var s in services)
                if (s.IsDeviceClass(deviceClass))
                {
                    selected = s;
                    break;
                }

            return selected;
        }

        internal static IOService GetService(int device_addr)
        {
            IOService selected = null;
            foreach (var s in services)
                if (s.IsAddressOwnerOf(device_addr))
                {
                    selected = s;
                    break;
                }

            return selected;
        }
    }

}
