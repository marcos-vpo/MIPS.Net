using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net
{
    public interface MemBusSignalCapture
    {
        void OnMemBus(int addr, byte[] data, bool write, bool read, bool interruptionSignal);
    }
}
