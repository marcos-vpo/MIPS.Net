using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.IO
{
    public class HwButton : IHardwareButton
    {
        public object Tag { get; set; }
        public byte ID { get; set; }
        public int InterruptionCodeClick { get; set; }


        public delegate void Init();
        public event Init OnInit;

        public delegate void OnHalt();
        public event OnHalt Halted;

        private IOBUS _io_bus;
        public void BusConnected(IOBUS bus)
        {
            _io_bus = bus;
        }

        public void SendClick(Func<int> callback)
        {
            Task.Run(() =>
            {
                byte[] intCodeBin = BitConverter.GetBytes(InterruptionCodeClick);
                _io_bus.SendBus('I', (int)ID, intCodeBin, (e) =>
                {
                    callback();
                    return 0;
                });
            });
        }

        public void TurnOn()
        {
            OnInit?.Invoke();
        }

        public void TurnOff()
        {
            Halted?.Invoke();
        }
    }
}
