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
    public class ANDI_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            string asm = @"
addiu $t0, $t0, 5
addi $t1, $t1, 5
andi $v1, $t0, 50";

            byte[] program = MIPSCompiler.CompileInstructions(asm);


            MIPS_CPU cpu = new MIPS_CPU(debugger: null);
            cpu.Process(program);

            int rVal = cpu.Registers["$t0"];
            Assert.IsTrue(rVal == 5);
        }
    }
}
