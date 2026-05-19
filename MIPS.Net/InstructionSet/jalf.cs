using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;
using MIPS.Net.SoC.__program;

namespace MIPS.Net.InstructionSet
{
    public class jalf
    {
        // Salta para o endereço especificado e salva o endereço da próxima instrução em $ra.
        public static void __call(int instruction, ref Registers registers)
        {
            if (!MIPS_CPU.Instance.FFIEnabled)
            {
                // provocará um halt
                // no futuro quando houver um kernel/SO,
                // isso seria interpretado por ele como um reset
                registers["$pc"] = 0;
                return;
            }
            // Extrai o endereço de destino (target) da instrução
            int target = instruction & 0x03FFFFFF; // 26 bits menos significativos

            var program = MIPS_CPU.CurrentProgram;
            FfiMethod mtd = program.LinkedAssemblies.Item2[target];
            FfiAssembly asm = program.LinkedAssemblies.Item1[mtd.AssemblyIndex];

            int current_addr = registers["$pc"];


            string[] call = mtd.CallingName.Split(':');
            string classNm = call[0];
            string methodNm = call[1];



            object? obj = MIPS_CPU.CurrentProgram.Get_FFI_Obj(asm.Name, classNm);
            if (obj == null)
            {
                Type? tp = asm.Asm.GetTypes().FirstOrDefault(t => t.Name == classNm);
                if (tp == null) throw new Exception();
                obj = Activator.CreateInstance(tp);
                MIPS_CPU.CurrentProgram.Set_FFI_Obj(asm.Name, classNm, obj);
            }

            bool insideInterruption = MIPS_CPU.Instance.IsInterrupted();

            MethodInfo? method = obj.GetType()?.GetMethod(methodNm);
            if (method == null) throw new Exception($"FFI method '{methodNm}' not found in '{classNm}'");


            List<object> args = new List<object>();
            for (int p = 0; p < method.GetParameters().Length; p++)
            {
                ParameterInfo pi = method.GetParameters()[p];
                if (pi.ParameterType != typeof(int)) throw new Exception("Only Int32 parameters are suported");
                string name = pi.Name;
                args.Add(registers[$"${name}"]);
            }

            if (insideInterruption)
            {
                try
                {

                    object? resVal = method.Invoke(obj, args.ToArray());
                    if (resVal != null)
                    {
                        if (resVal is int)
                            MIPS_CPU.Instance.Registers[$"$v1"] = (int)resVal;
                    }
                    return;
                }
                catch (Exception ex)
                {

                    return;
                }
            }


            Task.Run(() =>
            {
                try
                {


                    object? resVal = obj.GetType()?.GetMethod(methodNm)?.Invoke(obj, args.ToArray());

                    if (resVal != null)
                    {
                        //      if (resVal is int)
                        //            MIPS_CPU.Instance.Registers[$"$v0"] = (int)resVal;
                    }
//
                //    Thread.Sleep(100);
                    if (MIPS_CPU.Instance.IsInterrupted() == false)
                    {
                        while (MIPS_CPU.InLock())
                            Thread.Sleep(100);

                        MIPS_CPU.RequestLock();
                        MIPS_CPU.Instance.Registers[$"$pc"] = current_addr + 4;
                        MIPS_CPU.Release();
                    }
                }
                catch (Exception ex)
                {
                    MIPS_CPU.Instance.Registers[$"$pc"] = 0;// current_addr + 4;
                }
            });

            registers["$pc"] = MIPS_CPU.Instance.FFIWaitUpAddr - 4;
        }

        public static byte[] __parse(string instruction, Registers registers)
        {
            // 1. Remover espaços em branco e quebrar a string em partes
            string[] parts = instruction.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 2. Verificar se a instrução é do tipo "jal"
            if (parts[0] != "jalf")
            {
                throw new ArgumentException("Invalid instructioj. Expected 'jalf'.");
            }

            // 3. Extrair o endereço de destino (target)
            int target = int.Parse(parts[1]); // Assumindo que o endereço é um inteiro

            // 4. Construir o valor da instrução MIPS (formato J-Type)
            int instructionValue = 0;
            int OPCODE_JALF = 0x3F;

            instructionValue |= (OPCODE_JALF & 0x3F) << 26;
            instructionValue |= target & 0x03FFFFFF;

            // 5. Retornar o valor da instrução
            return BitConverter.GetBytes(instructionValue);
        }
    }
}
