using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.abi;
using MIPS.Net.SoC;

namespace MIPS.Net.Compiler
{
    public class MIPSCompiler
    {
        public static byte[] CompileInstructions(string asm_code)
        {
            asm_code = asm_code.Trim().Replace("\r", "");
            string[] steps = asm_code.Split('\n');

            byte[] mips_program = new byte[steps.Length * 4];
            int pos = 0;

            Registers rx = new Registers();

            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i].Trim();
                if (step.StartsWith("#")) continue;

                if (step.Contains("#"))
                    step = step.Substring(0, step.IndexOf("#"));
                step = step.Trim();

          
                byte[] instruction_unit = InstructionParser.__parse(step, rx);
                Array.Copy(instruction_unit, 0, mips_program, pos, instruction_unit.Length);
                pos += instruction_unit.Length;
            }

            return mips_program;
        }
    }
}
