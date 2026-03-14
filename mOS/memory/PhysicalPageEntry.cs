using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.memory
{
    internal struct PhysicalPageEntry
    {

        public override string ToString()
        {
            string stat = "UNKNOWN";
            if (STATUS == PageStatus.ACTIVE) stat = "ACTIVE";
            else if (STATUS == PageStatus.INACTIVE) stat = "INACTIVE";
            else if(STATUS == PageStatus.WIRED) stat = "WIRED";
            else if (STATUS == PageStatus.FREE) stat = "FREE";

            return $"[ENTRY: {ENTRY_ADDR}] [PHYS: {PHYS_ADDR}] [STAT: {stat}]";
        }

        public int ENTRY_INDEX { get; set; }
        public int ENTRY_ADDR { get; set; }
        public int PageIndex { get; set; }
        public int PHYS_ADDR { get; set; }
        public byte STATUS { get; set; }
        public long LAST_ACCESS { get; set; }

        public PhysicalPageEntry(int entry_index, int entry_addr, int page_index, int phys_addr, byte status, long last_access)
        {
            ENTRY_INDEX = entry_index;
            ENTRY_ADDR = entry_addr;
            PageIndex = page_index;
            PHYS_ADDR = phys_addr;
            STATUS = status;
            LAST_ACCESS = last_access;
        }
    }
}
