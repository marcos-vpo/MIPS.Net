using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOSLib.types;

namespace mOSLib.functions
{
    internal class console : ABIManaged
    {
        private readonly sys sys;
        private readonly mem mem;
        public console(sys sys, mem mem)
        {
            this.sys = sys;
            this.mem = mem;
        }

        public char read_char()
        {
            sys.syscall(v0: call_codes.READ_CHAR, k0: false);
            byte[] b = new byte[1];
            m_read(0, ref b);
            return (char)b[0];
        }

        public mString read_line()
        {
            sys.syscall(v0: call_codes.READ_LINE, k0: false);

            byte[] return_buffer = new byte[64];
            m_read(0, ref return_buffer);

            List<int> return_data = new List<int>();
            for (int i = 0; i < return_buffer.Length; i += 4)
            {
                int page = BitConverter.ToInt32(return_buffer, i);

                if (page == 0)
                {
                    break;
                }

       
                return_data.Add(page);
            }

            int dataLen = return_data[0];
            int start_addr = return_data[1];

            mem.attatch(validPages: return_data[2..]);
              
            mString mstr = mem.read<mString>(start_addr); 
            return mstr;
        }
    }
}
