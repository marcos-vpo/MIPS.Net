using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIPS.Net.SoC;

namespace NetPC.Debugger
{
    public partial class ExtractTuple : Form
    {
        public ExtractTuple()
        {
            InitializeComponent();
            dataGridValues.AutoGenerateColumns = false;
        }

        private int baseAddr = 0;
        private int length = 0;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ExtractTuple_Load(object sender, EventArgs e)
        {
            FillStructs();
        }

        private void FillStructs()
        {
            listStructs.Items.Clear();
            IReadOnlyCollection<StructDefinition> structs = StructsRepository.All();
            foreach (var s in structs)
                listStructs.Items.Add(s.Name ?? "unamed");
        }

        private void ResetMemView()
        {
            panelMem.BackColor = Color.LightCyan;
            lbBaseAddr.Text = baseAddr.ToString();
            lbFinalAddr.Text = (baseAddr + length).ToString();
        }

        private void btnAddStruct_Click(object sender, EventArgs e)
        {
            DefineStruct ds = new DefineStruct();
            ds.ShowDialog();

            FillStructs();
        }

        private void listStructs_DoubleClick(object sender, EventArgs e)
        {
            if (listStructs.SelectedItem == null) return;

            StructDefinition? sd = StructsRepository.Get(listStructs.SelectedItem.ToString());
            if (sd == null) return;

            baseAddr = sd.BaseAddress;
            length = sd.Length;

            FillValues(sd);


            FillMemoryViewBar(sd);
        }

        private void FillValues(StructDefinition sd)
        {
            List<StructValuePair> values = new List<StructValuePair>();

            int readAddr = baseAddr;
            foreach(var field in sd.Fields)
            {
                byte[] fValB = new byte[field.FieldLen];

                DMA.RequestData(readAddr, ref fValB);

                object? val = field.DecodeVal(fValB);

                values.Add(new StructValuePair
                {
                    Name = field.FieldName,
                    Value = val
                });

                readAddr += field.FieldLen;
            }

            dataGridValues.DataSource = null;
            dataGridValues.DataSource = values;
        }

        private void FillMemoryViewBar(StructDefinition sd)
        {
            ResetMemView();

            Graphics g = panelMem.CreateGraphics();
            g.Clear(panelMem.BackColor);

            int panelWidth = panelMem.Width;
            int panelHeight = panelMem.Height;

            int barHeight = panelHeight - 4; // altura da faixa (ajuste fino)
            int y = 2;

            int xOffset = 0;

            Random rnd = new Random();

            using Font font = new Font("Segoe UI", 9, FontStyle.Bold);
            using StringFormat sfmt = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            for (int i = 0; i < sd.Fields.Count; i++)
            {
                StructField sf = sd.Fields[i];

                int fieldLen = sf.FieldLen;

                // percentual do campo dentro da struct
                float percent = (float)fieldLen / sd.Length;

                // largura proporcional
                int rectWidth = (int)(panelWidth * percent);

                // garante que o último campo feche a barra inteira (evita erro de arredondamento)
                if (i == sd.Fields.Count - 1)
                    rectWidth = panelWidth - xOffset;

                Rectangle rect = new Rectangle(xOffset, y, rectWidth, barHeight);

                // cor randômica
                Color color = Color.FromArgb(
                    255,
                    rnd.Next(50, 200),
                    rnd.Next(50, 200),
                    rnd.Next(50, 200));

                using Brush brush = new SolidBrush(color);
                using Pen pen = new Pen(Color.White);

                g.FillRectangle(brush, rect);
                g.DrawRectangle(pen, rect);

                // texto no centro
                string text = $"{sf.FieldName} ({fieldLen}b)";

                // desenha texto apenas se couber minimamente
                if (rectWidth > 40)
                {
                    using Brush textBrush = new SolidBrush(Color.White);
                    g.DrawString(text, font, textBrush, rect, sfmt);
                }

                xOffset += rectWidth;
            }

            g.Dispose();
        }

        private void btEditStruct_Click(object sender, EventArgs e)
        {
            if (listStructs.SelectedItem == null) return;
            StructDefinition? sd = StructsRepository.Get(listStructs.SelectedItem.ToString());
            if (sd == null) return;

            DefineStruct ds = new DefineStruct(editingStruct: sd);
            ds.ShowDialog();

            FillStructs();
        }

        private void btRemoveStruct_Click(object sender, EventArgs e)
        {
            if (listStructs.SelectedItem == null) return;
            StructDefinition? sd = StructsRepository.Get(listStructs.SelectedItem.ToString());
            if (sd == null) return;

            StructsRepository.Remove(sd);

            FillStructs();
        }
    }
}
