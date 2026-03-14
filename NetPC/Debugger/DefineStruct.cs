using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetPC.Debugger
{
    public partial class DefineStruct : Form
    {
        private StructDefinition _struct;
        public DefineStruct(StructDefinition? editingStruct = null)
        {
            InitializeComponent();
            _struct = editingStruct ?? new StructDefinition();

            dataGrid.AutoGenerateColumns = false;
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            List<FieldType> fields = new List<FieldType>()
            {
                FieldType.fShort  ,
                FieldType.fInt    ,
                FieldType.fLong   ,
                FieldType.fFloat  ,
                FieldType.fDouble ,
                FieldType.fBool   ,
                FieldType.fString ,
                FieldType.fChar   ,
                FieldType.fByte
            };

            cbFType.DataSource = fields;
            txFLen.Text = "2";

            if (editingStruct != null)
            {
                txStrName.Text = _struct.Name;
                dataGrid.DataSource = _struct.Fields;
                txStrName.ReadOnly = true;
                txStrName.BackColor = Color.White;
            }
        }

        private void cbFType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldType val = (FieldType)cbFType.SelectedItem;
            if (val == FieldType.fString)
            {
                txFLen.Enabled = true;
                txFLen.Text = "1";
            }
            else
            {
                if (val == FieldType.fShort) txFLen.Text = "2";
                else if (val == FieldType.fInt) txFLen.Text = "4";
                else if (val == FieldType.fLong) txFLen.Text = "8";
                else if (val == FieldType.fFloat) txFLen.Text = "4";
                else if (val == FieldType.fDouble) txFLen.Text = "8";
                else if (val == FieldType.fBool) txFLen.Text = "1";
                else if (val == FieldType.fChar) txFLen.Text = "1";
                else if (val == FieldType.fByte) txFLen.Text = "1";
                txFLen.Enabled = false;
            }
        }

        private void txStrName_TextChanged(object sender, EventArgs e)
        {
            gpAdd.Enabled = !string.IsNullOrEmpty(txStrName.Text);
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            _struct.Fields.Add(new StructField
            {
                Order = _struct.Fields.Count,
                FieldName = txFieldName.Text,
                FieldType = (FieldType)cbFType.SelectedItem,
                FieldLen = int.Parse(txFLen.Text)
            });

            dataGrid.DataSource = null;
            dataGrid.DataSource = _struct.Fields;
            btSave.Enabled = true;

            txFieldName.Text = "";
            txFieldName.Focus();
        }

        private void btOrderUp_Click(object sender, EventArgs e)
        {
            if (_struct.Fields.Count == 0) return;
            if (dataGrid.CurrentRow == null) return;
            StructField current = _struct.Fields[dataGrid.CurrentRow.Index];

            if (current.Order == (_struct.Fields.Count - 1)) return;

            current.Order += 1;

            StructField next = _struct.Fields[dataGrid.CurrentRow.Index + 1];
            next.Order -= 1;

            _struct.Fields[dataGrid.CurrentRow.Index] = next;
            _struct.Fields[dataGrid.CurrentRow.Index + 1] = current;

            dataGrid.DataSource = null;
            dataGrid.DataSource = _struct.Fields;

            btSave.Enabled = true;
        }

        private void btOrderDown_Click(object sender, EventArgs e)
        {
            if (_struct.Fields.Count == 0) return;
            if (dataGrid.CurrentRow == null) return;
            StructField current = _struct.Fields[dataGrid.CurrentRow.Index];

            // Já é o primeiro
            if (current.Order == 0) return;

            current.Order -= 1;

            StructField prev = _struct.Fields[dataGrid.CurrentRow.Index - 1];
            prev.Order += 1;

            _struct.Fields[dataGrid.CurrentRow.Index] = prev;
            _struct.Fields[dataGrid.CurrentRow.Index - 1] = current;

            dataGrid.DataSource = null;
            dataGrid.DataSource = _struct.Fields;
            btSave.Enabled = true;
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (_struct.Fields.Count == 0) return;
            if (dataGrid.CurrentRow == null) return;
            StructField current = _struct.Fields[dataGrid.CurrentRow.Index];

            _struct.Fields.Remove(current);

            for (int i = 0; i < _struct.Fields.Count; i++)
                _struct.Fields[i].Order = i;

            dataGrid.DataSource = null;
            dataGrid.DataSource = _struct.Fields;
            btSave.Enabled = _struct.Fields.Count > 0;
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            _struct.Name = txStrName.Text;
            _struct.BaseAddress = int.Parse(txStrAddress.Text);
            StructsRepository.Add(_struct);
            Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
