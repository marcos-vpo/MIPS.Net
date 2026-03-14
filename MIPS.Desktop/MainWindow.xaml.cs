using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MIPS.Desktop.Utils;
using MIPS.Desktop.ViewModel;
using MIPS.Net.IO;
using MIPS.Net.SoC;

namespace MIPS.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IClockCapture
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (frequency > 1000) lbFreq.Content = $"{(frequency / 1000):N2}MHz";
                    else lbFreq.Content = $"{(frequency):N2}kHz";
                });
            }
            catch
            {

            }
        }

        private SystemVM? current_sys = null;
        private void OpenSystem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            MenuItem mi = (MenuItem)sender;
            current_sys = (SystemVM)mi.Tag;
            lbSystemName.Text = current_sys.Name;
            lbVendorName.Content = current_sys.Vendor;
            lbMemSize.Text = $"{current_sys.MemorySize}\nbytes";
            InitMotherB(current_sys);
        }
        MotherBoard mb = null;

        private void InitMotherB(SystemVM sysHw)
        {

            if (mb != null)
            {
                mb.OnSysHalted -= Mb_OnSysHalted;
                mb.MemBUS.OnStatusChanged -= MemBUS_OnStatusChanged;
                mb.IOBus.OnStatusChanged -= IOBus_OnStatusChanged;
            }
            var ports = new List<IOPort>();
            var buttons = new List<IHardwareButton>();
            panelPorts.Children.Clear();
            panelButtons.Children.Clear();

            sysHw.Ports.ForEach(p =>
            {
                if (p.Class == (byte)0x20)
                {
                    USBPort usb_port = new USBPort();
                    usb_port.Tag = p;
                    usb_port.ID = p.ID;
                    ports.Add(usb_port);

                    UIPort ui_port = new UIPort(usb_port);
                    panelPorts.Children.Add(ui_port);
                }
            });

            sysHw.Buttons.ForEach(p =>
            {
                HwButton btn = new HwButton();
                btn.Tag = p;
                btn.ID = p.ID;
                buttons.Add(btn);

                UIButtonHw uiBtn = new UIButtonHw(p.Text, btn);
                panelButtons.Children.Add(uiBtn);
            });

            mb = new MotherBoard(sysHw.MemorySize, ports, buttons);
            //    mb.ConnectDevice(address: 15, dev: hwGenerico);
            mb.OnSysHalted += Mb_OnSysHalted;
            mb.MemBUS.OnStatusChanged += MemBUS_OnStatusChanged;
            mb.IOBus.OnStatusChanged += IOBus_OnStatusChanged;
        }

        private void mnCreateDev_Click(object sender, RoutedEventArgs e)
        {
            new CreateDevice().ShowDialog();
        }

        private void mnCreteSystem_Click(object sender, RoutedEventArgs e)
        {
            new CreateSystem().ShowDialog();
        }

        private void mnOpenSys_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTurnOnOff_Click(object sender, RoutedEventArgs e)
        {
            if ("OFF".Equals(btnTurnOnOff.Content))
            {
                mb.TurnOff();
                btnTurnOnOff.Content = "ON";
                btnTurnOnOff.BorderBrush = Brushes.DarkRed;
                return;
            }
            btnTurnOnOff.BorderBrush = Brushes.MediumSeaGreen;
            btnTurnOnOff.Content = "OFF";

            //    

            if (current_sys == null) return;

            MIPS_CPU.Capture = this;

            string firmwarePath = System.IO.Path.Combine(current_sys.path, "firmware.mex");
            if (!File.Exists(firmwarePath))
            {
                MessageBox.Show($"Firmware executable not found. Compile your firmware in .asm to that \"firmware.mex\" and put it to '{current_sys.Name}' folder",
                    "Firmware not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            mb.TurnOn(null);

            byte[] programFile = File.ReadAllBytes(firmwarePath);
            int addr = current_sys.MemorySize - programFile.Length;
            DMA.StoreData(addr, ref programFile);

            mb.CPU.Registers["$pc"] = addr;

            mb.CPU.RunClock();
        }

        private void IOBus_OnStatusChanged(IOBUS bus)
        {
            if (!realTimeOpers) return;
            short[] io_addr_bus = IOBUS.AddressPins();
            short[] io_data_bus = IOBUS.DataPins();
            Application.Current.Dispatcher.Invoke(() =>
            {
                PaintIOAddr(io_addr_bus);
                PaintIOData(io_data_bus);

                if (current_sys.Buttons.Any(b => b.ID == (byte)bus.AddressBus))
                    PaintButtonsAddr(io_addr_bus);
                else
                    PaintButtonsAddr(new short[32]);

                if (bus.ReadSignal)
                {
                    lbOpIO.Text = $"▲ RD [{bus.AddressBus}]";
                    if (bus.InterruptionSignal) lbOpIO.Text += $" IR\n█\n*{bus.AddressBus}";
                }
                else if (bus.WriteSignal)
                {
                    lbOpIO.Text = $"▼ WT [{bus.AddressBus}]";
                    if (bus.InterruptionSignal) lbOpIO.Text += $" IR\n█\n*{bus.AddressBus}";
                }
                else if (bus.InterruptionSignal) lbOpIO.Text = $"IR\n█\n*{bus.AddressBus}";
                else lbOpIO.Text = "";
            });

            Thread.Sleep(25);
        }


        private void Mb_OnSysHalted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                frequency = 0;
                btnTurnOnOff.Content = "ON";
                btnTurnOnOff.BorderBrush = Brushes.DarkRed;
                elClock.Stroke = Brushes.Red;
                elClock.Fill = Brushes.Red;
                PaintMemData(new short[32]);
                PaintMemAddr(new short[32]);
            });
        }

        private void MemBUS_OnStatusChanged(MemoryBUS bus)
        {
            if (!realTimeOpers) return;
            if (Application.Current == null) return;

            short[] mem_addr_bus = MemoryBUS.AddressPins();
            short[] mem_data_bus = MemoryBUS.DataPins();
            Application.Current.Dispatcher.Invoke(() =>
            {
                PaintMemAddr(mem_addr_bus);
                PaintMemData(mem_data_bus);
                if (bus.ReadSignal)
                {
                    lbOpRAM.Text = $"RD\n◄\n*{bus.AddressBus}";
                    if (bus.InterruptionSignal) lbOpRAM.Text += $"\nIR\n█\n*{bus.AddressBus}";
                }
                else if (bus.WriteSignal)
                {
                    lbOpRAM.Text = $"WT\n►\n*{bus.AddressBus}";
                    if (bus.InterruptionSignal) lbOpRAM.Text += $"\nIR\n█\n*{bus.AddressBus}";
                }
                else if (bus.InterruptionSignal) lbOpRAM.Text = $"IR\n█\n*{bus.AddressBus}";
                else lbOpRAM.Text = "";
            });
        }

        private Brush cpuGree = (Brush)new BrushConverter().ConvertFrom("#FF7DFF00");
        private Brush lineActive = Brushes.Gold;
        private double frequency;
        public void MIPS_OnClock(short clock_status, double freq, int enrgLevel)
        {
            this.frequency = freq;
            if (!realTimeClock && !realTimeRegs) return;
            if (Application.Current == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                elClock.Fill = clock_status == 0 ? Brushes.White : cpuGree;
                elClock.Stroke = elClock.Fill;

                if (realTimeRegs)
                {
                    var vals = MotherBoard.Instance.CPU.Registers.GetValues();
                    lbRegs.Text = vals;

                }
            });

            Thread.Sleep(5);
        }
        private BrushConverter conv = new BrushConverter();
        private void PaintMemData(short[] mem_data_bus)
        {
            if (!realTimeBus) return;
            mb_left_d0.BorderBrush = (Brush)(mem_data_bus[0] == 0 ? Brushes.White : lineActive);
            mb_left_d1.BorderBrush = (Brush)(mem_data_bus[1] == 0 ? Brushes.White : lineActive);
            mb_left_d2.BorderBrush = (Brush)(mem_data_bus[2] == 0 ? Brushes.White : lineActive);
            mb_left_d3.BorderBrush = (Brush)(mem_data_bus[3] == 0 ? Brushes.White : lineActive);
            mb_left_d4.BorderBrush = (Brush)(mem_data_bus[4] == 0 ? Brushes.White : lineActive);
            mb_left_d5.BorderBrush = (Brush)(mem_data_bus[5] == 0 ? Brushes.White : lineActive);
            mb_left_d6.BorderBrush = (Brush)(mem_data_bus[6] == 0 ? Brushes.White : lineActive);
            mb_left_d7.BorderBrush = (Brush)(mem_data_bus[7] == 0 ? Brushes.White : lineActive);
            mb_left_d8.BorderBrush = (Brush)(mem_data_bus[8] == 0 ? Brushes.White : lineActive);
            mb_left_d9.BorderBrush = (Brush)(mem_data_bus[9] == 0 ? Brushes.White : lineActive);
            mb_left_d10.BorderBrush = (Brush)(mem_data_bus[10] == 0 ? Brushes.White : lineActive);
            mb_left_d11.BorderBrush = (Brush)(mem_data_bus[11] == 0 ? Brushes.White : lineActive);
            mb_left_d12.BorderBrush = (Brush)(mem_data_bus[12] == 0 ? Brushes.White : lineActive);
            mb_left_d13.BorderBrush = (Brush)(mem_data_bus[13] == 0 ? Brushes.White : lineActive);
            mb_left_d14.BorderBrush = (Brush)(mem_data_bus[14] == 0 ? Brushes.White : lineActive);
            mb_left_d15.BorderBrush = (Brush)(mem_data_bus[15] == 0 ? Brushes.White : lineActive);
            mb_left_d16.BorderBrush = (Brush)(mem_data_bus[16] == 0 ? Brushes.White : lineActive);
            mb_left_d17.BorderBrush = (Brush)(mem_data_bus[17] == 0 ? Brushes.White : lineActive);
            mb_left_d18.BorderBrush = (Brush)(mem_data_bus[18] == 0 ? Brushes.White : lineActive);
            mb_left_d19.BorderBrush = (Brush)(mem_data_bus[19] == 0 ? Brushes.White : lineActive);
            mb_left_d20.BorderBrush = (Brush)(mem_data_bus[20] == 0 ? Brushes.White : lineActive);
            mb_left_d21.BorderBrush = (Brush)(mem_data_bus[21] == 0 ? Brushes.White : lineActive);
            mb_left_d22.BorderBrush = (Brush)(mem_data_bus[22] == 0 ? Brushes.White : lineActive);
            mb_left_d23.BorderBrush = (Brush)(mem_data_bus[23] == 0 ? Brushes.White : lineActive);
            mb_left_d24.BorderBrush = (Brush)(mem_data_bus[24] == 0 ? Brushes.White : lineActive);
            mb_left_d25.BorderBrush = (Brush)(mem_data_bus[25] == 0 ? Brushes.White : lineActive);
            mb_left_d26.BorderBrush = (Brush)(mem_data_bus[26] == 0 ? Brushes.White : lineActive);
            mb_left_d27.BorderBrush = (Brush)(mem_data_bus[27] == 0 ? Brushes.White : lineActive);
            mb_left_d28.BorderBrush = (Brush)(mem_data_bus[28] == 0 ? Brushes.White : lineActive);
            mb_left_d29.BorderBrush = (Brush)(mem_data_bus[29] == 0 ? Brushes.White : lineActive);
            mb_left_d30.BorderBrush = (Brush)(mem_data_bus[30] == 0 ? Brushes.White : lineActive);
            mb_left_d31.BorderBrush = (Brush)(mem_data_bus[31] == 0 ? Brushes.White : lineActive);

            mb_right_d0.BorderBrush = (Brush)(mem_data_bus[0] == 0 ? Brushes.White : lineActive);
            mb_right_d1.BorderBrush = (Brush)(mem_data_bus[1] == 0 ? Brushes.White : lineActive);
            mb_right_d2.BorderBrush = (Brush)(mem_data_bus[2] == 0 ? Brushes.White : lineActive);
            mb_right_d3.BorderBrush = (Brush)(mem_data_bus[3] == 0 ? Brushes.White : lineActive);
            mb_right_d4.BorderBrush = (Brush)(mem_data_bus[4] == 0 ? Brushes.White : lineActive);
            mb_right_d5.BorderBrush = (Brush)(mem_data_bus[5] == 0 ? Brushes.White : lineActive);
            mb_right_d6.BorderBrush = (Brush)(mem_data_bus[6] == 0 ? Brushes.White : lineActive);
            mb_right_d7.BorderBrush = (Brush)(mem_data_bus[7] == 0 ? Brushes.White : lineActive);
            mb_right_d8.BorderBrush = (Brush)(mem_data_bus[8] == 0 ? Brushes.White : lineActive);
            mb_right_d9.BorderBrush = (Brush)(mem_data_bus[9] == 0 ? Brushes.White : lineActive);
            mb_right_d10.BorderBrush = (Brush)(mem_data_bus[10] == 0 ? Brushes.White : lineActive);
            mb_right_d11.BorderBrush = (Brush)(mem_data_bus[11] == 0 ? Brushes.White : lineActive);
            mb_right_d12.BorderBrush = (Brush)(mem_data_bus[12] == 0 ? Brushes.White : lineActive);
            mb_right_d13.BorderBrush = (Brush)(mem_data_bus[13] == 0 ? Brushes.White : lineActive);
            mb_right_d14.BorderBrush = (Brush)(mem_data_bus[14] == 0 ? Brushes.White : lineActive);
            mb_right_d15.BorderBrush = (Brush)(mem_data_bus[15] == 0 ? Brushes.White : lineActive);
            mb_right_d16.BorderBrush = (Brush)(mem_data_bus[16] == 0 ? Brushes.White : lineActive);
            mb_right_d17.BorderBrush = (Brush)(mem_data_bus[17] == 0 ? Brushes.White : lineActive);
            mb_right_d18.BorderBrush = (Brush)(mem_data_bus[18] == 0 ? Brushes.White : lineActive);
            mb_right_d19.BorderBrush = (Brush)(mem_data_bus[19] == 0 ? Brushes.White : lineActive);
            mb_right_d20.BorderBrush = (Brush)(mem_data_bus[20] == 0 ? Brushes.White : lineActive);
            mb_right_d21.BorderBrush = (Brush)(mem_data_bus[21] == 0 ? Brushes.White : lineActive);
            mb_right_d22.BorderBrush = (Brush)(mem_data_bus[22] == 0 ? Brushes.White : lineActive);
            mb_right_d23.BorderBrush = (Brush)(mem_data_bus[23] == 0 ? Brushes.White : lineActive);
            mb_right_d24.BorderBrush = (Brush)(mem_data_bus[24] == 0 ? Brushes.White : lineActive);
            mb_right_d25.BorderBrush = (Brush)(mem_data_bus[25] == 0 ? Brushes.White : lineActive);
            mb_right_d26.BorderBrush = (Brush)(mem_data_bus[26] == 0 ? Brushes.White : lineActive);
            mb_right_d27.BorderBrush = (Brush)(mem_data_bus[27] == 0 ? Brushes.White : lineActive);
            mb_right_d28.BorderBrush = (Brush)(mem_data_bus[28] == 0 ? Brushes.White : lineActive);
            mb_right_d29.BorderBrush = (Brush)(mem_data_bus[29] == 0 ? Brushes.White : lineActive);
            mb_right_d30.BorderBrush = (Brush)(mem_data_bus[30] == 0 ? Brushes.White : lineActive);
            mb_right_d31.BorderBrush = (Brush)(mem_data_bus[31] == 0 ? Brushes.White : lineActive);
        }

        private void PaintMemAddr(short[] mem_addr_bus)
        {
            if (!realTimeBus) return;
            mb_left_a1.BorderBrush = (Brush)(mem_addr_bus[1] == 0 ? Brushes.White : lineActive);
            mb_left_a2.BorderBrush = (Brush)(mem_addr_bus[2] == 0 ? Brushes.White : lineActive);
            mb_left_a3.BorderBrush = (Brush)(mem_addr_bus[3] == 0 ? Brushes.White : lineActive);
            mb_left_a4.BorderBrush = (Brush)(mem_addr_bus[4] == 0 ? Brushes.White : lineActive);
            mb_left_a5.BorderBrush = (Brush)(mem_addr_bus[5] == 0 ? Brushes.White : lineActive);
            mb_left_a6.BorderBrush = (Brush)(mem_addr_bus[6] == 0 ? Brushes.White : lineActive);
            mb_left_a7.BorderBrush = (Brush)(mem_addr_bus[7] == 0 ? Brushes.White : lineActive);
            mb_left_a8.BorderBrush = (Brush)(mem_addr_bus[8] == 0 ? Brushes.White : lineActive);
            mb_left_a9.BorderBrush = (Brush)(mem_addr_bus[9] == 0 ? Brushes.White : lineActive);
            mb_left_a10.BorderBrush = (Brush)(mem_addr_bus[10] == 0 ? Brushes.White : lineActive);
            mb_left_a11.BorderBrush = (Brush)(mem_addr_bus[11] == 0 ? Brushes.White : lineActive);
            mb_left_a12.BorderBrush = (Brush)(mem_addr_bus[12] == 0 ? Brushes.White : lineActive);
            mb_left_a13.BorderBrush = (Brush)(mem_addr_bus[13] == 0 ? Brushes.White : lineActive);
            mb_left_a14.BorderBrush = (Brush)(mem_addr_bus[14] == 0 ? Brushes.White : lineActive);
            mb_left_a15.BorderBrush = (Brush)(mem_addr_bus[15] == 0 ? Brushes.White : lineActive);
            mb_left_a16.BorderBrush = (Brush)(mem_addr_bus[16] == 0 ? Brushes.White : lineActive);
            mb_left_a17.BorderBrush = (Brush)(mem_addr_bus[17] == 0 ? Brushes.White : lineActive);
            mb_left_a18.BorderBrush = (Brush)(mem_addr_bus[18] == 0 ? Brushes.White : lineActive);
            mb_left_a19.BorderBrush = (Brush)(mem_addr_bus[19] == 0 ? Brushes.White : lineActive);
            mb_left_a20.BorderBrush = (Brush)(mem_addr_bus[20] == 0 ? Brushes.White : lineActive);
            mb_left_a21.BorderBrush = (Brush)(mem_addr_bus[21] == 0 ? Brushes.White : lineActive);
            mb_left_a22.BorderBrush = (Brush)(mem_addr_bus[22] == 0 ? Brushes.White : lineActive);
            mb_left_a23.BorderBrush = (Brush)(mem_addr_bus[23] == 0 ? Brushes.White : lineActive);
            mb_left_a24.BorderBrush = (Brush)(mem_addr_bus[24] == 0 ? Brushes.White : lineActive);
            mb_left_a25.BorderBrush = (Brush)(mem_addr_bus[25] == 0 ? Brushes.White : lineActive);
            mb_left_a26.BorderBrush = (Brush)(mem_addr_bus[26] == 0 ? Brushes.White : lineActive);
            mb_left_a27.BorderBrush = (Brush)(mem_addr_bus[27] == 0 ? Brushes.White : lineActive);
            mb_left_a28.BorderBrush = (Brush)(mem_addr_bus[28] == 0 ? Brushes.White : lineActive);
            mb_left_a29.BorderBrush = (Brush)(mem_addr_bus[29] == 0 ? Brushes.White : lineActive);
            mb_left_a30.BorderBrush = (Brush)(mem_addr_bus[30] == 0 ? Brushes.White : lineActive);
            mb_left_a31.BorderBrush = (Brush)(mem_addr_bus[31] == 0 ? Brushes.White : lineActive);

            mb_right_a1.BorderBrush = (Brush)(mem_addr_bus[1] == 0 ? Brushes.White : lineActive);
            mb_right_a2.BorderBrush = (Brush)(mem_addr_bus[2] == 0 ? Brushes.White : lineActive);
            mb_right_a3.BorderBrush = (Brush)(mem_addr_bus[3] == 0 ? Brushes.White : lineActive);
            mb_right_a4.BorderBrush = (Brush)(mem_addr_bus[4] == 0 ? Brushes.White : lineActive);
            mb_right_a5.BorderBrush = (Brush)(mem_addr_bus[5] == 0 ? Brushes.White : lineActive);
            mb_right_a6.BorderBrush = (Brush)(mem_addr_bus[6] == 0 ? Brushes.White : lineActive);
            mb_right_a7.BorderBrush = (Brush)(mem_addr_bus[7] == 0 ? Brushes.White : lineActive);
            mb_right_a8.BorderBrush = (Brush)(mem_addr_bus[8] == 0 ? Brushes.White : lineActive);
            mb_right_a9.BorderBrush = (Brush)(mem_addr_bus[9] == 0 ? Brushes.White : lineActive);
            mb_right_a10.BorderBrush = (Brush)(mem_addr_bus[10] == 0 ? Brushes.White : lineActive);
            mb_right_a11.BorderBrush = (Brush)(mem_addr_bus[11] == 0 ? Brushes.White : lineActive);
            mb_right_a12.BorderBrush = (Brush)(mem_addr_bus[12] == 0 ? Brushes.White : lineActive);
            mb_right_a13.BorderBrush = (Brush)(mem_addr_bus[13] == 0 ? Brushes.White : lineActive);
            mb_right_a14.BorderBrush = (Brush)(mem_addr_bus[14] == 0 ? Brushes.White : lineActive);
            mb_right_a15.BorderBrush = (Brush)(mem_addr_bus[15] == 0 ? Brushes.White : lineActive);
            mb_right_a16.BorderBrush = (Brush)(mem_addr_bus[16] == 0 ? Brushes.White : lineActive);
            mb_right_a17.BorderBrush = (Brush)(mem_addr_bus[17] == 0 ? Brushes.White : lineActive);
            mb_right_a18.BorderBrush = (Brush)(mem_addr_bus[18] == 0 ? Brushes.White : lineActive);
            mb_right_a19.BorderBrush = (Brush)(mem_addr_bus[19] == 0 ? Brushes.White : lineActive);
            mb_right_a20.BorderBrush = (Brush)(mem_addr_bus[20] == 0 ? Brushes.White : lineActive);
            mb_right_a21.BorderBrush = (Brush)(mem_addr_bus[21] == 0 ? Brushes.White : lineActive);
            mb_right_a22.BorderBrush = (Brush)(mem_addr_bus[22] == 0 ? Brushes.White : lineActive);
            mb_right_a23.BorderBrush = (Brush)(mem_addr_bus[23] == 0 ? Brushes.White : lineActive);
            mb_right_a24.BorderBrush = (Brush)(mem_addr_bus[24] == 0 ? Brushes.White : lineActive);
            mb_right_a25.BorderBrush = (Brush)(mem_addr_bus[25] == 0 ? Brushes.White : lineActive);
            mb_right_a26.BorderBrush = (Brush)(mem_addr_bus[26] == 0 ? Brushes.White : lineActive);
            mb_right_a27.BorderBrush = (Brush)(mem_addr_bus[27] == 0 ? Brushes.White : lineActive);
            mb_right_a28.BorderBrush = (Brush)(mem_addr_bus[28] == 0 ? Brushes.White : lineActive);
            mb_right_a29.BorderBrush = (Brush)(mem_addr_bus[29] == 0 ? Brushes.White : lineActive);
            mb_right_a30.BorderBrush = (Brush)(mem_addr_bus[30] == 0 ? Brushes.White : lineActive);
            mb_right_a31.BorderBrush = (Brush)(mem_addr_bus[31] == 0 ? Brushes.White : lineActive);
        }

        private void PaintButtonsAddr(short[] io_addr_bus)
        {
            if (!realTimeBus) return;
            mb_buttons_a0.BorderBrush = (Brush)(io_addr_bus[0] == 0 ? Brushes.White : lineActive);
            mb_buttons_a1.BorderBrush = (Brush)(io_addr_bus[1] == 0 ? Brushes.White : lineActive);
            mb_buttons_a2.BorderBrush = (Brush)(io_addr_bus[2] == 0 ? Brushes.White : lineActive);
            mb_buttons_a3.BorderBrush = (Brush)(io_addr_bus[3] == 0 ? Brushes.White : lineActive);
            mb_buttons_a4.BorderBrush = (Brush)(io_addr_bus[4] == 0 ? Brushes.White : lineActive);
            mb_buttons_a5.BorderBrush = (Brush)(io_addr_bus[5] == 0 ? Brushes.White : lineActive);
            mb_buttons_a6.BorderBrush = (Brush)(io_addr_bus[6] == 0 ? Brushes.White : lineActive);
            mb_buttons_a7.BorderBrush = (Brush)(io_addr_bus[7] == 0 ? Brushes.White : lineActive);
            mb_buttons_a8.BorderBrush = (Brush)(io_addr_bus[8] == 0 ? Brushes.White : lineActive);
            mb_buttons_a9.BorderBrush = (Brush)(io_addr_bus[9] == 0 ? Brushes.White : lineActive);
            mb_buttons_a10.BorderBrush = (Brush)(io_addr_bus[10] == 0 ? Brushes.White : lineActive);
            mb_buttons_a11.BorderBrush = (Brush)(io_addr_bus[11] == 0 ? Brushes.White : lineActive);
            mb_buttons_a12.BorderBrush = (Brush)(io_addr_bus[12] == 0 ? Brushes.White : lineActive);
            mb_buttons_a13.BorderBrush = (Brush)(io_addr_bus[13] == 0 ? Brushes.White : lineActive);
            mb_buttons_a14.BorderBrush = (Brush)(io_addr_bus[14] == 0 ? Brushes.White : lineActive);
            mb_buttons_a15.BorderBrush = (Brush)(io_addr_bus[15] == 0 ? Brushes.White : lineActive);
            mb_buttons_a16.BorderBrush = (Brush)(io_addr_bus[16] == 0 ? Brushes.White : lineActive);
            mb_buttons_a17.BorderBrush = (Brush)(io_addr_bus[17] == 0 ? Brushes.White : lineActive);
            mb_buttons_a18.BorderBrush = (Brush)(io_addr_bus[18] == 0 ? Brushes.White : lineActive);
            mb_buttons_a19.BorderBrush = (Brush)(io_addr_bus[19] == 0 ? Brushes.White : lineActive);
            mb_buttons_a20.BorderBrush = (Brush)(io_addr_bus[20] == 0 ? Brushes.White : lineActive);
            mb_buttons_a21.BorderBrush = (Brush)(io_addr_bus[21] == 0 ? Brushes.White : lineActive);
            mb_buttons_a22.BorderBrush = (Brush)(io_addr_bus[22] == 0 ? Brushes.White : lineActive);
            mb_buttons_a23.BorderBrush = (Brush)(io_addr_bus[23] == 0 ? Brushes.White : lineActive);
            mb_buttons_a24.BorderBrush = (Brush)(io_addr_bus[24] == 0 ? Brushes.White : lineActive);
            mb_buttons_a25.BorderBrush = (Brush)(io_addr_bus[25] == 0 ? Brushes.White : lineActive);
            mb_buttons_a26.BorderBrush = (Brush)(io_addr_bus[26] == 0 ? Brushes.White : lineActive);
            mb_buttons_a27.BorderBrush = (Brush)(io_addr_bus[27] == 0 ? Brushes.White : lineActive);
            mb_buttons_a28.BorderBrush = (Brush)(io_addr_bus[28] == 0 ? Brushes.White : lineActive);
            mb_buttons_a29.BorderBrush = (Brush)(io_addr_bus[29] == 0 ? Brushes.White : lineActive);
            mb_buttons_a30.BorderBrush = (Brush)(io_addr_bus[30] == 0 ? Brushes.White : lineActive);
            mb_buttons_a31.BorderBrush = (Brush)(io_addr_bus[31] == 0 ? Brushes.White : lineActive);
        }


        private void PaintIOData(short[] io_data_bus)
        {
            if (!realTimeBus) return;
            db_d0.BorderBrush = (Brush)(io_data_bus[0] == 0 ? Brushes.White : lineActive);
            db_d1.BorderBrush = (Brush)(io_data_bus[1] == 0 ? Brushes.White : lineActive);
            db_d2.BorderBrush = (Brush)(io_data_bus[2] == 0 ? Brushes.White : lineActive);
            db_d3.BorderBrush = (Brush)(io_data_bus[3] == 0 ? Brushes.White : lineActive);
            db_d4.BorderBrush = (Brush)(io_data_bus[4] == 0 ? Brushes.White : lineActive);
            db_d5.BorderBrush = (Brush)(io_data_bus[5] == 0 ? Brushes.White : lineActive);
            db_d6.BorderBrush = (Brush)(io_data_bus[6] == 0 ? Brushes.White : lineActive);
            db_d7.BorderBrush = (Brush)(io_data_bus[7] == 0 ? Brushes.White : lineActive);
            db_d8.BorderBrush = (Brush)(io_data_bus[8] == 0 ? Brushes.White : lineActive);
            db_d9.BorderBrush = (Brush)(io_data_bus[9] == 0 ? Brushes.White : lineActive);
            db_d10.BorderBrush = (Brush)(io_data_bus[10] == 0 ? Brushes.White : lineActive);
            db_d11.BorderBrush = (Brush)(io_data_bus[11] == 0 ? Brushes.White : lineActive);
            db_d12.BorderBrush = (Brush)(io_data_bus[12] == 0 ? Brushes.White : lineActive);
            db_d13.BorderBrush = (Brush)(io_data_bus[13] == 0 ? Brushes.White : lineActive);
            db_d14.BorderBrush = (Brush)(io_data_bus[14] == 0 ? Brushes.White : lineActive);
            db_d15.BorderBrush = (Brush)(io_data_bus[15] == 0 ? Brushes.White : lineActive);
            db_d16.BorderBrush = (Brush)(io_data_bus[16] == 0 ? Brushes.White : lineActive);
            db_d17.BorderBrush = (Brush)(io_data_bus[17] == 0 ? Brushes.White : lineActive);
            db_d18.BorderBrush = (Brush)(io_data_bus[18] == 0 ? Brushes.White : lineActive);
            db_d19.BorderBrush = (Brush)(io_data_bus[19] == 0 ? Brushes.White : lineActive);
            db_d20.BorderBrush = (Brush)(io_data_bus[20] == 0 ? Brushes.White : lineActive);
            db_d21.BorderBrush = (Brush)(io_data_bus[21] == 0 ? Brushes.White : lineActive);
            db_d22.BorderBrush = (Brush)(io_data_bus[22] == 0 ? Brushes.White : lineActive);
            db_d23.BorderBrush = (Brush)(io_data_bus[23] == 0 ? Brushes.White : lineActive);
            db_d24.BorderBrush = (Brush)(io_data_bus[24] == 0 ? Brushes.White : lineActive);
            db_d25.BorderBrush = (Brush)(io_data_bus[25] == 0 ? Brushes.White : lineActive);
            db_d26.BorderBrush = (Brush)(io_data_bus[26] == 0 ? Brushes.White : lineActive);
            db_d27.BorderBrush = (Brush)(io_data_bus[27] == 0 ? Brushes.White : lineActive);
            db_d28.BorderBrush = (Brush)(io_data_bus[28] == 0 ? Brushes.White : lineActive);
            db_d29.BorderBrush = (Brush)(io_data_bus[29] == 0 ? Brushes.White : lineActive);
            db_d30.BorderBrush = (Brush)(io_data_bus[30] == 0 ? Brushes.White : lineActive);
            db_d31.BorderBrush = (Brush)(io_data_bus[31] == 0 ? Brushes.White : lineActive);
        }

        private void PaintIOAddr(short[] io_addr_bus)
        {
            if (!realTimeBus) return;
            db_a0.BorderBrush = (Brush)(io_addr_bus[0] == 0 ? Brushes.White : lineActive);
            db_a1.BorderBrush = (Brush)(io_addr_bus[1] == 0 ? Brushes.White : lineActive);
            db_a2.BorderBrush = (Brush)(io_addr_bus[2] == 0 ? Brushes.White : lineActive);
            db_a3.BorderBrush = (Brush)(io_addr_bus[3] == 0 ? Brushes.White : lineActive);
            db_a4.BorderBrush = (Brush)(io_addr_bus[4] == 0 ? Brushes.White : lineActive);
            db_a5.BorderBrush = (Brush)(io_addr_bus[5] == 0 ? Brushes.White : lineActive);
            db_a6.BorderBrush = (Brush)(io_addr_bus[6] == 0 ? Brushes.White : lineActive);
            db_a7.BorderBrush = (Brush)(io_addr_bus[7] == 0 ? Brushes.White : lineActive);
            db_a8.BorderBrush = (Brush)(io_addr_bus[8] == 0 ? Brushes.White : lineActive);
            db_a9.BorderBrush = (Brush)(io_addr_bus[9] == 0 ? Brushes.White : lineActive);
            db_a10.BorderBrush = (Brush)(io_addr_bus[10] == 0 ? Brushes.White : lineActive);
            db_a11.BorderBrush = (Brush)(io_addr_bus[11] == 0 ? Brushes.White : lineActive);
            db_a12.BorderBrush = (Brush)(io_addr_bus[12] == 0 ? Brushes.White : lineActive);
            db_a13.BorderBrush = (Brush)(io_addr_bus[13] == 0 ? Brushes.White : lineActive);
            db_a14.BorderBrush = (Brush)(io_addr_bus[14] == 0 ? Brushes.White : lineActive);
            db_a15.BorderBrush = (Brush)(io_addr_bus[15] == 0 ? Brushes.White : lineActive);
            db_a16.BorderBrush = (Brush)(io_addr_bus[16] == 0 ? Brushes.White : lineActive);
            db_a17.BorderBrush = (Brush)(io_addr_bus[17] == 0 ? Brushes.White : lineActive);
            db_a18.BorderBrush = (Brush)(io_addr_bus[18] == 0 ? Brushes.White : lineActive);
            db_a19.BorderBrush = (Brush)(io_addr_bus[19] == 0 ? Brushes.White : lineActive);
            db_a20.BorderBrush = (Brush)(io_addr_bus[20] == 0 ? Brushes.White : lineActive);
            db_a21.BorderBrush = (Brush)(io_addr_bus[21] == 0 ? Brushes.White : lineActive);
            db_a22.BorderBrush = (Brush)(io_addr_bus[22] == 0 ? Brushes.White : lineActive);
            db_a23.BorderBrush = (Brush)(io_addr_bus[23] == 0 ? Brushes.White : lineActive);
            db_a24.BorderBrush = (Brush)(io_addr_bus[24] == 0 ? Brushes.White : lineActive);
            db_a25.BorderBrush = (Brush)(io_addr_bus[25] == 0 ? Brushes.White : lineActive);
            db_a26.BorderBrush = (Brush)(io_addr_bus[26] == 0 ? Brushes.White : lineActive);
            db_a27.BorderBrush = (Brush)(io_addr_bus[27] == 0 ? Brushes.White : lineActive);
            db_a28.BorderBrush = (Brush)(io_addr_bus[28] == 0 ? Brushes.White : lineActive);
            db_a29.BorderBrush = (Brush)(io_addr_bus[29] == 0 ? Brushes.White : lineActive);
            db_a30.BorderBrush = (Brush)(io_addr_bus[30] == 0 ? Brushes.White : lineActive);
            db_a31.BorderBrush = (Brush)(io_addr_bus[31] == 0 ? Brushes.White : lineActive);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var wait = new Wait();
            wait.Topmost = true;
            this.IsEnabled = false;
            wait.Show();

            Task.Run(() =>
            {
                var systems = Systems.GetSystems();
                return systems;
            }).ContinueWith((t) =>
            {
                var systems = t.Result;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    systems.ForEach(s =>
                    {
                        var openSystem = new MenuItem
                        {
                            Header = s.Name,
                            Tag = s
                        };
                        openSystem.Click += OpenSystem_Click;
                        menuOpenSys.Items.Add(openSystem);

                        var editSystem = new MenuItem
                        {
                            Header = s.Name,
                            Tag = s
                        };
                        editSystem.Click += EditSystem_Click;
                        menuEditSys.Items.Add(editSystem);
                    });

                    if (systems.Count > 0)
                    {
                        current_sys = systems[0];
                        lbSystemName.Text = current_sys.Name;
                        lbVendorName.Content = current_sys.Vendor;
                        lbMemSize.Text = $"{current_sys.MemorySize}\nbytes";
                        InitMotherB(current_sys);
                    }
                    this.IsEnabled = true;
                    wait.Close();
                });
            });



        }

        private void EditSystem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            MenuItem mi = (MenuItem)sender;
            var system = (SystemVM)mi.Tag;
            new CreateSystem(system).ShowDialog();
        }

        bool realTimeBus = true;
        private void ckRTBus_Checked(object sender, RoutedEventArgs e)
        {
            realTimeBus = true;
            ckRTBus.Foreground = Brushes.Gold;
        }

        private void ckRTBus_Unchecked(object sender, RoutedEventArgs e)
        {
            realTimeBus = false;
            ckRTBus.Foreground = Brushes.White;
        }

        bool realTimeRegs = true;
        private bool realTimeClock;
        private bool realTimeOpers;

        private void ckRTRegs_Checked(object sender, RoutedEventArgs e)
        {
            realTimeRegs = true;
            ckRTRegs.Foreground = Brushes.Gold;
        }

        private void btnRefreshRegs_Click(object sender, RoutedEventArgs e)
        {
            var vals = MotherBoard.Instance.CPU.Registers.GetValues();
            lbRegs.Text = vals;
            if (lbRegs.Height < vals.Length)
                lbRegs.Height = (vals.Length * 1.70d);
        }

        private void ckRTRegs_Unchecked(object sender, RoutedEventArgs e)
        {
            realTimeRegs = false;
            ckRTRegs.Foreground = Brushes.White;
        }

        private void ckRTClock_Checked(object sender, RoutedEventArgs e)
        {
            realTimeClock = true;
            ckRTClock.Foreground = Brushes.Gold;
        }

        private void ckRTClock_Unchecked(object sender, RoutedEventArgs e)
        {
            realTimeClock = false;
            ckRTClock.Foreground = Brushes.White;
        }


        private void ckRTOpers_Checked(object sender, RoutedEventArgs e)
        {
            realTimeOpers = true;
            ckRTOpers.Foreground = Brushes.Gold;
        }


        private void ckRTOpers_Unchecked(object sender, RoutedEventArgs e)
        {
            realTimeOpers = false;
            ckRTOpers.Foreground = Brushes.White;
        }

        private void ckEnableInstr_Checked(object sender, RoutedEventArgs e)
        {
            realTimeBus = true;
            realTimeClock = true;
            realTimeOpers = true;
            realTimeRegs = true;
            ckRTBus.IsChecked = true;
            ckRTClock.IsChecked = true;
            ckRTOpers.IsChecked = true;
            ckRTRegs.IsChecked = true;
        }

        private void ckEnableInstr_Unchecked(object sender, RoutedEventArgs e)
        {
            realTimeBus = false;
            realTimeClock = false;
            realTimeOpers = false;
            realTimeRegs = false;
            ckRTBus.IsChecked = false;
            ckRTClock.IsChecked = false;
            ckRTOpers.IsChecked = false;
            ckRTRegs.IsChecked = false;
        }
    }
}