using MIPS.Compiler;
using MIPS.Net.SoC;

namespace MIPS.Tests
{
    [TestClass]
    public class SB_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            // string assembly
            string asm = @"
addi $t0, $$t0, 5
addi $t0, $t0, 50
sb $t0, 10($sp)";

            byte[] program = MIPSCompiler.CompileInstructions(asm);

            MotherBoard mb = new MotherBoard(20, new List<IOPort>(), new List<IHardwareButton>());
            mb.TurnOn();

            mb.CPU.Process(program);

            Assert.IsTrue(mb.Memory[10] == (byte)55);
        }
    }
}