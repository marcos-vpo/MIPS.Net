using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class jr
    {    // Retorna para o endereço armazenado em $ra.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai o registrador de destino (rs)
            int rs = (instruction >> 21) & 0x1F;

            // Se o registrador rs for $ra, atualiza o PC com o valor de $ra
            if (rs == 31) // Código do registrador $ra
            {
                var  ra = registers[Registers.RA];

                registers[Registers.PC] = ra;//registers["$ra"]; // Atualiza o PC com o valor de $ra
            }
            else
            {
                registers[Registers.RA] = registers[Registers.PC];
                registers[Registers.PC] = (registers[rs] - 4);
              
                MIPS_CPU.ProgramSwitch();
            }
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "jr"
            if (parts[0] != "jr")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'jr'.");
            }

            // 3. Extrair o registrador de destino (rs)
            string rs = parts[1].Substring(1); // Remove o '$' do nome do registrador
                                               //     rs = rs.Substring(0, rs.Length - 1); // Remove o '$'

            // 4. Converter o nome do registrador para seu código numérico
            int rsValue = registers.CodeByName(rs);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= 0x30 << 26; // Opcode para jr ( 0x30)
            instructionValue |= rsValue << 21; // Código do registrador de destino
            instructionValue |= 0x00 << 16; // rd (0)
            instructionValue |= 0x08 << 11; // funct (0x08)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
