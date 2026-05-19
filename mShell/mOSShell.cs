
using MIPS.Abi;
using mOSLib;
using mOSLib.functions;
using mOSLib.types; 
namespace mShell
{
    public class mOSShell  
    {
        private  libmOS lib;
     
        [Extern]
        public void ShellMain()
        {
            if(lib == null)  lib = libmOS.init();

            char ch = lib.console_readchar();

            mString str = new mString("Teste 123");
            lib.mem_write(str);

            mString str2 = new mString("Uma outra string muito maior que a primeira em termos de.");
            lib.mem_write(str2);

            mString strLeituraStr1 = lib.mem_read<mString>(addr: 0);
            mString strLeituraStr2 = lib.mem_read<mString>(addr: 26);
            strLeituraStr2.Value = "Uma outra string muito maior que a primeira em termos de.";


            lib.mem_write(strLeituraStr2); 
         
        }


    }
}
