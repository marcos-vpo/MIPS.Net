using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.IOKit.Devices
{
    internal class IODisplayCommand
    {
        public const byte TEXT_PRINT_CHAR = 0x1A;
        public const byte TEXT_FLUSH = 0x1C;
        public const byte TEXT_REQUEST_RESOLUTION = 0x99;
        public const byte TEXT_SCROLL_UP = 0x1E;
        public const byte TEXT_CLEAR = 0x2F;
    }
}
