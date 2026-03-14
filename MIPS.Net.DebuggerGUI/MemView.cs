using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIPS.Net.DebuggerGUI
{
    public partial class MemView : UserControl
    {

        public MemView()
        {
            InitializeComponent();


        }



        public void SetBytes(byte[] byteArray)
        {
            lbTitl.Text += "(UPDATING!!!)";
            Task.Run(() =>
            {
                string hext = FillBytes(byteArray);
                Thread.Sleep(400);
                return hext;
            }).ContinueWith((t) =>
            {
                string tx = t.Result.ToString();
                Invoke(() =>
                {
                    if (rdoText.Checked)
                    {
                        richTextBox1.BringToFront();
                        richTextBox1.Text = tx;
                    }
                    else
                    {
                        dataGrid.BringToFront();

                        string[] lines = tx.Split('\r');
                        foreach(string ln in lines)
                        {
                            string[] parts = ln.Split(' ');
                            dataGrid.Rows.Add(parts);
                        }

                    }

                    lbTitl.Text = lbTitl.Text.Replace("(UPDATING!!!)", "");
                });
            });
        }

        private static string FillBytes(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder();
            int bytesPerLine = 16;

            for (int i = 0; i < byteArray.Length; i += bytesPerLine)
            {
                hex.AppendFormat("{0:X4}: ", i); // Endereço inicial da linha

                // Adiciona os bytes da linha
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < byteArray.Length)
                    {
                        hex.AppendFormat("{0:X2} ", byteArray[i + j]);
                    }
                    else
                    {
                        hex.Append("   "); // Espaços em branco para bytes faltantes
                    }
                }

                hex.Append(""); // Espaço entre hex e ASCII
                                 // Adiciona a representação ASCII
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < byteArray.Length)
                    {
                        char c = (char)byteArray[i + j];
                        hex.Append(char.IsControl(c) ? '.' : c);
                    }
                    else
                    {
                        hex.Append(' '); // Espaço para bytes faltantes
                    }
                }

                hex.AppendLine(); // Nova linha após cada linha de bytes
            }

            return hex.ToString();
        }

        private void rdoGrid_CheckedChanged(object sender, EventArgs e)
        {
            dataGrid.BringToFront();
        }

        private void rdoText_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.BringToFront();
        }
    }
}
