using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class li
    {

        // Carrega um valor imediato em um registrador.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int immediate = instruction & 0xFFFF;

            // Armazena o valor imediato no registrador de destino (rt)
            registers[rt] = immediate;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            if (instruction.Contains("li $v0, 68202"))
            {

            }
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "li"
            if (parts[0] != "li")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'li'.");
            }

            // 3. Extrair o registrador de destino e o valor imediato
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            int imm = 0;
            if (parts[2].StartsWith("'"))
            {
                string chars = parts[2].Replace("'", "").Replace("\\", @"\");
                byte[] bchars = new byte[4];

                if (chars.Contains("\\0"))
                    bchars[0] = (byte)'\0';
                else if (chars.Contains("\\n"))
                    bchars[0] = (byte)'\n';
                else if (chars.Contains("\\r"))
                    bchars[0] = (byte)'\r';
                else

                    for (int i = 0; i < chars.Length; i++)
                    {
                        bchars[i] = (byte)(chars[i] & 0xff);
                    }

                imm = BitConverter.ToInt32(bchars, 0);
            }
            else
            {
                imm = parts[2].Contains("x")
                   ? Convert.ToInt32(parts[2], 16)
                   : int.Parse(parts[2]);// Converte o valor imediato para short
            }
            rd = rd.Substring(0, rd.Length - 1);

            // 4. Converter o nome do registrador para seu código numérico
            int rdValue = registers.CodeByName(rd);

            // 5. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 0x3E << 26; // Opcode para li (0x9D) 
            instructionValue |= 0 << 21; // rs (0)
            instructionValue |= rdValue << 16;
            instructionValue |= imm & 0xFFFF;  // Valor imediato (16 bits)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
