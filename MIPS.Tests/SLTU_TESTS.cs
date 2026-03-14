using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Compiler;
using MIPS.Net.SoC;

namespace MIPS.Tests
{
    [TestClass]
    public class SLTU_TESTS
    {
        [TestMethod]
        public void SLTU_FALSE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t2, 2
        li $t0, 10
        li $t1, 5
        sltu $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // Cria uma instância do emulador MIPS
            MotherBoard mb = new MotherBoard(256, new List<IOPort>(), new List<IHardwareButton>());
            mb.TurnOn();

            // Executa o programa
            mb.CPU.Process(program);

            // Verifica o resultado da instrução sltu
            int rVal = mb.CPU.Registers["$t2"];
            Assert.AreEqual(0, rVal); // $t2 deve ser 0 (Falso), pois $t0 (10) não é menor que $t1 (5) em valores sem sinal
        }



        [TestMethod]
        public void SLT_TRUE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t2, 2
        li $t0, 8
        li $t1, 10
        sltu $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // Cria uma instância do emulador MIPS
            MotherBoard mb = new MotherBoard(256, new List<IOPort>(), new List<IHardwareButton>());
            mb.TurnOn();

            // Executa o programa
            mb.CPU.Process(program);

            // Verifica o resultado da instrução sltu
            int rVal = mb.CPU.Registers["$t2"];
            Assert.AreEqual(1, rVal); // $t2 deve ser 1 (Verdadeiro), pois $t0 (8) é menor que $t1 (10) em valores sem sinal
        }
    }
}
