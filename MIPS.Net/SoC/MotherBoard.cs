using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.Compiler;
using MIPS.Net.Debugger;

namespace MIPS.Net.SoC
{
    public class MotherBoard
    {
        private readonly int ramSize;
        private readonly List<IOPort> externalPorts = new List<IOPort>();
        private readonly List<IHardwareButton> hardwareButtons;
        private MemBusSignalCapture? mem_bus_cap;

        internal MIPS_CPU CPU { get; private set; }
        public Memory Memory { get; private set; }
        internal DMA DMA { get; private set; }

        private IOBusSignalCapture? io_bus_cap;

        public MemoryBUS MemBUS { get; private set; }
        public IOBUS IOBus { get; private set; }
        public InterruptionController IntrController { get; private set; }

        public IReadOnlyCollection<IOPort> Ports => externalPorts?.AsReadOnly();
        public IReadOnlyCollection<IHardwareButton> Buttons => hardwareButtons?.AsReadOnly();

        public static MotherBoard Instance { get; private set; }
        public bool IsOn { get; private set; }
        public double CPUFrequency => MIPS_CPU.Frequency;

        public MotherBoard(int ramSize,
            List<IOPort> externalPorts = null,
            List<IHardwareButton> buttons = null)
        {
            Instance = this;
            MemBUS = new MemoryBUS();
            IOBus = new IOBUS();

            this.ramSize = ramSize;
            this.externalPorts = externalPorts;
            this.hardwareButtons = buttons;
        }

        public void TurnOn(byte[] firmware, DebuggerBridge? debugger = null, IOBusSignalCapture? iobusCap = null,
            MemBusSignalCapture? memBusCap = null)
        {
            CPU = new MIPS_CPU(debugger);
            IntrController = new InterruptionController(CPU.Registers);
            Memory = new Memory(ramSize);
            DMA = new DMA(Memory);

            this.io_bus_cap = iobusCap;
            this.mem_bus_cap = memBusCap;

            IOBus.OnSignalReceived += IOBus_OnSignalReceived;
       //     IOBus.OnStatusChanged += IOBus_OnStatusChanged;
            MemBUS.OnSignalReceived += SystemBUS_OnSignalReceived;

            IsOn = true;

            int addr = Memory.Size - firmware.Length;
            DMA.StoreData(addr, ref firmware);

            CPU.Registers["$pc"] = addr;
            CPU.RunClock();
        }

        private void IOBus_OnStatusChanged(IOBUS bus)
        {
            io_bus_cap?.OnIOBus(IOBus.AddressBus, new byte[0], IOBus.WriteSignal, IOBus.ReadSignal, IOBus.InterruptionSignal);
        }

        public static void InitializePort(byte deviceID, int port, int deviceAddr)
        {
            IOPort? io_port = Instance.externalPorts.FirstOrDefault(p => p.ID == deviceID);
            if (io_port == null) return;
            io_port.DevicePort = port;

            InterruptionEntry[] port_interruptions = Instance.IntrController.GetInterruptions()
                .Where(i => i.DeviceMemoryAddress == deviceAddr)
                .ToArray();

            Instance.IOBus.MapPort(io_port, deviceAddr);
            io_port.BusConnected(deviceAddr, Instance.IOBus, port_interruptions);
            io_port.TurnOn();
        }

        internal static void InitializeHardwareButton(byte devID, int interruptionCodeClick)
        {
            IHardwareButton? hwBtn = Instance.hardwareButtons.FirstOrDefault(b => b.ID == devID);
            if (hwBtn == null) return;
            hwBtn.InterruptionCodeClick = interruptionCodeClick;

            hwBtn.BusConnected(Instance.IOBus);
            hwBtn.TurnOn();
        }
        private void IOBus_OnSignalReceived(Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            if (IOBus.InterruptionSignal)
            {
                io_bus_cap?.OnIOBus(IOBus.AddressBus, new byte[0], IOBus.WriteSignal, IOBus.ReadSignal, IOBus.InterruptionSignal);
                MIPS_CPU.Instance.DBG?.IOAccess(IOBus.AddressBus);
                if (IOBus.DataBus.Length == 0)
                    callBack(new KeyValuePair<bool, byte[]>(false, new byte[0]));
                else
                    IntrController.CallInterruption(BitConverter.ToInt32(IOBus.DataBus), callBack);
            }
        }

