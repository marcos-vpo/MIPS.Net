using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class sge
    {
        // Compara dois registradores e define a flag "Greater Than or Equal" (GE) se o primeiro for maior ou igual ao segundo.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int rd = (instruction >> 11) & 0x1F;

            // Compara os valores dos registradores rs e rt
            if (registers[rs] >= registers[rt])
            {
                // Define o registrador rd como 1 (True)
                registers[rd] = 1;
            }
            else
            {
                // Define o registrador rd como 0 (False)
                registers[rd] = 0;
            }
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "sge"
            if (parts[0] != "sge")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'sge'.");
            }

            // 3. Extrair os registradores
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            string rs = parts[2].Substring(1);
            string rt = parts[3];

            rd = rd.Substring(0, rd.Length - 1);
            rs = rs.Substring(0, rs.Length - 1);
            rt = rt.Substring(0, rt.Length);

            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rdValue = registers.CodeByName(rd);
            int rsValue = registers.CodeByName(rs);
            int rtValue = registers.CodeByName(rt);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0x00 << 26; // Opcode para sge (0)
            instructionValue |= rsValue << 21;
            instructionValue |= rtValue << 16;
            instructionValue |= rdValue << 11;
            instructionValue |= 0x22; // Funct para sge (0x22)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }

    }
}
