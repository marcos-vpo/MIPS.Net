namespace MIPS.Net.DebuggerGUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            memView1.SetBytes(File.ReadAllBytes(@"C:\Users\marco\Área de Trabalho\MIPS\firmware.asm"));
        }

        private void memView1_Load(object sender, EventArgs e)
        {

        }
    }
}
