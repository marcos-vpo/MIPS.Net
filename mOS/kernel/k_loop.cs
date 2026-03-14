using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.kernel
{
    internal enum k_loo_action
    {
        go_to_process = 1
    }
    internal class k_loop
    {
        public static void args(
          k_loo_action action,
          int addr,
          int arg0 = 0,
          int arg1 = 0,
          int arg2 = 0,
          int arg3 = 0)
        {
            byte[] b = new byte[4 * 6];

            Buffer.BlockCopy(BitConverter.GetBytes((int)action), 0, b, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(addr), 0, b, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(arg0), 0, b, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(arg1), 0, b, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(arg2), 0, b, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(arg3), 0, b, 20, 4);

            mos_kernel.write_loop_args(ref b);
        }
    }
}
