using MIPS.Compiler;
using MIPS.Net.SoC;
using MIPS.Net.SoC.__program;

namespace MIPS.Tests
{
    public class DispositivoGenerico 
    {

        public DispositivoGenerico()
        {
            // _buffer = program;

            RegistersBufferLen = 2;
            DataBufferLen = 2;

        }

        public void PressHardwareButton()
        {
            _bus.SendBus('I', _addr, null, (KeyValuePair<bool, byte[]> interrupt_ret) =>
            {
                if (interrupt_ret.Key == true)
                {
                    _bus.SendBus('W', _addr, _buffer, (KeyValuePair<bool, byte[]> wrt_ret) =>
                    {
                        if (wrt_ret.Key == true)
                        {
                            // data send ok
                        }
                        return 0;
                    });
                }
                return 0;
            });
        }

        public int RegistersBufferLen { get; private set; }
        public int DataBufferLen { get; private set; }

        private int _addr;
        private MemoryBUS _bus;
        public void BusConnected(int addr, MemoryBUS bus)
        {
            _addr = addr;
            _bus = bus;
        }

        public int GetMemAddress()
        {
            return _addr;
        }

        private byte[] _buffer = new byte[2];

        public void ReadRequest(byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            Console.Write($"*** MSG DISPOSITIVO: LENDO MEMORIA INTERNA DO DISPOSITIVO ***");
            callBack(new KeyValuePair<bool, byte[]>(true, _buffer));
        }

        public void WriteRequest(byte[] data, byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            int s = deviceMemory[0];
            int count = deviceMemory[1];

            for (int i = s; i < count; i++)
            {
                Console.Write($"*** MSG DISPOSITIVO: {i + 1} ***");
            }
            byte[] store_res = new byte[1] { (byte)(data[0] + count) };
            Console.Write($"*** MSG DISPOSITIVO: GRAVAR {store_res[0]} NA MEMORIA INTERNA DO DISPOSITIVO ***");

            Array.Copy(store_res, 0, _buffer, 0, store_res.Length);
            callBack(new KeyValuePair<bool, byte[]>(true, null));
        }
    }


    [TestClass]
    public class MotherBoard_BUS_Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            /*
            string asm = @"
addi $sp, $sp, 15
addi $t0, $t0, 0
sb $t0, 0($sp)
addi $t1, $t1, 10
sb $t1, 1($sp)
addi $t2, $t2, 20
sb $t2, 2($sp)
lb $v0, 2($sp)";
             
           // byte[] program = MIPSCompiler.CompileProgram(asm);

            DispositivoGenerico hwGenerico = new DispositivoGenerico();

            MotherBoard mb = new MotherBoard();
            mb.ConnectDevice(address: 15, dev: hwGenerico);
            mb.TurnOn();

            byte[] programFile = File.ReadAllBytes(@"C:\Users\marco\Área de Trabalho\MIPS\firmware.mex");
            DMA.StoreData(1000, ref programFile);

            ProgramContext ctx = new ProgramContext();
            ctx.Load(1000);

            //mb.CPU.Process(program);

            int val_v0 = mb.CPU.Registers["$v0"];
            Assert.IsTrue(val_v0 == 30);
            return;
            */
        }


    }
}