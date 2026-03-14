using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    internal class xori
    { 
        // Realiza uma operação XOR bit a bit entre um registrador e um valor imediato.
        public static void __call(int instruction, ref Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            int result = registers[rs] ^ immediate;
            registers[rt] = result;
        }
    }
}
