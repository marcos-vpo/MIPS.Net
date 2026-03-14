
using System.Collections;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using MIPS.Abi;

namespace MIPS.Compiler
{
    internal class Program
    {
        static void Main(string[] args)
        { 
          if (args[0] == "-c")
                ExecCompile(args);
        }

        static List<string> exportedMethods = new List<string>();


        private static void ExecCompile(string[] args)
        {
            string fName = args[1];


            List<string> lines = File.ReadAllLines(fName).ToList();
            string plainTxt = File.ReadAllText(fName);

            if (plainTxt.Contains(".file"))
            {

                lines = File.ReadAllLines(fName).ToList();
                plainTxt = File.ReadAllText(fName);
            }



            Dictionary<int, string> data_entries = new Dictionary<int, string>();
            Dictionary<int, string> rotules = new Dictionary<int, string>();

            string linksStr = "";
            string dataSec = "";
            string textSec = "";

            string[] parts = plainTxt.Split(".text");
            dataSec = parts[0].Replace(".data", "");
            textSec = parts[1];

            foreach (string ln in lines)
            {
                if (ln.StartsWith(".link")) linksStr += $"{ln.Trim()}\n";
                else if (ln.StartsWith(".include"))
                {
                    string fImportName = ln.Split(' ')[1].Replace("\"", "");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(".include ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(fImportName);

                    var txtImp = File.ReadAllText(fImportName);


                    string[] divSections = txtImp.Split(".text");
                    string importData = divSections[0].Replace(".data", "");

                    dataSec += importData;
                    textSec += divSections[1];

                }
            }

            string sourceTxt = $@"
.data
{dataSec}
.text
{textSec}".Replace(".include", "#.include");

            lines = sourceTxt.Replace("\n", "").Split("\r").ToList();
            //pré-map rotules
            bool text = false;
            bool data = false;
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;
                if (string.IsNullOrEmpty(line)) continue;
                string ln = line.Trim();
                if (ln.StartsWith("#"))
                    continue;
                if (ln.Contains("#"))
                {
                    ln = ln.Substring(0, ln.IndexOf("#") - 1).Trim();
                }
                if (ln.StartsWith(".text")) { data = false; text = true; }
                if (ln.StartsWith(".data")) { data = true; text = false; }


                if (ln.Contains(":") && !ln.StartsWith("jal ") && !ln.StartsWith("jr ") && !ln.StartsWith("j "))
                {
                    if (text) rotules.Add(rotules.Count, ln.Split(':')[0]);
                    //         if (data) data_entries.Add(data_entries.Count, ln.Split(':')[0]);
                }

            }

            int removed = lines.RemoveAll(l => string.IsNullOrEmpty(l.Trim()) || l.TrimEnd().EndsWith('\n'));


            MemoryStream ms_data = new MemoryStream();
            MemoryStream ms_rotules = new MemoryStream();
            MemoryStream ms_text = new MemoryStream();
            MemoryStream ms_dotnet = new MemoryStream();

            int rotuleN = 0;
            int rotuleLen = 0;
            using (MemoryStream ms_file = new MemoryStream())
            {
                // magical byte (program-identifier)
                ms_file.Write(new byte[1] { 0xD7 });

                ms_file.Write(new byte[1] { 1 });// version
                ms_file.Write(new byte[4] { 0, 0, 0, 0 });     // total program length
                ms_file.Write(BitConverter.GetBytes((int)0));// .data size
                bool _data = false;
                bool _text = false;

                DebuggerTracing dt = new DebuggerTracing();
                int lnnb = 0;
                foreach (string line in lines)
                {
                    lnnb += 1;
                    try
                    {
                        string ln = line.Trim();
                        if (ln.StartsWith("#"))
                            continue;

                        if (ln.Contains("#"))
                        {
                            ln = ln.Substring(0, ln.IndexOf("#") - 1).Trim();
                        }
                        if (ln.StartsWith(".data"))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(".data");
                            Console.ForegroundColor = ConsoleColor.White;

                            if (_text == true)
                                _text = false;
                            _data = true;
                            continue;
                        }
                        if (ln.StartsWith(".text"))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(".text");
                            Console.ForegroundColor = ConsoleColor.White;

                            if (_data == true)
                                _data = false;
                            _text = true;
                            continue;
                        }

                        if (_data)
                        {
                            if (ln.StartsWith(".link"))
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine(".link");
                                Console.ForegroundColor = ConsoleColor.White;

                                string dotnetAsm = ln.Replace(".link ", "").Trim().Replace("\"", "");
                                if (!File.Exists(dotnetAsm)) throw new Exception($"ln {lnnb}, {ln} \n Linked assembly '{dotnetAsm}' not found.");



                                MemoryStream ms_netasm_map = new MemoryStream();
                                ms_netasm_map.Write(new byte[] { 0xD9 }); // inicio de tabela de simbolos
                         
                                byte[] dotnetBytes = File.ReadAllBytes(dotnetAsm);
                                List<long> addressesToMap = new List<long>();

                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write($"[linker] ");
                                Console.ForegroundColor = ConsoleColor.White;


                                using (MemoryStream msAsmNet = new MemoryStream(dotnetBytes))
                                {
                                    AssemblyLoadContext alc = new AssemblyLoadContext("compiler_tmp_ctx", true);
                                    Assembly asmNet = alc.LoadFromStream(msAsmNet);
                                    Console.WriteLine($"{dotnetAsm} ({asmNet.FullName}), {dotnetBytes.Length} bytes");

                                    Type abiManagedType = typeof(ABIManaged);

                                    foreach (Type type in asmNet.GetTypes())
                                    {
                                        // Filtrar apenas classes que herdam de ABIManaged (exceto a própria ABIManaged, se existir no assembly)
                                //        if (!abiManagedType.IsAssignableFrom(type) || type == abiManagedType)
                                     //       continue;

                                        foreach (MethodInfo method in type.GetMethods(
                                                     BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Static |
                                                     BindingFlags.Instance))
                                        {
                                            // Pegar somente métodos anotados com [Extern]
                                            var attr = method.GetCustomAttribute<Extern>();
                                            if (attr != null)
                                            {
                                                string call_name = $"{type.Name}:{method.Name}";
                                                uint hash = Utils.Fnv1a(call_name);
                                                byte[] call_hash_b = BitConverter.GetBytes((int)hash);
                                                exportedMethods.Add(call_name);

                                                Console.Write($"    {call_name}".PadRight(40, ' '));

                                                string result = string.Join(" ", call_hash_b.Select(b => $"0x{b:X2}"));

                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.Write("[ ");

                                                Console.ForegroundColor = ConsoleColor.Blue;
                                                Console.Write($"{result}");
                                                Console.ForegroundColor = ConsoleColor.White;
                                                Console.Write(" ] ");

                                                Console.ForegroundColor = ConsoleColor.Magenta;
                                                Console.WriteLine($"-> {(hash.ToString().PadRight(12, ' '))}");
                                                Console.ForegroundColor = ConsoleColor.White;
                                                // hash da chamada

                                                ms_netasm_map.Write(call_hash_b);
                                                addressesToMap.Add(ms_netasm_map.Position);
                                                ms_netasm_map.Write(new byte[4] { 0, 0, 0, 0 });              // endereco do netasm 
                                                ms_netasm_map.Write(Encoding.UTF8.GetBytes(call_name)); // nome da chamada
                                                ms_netasm_map.Write(new byte[1] { 0 });// terminação do nome da chamada
                                            }
                                        }
                                    }

                                    alc.Unload();
                                }

                                ms_netasm_map.Write(new byte[] { 0xD8 }); // fim da tab de simbolos

                                byte[] sizeB = BitConverter.GetBytes(dotnetBytes.Length);
                                ms_netasm_map.Write(sizeB);  // tamanho do assembly
                                ms_netasm_map.Write(dotnetBytes);   // assembly dotnet


                                ms_dotnet.Write(ms_netasm_map.ToArray());
                            }
                            else
                            {
                                string[] variable = ln.Split(':');
                                string name = variable[0];
                                if (string.IsNullOrEmpty(name)) continue;

                                string[] definition = variable[1].Trim().Split(' ');
                                string type = definition[0];

                                var nme = $"    {name}: {type} ".PadRight(40, ' ');
                                Console.Write(nme);

                                dt.AddDataEntry(index: data_entries.Count, declaration: nme, ln: lnnb);

                                if (type == ".byte") ByteData(ms_data, definition);
                                else if (type == ".half") HalfData(ms_data, definition);
                                else if (type == ".word") WordData(ms_data, definition);
                                else if (type == ".asciiz") AsciizData(ms_data, definition);
                                else if (type == ".space") SpaceData(ms_data, name, definition);
                                else throw new Exception($"Unknown type '{type}'");

                                int totalDataLen = (int)ms_data.Length;

                                data_entries.Add(data_entries.Count, name);

                                ms_file.Position -= 4;
                                ms_file.Write(BitConverter.GetBytes(totalDataLen));

                                Console.WriteLine("");
                            }
                        }

                        if (_text)
                        {
                            if (ln.StartsWith(".")) continue;

                            if (ln.Contains(":") && !ln.StartsWith("jal"))
                            {
                                if(ln.Contains("_sys_program_exit"))
                                {

                                }
                                if ((ms_text.Length > 0))
                                {
                                    // rotule end (4b)
                                    /*
                                    rotuleLen += 4;
                                    ms_rotules.Position -= 4;
                                    ms_rotules.Write(BitConverter.GetBytes(rotuleLen));
                                    */

                                    ms_text.Write(BitConverter.GetBytes((int)0));
                                    rotuleLen = 0;
                                }

                                ms_rotules.Write(BitConverter.GetBytes(rotuleN));
                                ms_rotules.Write(BitConverter.GetBytes((int)ms_text.Position));
                                ms_rotules.Write(BitConverter.GetBytes(0)); // rotule length

                                //      string[] rotuleX = ln.Split(':');
                                //        rotules.Add(rotules.Count, rotuleX[0]);

                                dt.AddRotule(rotuleN, name: ln, ln: lnnb);

                                rotuleN += 1;

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[{rotuleN - 1}] {ln}");
                                Console.ForegroundColor = ConsoleColor.White;
                                continue;
                            }

                            if (ln == "") continue;
                            if (ln.StartsWith("jal") && ln.Contains(":")) // ffi calling
                            {
                                string ffiName = ln.Replace("jal .", "");
                                bool reached = false;
                                for (int ffi = 0; ffi < exportedMethods.Count; ffi++)
                                {
                                    if (exportedMethods[ffi] == ffiName)
                                    {
                                        ln = $"jalf {ffi}";
                                        reached = true;
                                        break;
                                    }
                                }

                                if (!reached) throw new Exception($"Unknown foreign function '{ffiName}' in any linked assemblies.");
                            }

                            string oriLn = ln;
                            Console.Write($"    {ln} ".PadRight(40, ' '));
                            foreach (KeyValuePair<int, string> d in data_entries)
                            {
                                if (ln.Split(' ').Contains(d.Value))
                                    ln = ln.Replace(d.Value, d.Key.ToString());
                            }

                            foreach (KeyValuePair<int, string> r in rotules)
                            {
                                if (ln.Split(' ').Contains($".{r.Value}"))
                                {
                                    ln = ln.Replace($".{r.Value}", r.Key.ToString());

                                    if (ln.StartsWith("la"))
                                        ln = ln.Replace("la", "lar");
                                }
                            }


                            byte[] inst_bytes = MIPSCompiler.CompileInstructions(ln);
                            ms_text.Write(inst_bytes);

                            rotuleLen += 4;
                            ms_rotules.Position -= 4;
                            ms_rotules.Write(BitConverter.GetBytes(rotuleLen));


                            dt.AddInstruction(code: BitConverter.ToInt32(inst_bytes), definition: oriLn, hex: string.Join(" ", inst_bytes.Select(b => $"0x{b:X2}")), ln: lnnb);

                            string result = string.Join(" ", inst_bytes.Select(b => $"0x{b:X2}"));

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($"[ ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{result}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write($" ] ");
                            Console.WriteLine($" -> {(BitConverter.ToInt32(inst_bytes).ToString().PadRight(12, ' '))}");

                        }
                    }
                    catch (Exception ex)
                    {
                        var fi = new FileInfo(fName);

                        var exec = fi.Name + ".mex";
                        if (File.Exists(exec))
                            File.Delete(exec);

                        Console.WriteLine($"Compile Error: {ex.Message} -- at line {lnnb} '{line}'");
                        Environment.Exit(0);
                    }
                }

