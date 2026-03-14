

using MIPS.Net.IO;
using MIPS.Net.SoC;
using MIPS.SDK;

namespace AbiTest
{

    public class Program
    {
        [STAThread()]
        public static void Main(string[] args)
        {
            MotherBoard mb = new MotherBoard(10000000,
                new List<IOPort>()
                {
                    new USBPort(){ ID = 0x01, Class = DeviceClass.STORAGE },
                    new USBPort(){ ID = 0x02, Class = DeviceClass.DISPLAY },
                    new USBPort(){ ID = 0x03, Class = DeviceClass.KEYBOARD },
                    new USBPort(){ ID = 0x04, Class = DeviceClass.MOUSE },
                    new USBPort(){ ID = 0x05, Class = DeviceClass.NETWORK },
                    new USBPort(){ ID = 0x06, Class = DeviceClass.PRINTER }
                },
                new List<IHardwareButton>()
                {
                    new HwButton(){ ID = 0x01 },
                    new HwButton(){ ID = 0x02 },
                    new HwButton(){ ID = 0x03 },
                    new HwButton(){ ID = 0x04 },

                });

            MIPSMachineDebugger machine = new MIPSMachineDebugger();
            machine.DeployAndDebug(mb, typeof(Program).Assembly, firmware: @"C:\mOS\firmware\firmware.mex");
        }
    }
}
