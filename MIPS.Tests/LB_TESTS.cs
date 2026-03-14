using MIPS.Compiler;
using MIPS.Net.SoC;

namespace MIPS.Tests
{
    [TestClass]
    public class LB_TESTS
    {
        [TestMethod]
        public void TestMethod1()
        {
            // string assembly
            string asm = @"
addi $sp, $sp, 5
addi $t0, $t0, 5
sb $t0, 12($sp)
lb $v0, 12($sp)";

            // compilar pra bin
            byte[] program = MIPSCompiler.CompileInstructions(asm);


            MotherBoard mb = new MotherBoard(256, new List<IOPort>(), new List<IHardwareButton>());
            mb.TurnOn();

            // mandar processar
            mb.CPU.Process(program);

            // conferir o estado do registrador
            int rVal = mb.CPU.Registers["$v0"];
            Assert.IsTrue(rVal == 5);
        }
    }
}