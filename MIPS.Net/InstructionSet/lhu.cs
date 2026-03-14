using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    internal class lhu
    {   
        // Carrega uma meia-palavra da memória em um registrador, tratando o valor como sem sinal.
        public static void __call(int instruction, ref Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int offset = instruction & 0xFFFF;

            int address = registers[rs] + offset;

            // TODO: Implementar acesso à memória
            // ...
            registers[rt] = 0;/* valor da meia-palavra lido da memória (sem sinal) */;
        }
    }
}
