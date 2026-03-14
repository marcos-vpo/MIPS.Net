 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.memory;

namespace mOS.syscalls
{
    internal class mem
    {

        [Extern]
        public int page_start_addr()
        {
            int pagesStartAddr = PhysicalPageManager.PAGES_START_ADDR;
            return pagesStartAddr;
        }
    }
}
