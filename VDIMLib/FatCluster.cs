using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDIMLib
{
    public class FatCluster
    {
        public long START;


        public int ENTRY;
        public int ORDER;
        public long STOP;
        public long LEN;

        public long SIZE => (STOP - (START));

        public override string ToString()
        {
            if (ENTRY == 0) return $"(0x{START}) unused cluster";
            return $"(0x{START}) EntryId: {ENTRY}";
        }

        internal void Free()
        {
            ENTRY = 0;
            LEN = 0;
            ORDER = 0;
        }

        public FatCluster()
        {
            ENTRY = -1;
        }

        public FatCluster(long start, long stop, long dataLen, int entry, int order)
        {
            START = start;
            STOP = stop;
            LEN = dataLen;
            ENTRY = entry;
            ORDER = order;
        }
    }
}
