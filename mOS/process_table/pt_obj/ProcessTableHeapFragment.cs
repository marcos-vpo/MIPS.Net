using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.process_table
{
    internal class ProcessTableHeapFragment
    {

        public ProcessTableHeapFragment(int virtualHeapAddr, int length)
        {
            VirtualHeapStart = virtualHeapAddr;
            Length = length;
        }

        public override string ToString()
        {
            return $"[*{VirtualHeapStart}, {Length}b";
        }

        public int VirtualHeapStart { get; set; }
        public int Length { get; set; }
    }
}
