using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.kernel;
using mOS.memory;
using mOS.process;
using mOS.process_heap.ph_obj;

namespace mOS.syscalls
{
    public class mem
    {

        [Extern]
        public int page_start_addr()
        {
            int pagesStartAddr = mos_kernel.ProcessManager.CurrentProcessVirtualStart();
            return pagesStartAddr;
        }

        [Extern]
        public int process_alloc(int a2)
        {
            int size = a2;
            AllocResult vAddr = mos_kernel.ProcessManager.CurrentProcessAlloc(size);
            return vAddr.BaseVirtualAddr;
        }

        [Extern]
        public int process_free(int a2, int a3)
        {
            int physProgram = a2;
            int physPage = a3;

            ProccessManager pm = mos_kernel.ProcessManager;

            pm.CurrentProcessFree(physProgram, physPage);

            return 1;
        }
    }
}
