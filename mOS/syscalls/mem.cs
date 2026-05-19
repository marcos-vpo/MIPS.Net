using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.kernel;
using mOS.memory;

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
            int vAddr = mos_kernel.ProcessManager.CurrentProcessAlloc(size);
            return vAddr;
        }
    }
}