        private void SystemBUS_OnSignalReceived(Func<KeyValuePair<bool, byte[]>, int> callBack = null)
        {
            MIPS_CPU.Instance.DBG?.MemAccess(MemBUS.AddressBus);
            if (MemBUS.ReadSignal)
            {
                mem_bus_cap?.OnMemBus(MemBUS.AddressBus, MemBUS.DataBus, MemBUS.WriteSignal, MemBUS.ReadSignal, MemBUS.InterruptionSignal);
                HandleReadRequest(callBack);
                return;
            }

            if (MemBUS.WriteSignal)
            {
                mem_bus_cap?.OnMemBus(MemBUS.AddressBus, MemBUS.DataBus, MemBUS.WriteSignal, MemBUS.ReadSignal, MemBUS.InterruptionSignal);
                HandleWriteRequest(callBack);
                return;
            }

            if (MemBUS.InterruptionSignal)
            {
                mem_bus_cap?.OnMemBus(MemBUS.AddressBus, MemBUS.DataBus, MemBUS.WriteSignal, MemBUS.ReadSignal, MemBUS.InterruptionSignal);
                int intrCode = MemBUS.AddressBus;
                IntrController.CallInterruption(intrCode, callBack);
                return;
            }

          
        }

        private void HandleWriteRequest(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            try
            {
                byte[] b = MemBUS.DataBus; // buffer-size is defined in databus lines
                DMA.StoreData(MemBUS.AddressBus, ref b);
                callBack(new KeyValuePair<bool, byte[]>(true, b));
            }
            catch (Exception ex)
            {
                callBack(new KeyValuePair<bool, byte[]>(false, null));
            }
        }

        private void HandleReadRequest(Func<KeyValuePair<bool, byte[]>, int> callBack)
        {
            try
            {
                byte[] b = new byte[MemBUS.DataBus.Length]; // buffer-size is defined in databus lines

                DMA.RequestData(MemBUS.AddressBus, ref b);
                callBack(new KeyValuePair<bool, byte[]>(true, b));
            }
            catch (Exception ex)
            {
                callBack(new KeyValuePair<bool, byte[]>(false, null));
            }
        }

        public delegate void SysHalt();
        public event SysHalt OnSysHalted;

        internal static void OnCPUHalted()
        {
            /*
            foreach (var dev in Instance.externalPorts)
            {
                InterruptionEntry ie = InterruptionController.Instance.GetInterruption(dev.INTR_DEV_DISCONNECT);

                if (ie == null) continue;
                if (ie.HandlerAddress == 0) continue;

                byte[] ins = new byte[4];

                int addr = ie.HandlerAddress;

                if (!Instance.CPU.IsInterrupted())
                    Instance.CPU.SendInterruption(ie);
                
                while (true)
                {
                    DMA.RequestData(addr, ref ins);
                    if (ins.Sum(i => i) == 0)
                        break;

                    Instance.CPU.Registers["$a0"] = dev.INTR_DEV_DISCONNECT;
                    Instance.CPU.Registers["$a1"] = dev.GetMemAddress();

                    AutoResetEvent are = new AutoResetEvent(false);
                    Instance.CPU.Process(ins);

                    Task.Run(() =>
                    {
                        Thread.Sleep(300);
                        are.Set();

                    });

                    are.WaitOne();
                    addr += 4;
                }
                
            }
            */

            Instance.IOBus.OnSignalReceived -= Instance.IOBus_OnSignalReceived;
            Instance.IOBus.OnStatusChanged -= Instance.IOBus_OnStatusChanged;
            Instance.MemBUS.OnSignalReceived -= Instance.SystemBUS_OnSignalReceived;

            Instance.externalPorts.ForEach(p => p.TurnOff());
            Instance.hardwareButtons.ForEach(p => p.TurnOff());

            Instance.IOBus.Reset();
            Instance.MemBUS.Reset();

            Instance.Memory.Reset();

            Instance.OnSysHalted?.Invoke();
            Instance.IsOn = false;

            //    MIPS_CPU.Instance.Registers.Reset();
        }

        public void TurnOff()
        {

            CPU.RequestHalt();
        }

        public void BusNotifications(bool inst_enabled)
        {
            MIPS_CPU.BUS_NOTIFICATION(inst_enabled);
        }
    }
}
