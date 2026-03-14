using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.process
{
    internal class ProcessStatus
    {
        public const byte RUNING = 0x5C;
        public const byte IO_BLOCKED = 0x5F;
        public const byte PAUSED = 0x6D;
        public const byte CLOSED = 0x7E;
    }
}
