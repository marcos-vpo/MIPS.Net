using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.process_table
{
    internal class ProcessTableHeapPage
    {
        public ProcessTableHeapPage(int emptyEntryAddr, int order, int page)
        {
            EntryAddr = emptyEntryAddr;
            Order = order;
            PageIndex = page;
        }

        public int EntryAddr { get; private set; }


        public int Order { get; private set; }
        public int PageIndex { get; private set; }  
    }
}
