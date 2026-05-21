using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.kernel;
using mOS.mk_shell;

namespace mOS.syscalls
{
    public class sys : ABIManaged
    {
        [Extern]
        public void p_exit()
        {
            mos_kernel.ProcessManager.CurrentProgramExit();
        }

        [Extern]
        public void p_pause()
        {
            mos_kernel.ProcessManager.CurrentProgramPause();
        }

        [Extern]
        public void mk_shell()
        {
            MkShell m = new MkShell();
            m.Start(welcome: true);
        }

        [Extern]
        public int p_get_current_pid()
        {
          return   mos_kernel.ProcessManager.CurrentProcessId();
        }

        [Extern]
        public int p_get_addr()
        {
            return mos_kernel.ProcessManager.CurrentProcessAddr();
        }
    }
}
