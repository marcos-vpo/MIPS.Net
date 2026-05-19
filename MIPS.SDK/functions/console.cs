using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;

namespace mOSLib.functions
{
    internal class console: ABIManaged
    {
        private readonly sys sys;

        public console(sys sys)
        {
            this.sys = sys;
        }
        public char read_char()
        {
            sys.syscall(v0: 11, k0: false);
            byte[] b = new byte[1];
            m_read(0, ref b);
            return (char)b[0];
        }
    }
}
