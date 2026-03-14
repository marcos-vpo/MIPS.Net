using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.process_heap.ph_obj
{
    internal class ProcessHeapPage
    {
        public ProcessHeapPage(int emptyEntryAddr,
            int pId,
            int order, 
            int page,
            int usage)
        {
            EntryAddr = emptyEntryAddr;
          
            ProcessId = pId;    
            Order = order;
            PageIndex = page;
            Usage = usage;
        }

        public int EntryAddr { get; private set; }

        public int ProcessId { get; private set; }
        public int Order { get; private set; }
        public int PageIndex { get; private set; }
        public int Usage { get; private set; }
    }
}
