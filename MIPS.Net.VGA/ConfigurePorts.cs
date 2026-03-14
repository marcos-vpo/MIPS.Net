using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIPS.Net.VGA
{
    public partial class ConfigurePorts : Form
    {
        public ConfigurePorts()
        {
            InitializeComponent();

            if (File.Exists(@"PORTS.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines(@"PORTS.txt");
                    txDisplay.Text = lines[0];
                    txKeyboard.Text = lines[1];
                    txMouse.Text = lines[2];
                }
                catch { }
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            string cfg = $@"{txDisplay.Text}
{txKeyboard.Text}
{txMouse.Text}";
            File.WriteAllText(@"PORTS.txt", cfg);

            Close();
        }
    }
}
