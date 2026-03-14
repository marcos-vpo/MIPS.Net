using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{

    public class move
    {
        // Copia o conteúdo de um registrador para outro.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rd = (instruction >> 11) & 0x1F;  // rt não é usado, então ignora

            // Copia o valor do registrador rs para o registrador rd
            registers[rd] = registers[rs];
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "mov"
            if (parts[0] != "move")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'move'.");
            }

            // 3. Extrair os registradores
            string rd = parts[1]; 
            string rs = parts[2];

            rd = rd.Substring(0, rd.Length - 1);
            rs = rs.Trim();

            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rdValue = registers.CodeByName(rd);
            int rsValue = registers.CodeByName(rs);
            int rtValue = 0; // rt não é usado, definimos como 0

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0x0A << 26; 
            instructionValue |= rsValue << 21;
            instructionValue |= rtValue << 16;
            instructionValue |= rdValue << 11;
            instructionValue |= 0 << 6;       // shamt = 0
            instructionValue |= 0 << 0;       // funct = 0

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
