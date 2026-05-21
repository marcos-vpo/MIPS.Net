using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOSLib.functions
{
    internal class call_codes
    {
        public const int PROCESS_EXIT = 1;
        public const int PROCESS_STOP = 99;
        public const int MEM_PAGE_START = 4;
        public const int MEM_TOTAL_HEAP = 5;
        public const int MALLOC = 3;
        public const int FREE = 8;
        public const int PROGRAM_ADDR = 9;
        public const int READ_CHAR = 11;
        public const int READ_LINE = 12;
        public const int ADD_MMU_TLB = 510;
        public const int REM_MMU_TLB = 511;
        public const int SELF_PID = 2;
    }
}
