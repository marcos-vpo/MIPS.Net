using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace MIPS.Net.SoC.__program
{
    public class ProgramReader : IDisposable
    {
        public int address = 0;

        public ProgramReader(int address)
        {
            this.address = address;
        }

        public void Dispose()
        {
        }

        internal DataEntry[] ReadDataEntries(int dataSectionSize)
        {
            List<DataEntry> res = new List<DataEntry>();

            int currentAddr = address;
            int index = 0;
            int readed = 0;

            while (readed < dataSectionSize)
            {
                byte[] data_type = new byte[2]; // short value

                DMA.RequestData(address, ref data_type);
                short dataType = BitConverter.ToInt16(data_type);
                address += 2;

                byte[] entry_len = new byte[4]; // int value
                DMA.RequestData(address, ref entry_len);
                int dataLen = BitConverter.ToInt32(entry_len);
                address += 4;

                byte[] entry_data = new byte[dataLen];
                DMA.RequestData(address, ref entry_data);

                DataEntry e = new DataEntry
                {
                    Address = currentAddr,
                    Index = index,
                    Type = (DataType)dataType,
                    Data = entry_data
                };

                res.Add(e);

                address += entry_data.Length;
                index += 1;
                currentAddr = address;

                readed += data_type.Length + entry_len.Length + entry_data.Length;
            }
            return res.ToArray();
        }

        internal int ReadDataSectionSize()
        {
            byte[] data_s = new byte[4];
            DMA.RequestData(address, ref data_s);
            address += data_s.Length;
            return BitConverter.ToInt32(data_s);
        }

        private Tuple<List<FfiAssembly>, List<FfiMethod>> ffi_linkeds;

        internal Tuple<List<FfiAssembly>, List<FfiMethod>> ReadLinkedAssemblies(int endFileOrStartDotNet)
        {
            if (ffi_linkeds != null) return ffi_linkeds;

            List<FfiAssembly> asms = new List<FfiAssembly>();
            List<FfiMethod> methods = new List<FfiMethod>();

            int addr = endFileOrStartDotNet; //address + rotules_size;
 

            byte[] data_s = new byte[1];
            DMA.RequestData(addr, ref data_s);
        
            byte terminator = 0;

            while (terminator != 0xD8)
            {
                addr = ReadFfiMethod(methods, addr);
                addr += 1; // mthd name terminator
                byte[] terminator_table = new byte[1];
                DMA.RequestData(addr, ref terminator_table);

                if (terminator_table[0] == 0xD8)
                {
                    try
                    {
                        addr += 1;

                        byte[] netAsmSizeB = new byte[4];
                        DMA.RequestData(addr, ref netAsmSizeB);
                        addr += 4;

                        int asmSize = BitConverter.ToInt32(netAsmSizeB);
                        byte[] netAsmFull = new byte[asmSize];
                        DMA.RequestData(addr, ref netAsmFull);
                        addr += asmSize;


                        Assembly asmNet = null;
                        MemoryStream msNetAsm = new MemoryStream(netAsmFull);
                        AssemblyLoadContext netCtx = new AssemblyLoadContext("pre-load", isCollectible: true);
                        asmNet = netCtx.LoadFromStream(msNetAsm);


                        FfiAssembly ffiAsm = null;
                        if (MIPS_CPU.FfiAtatchedDebug == null)
                        {

                            ffiAsm = new FfiAssembly() { Name = asmNet.GetName().Name, Asm = asmNet, Ctx = netCtx };
                        }
                        else// if (MIPS_CPU.FfiAtatchedDebug.FullName == asmNet.FullName)
                        {
                            if (asmNet.FullName == MIPS_CPU.FfiAtatchedDebug.FullName)
                            {
                                netCtx.Unload();
                                asmNet = MIPS_CPU.FfiAtatchedDebug;
                                ffiAsm = new FfiAssembly() { Name = asmNet.GetName().Name, Asm = asmNet };
                            }
                            else ffiAsm = new FfiAssembly() { Name = asmNet.GetName().Name, Asm = asmNet, Ctx = netCtx };
                        }

                        asms.Add(ffiAsm);
                        var nonLinkeds = methods.Where(m => m.Linked == false).ToList();
                        nonLinkeds.ForEach(m => m.AssemblyIndex = asms.Count - 1);
                        msNetAsm.Close();
                        msNetAsm.Dispose();

                        //         addr += 1;
                        byte[] program_terminator_b = new byte[1];
                        DMA.RequestData(addr, ref program_terminator_b);
                        if (program_terminator_b[0] == 0xDF)
                            break;
                        else addr -= 1;

                        addr += asmSize;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }


                }
                else addr -= 1;

                //     addr += 1;
            }


            ffi_linkeds = new Tuple<List<FfiAssembly>, List<FfiMethod>>(asms, methods);
            return ffi_linkeds;
        }

        private static int ReadFfiMethod(List<FfiMethod> methods, int addr)
        {




            byte[] call_hash_b = new byte[4];
            DMA.RequestData(addr, ref call_hash_b);
            addr += 4;

            byte[] asm_pos_b = new byte[4];
            DMA.RequestData(addr, ref asm_pos_b);
            addr += 4;

            List<byte> call_name_str = new List<byte>();

            addr += 1;
            byte[] name_b = new byte[1];
            DMA.RequestData(addr, ref name_b);
            call_name_str.Add(name_b[0]);
            addr += 1;

            while (name_b[0] != 0)
            {
                DMA.RequestData(addr, ref name_b);

                if (name_b[0] == 0)
                {
                    //       addr += 1;
                    break;
                }
                call_name_str.Add(name_b[0]);
                addr += 1;
            }

            int hash = BitConverter.ToInt32(call_hash_b);
            string name = Encoding.UTF8.GetString(call_name_str.ToArray());

            methods.Add(new FfiMethod() { CallingName = name });
            return addr;
        }

        internal RotuleMapping[] ReadRotulesMap(int rotuleIndexSize)
        {
            int initialAddr = address;
            List<RotuleMapping> res = new List<RotuleMapping>();
            int readed = 0;
            while (readed < rotuleIndexSize)
            {
                byte[] rotule_indx = new byte[4]; // short value
                DMA.RequestData(address, ref rotule_indx);
                int rotuleIndx = BitConverter.ToInt32(rotule_indx);
                address += 4;

                byte[] rotule_relative_addr = new byte[4]; // short value
                DMA.RequestData(address, ref rotule_relative_addr);
                int relativeAddr = BitConverter.ToInt32(rotule_relative_addr);
                address += 4;

                byte[] rotule_len = new byte[4]; // short value
                DMA.RequestData(address, ref rotule_len);
                int rotuleLen = BitConverter.ToInt32(rotule_len);
                address += 4;

                res.Add(new RotuleMapping
                {
                    Index = rotuleIndx,
                    RelativeAddr = relativeAddr,
                    AbsoluteAddr = (initialAddr + rotuleIndexSize + 4 + relativeAddr),
                    RotuleLength = rotuleLen
                });

                readed += 12;
            }
            return res.ToArray();
        }

        public int rotules_size = 0;

        internal int ReadRotulesSectionSize()
        {
            byte[] data_s = new byte[4];
            DMA.RequestData(address, ref data_s);
            address += data_s.Length;
            rotules_size = BitConverter.ToInt32(data_s);
            return BitConverter.ToInt32(data_s);
        }
    }
}
