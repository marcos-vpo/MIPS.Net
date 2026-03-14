using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using MIPS.Compiler;
using MIPS.Net;
using MIPS.Net.IO;
using MIPS.Net.SoC;
using NetPC.Controllers;
using NetPC.Debugger;
using NetPC.Properties;

namespace NetPC
{
    public partial class NetPCForm : Form, IKeyboardAdapter, IDeviceMediator, IClockCapture, IOBusSignalCapture, MemBusSignalCapture
    {
        private bool auto_play = false;
        private string firmwarePath = "";
        public NetPCForm(MotherBoard motherBoard = null, string firmwarePath = "")
        {
            InitializeComponent();
            this.firmwarePath = firmwarePath;
            this.motherBoard = motherBoard;
            if (motherBoard != null) auto_play = true;

            lbInstEnabled.Checked = true;
        }

        MotherBoard motherBoard = null;
        System.Timers.Timer clockT;
        private void btTurnOnOff_Click(object sender, EventArgs e)
        {
            InitLines();

            if (clockT == null)
            {
                clockT = new System.Timers.Timer();
                clockT.Interval = 500;
                clockT.Elapsed += ClockT_Elapsed;
                clockT.Start();
            }

            if (motherBoard == null)
            {
                var usb01 = new USBPort()
                {
                    // display port
                    ID = 0x01,
                    Class = 0x20
                };

                var usb02 = new USBPort()
                {
                    // display port
                    ID = 0x02,
                    Class = 0x20
                };

                var usb03 = new USBPort()
                {
                    // display port
                    ID = 0x03,
                    Class = 0x20
                };

                var usb04 = new USBPort()
                {
                    // display port
                    ID = 0x04,
                    Class = 0x20
                };

                motherBoard = new MotherBoard((1024 * 1024) * 20, new List<IOPort>
                {
                  usb01, usb02, usb03, usb04
                }, new List<IHardwareButton>());

                flowLayoutPanel1.Controls.Clear();
                if (motherBoard.Ports != null)
                    foreach (var port in motherBoard.Ports)
                        flowLayoutPanel1.Controls.Add(new UIPort((USBPort)port, this));
            }

            motherBoard.OnSysHalted += MotherBoard_OnSysHalted;
            ///  motherBoard.MemBUS.OnSignalReceived += MemBUS_OnSignalReceived;
            //    motherBoard.IOBus.OnSignalReceived += IOBus_OnSignalReceived;
            //   motherBoard.IOBus.OnStatusChanged += IOBus_OnStatusChanged;
            MIPS_CPU.Capture = this;


            if (flowLayoutPanel1.Controls.Count == 0)
                if (motherBoard.Ports != null)
                    foreach (var port in motherBoard.Ports)
                        flowLayoutPanel1.Controls.Add(new UIPort((USBPort)port, this));

            if (motherBoard.IsOn)
            {
                motherBoard.TurnOff();
                File.WriteAllText("FREQUENCY_METRICS.txt", sb.ToString());
                sb.Clear();
                MaximizeBox = true;
                btTurnOnOff.BackgroundImage = Resources.power_off;
            }
            else
            {
                var debugger = new DebuggerAdapter((int r) =>
                {
                    return Invoke(() =>
                    {
                        DebuggerView dv = new DebuggerView();
                        dv.Show();
                        return dv;
                    });
                });

          
                MaximizeBox = false;

                btTurnOnOff.BackgroundImage = Resources.power_on;
                halted = false;

                byte[] firmware = null;
                try
                {
                    string fp = (string.IsNullOrEmpty(firmwarePath)
                        ? @".\firmware\fast_video.mex"
                        : firmwarePath);
                    firmware = File.ReadAllBytes(fp);
                }
                catch
                {
                    MessageBox.Show("Firmware canot be loaded");
                    Environment.Exit(0);
                }

                motherBoard.TurnOn(firmware, debugger, this, this);
            }

            sanDisplay1.Focus();
        }


        public void OnIOBus(int addr, byte[] data, bool write, bool read, bool interruptionSignal)
        {
            if (inst_enabled == false) return;
            lock (lck)
            {
                Func<int> upd = new Func<int>(() =>
                {
                    try
                    {
                        panel_io_interruption.Visible = interruptionSignal;

                        Thread.Sleep(10);

                        panel_io_interruption.Visible = false;
                    }
                    catch (Exception e)
                    {

                    }
                    return 0;
                });

                if (InvokeRequired) BeginInvoke(() => upd());
                else upd();
            }
        }

