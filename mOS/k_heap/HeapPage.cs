using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.k_heap
{
    internal class HeapPage
    {
        public HeapPage(int emptyEntryAddr, int order, int page)
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
