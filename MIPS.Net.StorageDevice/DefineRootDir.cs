using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIPS.Net.StorageDevice
{
    public partial class DefineRootDir : Form
    {
        public DefineRootDir()
        {
            InitializeComponent();
        }

        private void liSelPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowDialog();
            txPath.Text = fb.SelectedPath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            File.WriteAllText("root.txt", txPath.Text);
            Close();
        }
    }
}
