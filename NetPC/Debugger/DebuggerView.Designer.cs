namespace NetPC
{
    partial class DebuggerView
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridMem = new DataGridView();
            dataGridRegisters = new DataGridView();
            dataGridProgram = new DataGridView();
            txLabel = new TextBox();
            label1 = new Label();
            btnStepNext = new Button();
            button1 = new Button();
            label2 = new Label();
            label3 = new Label();
            button2 = new Button();
            label4 = new Label();
            button3 = new Button();
            Column7 = new DataGridViewTextBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridMem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridRegisters).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridProgram).BeginInit();
            SuspendLayout();
            // 
            // dataGridMem
            // 
            dataGridMem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dataGridMem.BorderStyle = BorderStyle.Fixed3D;
            dataGridMem.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridMem.Location = new Point(0, 142);
            dataGridMem.Name = "dataGridMem";
            dataGridMem.Size = new Size(240, 491);
            dataGridMem.TabIndex = 2;
            // 
            // dataGridRegisters
            // 
            dataGridRegisters.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dataGridRegisters.BorderStyle = BorderStyle.Fixed3D;
            dataGridRegisters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridRegisters.Location = new Point(246, 142);
            dataGridRegisters.Name = "dataGridRegisters";
            dataGridRegisters.Size = new Size(240, 491);
            dataGridRegisters.TabIndex = 3;
            // 
            // dataGridProgram
            // 
            dataGridProgram.AllowUserToAddRows = false;
            dataGridProgram.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridProgram.BorderStyle = BorderStyle.Fixed3D;
            dataGridProgram.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridProgram.Columns.AddRange(new DataGridViewColumn[] { Column7, Column1, Column5, Column6 });
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = Color.Goldenrod;
            dataGridViewCellStyle1.SelectionForeColor = Color.Black;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dataGridProgram.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridProgram.Location = new Point(492, 100);
            dataGridProgram.MultiSelect = false;
            dataGridProgram.Name = "dataGridProgram";
            dataGridProgram.RowHeadersVisible = false;
            dataGridProgram.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridProgram.Size = new Size(565, 533);
            dataGridProgram.TabIndex = 4;
            // 
            // txLabel
            // 
            txLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txLabel.Location = new Point(592, 70);
            txLabel.Name = "txLabel";
            txLabel.ReadOnly = true;
            txLabel.Size = new Size(452, 23);
            txLabel.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(492, 74);
            label1.Name = "label1";
            label1.Size = new Size(95, 15);
            label1.TabIndex = 6;
            label1.Text = "CURRENT LABEL";
            // 
            // btnStepNext
            // 
            btnStepNext.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnStepNext.Location = new Point(492, 30);
            btnStepNext.Name = "btnStepNext";
            btnStepNext.Size = new Size(93, 32);
            btnStepNext.TabIndex = 7;
            btnStepNext.Text = "Step Next >";
            btnStepNext.UseVisualStyleBackColor = true;
            btnStepNext.Click += button1_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Gray;
            button1.BackgroundImage = Properties.Resources.extract_data;
            button1.BackgroundImageLayout = ImageLayout.Center;
            button1.FlatAppearance.BorderColor = Color.LightGray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(108, 30);
            button1.Name = "button1";
            button1.Size = new Size(37, 35);
            button1.TabIndex = 8;
            button1.TextAlign = ContentAlignment.BottomCenter;
            button1.TextImageRelation = TextImageRelation.TextAboveImage;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(108, 68);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 9;
            label2.Text = "Extract";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(12, 70);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 11;
            label3.Text = "Go To\r\n";
            // 
            // button2
            // 
            button2.BackColor = Color.Gray;
            button2.BackgroundImage = Properties.Resources.go_to_addr1;
            button2.BackgroundImageLayout = ImageLayout.Center;
            button2.FlatAppearance.BorderColor = Color.LightGray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(12, 30);
            button2.Name = "button2";
            button2.Size = new Size(37, 35);
            button2.TabIndex = 10;
            button2.TextAlign = ContentAlignment.BottomCenter;
            button2.TextImageRelation = TextImageRelation.TextAboveImage;
            button2.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(60, 70);
            label4.Name = "label4";
            label4.Size = new Size(35, 30);
            label4.TabIndex = 13;
            label4.Text = "Write\r\ndata";
            // 
            // button3
            // 
            button3.BackColor = Color.Gray;
            button3.BackgroundImage = Properties.Resources.edit1;
            button3.BackgroundImageLayout = ImageLayout.Center;
            button3.FlatAppearance.BorderColor = Color.LightGray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(60, 30);
            button3.Name = "button3";
            button3.Size = new Size(37, 35);
            button3.TabIndex = 12;
            button3.TextAlign = ContentAlignment.BottomCenter;
            button3.TextImageRelation = TextImageRelation.TextAboveImage;
            button3.UseVisualStyleBackColor = false;
            // 
            // Column7
            // 
            Column7.DataPropertyName = "Address";
            Column7.HeaderText = "Address";
            Column7.Name = "Column7";
            // 
            // Column1
            // 
            Column1.DataPropertyName = "InstructionCode";
            Column1.HeaderText = "Instr.";
            Column1.Name = "Column1";
            Column1.Width = 80;
            // 
            // Column5
            // 
            Column5.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column5.DataPropertyName = "SrcCode";
            Column5.HeaderText = "SourceOrigin";
            Column5.Name = "Column5";
            // 
            // Column6
            // 
            Column6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column6.DataPropertyName = "SrcFile";
            Column6.HeaderText = "SourceFile";
            Column6.Name = "Column6";
            // 
            // DebuggerView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            ClientSize = new Size(1056, 633);
            Controls.Add(label4);
            Controls.Add(button3);
            Controls.Add(label3);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(btnStepNext);
            Controls.Add(label1);
            Controls.Add(txLabel);
            Controls.Add(dataGridProgram);
            Controls.Add(dataGridRegisters);
            Controls.Add(dataGridMem);
            Name = "DebuggerView";
            Text = "DebuggerView";
            FormClosing += DebuggerView_FormClosing;
            Load += DebuggerView_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridMem).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridRegisters).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridProgram).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public DataGridView dataGridMem;
        public DataGridView dataGridRegisters;
        public DataGridView dataGridProgram;
        private TextBox txLabel;
        private Label label1;
        public Button btnStepNext;
        private Button button1;
        private Label label2;
        private Label label3;
        private Button button2;
        private Label label4;
        private Button button3;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
    }
}