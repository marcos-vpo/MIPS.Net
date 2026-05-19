using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.process_heap.ph_obj
{
    internal struct AllocResult
    {
        public int BaseVirtualAddr { get; private set; }
        public int[] PhysicalPages { get; private set; }

        public AllocResult(int baseVirtualAddr, int[] physicalPages)
        {
            BaseVirtualAddr = baseVirtualAddr;
            PhysicalPages = physicalPages;
        }
    }
}
