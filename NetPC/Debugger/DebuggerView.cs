using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MIPS.Net.Debugger;
using MIPS.Net.SoC;
using NetPC.Debugger;

namespace NetPC
{
    public partial class DebuggerView : Form
    {
        public DebuggerView()
        {
            InitializeComponent();

            InitDataGridMem();
            InitDataGridRegisters();
            dataGridProgram.AutoGenerateColumns = false;
            // Define o número de linhas
        }

        private void InitDataGridRegisters()
        {
            //    dataGridView = new DataGridView();
            dataGridRegisters.AllowUserToAddRows = false;
            dataGridRegisters.AllowUserToDeleteRows = false;
            dataGridRegisters.AllowUserToResizeRows = false;
            dataGridRegisters.ReadOnly = true;
            dataGridRegisters.VirtualMode = true; // Habilita a virtualização!
            dataGridRegisters.CellValueNeeded += DataGridRegisters_CellValueNeeded; ; // Evento para carregar os valores
            dataGridRegisters.RowHeadersVisible = false;
            dataGridRegisters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridRegisters.RowsDefaultCellStyle.BackColor = Color.Gray;
            dataGridRegisters.RowsDefaultCellStyle.ForeColor = Color.White;
            dataGridRegisters.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 127, 247);

            // Configuração das colunas (exemplo: coluna para índice e coluna para valor hexadecimal)
            dataGridRegisters.ColumnCount = 2; // Agora temos 3 colunas
            dataGridRegisters.Columns[0].HeaderText = "Register";
            dataGridRegisters.Columns[1].HeaderText = "Value";
            dataGridRegisters.Columns[0].Width = 100;
            dataGridRegisters.Columns[1].Width = 120;

            dataGridRegisters.RowCount = MIPS_CPU.Instance.Registers.Count();

        }

