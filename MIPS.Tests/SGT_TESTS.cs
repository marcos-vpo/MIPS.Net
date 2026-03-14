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
    public class SGT_TESTS
    {
        [TestMethod]
        public void SGT_FALSE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t0, 5
        li $t1, 10
        sgt $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // apenas demonstrcao, seria a instancia da cpu
            // obtida da Motherboard
            MIPS_CPU cpu = new MIPS_CPU();

            // Executa o programa
            cpu.Process(program);

            // Verifica o resultado da instrução sgt
            int rVal = cpu.Registers["$t2"];
            Assert.AreEqual(0, rVal); // $t2 deve ser 0 (falso), pois $t0 (5) não é maior que $t1 (10)
        }



        [TestMethod]
        public void SGT_TRUE()
        {
            // Instruções em Assembly
            string asm = @"
        li $t0, 10
        li $t1, 5
        sgt $t2, $t0, $t1 
    ";

            // Compila as instruções para código binário
            byte[] program = MIPSCompiler.CompileInstructions(asm);

            // Cria uma instância do emulador MIPS
            MIPS_CPU cpu = new MIPS_CPU();

            // Executa o programa
            cpu.Process(program);

            // Verifica o resultado da instrução sgt
            int rVal = cpu.Registers["$t2"];
            Assert.AreEqual(1, rVal); // $t2 deve ser 1 (verdadeiro), pois $t0 (10) é maior que $t1 (5)
        }
    }
}
