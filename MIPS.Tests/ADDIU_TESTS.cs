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
    public class ADDIU_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            string asm = @"
addiu $t0, $t0, 5";

            byte[] program = MIPSCompiler.CompileInstructions(asm);


            MIPS_CPU cpu = new MIPS_CPU();
            cpu.Process(program);

            int rVal = cpu.Registers["$t0"];
            Assert.IsTrue(rVal == 5);
        }
    }
}
