using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class mflo
    {
        // Move o valor do registrador especial Lo para um registrador de uso geral rd.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai o campo rd da instrução
            int rd = (instruction >> 11) & 0x1F;

            // Move o valor de Lo para rd
            registers[rd] = registers[Registers.LO];
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "mflo"
            if (parts[0] != "mflo")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'mflo'.");
            }

            // 3. Extrair o registrador de destino (rd)
            string rd = parts[1].Substring(1); // Remove o '$'

            // 4. Converter o nome do registrador para seu valor numérico
            int rdValue = registers.CodeByName(rd);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0x00 << 26; // Opcode para instruções especiais (0x00)
            instructionValue |= 0x00 << 21; // rs = 0 (não usado por mflo)
            instructionValue |= 0x00 << 16; // rt = 0 (não usado por mflo)
            instructionValue |= rdValue << 11;
            instructionValue |= 0x00 << 6;  // shamt = 0
            instructionValue |= 0x12 << 0;  // funct = 0x12 (18) para mflo

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}