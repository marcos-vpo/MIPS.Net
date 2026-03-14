using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Compiler
{
    public class MIPSCompiler
    {
        public static byte[] CompileInstructions(string asm_code)
        {
           return MIPS.Net.Compiler.MIPSCompiler.CompileInstructions(asm_code);
        }
    }
}
