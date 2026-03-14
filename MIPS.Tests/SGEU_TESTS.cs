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
    public class SGEU_TESTS
    {
        [TestMethod]
        public void SGEU_FALSE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t0, 5
        li $t1, 10
        sgeu $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // Cria uma instância do emulador MIPS
            MIPS_CPU cpu = new MIPS_CPU();

            // Executa o programa
            cpu.Process(program);

            // Verifica o resultado da instrução sgeu
            int rVal = cpu.Registers["$t2"];
            Assert.AreEqual(0, rVal); // $t2 deve ser 0 (falso), pois $t0 (5) não é maior ou igual a $t1 (10) em valores sem sinal
        }



        [TestMethod]
        public void SGEU_TRUE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t0, 10
        li $t1, 5
        sgeu $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // Cria uma instância do emulador MIPS
            MIPS_CPU cpu = new MIPS_CPU();

            // Executa o programa
            cpu.Process(program);

            // Verifica o resultado da instrução sgeu
            int rVal = cpu.Registers["$t2"];
            Assert.AreEqual(1, rVal); // $t2 deve ser 1 (verdadeiro), pois $t0 (10) é maior ou igual a $t1 (5) em valores sem sinal
        }
    }
}
