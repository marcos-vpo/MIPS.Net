
using MIPS.Abi;
using mOSLib;

namespace mShell
{
    public class mOSShell : ABIManaged
    {

        [Extern]
        public void ShellMain()
        {
            int pId = mProcess.getpid();

        }

      
    }
}
