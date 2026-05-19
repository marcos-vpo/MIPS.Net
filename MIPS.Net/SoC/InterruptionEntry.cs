using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Net.SoC
{
    public class InterruptionEntry
    {
        public int InterruptionAddress { get; set; }
        public int Code { get; set; }
        public int HandlerAddress { get; set; }
        public int DeviceMemoryAddress { get; set; }


        public override string ToString()
        {
            return $"[{Code}]: INTR_ADDR *{InterruptionAddress}, HNDLR_ADDR: {HandlerAddress}, DEV_PORT_ADDR: {DeviceMemoryAddress}";
        }

       //    private AutoResetEvent _are = new AutoResetEvent(false);

        public bool IsProcessing { get; private set; }
        public bool Ready { get; internal set; }

        public void WaitInterruptionExecution(bool wait = false)
        {
            IsProcessing = true;
        //         _are.WaitOne();

        //    if (wait)
         //      while (IsProcessing)
          //          Thread.Sleep(100);
        }

        public void End()
        {
            IsProcessing = false;
       //        if(_are != null )
       //     {
         //               _are.Set();
       //     }

        }
    }
}