                rotuleLen += 4;
                ms_rotules.Position -= 4;
                ms_rotules.Write(BitConverter.GetBytes(rotuleLen));
                ms_text.Write(BitConverter.GetBytes((int)0));
                
                int filepos = (int)ms_file.Position;

                ms_file.Write(ms_data.ToArray());

                ms_file.Write(BitConverter.GetBytes((int)ms_rotules.Length));
                ms_file.Write(ms_rotules.ToArray());

                ms_file.Write(BitConverter.GetBytes((int)ms_text.Length));
                ms_file.Write(ms_text.ToArray());


                ms_file.Write(ms_dotnet.ToArray());
                ms_file.Write(new byte[1] { 0xDF });
                // ms_file.Write(new byte[256]);

                ms_file.Position = 2;
                ms_file.Write(BitConverter.GetBytes((int)ms_file.Length - 6));
                ms_file.Position = filepos;

                string exeFName = (new FileInfo(fName).Name + ".mex").Replace(".asm", "");
                using (FileStream fs = new FileStream(exeFName, FileMode.OpenOrCreate))
                {
                    fs.Write(ms_file.ToArray());
                }



                if (args.Contains("-s")) File.WriteAllLines($"{fName}_full.asm", lines);
                if (args.Contains("-d"))
                {
                    string fl = Path.Combine(Directory.GetCurrentDirectory(), $"{fName}_full.asm");
                    dt.SourceFile = fl;
                    File.WriteAllLines($"{fName}_full.asm", lines);
                    File.WriteAllText($"{fName}_full.mdbx", System.Text.Json.JsonSerializer.Serialize(dt));
                }
            }
        }

        private static void SpaceData(MemoryStream ms_data, string name, string[] definition)
        {
            if (name.Length > 20) throw new Exception($"[.data] invalid declaration: '{name}'; cannot exceed maximum 20 characters");
            int bSize = 0;
            int.TryParse(definition[1], out bSize);

            byte[] b_name = new byte[20];
            for (int i = 0; i < name.Length; i++)
            {
                b_name[i] = (byte)name[i];
                if (i == 20) break;
            }

     

            short typeIdentifier = 6;
            byte[] b_ident = BitConverter.GetBytes(typeIdentifier);
            ms_data.Write(b_ident); // 2b

       //     ms_data.Write(b_name);

            byte[] b_space = new byte[bSize];
            int len = b_space.Length;
            byte[] b_len = BitConverter.GetBytes(len);
            ms_data.Write(b_len); //4b

            ms_data.Write(b_space);


            Console.Write($"[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{len} bytes");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ]");
        }

        private static void AsciizData(MemoryStream ms_data, string[] definition)
        {
            string var_str = "";
            for (int i = 1; i < definition.Length; i++)
            {
                if (i == (definition.Length - 1))
                    var_str += definition[i];
                else
                    var_str += $"{definition[i]} ";
            }

            var_str = var_str.Replace("\"", "").Trim();

            short typeIdentifier = 5;
            byte[] b_ident = BitConverter.GetBytes(typeIdentifier);
            ms_data.Write(b_ident); // 2b

            byte[] b_string = Encoding.ASCII.GetBytes(var_str);
            int len = b_string.Length + 1;
            byte[] b_len = BitConverter.GetBytes(len);
            ms_data.Write(b_len); //4b


            ms_data.Write(b_string);
            ms_data.Write(new byte[1] { (byte)'\0' }, 0, 1);

            string result = string.Join(" ", b_string.Select(b => $"0x{b:X2}"));
            Console.Write($"[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(result);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ]");
        }

        private static void WordData(MemoryStream ms_data, string[] definition)
        {
            int var_int = Convert.ToInt32(definition[1]);
            short typeIdentifier = 4;
            byte[] b_ident = BitConverter.GetBytes(typeIdentifier);
            ms_data.Write(b_ident); // 2b

            int len = 4;
            byte[] b_len = BitConverter.GetBytes(len);
            ms_data.Write(b_len); //4b

            byte[] b_int = BitConverter.GetBytes(var_int);
            ms_data.Write(b_int);

            string result = string.Join(" ", b_int.Select(b => $"0x{b:X2}"));
            Console.Write($"[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(result);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ]");
        }

        private static void HalfData(MemoryStream ms_data, string[] definition)
        {
            short var_short = Convert.ToInt16(definition[1], 16);
            short typeIdentifier = 2;
            byte[] b_ident = BitConverter.GetBytes(typeIdentifier);
            ms_data.Write(b_ident); // 2b

            int len = 4;
            byte[] b_len = BitConverter.GetBytes(len);
            ms_data.Write(b_len); //4b

            byte[] b_short = BitConverter.GetBytes(var_short);
            ms_data.Write(b_short);

            string result = string.Join(" ", b_short.Select(b => $"0x{b:X2}"));
            Console.Write($"[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(result);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ]");
        }

        private static void ByteData(MemoryStream ms_data, string[] definition)
        {
            byte[] var_bytes = new byte[definition.Length - 1];
            for (int i = 0; i < var_bytes.Length; i++)
                var_bytes[i] = Convert.ToByte(definition[i + 1], 16);

            short typeIdentifier = 1;
            byte[] b_ident = BitConverter.GetBytes(typeIdentifier);
            ms_data.Write(b_ident); // 2b

            int len = var_bytes.Length;
            byte[] b_len = BitConverter.GetBytes(len);
            ms_data.Write(b_len); //n-b

            ms_data.Write(var_bytes);

            string result = string.Join(" ", var_bytes.Select(b => $"0x{b:X2}"));
            Console.Write($"[ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(result);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ]");
        }
    }
}
