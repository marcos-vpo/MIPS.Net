using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class lb
    {  // Carrega um byte da memória em um registrador.
        public static void __call(int instruction, Registers registers)
        {
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;

            // eu precisaria guardar 2 numeros aqui onde seria "offset"
            // em 16 bits estaria o valor para offset
            // e em outros 16 estaria o valor para o tamanho o buffer
            int offset = instruction & 0xFFFF;

            int address = registers[rs] + offset;

            MemoryBUS.SEND('R', address, new byte[1], (KeyValuePair<bool, byte[]> ret) =>
            {
                if (ret.Value != null)
                    registers[rt] = ret.Value[0];
                return 0;
            });
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "lb"
            if (parts[0] != "lb")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'lb'.");
            }

            // 3. Extrair os registradores e o valor imediato
            // 3. Extrair os registradores e o valor imediato
            string rt = parts[1].Substring(1); // Remove o '$' do nome do registrador
                                               //           string rs = parts[2].Substring(1);
            string offsetPart = parts[2]; // Extrai a parte do offset com parênteses

            // 4. Extrair o offset e o registrador de base
            int offset = int.Parse(offsetPart.Substring(0, offsetPart.IndexOf('('))); // Remove parênteses e converte para inteiro
            string baseRegisterName = offsetPart.Substring(offsetPart.IndexOf('(') + 1, offsetPart.IndexOf(')') - offsetPart.IndexOf('(') - 1); // Extrai o nome do registrador de base
                                                                                                                                                //      offset = (offset + registers[baseRegisterName]);

            rt = rt.Substring(0, rt.Length - 1);


            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rtValue = registers.CodeByName(rt);
            int rsValue = registers.CodeByName(baseRegisterName);

            // 5. Construir o valor da instrução MIPS (formato I-Type)
            int instructionValue = 0;
            instructionValue |= 0x20 << 26; // Opcode para lb (0x20)
            instructionValue |= rsValue << 21;
            instructionValue |= rtValue << 16;
            instructionValue |= offset & 0xFFFF; // Valor imediato (16 bits)

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
