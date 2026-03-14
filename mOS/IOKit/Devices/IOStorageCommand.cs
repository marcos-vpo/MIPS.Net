using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.IOKit.Devices
{
    internal class IOStorageCommand
    {
        public const byte FETCH_DIR_LIST = 0xFD;
        public const byte FETCH_FILES_LIST = 0xFF;
        public const byte IDLE = 0x0F;
        public const byte F_READ_ALL = 0xF0;
        public const byte F_OPEN = 0xF1;
        public const byte F_CLOSE = 0xF2;
        public const byte F_READ_BLOCK = 0xF3;
        public const byte F_WRITE_BLOCK = 0xF4;
        public const byte F_SET_POSITION = 0xF5;
        public const byte F_FLUSH = 0xF6;
        public const byte F_LENGTH = 0xF7;


    }

}
