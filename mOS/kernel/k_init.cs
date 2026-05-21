using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.IOKit;
using mOS.IOKit.Devices;
using mOS.IOKit.Devices.mOS.IOKit.Devices;
using mOS.k_heap;
using mOS.memory;

namespace mOS.kernel
{
    internal class k_init : ABIManaged
    {
        public static int start_mem_addr { get; private set; }
        public static int kernel_heap_map { get; private set; }
        public static int process_heap_map { get; private set; }
        public static int process_table_map { get; private set; }
        public static int kernel_length { get; private set; }
        public static int kernel_start { get; private set; }
        public static int usb04_addr { get; private set; }
        public static int usb03_addr { get; private set; }
        public static int usb02_addr { get; private set; }
        public static int usb01_addr { get; private set; }
        public static int kernel_loop_args_addr { get; private set; }

        public void init_memory(PhysicalPageManager ppm)
        {
            byte[] mb = new byte[4];
            int addr = 8000000;

            m_read(addr, ref mb);
            usb01_addr = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            usb02_addr = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            usb03_addr = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            usb04_addr = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            kernel_start = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            kernel_length = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            process_table_map = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            process_heap_map = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            kernel_heap_map = BitConverter.ToInt32(mb);
            addr += 4;

            m_read(addr, ref mb);
            kernel_loop_args_addr = BitConverter.ToInt32(mb);
            addr += 4;

            start_mem_addr = kernel_start + kernel_length + 1;


            ppm._INIT(start_mem_addr);

        }
         

        public void init_devices()
        {
            mos_kernel.IORegistry.Register(usb01_addr);
            mos_kernel.IORegistry.Register(usb02_addr);
            mos_kernel.IORegistry.Register(usb03_addr);
            mos_kernel.IORegistry.Register(usb04_addr);
        }

    }
}
