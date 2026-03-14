namespace NetPC.Debugger
{
    partial class ExtractTuple
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
            panelMem = new Panel();
            lbBaseAddr = new Label();
            lbFinalAddr = new Label();
            dataGridValues = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            label1 = new Label();
            label2 = new Label();
            listStructs = new ListBox();
            btnAddStruct = new Button();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridValues).BeginInit();
            SuspendLayout();
            // 
            // panelMem
            // 
            panelMem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelMem.BackColor = Color.LightCyan;
            panelMem.Location = new Point(12, 12);
            panelMem.Name = "panelMem";
            panelMem.Size = new Size(721, 50);
            panelMem.TabIndex = 0;
            // 
            // lbBaseAddr
            // 
            lbBaseAddr.AutoSize = true;
            lbBaseAddr.Location = new Point(12, 65);
            lbBaseAddr.Name = "lbBaseAddr";
            lbBaseAddr.Size = new Size(13, 15);
            lbBaseAddr.TabIndex = 1;
            lbBaseAddr.Text = "0";
            // 
            // lbFinalAddr
            // 
            lbFinalAddr.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbFinalAddr.AutoSize = true;
            lbFinalAddr.Location = new Point(676, 65);
            lbFinalAddr.Name = "lbFinalAddr";
            lbFinalAddr.Size = new Size(13, 15);
            lbFinalAddr.TabIndex = 0;
            lbFinalAddr.Text = "0";
            // 
            // dataGridValues
            // 
            dataGridValues.AllowUserToAddRows = false;
            dataGridValues.AllowUserToDeleteRows = false;
            dataGridValues.AllowUserToOrderColumns = true;
            dataGridValues.AllowUserToResizeColumns = false;
            dataGridValues.AllowUserToResizeRows = false;
            dataGridValues.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridValues.BackgroundColor = SystemColors.ButtonFace;
            dataGridValues.BorderStyle = BorderStyle.Fixed3D;
            dataGridValues.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridValues.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dataGridValues.Location = new Point(235, 109);
            dataGridValues.Name = "dataGridValues";
            dataGridValues.ReadOnly = true;
            dataGridValues.RowHeadersVisible = false;
            dataGridValues.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridValues.Size = new Size(498, 368);
            dataGridValues.TabIndex = 1;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Name";
            Column1.HeaderText = "Field";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column2.DataPropertyName = "Value";
            Column2.HeaderText = "Value";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label1.Location = new Point(12, 87);
            label1.Name = "label1";
            label1.Size = new Size(72, 15);
            label1.TabIndex = 3;
            label1.Text = "Select struct";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.Location = new Point(235, 87);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 4;
            label2.Text = "Values";
            // 
            // listStructs
            // 
            listStructs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listStructs.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listStructs.FormattingEnabled = true;
            listStructs.ItemHeight = 21;
            listStructs.Location = new Point(12, 111);
            listStructs.Name = "listStructs";
            listStructs.Size = new Size(217, 340);
            listStructs.TabIndex = 2;
            listStructs.DoubleClick += listStructs_DoubleClick;
            // 
            // btnAddStruct
            // 
            btnAddStruct.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddStruct.Location = new Point(12, 454);
            btnAddStruct.Name = "btnAddStruct";
            btnAddStruct.Size = new Size(52, 23);
            btnAddStruct.TabIndex = 6;
            btnAddStruct.Text = "Add";
            btnAddStruct.UseVisualStyleBackColor = true;
            btnAddStruct.Click += btnAddStruct_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Location = new Point(70, 454);
            button3.Name = "button3";
            button3.Size = new Size(57, 23);
            button3.TabIndex = 7;
            button3.Text = "Edit";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btEditStruct_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Location = new Point(133, 454);
            button4.Name = "button4";
            button4.Size = new Size(64, 23);
            button4.TabIndex = 8;
            button4.Text = "Remove";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btRemoveStruct_Click;
            // 
            // ExtractTuple
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            ClientSize = new Size(745, 489);
            Controls.Add(lbFinalAddr);
            Controls.Add(lbBaseAddr);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(btnAddStruct);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(listStructs);
            Controls.Add(dataGridValues);
            Controls.Add(panelMem);
            Name = "ExtractTuple";
            Text = "ExtractTuple";
            Load += ExtractTuple_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridValues).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelMem;
        private DataGridView dataGridValues;
        private Label label1;
        private Label label2;
        private ListBox listStructs;
        private Label lbBaseAddr;
        private Label lbFinalAddr;
        private Button btnAddStruct;
        private Button button3;
        private Button button4;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
    }
}