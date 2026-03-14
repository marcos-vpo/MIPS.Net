using MIPS.Abi;
using mOS.IOKit;
using mOS.k_heap;
using mOS.memory;
using mOS.process;


namespace mOS.kernel
{
    public class mos_kernel : ABIManaged
    {
        private PhysicalPageManager phys_page_manager;
        private PageAllocator page_allocator;
        private KernelHeapManager kernel_heap_manager;
        private IORegistry io_registry;
        private ProccessManager process_manager;


        internal static PhysicalPageManager PhysicalPageManager => instance.phys_page_manager;
        internal static KernelHeapManager KernelHeap => instance.kernel_heap_manager;
        internal static IORegistry IORegistry => instance.io_registry;
        internal static ProccessManager ProcessManager => instance.process_manager;

        public static long Uptime { get; internal set; }

        private static mos_kernel instance;


        [Extern]
        public int start()
        {
            instance = this;

            phys_page_manager = new PhysicalPageManager();
            page_allocator = new PageAllocator(phys_page_manager);
            io_registry = new IORegistry();

            k_init k_init = new k_init();
            k_init.init_memory(phys_page_manager);
            k_init.init_devices();
            k_init.init_interrupts();


            kernel_heap_manager = new KernelHeapManager(k_init.kernel_heap_map, page_allocator);
            process_manager = new ProccessManager(k_init.process_table_map, k_init.process_heap_map, page_allocator);

          //  MkShell minimal_kernel_shell = new MkShell();
          //  minimal_kernel_shell.Start();

            return 0; // nunca deveria retornar, mas mantém ABI feliz
        }

        internal static void write_loop_args(ref byte[] b)
        {
            instance.m_write(k_init.kernel_loop_args_addr, ref b);
        }
    }
}

