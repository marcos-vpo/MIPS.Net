using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class lui
    {
        // Carrega um valor imediato de 16 bits em um registrador.
        public static void __call(int instruction, ref Registers registers)
        {
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            registers[rt] = immediate << 16;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "lui"
            if (parts[0] != "lui")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'lui'.");
            }

            // 3. Extrair os operandos: rt, immediate
            // Assumindo o formato: lui $rt, immediate
            string rtString = parts[1].Replace(",", "").Substring(1); // Remove ',', '$'
            string immediateString = parts[2];

            // 4. Converter o nome do registrador para seu código numérico
            int rtValue = registers.CodeByName(rtString);

            // 5. Converter o valor imediato para inteiro
            int imm;
            if (immediateString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                imm = Convert.ToInt32(immediateString, 16); // Hexadecimal
            }
            else
            {
                imm = int.Parse(immediateString); // Decimal
            }

            // 6. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;

            instructionValue |= 0x0F << 26; // Opcode para lui (0x0F = 15)
            instructionValue |= 0 << 21;    // rs (sempre 0 para lui)
            instructionValue |= rtValue << 16; // rt
            instructionValue |= imm & 0xFFFF;  // immediate (16 bits)

            // 7. Retornar o valor da instrução como um array de bytes
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
