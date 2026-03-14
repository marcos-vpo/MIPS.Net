using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class ori
    {
        // Realiza uma operação OR bit a bit entre um registrador e um valor imediato.
        public static void __call(int instruction, ref Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            int result = registers[rs] | immediate;
            registers[rt] = result;
        }


        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "ori"
            if (parts[0] != "ori")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'ori'.");
            }

            // 3. Extrair os operandos: rt, rs, immediate
            // Assumindo o formato: ori $rt, $rs, immediate
            string rtString = parts[1].Replace(",", "").Substring(1); // Remove ',', '$'
            string rsString = parts[2].Replace(",", "").Substring(1); // Remove ',', '$'
            string immediateString = parts[3];

            // 4. Converter os nomes dos registradores para seus códigos numéricos
            int rtValue = registers.CodeByName(rtString);
            int rsValue = registers.CodeByName(rsString);

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

            instructionValue |= 0x0D << 26; // Opcode para ori (0x0D = 13)
            instructionValue |= rsValue << 21; // rs
            instructionValue |= rtValue << 16; // rt
            instructionValue |= imm & 0xFFFF;  // immediate (16 bits)

            // 7. Retornar o valor da instrução como um array de bytes
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
