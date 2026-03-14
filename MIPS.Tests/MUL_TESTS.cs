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
    public class MUL_TESTS
    {
        [TestMethod]
        public void TestMul()
        {
            string asm = @"li $t2, 10
li $t3, 10
mul $t2, $t2, $t3";

            byte[] program = MIPSCompiler.CompileInstructions(asm);


            MIPS_CPU cpu = new MIPS_CPU();
            cpu.Process(program);

            int rVal = cpu.Registers["$t2"];
            Assert.IsTrue(rVal == 100);
        }
    }
}
