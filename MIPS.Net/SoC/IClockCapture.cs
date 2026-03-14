using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    public interface IClockCapture
    {
        void MIPS_OnClock(short clock_status, double frequency, int energyLevel);
    }
}
