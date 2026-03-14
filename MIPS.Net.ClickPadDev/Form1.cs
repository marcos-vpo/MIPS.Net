using Microsoft.VisualBasic.Devices;
using MIPS.DeviceLib;

namespace MIPS.Net.ClickPadDev
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        MDHardware hw = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            hw = new MDHardware();
            hw.OnDataReceived += Hw_OnDataReceived;
            hw.Connect(13560, 0x5D);
        }

        private void Hw_OnDataReceived(byte[] data, int readed)
        {
            // [ op + x + y + clr ]
            byte[] cords = new byte[1 + 4 + 4 + 1];
            Array.Copy(data, 10, cords, 0, cords.Length);

            if (cords[0] == 0x4A) // update pointer
            {
                var g = panelPad.CreateGraphics();
                g.Clear(panelPad.BackColor);

                int x = BitConverter.ToInt32(cords[2..5]);
                int y = BitConverter.ToInt32(cords[6..9]);
                byte color = cords[10];

                g.DrawRectangle(new Pen(Color.White), x, y, 5, 5);
            }
        }

        private void panelPad_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panelPad_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void panelPad_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;

            byte[] b = new byte[8];
            Array.Copy(BitConverter.GetBytes(x), 0, b, 0, 4);
            Array.Copy(BitConverter.GetBytes(y), 0, b, 4, 4);

            hw.SendData(b);
            lbOut.Text = $"OUT {hw.SEND_DATA_MS} ms";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            hw.SendData(BitConverter.GetBytes((int)10), () =>
            {
                BeginInvoke(() =>
                {
                    button1.Visible = true;
                    lbOut.Text = $"OUT {hw.SEND_DATA_MS} ms";
                });
                return 0;
            });
        
        }
    }
}
