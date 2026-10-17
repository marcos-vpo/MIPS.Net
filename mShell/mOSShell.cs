
using MIPS.Abi;
using mOSLib;
using mOSLib.functions;
using mOSLib.types;
namespace mShell
{
    public class mOSShell : ABIManaged
    {
        private libmOS lib;

        [Extern]
        public int ShellMain()
        {
            if (lib == null) lib = libmOS.init();
 
            lib.console_write_line(new mString("escreva algo: "));
            mString line = lib.console_readline();

            lib.console_write_line(new mString("você escreveu: " + line.Value));
              
            Thread.Sleep(500);
            lib.console_write_line(new mString("Pressione uma tecla para encerrar"));
            lib.console_readchar();
             
            return 1;  
        }

        [Extern]
        public int OnResume()
        {


            return 0;
        }
    }
}
