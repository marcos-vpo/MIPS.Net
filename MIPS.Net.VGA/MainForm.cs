using MIPS.DeviceLib;
using MIPS.Net.VGA.Controllers;
using static System.Windows.Forms.LinkLabel;

namespace MIPS.Net.VGA
{
    public partial class MainForm : Form, IDisplayAdapter, IKeyboardAdapter
    {
        public MainForm()
        {
            InitializeComponent();

            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            BeginInvoke(() => lbMSDisplay.Text = DisplayController.ReceiveRate);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(@"PORTS.txt")) new ConfigurePorts().ShowDialog();
            if (!File.Exists(@"PORTS.txt")) Environment.Exit(0);
            graphics = screen.CreateGraphics();


            if (Program.SYSTEM_PORT_AUTO > 0)
            {
                Thread.Sleep(1000);
                TURN_ON_OFF(null, null);
            }
        }

        private void KEYBOARD_INPUT(object sender, KeyEventArgs e)
        {
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
                Invoke(() => lbMsKeyboard.Text = KeyBoardController.PingSpeed);
                return 0;
            });
            ar.WaitOne();
        }

        private void MOUSE_MOVE(object sender, MouseEventArgs e)
        {

        }


        private void screen_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPorts_Click(object sender, EventArgs e)
        {
            new ConfigurePorts().ShowDialog();
        }

        bool is_on = false;


        private void TURN_ON_OFF(object sender, EventArgs e)
        {
            if (is_on == false) // go turn ON
            {
                string[] ports = File.ReadAllLines("ports.txt");
                int displayPort = 0;
                int keyboardPort = 0;
                int mousePort = 0;

                if (ports.Length >= 1) displayPort = int.Parse(ports[0]);
                if (ports.Length >= 2) keyboardPort = int.Parse(ports[1]);
                if (ports.Length >= 3) mousePort = int.Parse(ports[2]);

                if (displayPort > 0) DisplayController.INIT(displayPort, this);
                Thread.Sleep(100);
                if (keyboardPort > 0) KeyBoardController.INIT(keyboardPort, this);

                btnPorts.Enabled = false;
                btnTurnOnOff.BackColor = Color.DarkRed;
                btnTurnOnOff.Text = "TURN OFF";
                btnTurnOnOff.Enabled = false;
                this.Focus();
                is_on = true;
            }
            else // go turn OFF
            {
                DisplayController.TURN_OFF();
                KeyBoardController.TURN_OFF();

                btnPorts.Enabled = false;
                btnTurnOnOff.BackColor = Color.SeaGreen;
                btnTurnOnOff.Text = "TURN ON";
                this.Focus();
            }
        }

        public void DisplayStatus(bool connected)
        {
            if (connected) statusDisplay.BackColor = Color.Green;
            else statusDisplay.BackColor = Color.Red;
        }

        private Graphics graphics;

        public void ClearScreen()
        {
            graphics.Clear(screen.BackColor);
        }

        public void PrintTextLine(string line, int x, int y, Color color, object font)
        {
            graphics.DrawString(line, screen.Font, Brushes.White, x, y);
        }

        public void KeyboardStatus(bool connected)
        {
            if (connected) statusKeyBoard.BackColor = Color.Green;
            else statusKeyBoard.BackColor = Color.Red;
        }

        private void btnTurnOnOff_MouseEnter(object sender, EventArgs e)
        {
            if (is_on) btnTurnOnOff.Enabled = true;
        }

        private void btnTurnOnOff_MouseLeave(object sender, EventArgs e)
        {
            if (is_on) btnTurnOnOff.Enabled = false;
        }

        private void screen_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void screen_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        public void PrintChar(char b, int x, int y, Color white, object value)
        {
            screen.CreateGraphics().DrawString(b.ToString(), screen.Font, Brushes.White, x, y);
        }

        public Tuple<int, int> GetDimensions()
        {
            return new Tuple<int, int>(screen.Size.Width, screen.Size.Height);
        }

        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys[] k = new Keys[] {Keys.Enter, Keys.Control, Keys.Escape, Keys.Space};
            if(e.KeyCode == Keys.Enter)
            {
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
                    Invoke(() => lbMsKeyboard.Text = KeyBoardController.PingSpeed);
                    return 0;
                });
                ar.WaitOne();
            }
        }
    }
}
