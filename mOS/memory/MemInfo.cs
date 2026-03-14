using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOS.misc;

namespace mOS.memory
{
    internal class MemInfo
    {
        public int TotalMemory { get; private set; }

        public int Active { get; private set; }
        public int Inactive { get; private set; }   
        public int Wired { get; private set; }
        public int Free { get; private set; }

        public int TotalUsed => Active + Inactive;

        public void Update(int totalMemory, int active, int inactive, int free, int wired)
        {
            TotalMemory = totalMemory;
            Active = active;
            Inactive = inactive;
            Free = free;
            Wired = wired;
        }

        public override string ToString()
        {
            return $"[TOTAL: {TotalMemory.FormatSize()}] [USED: {TotalUsed.FormatSize()}] [ACTIVE: {Active.FormatSize()}] [INACTIVE: {Inactive.FormatSize()}] [WIRED: {Wired.FormatSize()}] [FREE: {Free.FormatSize()}]";
        }
    }
}