        public void OnMemBus(int addr, byte[] data, bool write, bool read, bool interruptionSignal)
        {
            try
            {


                if (inst_enabled == false) return;

             

                short[] b32 = MemoryBUS.DataPins(); // 32x 1 ou 0

                var pnl = panel_mem;
                g_mem.Clear(Color.SeaGreen);

                int largura = largura_m;
                int altura = altura_m;

                // PARA VERTICAL, o espaçamento é na largura
                float esp = largura / 32f;

                for (int i = 0; i < 32; i++)
                {
                    float x = i * esp;
                    if (b32[i] == 1)
                    {
                        g_mem.DrawLine(pen_lines, x, 0, x, altura);
                    }
                    else
                    {
                        //      g_mem.DrawLine(p_off_line, x, 0, x, altura);
                    }
                }

                if (InvokeRequired) BeginInvoke(() => panel_mem.Invalidate());
                else panel_mem.Invalidate();
            }
            catch { }
        }




        Bitmap bmp_mem;
        Graphics g_mem;
        Pen pen_lines;



        private void InitLines()
        {
            if (altura_m > 0) return;
            this.DoubleBuffered = true;
            panel_mem.DoubleBuffered(true);

            bmp_mem = new Bitmap(panel_mem.Width, panel_mem.Height);
            g_mem = Graphics.FromImage(bmp_mem);
            pen_lines = new Pen(Color.Yellow, 2);

            largura_m = bmp_mem.Width;
            altura_m = bmp_mem.Height;
        }

        int largura_m = 0;
        int altura_m = 0;
        int largura_io = 0;
        int altura_io = 0;

        private bool inst_enabled = false;


        private Pen p_off_line = new Pen(Brushes.DarkGray);

        bool halted = false;
        private void MotherBoard_OnSysHalted()
        {
            try
            {
                if (halted) return;
                motherBoard.OnSysHalted -= MotherBoard_OnSysHalted;
                //      motherBoard.MemBUS.OnSignalReceived -= MemBUS_OnSignalReceived;
                //   motherBoard.IOBus.OnSignalReceived -= IOBus_OnSignalReceived;
                //   motherBoard.IOBus.OnStatusChanged -= IOBus_OnStatusChanged;

                halted = true;
                //     clockT.Stop();
                //     clockT.Elapsed -= ClockT_Elapsed;

                DisplayController.TURN_OFF();
                KeyBoardController.TURN_OFF();
                //   motherBoard = null;
                Invoke(() =>
                {
                    MaximizeBox = true;
                    btTurnOnOff.BackgroundImage = Resources.power_off;
                    sanDisplay1.BackColor = Color.Black;
                    sanDisplay1.ClearScreen();
                });

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {

            }
        }

        StringBuilder sb = new StringBuilder();

        private void ClockT_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (motherBoard != null)
            {
                try
                {
                    BeginInvoke(() =>
                    {
                        try
                        {
                            var freq = motherBoard.CPUFrequency;
                            string unit = freq < 1000 ? "kHz" : "Mhz";
                            if (freq > 1000) freq = (freq / 1000);

                            this.Text = $"NetPC - MIPS - {freq:n2} {unit}";
                            if (halted) Text += $" - HALTED!";

                            panelFFI.BackColor = (MIPS_CPU.Instance.FFIEnabled
                                ? Color.LimeGreen
                                : Color.Gray);

                            panelMMU.BackColor = (MIPS_CPU.Instance.MMU.Enabled
                                ? Color.LimeGreen
                                : Color.Gray);

                            panelIntr.BackColor = (MIPS_CPU.Instance.IsInterrupted()
                              ? Color.LimeGreen
                              : Color.Gray);
                            //     sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] {Text}");
                        }
                        catch { }
                    });
                }
                catch { }
            }

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            /*
            if (MotherBoard.Instance == null) return;
            if (MotherBoard.Instance.IsOn)
                //    if (this.WindowState == FormWindowState.Minimized) return;
                Task.Run(() => Thread.Sleep(100))
                   .ContinueWith((t) => sanDisplay1.RestoreState());
            */
        }

        private static object lck = new object();
        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;

        }

