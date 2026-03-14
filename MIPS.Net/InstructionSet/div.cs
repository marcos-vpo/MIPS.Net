using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;

namespace MIPS.Net.InstructionSet
{
    public class div
    {
        // Divide o conteúdo de dois registradores e armazena o quociente em LO e o resto em HI.
        public static void __call(int instruction, ref Registers registers)
        {
            // Extrai os campos da instrução
            int rs = (instruction >> 21) & 0x1F;
            int rt = (instruction >> 16) & 0x1F;

            // Realiza a divisão
            int dividend = registers[rs];
            int divisor = registers[rt];

            if (divisor == 0)
            {
                Console.WriteLine("Warning: Division by zero!");
                registers[Registers.LO] = 0;  // Lo set to 0
                registers[Registers.HI] = 0;  // Hi set to 0
                return;
            }

            int quotient = dividend / divisor;
            int remainder = dividend % divisor;

            // Armazena o quociente no registrador Lo e o resto no registrador Hi
            registers[Registers.LO] = quotient;
            registers[Registers.HI] = remainder;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "div"
            if (parts[0] != "div")
            {
                throw new ArgumentException("Instrução inválida. Esperado 'div'.");
            }

            // 3. Extrair os registradores
            string rs = parts[1].Substring(1).TrimEnd(','); // Remove o '$', remove a vírgula e espaços extras.
            string rt = parts[2].Substring(1);          // Remove o '$',

            // 4. Converter os nomes dos registradores para seus valores numéricos
            int rsValue = registers.CodeByName(rs);
            int rtValue = registers.CodeByName(rt);

            // 5. Construir o valor da instrução MIPS (formato R-Type)
            int instructionValue = 0;
            instructionValue |= (0x00 << 26); // Opcode for special instructions (0x00)
            instructionValue |= (rsValue << 21);
            instructionValue |= (rtValue << 16);
            instructionValue |= (0 << 11);  // rd = 0 (not used by div)
            instructionValue |= (0 << 6);   // shamt = 0
            instructionValue |= (0x29 << 0); // funct =  (0x29) for div

            // 6. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
