namespace NetPC
{
    partial class MachineConfigs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MachineConfigs));
            label1 = new Label();
            label2 = new Label();
            txName = new TextBox();
            txMem = new NumericUpDown();
            label3 = new Label();
            rdoBytes = new RadioButton();
            rdoMegaB = new RadioButton();
            label4 = new Label();
            btAddPort = new Button();
            btRemovePort = new Button();
            dataGridPorts = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            dataGridButtons = new DataGridView();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            label6 = new Label();
            txFirmware = new TextBox();
            label5 = new Label();
            lbSelectFirmware = new LinkLabel();
            btnSave = new Button();
            btnNew = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)txMem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridPorts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridButtons).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(12, 56);
            label1.Name = "label1";
            label1.Size = new Size(123, 20);
            label1.TabIndex = 1;
            label1.Text = "Machine Configs";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 87);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 2;
            label2.Text = "Name";
            // 
            // txName
            // 
            txName.Location = new Point(12, 105);
            txName.Name = "txName";
            txName.Size = new Size(209, 23);
            txName.TabIndex = 3;
            // 
            // txMem
            // 
            txMem.Location = new Point(227, 105);
            txMem.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            txMem.Name = "txMem";
            txMem.Size = new Size(93, 23);
            txMem.TabIndex = 4;
            txMem.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(224, 87);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 5;
            label3.Text = "Memory";
            // 
            // rdoBytes
            // 
            rdoBytes.AutoSize = true;
            rdoBytes.Location = new Point(326, 87);
            rdoBytes.Name = "rdoBytes";
            rdoBytes.Size = new Size(53, 19);
            rdoBytes.TabIndex = 6;
            rdoBytes.Text = "bytes";
            rdoBytes.UseVisualStyleBackColor = true;
            // 
            // rdoMegaB
            // 
            rdoMegaB.AutoSize = true;
            rdoMegaB.Checked = true;
            rdoMegaB.Location = new Point(326, 107);
            rdoMegaB.Name = "rdoMegaB";
            rdoMegaB.Size = new Size(83, 19);
            rdoMegaB.TabIndex = 7;
            rdoMegaB.TabStop = true;
            rdoMegaB.Text = "megabytes";
            rdoMegaB.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 251);
            label4.Name = "label4";
            label4.Size = new Size(34, 15);
            label4.TabIndex = 8;
            label4.Text = "Ports";
            // 
            // btAddPort
            // 
            btAddPort.Image = Properties.Resources.add_1;
            btAddPort.ImageAlign = ContentAlignment.MiddleLeft;
            btAddPort.Location = new Point(79, 246);
            btAddPort.Name = "btAddPort";
            btAddPort.Size = new Size(56, 26);
            btAddPort.TabIndex = 10;
            btAddPort.Text = "add";
            btAddPort.TextAlign = ContentAlignment.MiddleRight;
            btAddPort.UseVisualStyleBackColor = true;
            // 
            // btRemovePort
            // 
            btRemovePort.Image = Properties.Resources.remove;
            btRemovePort.ImageAlign = ContentAlignment.MiddleLeft;
            btRemovePort.Location = new Point(143, 246);
            btRemovePort.Name = "btRemovePort";
            btRemovePort.Size = new Size(71, 26);
            btRemovePort.TabIndex = 11;
            btRemovePort.Text = "remove";
            btRemovePort.TextAlign = ContentAlignment.MiddleRight;
            btRemovePort.UseVisualStyleBackColor = true;
            // 
            // dataGridPorts
            // 
            dataGridPorts.AllowUserToAddRows = false;
            dataGridPorts.AllowUserToDeleteRows = false;
            dataGridPorts.AllowUserToOrderColumns = true;
            dataGridPorts.BackgroundColor = SystemColors.ButtonFace;
            dataGridPorts.BorderStyle = BorderStyle.Fixed3D;
            dataGridPorts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridPorts.Columns.AddRange(new DataGridViewColumn[] { Column1, Column5 });
            dataGridPorts.Location = new Point(9, 276);
            dataGridPorts.Name = "dataGridPorts";
            dataGridPorts.ReadOnly = true;
            dataGridPorts.RowHeadersVisible = false;
            dataGridPorts.Size = new Size(205, 223);
            dataGridPorts.TabIndex = 13;
            // 
            // Column1
            // 
            Column1.HeaderText = "HWID";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 50;
            // 
            // Column5
            // 
            Column5.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column5.HeaderText = "Description";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            // 
            // dataGridButtons
            // 
            dataGridButtons.AllowUserToAddRows = false;
            dataGridButtons.AllowUserToDeleteRows = false;
            dataGridButtons.AllowUserToOrderColumns = true;
            dataGridButtons.BackgroundColor = SystemColors.ButtonFace;
            dataGridButtons.BorderStyle = BorderStyle.Fixed3D;
            dataGridButtons.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridButtons.Columns.AddRange(new DataGridViewColumn[] { Column2, Column3, Column4 });
            dataGridButtons.Location = new Point(220, 276);
            dataGridButtons.Name = "dataGridButtons";
            dataGridButtons.ReadOnly = true;
            dataGridButtons.RowHeadersVisible = false;
            dataGridButtons.Size = new Size(226, 223);
            dataGridButtons.TabIndex = 17;
            // 
            // Column2
            // 
            Column2.HeaderText = "Icon";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 50;
            // 
            // Column3
            // 
            Column3.HeaderText = "HWID";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 50;
            // 
            // Column4
            // 
            Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column4.HeaderText = "Description";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(220, 251);
            label6.Name = "label6";
            label6.Size = new Size(71, 15);
            label6.TabIndex = 14;
            label6.Text = "HW Buttons";
            // 
            // txFirmware
            // 
            txFirmware.Location = new Point(12, 153);
            txFirmware.Multiline = true;
            txFirmware.Name = "txFirmware";
            txFirmware.Size = new Size(437, 38);
            txFirmware.TabIndex = 19;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 135);
            label5.Name = "label5";
            label5.Size = new Size(115, 15);
            label5.TabIndex = 18;
            label5.Text = "Firmware startup file";
            // 
            // lbSelectFirmware
            // 
            lbSelectFirmware.AutoSize = true;
            lbSelectFirmware.Location = new Point(326, 135);
            lbSelectFirmware.Name = "lbSelectFirmware";
            lbSelectFirmware.Size = new Size(120, 15);
            lbSelectFirmware.TabIndex = 20;
            lbSelectFirmware.TabStop = true;
            lbSelectFirmware.Text = "Select Firmware File...";
            // 
            // btnSave
            // 
            btnSave.Image = Properties.Resources.check_box;
            btnSave.ImageAlign = ContentAlignment.MiddleLeft;
            btnSave.Location = new Point(319, 12);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(130, 28);
            btnSave.TabIndex = 21;
            btnSave.Text = "Save this machine";
            btnSave.TextAlign = ContentAlignment.MiddleRight;
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            btnNew.Image = Properties.Resources.plus1;
            btnNew.ImageAlign = ContentAlignment.MiddleLeft;
            btnNew.Location = new Point(12, 12);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(56, 28);
            btnNew.TabIndex = 22;
            btnNew.Text = "New";
            btnNew.TextAlign = ContentAlignment.MiddleRight;
            btnNew.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Image = Properties.Resources.open;
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.Location = new Point(74, 12);
            button1.Name = "button1";
            button1.Size = new Size(72, 28);
            button1.TabIndex = 24;
            button1.Text = "Open...";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Image = Properties.Resources.copy;
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.Location = new Point(152, 12);
            button2.Name = "button2";
            button2.Size = new Size(99, 28);
            button2.TabIndex = 25;
            button2.Text = "Copy from...";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Image = Properties.Resources.remove;
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.Location = new Point(375, 246);
            button3.Name = "button3";
            button3.Size = new Size(71, 26);
            button3.TabIndex = 27;
            button3.Text = "remove";
            button3.TextAlign = ContentAlignment.MiddleRight;
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Image = Properties.Resources.add_1;
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.Location = new Point(311, 246);
            button4.Name = "button4";
            button4.Size = new Size(56, 26);
            button4.TabIndex = 26;
            button4.Text = "add";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.UseVisualStyleBackColor = true;
            // 
            // MachineConfigs
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(460, 520);
            Controls.Add(button3);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btnNew);
            Controls.Add(btnSave);
            Controls.Add(lbSelectFirmware);
            Controls.Add(txFirmware);
            Controls.Add(label5);
            Controls.Add(dataGridButtons);
            Controls.Add(label6);
            Controls.Add(dataGridPorts);
            Controls.Add(btRemovePort);
            Controls.Add(btAddPort);
            Controls.Add(label4);
            Controls.Add(rdoMegaB);
            Controls.Add(rdoBytes);
            Controls.Add(label3);
            Controls.Add(txMem);
            Controls.Add(txName);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MachineConfigs";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Machine Definitions";
            ((System.ComponentModel.ISupportInitialize)txMem).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridPorts).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridButtons).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private TextBox txName;
        private NumericUpDown txMem;
        private Label label3;
        private RadioButton rdoBytes;
        private RadioButton rdoMegaB;
        private Label label4;
        private Button btAddPort;
        private Button btRemovePort;
        private DataGridView dataGridPorts;
        private DataGridView dataGridButtons;
        private Label label6;
        private TextBox txFirmware;
        private Label label5;
        private LinkLabel lbSelectFirmware;
        private Button btnSave;
        private Button btnNew;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
    }
}