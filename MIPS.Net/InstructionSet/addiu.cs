using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class addiu
    {
        // Adiciona um valor imediato a um registrador, tratando os operandos como números sem sinal.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            // Calcula o resultado da adição sem sinal
            uint result = (uint)registers[rs] + (uint)immediate;

            // Armazena o resultado no registrador de destino (rt)
            registers[rt] = (int)result; // Converta de volta para int
        }


        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "addiu"
            if (parts[0] != "addiu")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'addiu'.");
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

            // 5. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 9 << 26; // Opcode para addiu (9)
            instructionValue |= rsValue << 21;
            instructionValue |= rdValue << 16;
            instructionValue |= imm << 0;  // Valor imediato no deslocamento correto (0)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