        private void sanDisplay1_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Hide();
        }

        private void sanDisplay1_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Cursor.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (halted == false) e.Cancel = true;
        }


        private void panel_mem_Paint(object sender, PaintEventArgs e)
        {
            if (inst_enabled == false) return;
            if (bmp_mem == null) return;
            Graphics g = e.Graphics;
            g.DrawImage(bmp_mem, 0, 0);
        }



        private void NetPCForm_Load(object sender, EventArgs e)
        {
            if (auto_play)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Invoke(() =>
                    {
                        btTurnOnOff_Click(null, null);
                    });
                });
            }
        }

        private void lbInstEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (motherBoard == null) return;
            inst_enabled = lbInstEnabled.Checked;
            motherBoard.BusNotifications(inst_enabled);
            sanDisplay1.Focus();
        }

        public void MIPS_OnClock(short clock_status, double frequency, int energyLevel)
        {
            if (!inst_enabled) return;
            panelClock.Invoke(() =>
            {
                panelClock.BackColor = (clock_status == 0
                    ? Color.Black
                    : Color.Green);
            });
        }




        public void TurnOnScreen(USBPort port)
        {

            DisplayController.INIT(port.DevicePort, sanDisplay1);
        
        }

        public void TurnOffScreen(USBPort port)
        {
            sanDisplay1.BackColor = Color.Black;
        }


        public void TurnOnStorage(USBPort port)
        {
            StorageController.INIT(port.DevicePort);
        }

        public void TurnOffStorage(USBPort port)
        {
            StorageController.TURN_OFF();
        }


        public void KeyboardStatus(bool connected)
        {

        }


        public void TurnOnKeyBoard(USBPort port)
        {
            KeyBoardController.INIT(port.DevicePort, this);
        }

        public void TurnOffKeyBoard(USBPort port)
        {
            KeyBoardController.TURN_OFF();
        }



        private void sanDisplay1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            return;
            Keys[] k = new Keys[] { Keys.Enter, Keys.Control, Keys.Escape, Keys.Space };
            if (e.KeyCode == Keys.Enter)
            {
                if (KeyBoardController.IsConnected() == false) return;

                AutoResetEvent ar = new AutoResetEvent(false);
                // Verifica se a tecla Shift está pressionada
                bool isShiftPressed = e.Shift;

                // Obtem o caractere da tecla pressionada
                char keyChar = (char)e.KeyCode;

                // Converte para minúscula se Shift não estiver pressionado
                if (!isShiftPressed)
                {
                    keyChar = char.ToLower(keyChar);
                }
                KeyBoardController.SendKey(e.Control, e.Shift, e.Alt, keyChar, () =>
                {
                    ar.Set();
                    Invoke(() => lbKeyboardPing.Text = $"Keyboard Ping: {KeyBoardController.PingSpeed}");
                    return 0;
                });
                ar.WaitOne();
            }
        }

        private static bool IsSpecialKey(Keys key)
        {
            switch (key)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.Menu:        // Alt
                case Keys.Enter:
                case Keys.Back:
                case Keys.Tab:
                case Keys.Escape:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }

            return false;
        }

        private void sanDisplay1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!KeyBoardController.IsConnected())
                return;

            // Blindagem: ignora tudo que NÃO for especial
            if (!IsSpecialKey(e.KeyCode))
                return;


            char keyChar = '\0';

            if (e.KeyCode == Keys.Enter) keyChar = (char)(byte)Keys.Enter; // ou '\n', dependendo do seu firmware
            if (e.KeyCode == Keys.Back) keyChar = (char)(byte)Keys.Back;

            AutoResetEvent ar = new AutoResetEvent(false);

            KeyBoardController.SendKey(
                e.Control,
                e.Shift,
                e.Alt,
                keyChar,
                () =>
                {
                    ar.Set();
                    return 0;
                }
            );

            ar.WaitOne();

            // Evita que o WinForms gere KeyPress depois
            e.SuppressKeyPress = true;
        }

        private void sanDisplay1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!KeyBoardController.IsConnected())
                return;

            // Blindagem: ignora caracteres de controle
            if (char.IsControl(e.KeyChar))
                return;

            AutoResetEvent ar = new AutoResetEvent(false);

            KeyBoardController.SendKey(
                false,
                false,
                false,
                e.KeyChar,   // aqui é caractere real
                () =>
                {
                    ar.Set();
                    return 0;
                }
            );

            ar.WaitOne();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