        private void DataGridRegisters_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < MotherBoard.Instance.Memory.Size)
            {
                try
                {
                    if (e.ColumnIndex == 0)
                    {
                        e.Value = MIPS_CPU.Instance.Registers.NameByIndex(e.RowIndex);
                    }
                    else if (e.ColumnIndex == 1)
                    {
                        e.Value = MIPS_CPU.Instance.Registers[e.RowIndex];
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        e.Value = MIPS_CPU.Instance.Registers[e.RowIndex].ToString("X2"); // Valor Hexadecimal
                    }
                }
                catch { }
            }
        }

        private void InitDataGridMem()
        {
            //    dataGridView = new DataGridView();
            dataGridMem.AllowUserToAddRows = false;
            dataGridMem.AllowUserToDeleteRows = false;
            dataGridMem.AllowUserToResizeRows = false;
            dataGridMem.ReadOnly = true;
            dataGridMem.VirtualMode = true; // Habilita a virtualização!
            dataGridMem.CellValueNeeded += DataGridView_CellValueNeeded; // Evento para carregar os valores
            dataGridMem.RowHeadersVisible = false;
            dataGridMem.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridMem.RowsDefaultCellStyle.BackColor = Color.Gray;
            dataGridMem.RowsDefaultCellStyle.ForeColor = Color.White;
            dataGridMem.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 127, 247);

            // Configuração das colunas (exemplo: coluna para índice e coluna para valor hexadecimal)
            dataGridMem.ColumnCount = 3; // Agora temos 3 colunas
            dataGridMem.Columns[0].HeaderText = "Idx. Dc.";
            dataGridMem.Columns[1].HeaderText = "Addr. Hx.";
            dataGridMem.Columns[2].HeaderText = "Val. Hx.";
            dataGridMem.Columns[0].Width = 50;
            dataGridMem.Columns[1].Width = 80;
            dataGridMem.Columns[2].Width = 80;

            dataGridMem.RowCount = MotherBoard.Instance.Memory.Size;
        }

        // Este evento é chamado quando o DataGridView precisa exibir uma célula
        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < MotherBoard.Instance.Memory.Size)
            {
                if (e.ColumnIndex == 0)
                {
                    e.Value = e.RowIndex; // Índice Decimal
                }
                else if (e.ColumnIndex == 1)
                {
                    e.Value = $"0x{e.RowIndex:X}"; // Índice Hexadecimal com "0x" (maiúsculo)
                }
                else if (e.ColumnIndex == 2)
                {
                    e.Value = MotherBoard.Instance.Memory[e.RowIndex].ToString("X2"); // Valor Hexadecimal
                }
            }
        }











        private void DebuggerView_Load(object sender, EventArgs e)
        {

        }

        private void btnGoTomem_Click(object sender, EventArgs e)
        {


        }

        internal void FillInstructions(List<RotuleInstructionVM> current_rtl_instructions)
        {
            if (current_rtl_instructions.Count == 0) return;
            Invoke(() =>
            {
                btnStepNext.Enabled = true;
                dataGridProgram.DataSource = current_rtl_instructions;
            });
        }

        internal void UpdateRegisters(Registers registers)
        {
            Invoke(() => dataGridRegisters.Refresh());
        }

        internal void SelectInstruction(int index)
        {
            if (index < 0)
            {
                btnStepNext.Enabled = true;
                return;
            }
            Invoke(() =>
            {

                var position = index;

                // 1. Verifica se a posição já está visível
                int firstVisible = dataGridProgram.FirstDisplayedScrollingRowIndex;
                int visibleRows = dataGridProgram.DisplayedRowCount(true); // Obtém o número de linhas totalmente visíveis
                bool isVisible = (position >= firstVisible && position < firstVisible + visibleRows);

                // 2. Rola e seleciona apenas se não estiver visível
                if (!isVisible)
                {
                    dataGridProgram.FirstDisplayedScrollingRowIndex = position;

                    // Seleciona a linha
                    dataGridProgram.ClearSelection();  // Remove qualquer seleção anterior
                    dataGridProgram.Rows[position].Selected = true;  // Seleciona a linha
                }
                else
                {
                    // Seleciona a linha
                    dataGridProgram.ClearSelection();  // Remove qualquer seleção anterior
                    dataGridProgram.Rows[position].Selected = true;  // Seleciona a linha
                }

                btnStepNext.Enabled = true;
            });
        }

        internal void IOAccess(int addr)
        {
            Invoke(() => SelectMem(addr));
        }

        internal void MemAccess(int addr)
        {
            Invoke(() => SelectMem(addr));
        }

        private void SelectMem(int addr)
        {
            var position = addr;

            // 1. Verifica se a posição já está visível
            int firstVisible = dataGridMem.FirstDisplayedScrollingRowIndex;
            int visibleRows = dataGridMem.DisplayedRowCount(true); // Obtém o número de linhas totalmente visíveis
            bool isVisible = (position >= firstVisible && position < firstVisible + visibleRows);

            // 2. Rola e seleciona apenas se não estiver visível
            if (!isVisible)
            {
                dataGridMem.FirstDisplayedScrollingRowIndex = position;

                // Seleciona a linha
                dataGridMem.ClearSelection();  // Remove qualquer seleção anterior
                dataGridMem.Rows[position].Selected = true;  // Seleciona a linha
            }
            else
            {
                // Seleciona a linha
                dataGridMem.ClearSelection();  // Remove qualquer seleção anterior
                dataGridMem.Rows[position].Selected = true;  // Seleciona a linha
            }
        }

        internal void SetRotule(string v)
        {
            Invoke(() => txLabel.Text = v);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnStepNext.Enabled = false;
            MIPS_CPU.Instance.DBG.Step(1);
        }

        private void DebuggerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            MIPS_CPU.Instance.DBG.Disable();
            MIPS_CPU.Instance.DBG = null;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ExtractTuple et = new ExtractTuple();
            et.Show();
        }
    }
}
