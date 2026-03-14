using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    public interface IHardwareButton
    {
        public object Tag { get; set; }
        public byte ID { get; set; }
        public int InterruptionCodeClick { get; set; }
        public void BusConnected(IOBUS bus);
  
        void SendClick(Func<int> callback);
        void TurnOff();
        void TurnOn();
    }
}
