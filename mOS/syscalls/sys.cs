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
        public void mk_shell()
        {
            MkShell m = new MkShell();
            m.Start(welcome: false);
        }

        [Extern]
        public void p_get_current_pid()
        {
             mos_kernel.ProcessManager.CurrentProcessId();
        }
    }
}
