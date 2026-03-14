using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.IOKit
{
    internal class IODeviceClass
    {
        public const byte STORAGE = 0x2D;
        public const byte DISPLAY = 0x3D;
        public const byte PRINTER = 0x4D;
        public const byte DEBUGGER = 0x5D;
        public const byte NETWORK = 0x6D;
        public const byte KEYBOARD = 0x7D;
        public const byte MOUSE = 0x8D;

        internal static bool IsValid(byte deviceClass)
        {
            if (deviceClass == STORAGE) return true;
            else if (deviceClass == DISPLAY) return true;
            else if (deviceClass == PRINTER) return true;
            else if (deviceClass == DEBUGGER) return true;
            else if (deviceClass == NETWORK) return true;
            else if (deviceClass == KEYBOARD) return true;
            else if (deviceClass == MOUSE) return true;
            else return false;
        }
    }
}
