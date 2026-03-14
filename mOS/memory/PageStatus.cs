using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.memory
{
    internal class PageStatus
    {
        public const byte ACTIVE = 0x00;
        public const byte INACTIVE = 0x01;
        public const byte WIRED = 0x02;
        public const byte FREE = 0x03;
    }
}
