using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using MIPS.DeviceLib;

namespace MIPS.Net.StorageDevice
{
    public partial class Form1 : Form
    {
        private string root_dir = @".\";
        public Form1()
        {
            InitializeComponent();

            if (!File.Exists(@".\root.txt")) new DefineRootDir().ShowDialog();
            if (!File.Exists(@".\root.txt")) Environment.Exit(0);

            root_dir = File.ReadAllText(@".\root.txt");
            txRoot.Text = root_dir;

            txPort.Text = Program.SYSTEM_PORT_AUTO.ToString();
            if (Program.SYSTEM_PORT_AUTO > 0)
            {
                Thread.Sleep(1000);
                button1_Click(null, null);
            }
            txPort.Text = "13555";
        }

        private void liSelPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowDialog();
            txRoot.Text = fb.SelectedPath;
            root_dir = fb.SelectedPath;
            File.WriteAllText("root.txt", txRoot.Text);
        }

        MDHardware hw = null;
        private void button1_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Black;
            hw = new MDHardware();

            hw.OnDataReceived += Hw_OnDataReceived;
            hw.OnReceiveFailed += Hw_OnReceiveFailed;

            hw.OnDataSendFailed += Hw_OnDataSendFailed;
            hw.OnDataSend += Hw_OnDataSend;

            hw.Connect(int.Parse(txPort.Text), Convert.ToByte(txDevType.Text, 16));

            button1.ForeColor = Color.Green;
        }

        private void Hw_OnDataReceived(byte[] data, int readed)
        {
            byte[] header = data[0..10];
            byte[] raw = new byte[data.Length - 10];

            Array.Copy(data, 10, raw, 0, raw.Length);

            byte operation = header[4];
            byte sinc_mode = header[5];

            if (operation == 0x00) ReadOperation(raw, readed - 10);
        }

        private void ReadOperation(byte[] raw, int len)
        {
            byte[] fNameRaw = new byte[len];
            Array.Copy(raw, 0, fNameRaw, 0, len);

            string fNameStr = Encoding.UTF8.GetString(fNameRaw);
            string fNameOri = fNameStr;
            int zeroIndx = fNameStr.IndexOf('\0');
            if (zeroIndx != -1)
                fNameStr = fNameStr.Substring(0, zeroIndx);
            fNameStr = root_dir + fNameStr.Replace("/", "\\");
            if (File.Exists(fNameStr))
            {

                byte[] fileRaw = File.ReadAllBytes(fNameStr);
                BeginInvoke(() => listBox1.Items.Add($"[R] {fNameOri} {fileRaw.Length}b"));
                hw.SendResponse(fileRaw);
            }
        }

        private void Hw_OnReceiveFailed(string msg, bool connected)
        {

        }

        private void Hw_OnDataSend()
        {

        }

        private void Hw_OnDataSendFailed(string msg, bool connected)
        {

        }


        bool busy = false;
        private void SEND_MOCK_DATA(object sender, EventArgs e)
        {

            byte[] helloBytes = Encoding.ASCII.GetBytes("Hello !!!");
            byte[] helloLen = BitConverter.GetBytes(helloBytes.Length);
            byte[] requestInterrupt = new byte[5] { 0x2b, 0, 0, 0, 0 };
            Array.Copy(helloLen, 0, requestInterrupt, 1, 4);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
