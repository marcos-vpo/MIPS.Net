using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class mfhi
    {
        // Move o valor do registrador especial Hi para um registrador de uso geral rd.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai o campo rd da instrução
            int rd = (instruction >> 11) & 0x1F;

            // Move o valor de Hi para rd
            registers[rd] = registers[Registers.HI];
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "mfhi"
            if (parts[0] != "mfhi")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'mfhi'.");
            }

            // 3. Extrair o registrador de destino (rd)
            string rd = parts[1].Substring(1); // Remove o '$'

            // 4. Converter o nome do registrador para seu valor numérico
            int rdValue = registers.CodeByName(rd);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0x00 << 26; // Opcode para instruções especiais (0x00)
            instructionValue |= 0x00 << 21; // rs = 0 (não usado por mfhi)
            instructionValue |= 0x00 << 16; // rt = 0 (não usado por mfhi)
            instructionValue |= rdValue << 11;
            instructionValue |= 0x00 << 6;  // shamt = 0
            instructionValue |= 0x10 << 0;  // funct = 0x10 (16) para mfhi

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}