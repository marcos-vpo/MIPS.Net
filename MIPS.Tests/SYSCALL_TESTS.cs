using MIPS.Compiler;
using MIPS.Net.SoC;

namespace MIPS.Tests
{
    [TestClass]
    public class SYSCALL_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            /**
             * Write val 50 in pos 252-255 of mem
             * copy block 4b from 252-255 -> to 12-15
             * reads word from 12-15 -> to $t0
             */
            string asm = @"
li $t1, 50
sw $t1, 252($sp)

li $a0, 252
li $a1, 12
li $a2, 4
li $v0, 110
syscall

lw $t0, 12($sp)";

            byte[] instructions = MIPSCompiler.CompileInstructions(asm);

            MotherBoard mb = new MotherBoard(256, new List<IOPort>(), new List<IHardwareButton>());
            mb.TurnOn();

            mb.CPU.Process(instructions);

            int rVal = mb.CPU.Registers["$t0"];
            Assert.IsTrue(rVal == 50);
        }
    }
}