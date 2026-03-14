using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net
{
    public interface IOBusSignalCapture
    {
        void OnIOBus(int addr, byte[] data, bool write, bool read, bool interruptionSignal);
    }
}
