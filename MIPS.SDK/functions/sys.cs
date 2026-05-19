using MIPS.Compiler;
using MIPS.Net.SoC;

namespace mOSLib.functions
{
    public class sys
    {
        public  int syscall(int v0, int? a0 = null, int? a1 = null, int? a2 = null, int? a3 = null, bool k0 = true, int k1 = 0)
        {
            // ASM mínimo: apenas o gatilho
            string asm = "syscall";

            int[] v1v2 = new int[2];

            MIPS_CPU cpu = MIPS_CPU.Instance;

            // Passagem de parâmetros diretamente nos registradores
            if (a0.HasValue) cpu.Registers["$a0"] = a0.Value;
            if (a1.HasValue) cpu.Registers["$a1"] = a1.Value;
            if (a2.HasValue) cpu.Registers["$a2"] = a2.Value;
            if (a3.HasValue) cpu.Registers["$a3"] = a3.Value;

            cpu.Registers["$v0"] = v0;
            cpu.Registers["$k0"] = k0 ? 1 : 0;


            if(k1 > 0)
            {
                cpu.Process(MIPSCompiler.CompileInstructions($"lar $k1, {k1}"));
            }
 

            byte[] assembly = MIPSCompiler.CompileInstructions(asm);

            cpu.Process(assembly);

            v1v2[0] = cpu.Registers["$v0"];
            v1v2[1] = cpu.Registers["$v1"];

            // Higiene pós-syscall
            cpu.Registers["$k0"] = 0;

            return v1v2[1];
        }
 
        public  int yield() { return 0; }
        public  int uptime() { return 0; }
         
    }
}
