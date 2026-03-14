using System.Reflection;
using MIPS.Net.SoC;
using NetPC;

namespace mOSLib.Debugger
{
    public class MIPSMachineDebugger
    {
        public void DeployAndDebug(MotherBoard mb, Assembly asm, string firmware)
        {
            MIPS_CPU.FFIDebug(asm);

            NetPCForm form = new NetPCForm(mb, firmware);
            form.ShowDialog();
        }
    }
}
