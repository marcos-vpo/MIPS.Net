using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class andi
    {  
        // Realiza uma operação AND bit a bit entre um registrador e um valor imediato.
        public static void __call(int instruction, ref Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            int result = registers[rs] & immediate;
            registers[rt] = result;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "andi"
            if (parts[0] != "andi")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'andi'.");
            }

            // 3. Extrair os registradores e o valor imediato
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            string rs = parts[2].Substring(1);
            int imm = int.Parse(parts[3]); // Converta o valor imediato para inteiro

            rd = rd.Substring(0, rd.Length - 1);
            rs = rs.Substring(0, rs.Length - 1);

            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rdValue = registers.CodeByName(rd);
            int rsValue = registers.CodeByName(rs);

            // 5. Construir o valor da instrução MIPS (assumindo formato I-Type)
            int instructionValue = 0;
            instructionValue |= 12 << 26; // Opcode para andi (12)
            instructionValue |= rsValue << 21;
            instructionValue |= rdValue << 16;
            instructionValue |= imm & 0xFFFF;  // Valor imediato (16 bits)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }

    }
}
