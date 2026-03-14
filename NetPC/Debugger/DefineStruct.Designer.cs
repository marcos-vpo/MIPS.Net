namespace NetPC.Debugger
{
    partial class DefineStruct
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGrid = new DataGridView();
            Order = new DataGridViewTextBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            label1 = new Label();
            txStrName = new TextBox();
            gpAdd = new GroupBox();
            btAdd = new Button();
            label5 = new Label();
            txFLen = new TextBox();
            label4 = new Label();
            cbFType = new ComboBox();
            label3 = new Label();
            txFieldName = new TextBox();
            label2 = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            btSave = new Button();
            txStrAddress = new TextBox();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGrid).BeginInit();
            gpAdd.SuspendLayout();
            SuspendLayout();
            // 
            // dataGrid
            // 
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.AllowUserToResizeColumns = false;
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.BackgroundColor = Color.White;
            dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGrid.Columns.AddRange(new DataGridViewColumn[] { Order, Column1, Column2 });
            dataGrid.Location = new Point(12, 219);
            dataGrid.Name = "dataGrid";
            dataGrid.ReadOnly = true;
            dataGrid.RowHeadersVisible = false;
            dataGrid.Size = new Size(354, 245);
            dataGrid.TabIndex = 0;
            // 
            // Order
            // 
            Order.DataPropertyName = "Order";
            Order.HeaderText = "Order";
            Order.Name = "Order";
            Order.ReadOnly = true;
            // 
            // Column1
            // 
            Column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column1.DataPropertyName = "FieldName";
            Column1.HeaderText = "Name";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "FieldType";
            Column2.HeaderText = "Type";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 21);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 1;
            label1.Text = "Struct Name";
            // 
            // txStrName
            // 
            txStrName.Location = new Point(105, 18);
            txStrName.Name = "txStrName";
            txStrName.Size = new Size(261, 23);
            txStrName.TabIndex = 2;
            txStrName.TextChanged += txStrName_TextChanged;
            // 
            // gpAdd
            // 
            gpAdd.Controls.Add(btAdd);
            gpAdd.Controls.Add(label5);
            gpAdd.Controls.Add(txFLen);
            gpAdd.Controls.Add(label4);
            gpAdd.Controls.Add(cbFType);
            gpAdd.Controls.Add(label3);
            gpAdd.Controls.Add(txFieldName);
            gpAdd.Controls.Add(label2);
            gpAdd.Enabled = false;
            gpAdd.Location = new Point(12, 71);
            gpAdd.Name = "gpAdd";
            gpAdd.Size = new Size(354, 113);
            gpAdd.TabIndex = 3;
            gpAdd.TabStop = false;
            gpAdd.Text = "Add Field";
            // 
            // btAdd
            // 
            btAdd.Location = new Point(207, 81);
            btAdd.Name = "btAdd";
            btAdd.Size = new Size(75, 23);
            btAdd.TabIndex = 11;
            btAdd.Text = "Add";
            btAdd.UseVisualStyleBackColor = true;
            btAdd.Click += btAdd_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(152, 86);
            label5.Name = "label5";
            label5.Size = new Size(35, 15);
            label5.TabIndex = 10;
            label5.Text = "bytes";
            // 
            // txFLen
            // 
            txFLen.Location = new Point(91, 82);
            txFLen.Name = "txFLen";
            txFLen.Size = new Size(59, 23);
            txFLen.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 86);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 8;
            label4.Text = "Length";
            label4.Click += label4_Click;
            // 
            // cbFType
            // 
            cbFType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbFType.FormattingEnabled = true;
            cbFType.Location = new Point(91, 53);
            cbFType.Name = "cbFType";
            cbFType.Size = new Size(191, 23);
            cbFType.TabIndex = 7;
            cbFType.SelectedIndexChanged += cbFType_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 56);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 6;
            label3.Text = "Data Type";
            // 
            // txFieldName
            // 
            txFieldName.Location = new Point(91, 24);
            txFieldName.Name = "txFieldName";
            txFieldName.Size = new Size(191, 23);
            txFieldName.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 29);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 4;
            label2.Text = "Field Name";
            // 
            // button2
            // 
            button2.Location = new Point(12, 190);
            button2.Name = "button2";
            button2.Size = new Size(62, 23);
            button2.TabIndex = 12;
            button2.Text = "Order +";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btOrderUp_Click;
            // 
            // button3
            // 
            button3.Location = new Point(80, 190);
            button3.Name = "button3";
            button3.Size = new Size(62, 23);
            button3.TabIndex = 13;
            button3.Text = "Order -";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btOrderDown_Click;
            // 
            // button4
            // 
            button4.Location = new Point(148, 190);
            button4.Name = "button4";
            button4.Size = new Size(62, 23);
            button4.TabIndex = 14;
            button4.Text = "Remove";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btRemove_Click;
            // 
            // btSave
            // 
            btSave.Enabled = false;
            btSave.Location = new Point(10, 470);
            btSave.Name = "btSave";
            btSave.Size = new Size(356, 36);
            btSave.TabIndex = 15;
            btSave.Text = "Save Struct";
            btSave.UseVisualStyleBackColor = true;
            btSave.Click += btSave_Click;
            // 
            // txStrAddress
            // 
            txStrAddress.Location = new Point(105, 47);
            txStrAddress.Name = "txStrAddress";
            txStrAddress.Size = new Size(105, 23);
            txStrAddress.TabIndex = 13;
            txStrAddress.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 50);
            label6.Name = "label6";
            label6.Size = new Size(76, 15);
            label6.TabIndex = 12;
            label6.Text = "Base Address";
            // 
            // DefineStruct
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(378, 519);
            Controls.Add(txStrAddress);
            Controls.Add(label6);
            Controls.Add(btSave);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(gpAdd);
            Controls.Add(txStrName);
            Controls.Add(label1);
            Controls.Add(dataGrid);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DefineStruct";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DefineStruct";
            ((System.ComponentModel.ISupportInitialize)dataGrid).EndInit();
            gpAdd.ResumeLayout(false);
            gpAdd.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGrid;
        private Label label1;
        private TextBox txStrName;
        private GroupBox gpAdd;
        private TextBox txFieldName;
        private Label label2;
        private Button btAdd;
        private Label label5;
        private TextBox txFLen;
        private Label label4;
        private ComboBox cbFType;
        private Label label3;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button btSave;
        private DataGridViewTextBoxColumn Order;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private TextBox txStrAddress;
        private Label label6;
    }
}