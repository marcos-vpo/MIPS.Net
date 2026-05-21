using MIPS.Abi;
using mOS.IOKit.Devices;
using mOS.kernel;
using mOS.process;
using mOS.process_heap.ph_obj;
using mOS.types;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace mOS.syscalls
{
    public class console : ABIManaged
    {
        [Extern]
        public int read_char()
        {
            using (IOConsoleService cs = new IOConsoleService())
            {
                var ret = cs.ReadKey();
                byte[] b = new byte[64];
                b[0] = ret.KeyCode;
                mos_kernel.ProcessManager.CurrentProcessWrite(0, ref b);

                return 0;
            }
        }

        [Extern]
        public int read_line()
        {
            using (IOConsoleService cs = new IOConsoleService())
            {
                var str = cs.ReadLine(print: false);

                var pm = mos_kernel.ProcessManager;
                ProcessEntry pe = pm.GetCurrent();
                AllocResult alloc = pm.CurrentProcessAlloc(str.Length);

                int current_p_code_size = pm.CurrentProcessCodeSize();
                int baseVirtualHeapAddr = alloc.BaseVirtualAddr - current_p_code_size;

                mString mStr = new mString(str);
                mStr.SetVirtualAddr(baseVirtualHeapAddr);
                byte[] objBin = mStr.Serialize();// Encoding.UTF8.GetBytes(str);

                int objTupleLen = 12 + objBin.Length;
                byte[] objTuple = new byte[objTupleLen];

                Array.Copy(BitConverter.GetBytes(mStr.mOSType), 0, objTuple, 0, 2);
                Array.Copy(BitConverter.GetBytes((short)0), 0, objTuple, 2, 2); // flags
                Array.Copy(BitConverter.GetBytes(-1), 0, objTuple, 4, 4);      // continuation (virtual)
                Array.Copy(BitConverter.GetBytes(objBin.Length), 0, objTuple, 8, 4); // payload size
                Array.Copy(objBin, 0, objTuple, 12, objBin.Length);
                 

                pm.CurrentProcessWrite(alloc.BaseVirtualAddr, ref objTuple);

                byte[] response = new byte[64];

                Array.Copy(BitConverter.GetBytes(objBin.Length), 0, response, 0, 4);
                Array.Copy(BitConverter.GetBytes(baseVirtualHeapAddr), 0, response, 4, 4);

                int posResp = 8;
                for(int i =  0; i < alloc.PhysicalPages.Length; i++)
                {
                    int page = alloc.PhysicalPages[i];
                    Array.Copy(BitConverter.GetBytes(page), 0, response, posResp, 4);
                    posResp += 4;
                }
                 
                mos_kernel.ProcessManager.CurrentProcessWrite(0, ref response);

                return 0;
            }
        }
    }
}
