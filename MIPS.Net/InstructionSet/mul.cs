using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class mul
    {
        // Multiplica o conteúdo de dois registradores, armazenando o resultado em outro registrador.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;
            int rd = (instruction >> 11) & 0x1F;

            // Calcula o resultado da multiplicação
            int result = registers[rs] * registers[rt];

            // Armazena o resultado no registrador de destino (rd)
            registers[rd] = result;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "mul"
            if (parts[0] != "mul")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'mul'.");
            }

            // 3. Extrair os registradores
            string rd = parts[1].Substring(1); // Remove o '$' do nome do registrador
            string rs = parts[2].Substring(1);
            string rt = parts[3].Substring(1);

            // Remover vírgula dos registradores, se houver
            rd = rd.Substring(0, rd.Length - 1);
            rs = rs.Substring(0, rs.Length - 1);
            //rt = rt.Substring(0, rt.Length - 1); // Remova a vírgula apenas se ela existir

            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rdValue = registers.CodeByName(rd);
            int rsValue = registers.CodeByName(rs);
            int rtValue = registers.CodeByName(rt);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0 << 26;     // Opcode para R-type = 0
            instructionValue |= rsValue << 21;
            instructionValue |= rtValue << 16;
            instructionValue |= rdValue << 11;
            instructionValue |= 0 << 6;        // shamt = 0
            instructionValue |= 0x1A;          // funct = 0x1A (26 em decimal)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
