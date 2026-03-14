using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class beq
    {
        // Desvia para o endereço especificado se os valores em rs e rt forem iguais.
        public static void __call(int instruction, ref Registers registers)
        {
            // 1. Extrair os registradores rs e rt da instrução.
            int rs = (instruction >> 21) & 0x1F;  // Bits 25-21
            int rt = (instruction >> 16) & 0x1F;  // Bits 20-16
            short offset = (short)(instruction & 0xFFFF); // Bits 15-0 (offset de 16 bits com sinal)

            // 2. Obter os valores dos registradores.
            int valor_rs = registers[rs];
            int valor_rt = registers[rt];

            // 3. Comparar os valores.
            if (valor_rs == valor_rt)
            {
                // 4. Calcular o endereço de destino.
                // O offset é um valor *em número de instruções*, então multiplicamos por 4.
                // Adicionamos 4 ao PC atual porque o PC é incrementado *antes* da instrução ser executada.
          //      int endereco_destino = registers["$pc"] + 4 + (offset * 4);

                //Verifica se o endereço de destino tem algum rotulo
                var rotule = MIPS_CPU.CurrentProgram.GetRotuleByRelativeAddr(offset);


                registers[Registers.RA] = registers[Registers.PC] + 4;
                // 5. Atualizar o PC.
                registers[Registers.PC] = rotule.AbsoluteAddr - 4;
            }
            else
            {
                // Se a condição não for atendida, o PC é incrementado normalmente
            //    MIPS_CPU.Instance.DBG?.ProgramSwitching(MIPS_CPU.CurrentProgram, registers["$pc"] + 4);
            }
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes.
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "beq".
            if (parts[0] != "beq")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'beq'.");
            }

            // 3. Extrair os registradores rs, rt e o label.
            string rs_str = parts[1].Replace(",", ""); // Remove a vírgula, se houver
            string rt_str = parts[2].Replace(",", ""); // Remove a vírgula, se houver
            string label = parts[3];

            // 4. Obter os números dos registradores.
            int rs = registers.CodeByName(rs_str);
            int rt = registers.CodeByName(rt_str);

            short offset = short.Parse(label);

            // 6. Construir o valor da instrução MIPS (formato I-Type).
            int instructionValue = 0;
            instructionValue |= 0x14 << 26;        // Opcode para beq (0x04)
            instructionValue |= rs << 21;           // rs (5 bits)
            instructionValue |= rt << 16;           // rt (5 bits)
            instructionValue |= (offset & 0xFFFF);  // offset (16 bits)

            // 7. Retornar o valor da instrução em bytes.
            return BitConverter.GetBytes(instructionValue);
        }
    }
}