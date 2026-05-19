using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.IOKit.Devices;
using mOS.kernel;

namespace mOS.syscalls
{
    public class console
    {
        [Extern]
        public int read_char()
        {
            using(IOConsoleService cs = new IOConsoleService())
            {
                var ret = cs.ReadKey();
                byte[] b = new byte[1];
                b[0] = ret.KeyCode;
                mos_kernel.ProcessManager.CurrentProcessWrite(0, ref b);

                return 0;
            }
        }
    }
}
