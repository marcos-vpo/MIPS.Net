using MIPS.Compiler;
using MIPS.Net.SoC;

namespace MIPS.Tests
{
    [TestClass]
    public class MOVE_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            string asm = @"li $t1, 10
move $t0, $t1";

            byte[] program = MIPSCompiler.CompileInstructions(asm);


            MIPS_CPU cpu = new  MIPS_CPU();
            cpu.Process(program);

            int rVal = cpu.Registers["$t0"];
            Assert.IsTrue(rVal == 10);
        }
    }
}