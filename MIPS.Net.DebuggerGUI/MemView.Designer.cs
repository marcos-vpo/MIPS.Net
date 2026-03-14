namespace MIPS.Net.DebuggerGUI
{
    partial class MemView
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            richTextBox1 = new RichTextBox();
            dataGridWatcher = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            label1 = new Label();
            label2 = new Label();
            richTextBox2 = new RichTextBox();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            lbTitl = new Label();
            rdoText = new RadioButton();
            rdoGrid = new RadioButton();
            dataGrid = new DataGridView();
            Column4 = new DataGridViewTextBoxColumn();
            Column20 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            Column9 = new DataGridViewTextBoxColumn();
            Column10 = new DataGridViewTextBoxColumn();
            Column11 = new DataGridViewTextBoxColumn();
            Column12 = new DataGridViewTextBoxColumn();
            Column13 = new DataGridViewTextBoxColumn();
            Column14 = new DataGridViewTextBoxColumn();
            Column15 = new DataGridViewTextBoxColumn();
            Column16 = new DataGridViewTextBoxColumn();
            Column17 = new DataGridViewTextBoxColumn();
            Column18 = new DataGridViewTextBoxColumn();
            Column19 = new DataGridViewTextBoxColumn();
            S = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridWatcher).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGrid).BeginInit();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Location = new Point(3, 25);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Size = new Size(499, 436);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // dataGridWatcher
            // 
            dataGridWatcher.AllowUserToAddRows = false;
            dataGridWatcher.AllowUserToDeleteRows = false;
            dataGridWatcher.AllowUserToOrderColumns = true;
            dataGridWatcher.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            dataGridWatcher.BackgroundColor = Color.White;
            dataGridWatcher.BorderStyle = BorderStyle.Fixed3D;
            dataGridWatcher.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridWatcher.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3 });
            dataGridWatcher.Location = new Point(3, 482);
            dataGridWatcher.Name = "dataGridWatcher";
            dataGridWatcher.ReadOnly = true;
            dataGridWatcher.RowHeadersVisible = false;
            dataGridWatcher.Size = new Size(299, 162);
            dataGridWatcher.TabIndex = 1;
            // 
            // Column1
            // 
            Column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column1.HeaderText = "Base";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column2.HeaderText = "Len";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // Column3
            // 
            Column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column3.HeaderText = "Type";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 464);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 2;
            label1.Text = "MEM WATCHER";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(308, 464);
            label2.Name = "label2";
            label2.Size = new Size(47, 15);
            label2.TabIndex = 3;
            label2.Text = "VALUES";
            // 
            // richTextBox2
            // 
            richTextBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            richTextBox2.Location = new Point(308, 482);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(194, 162);
            richTextBox2.TabIndex = 4;
            richTextBox2.Text = "";
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(154, 464);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(92, 15);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Add Watching...";
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(434, 7);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(64, 15);
            linkLabel2.TabIndex = 6;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Save to file";
            // 
            // lbTitl
            // 
            lbTitl.AutoSize = true;
            lbTitl.Location = new Point(3, 7);
            lbTitl.Name = "lbTitl";
            lbTitl.Size = new Size(92, 15);
            lbTitl.TabIndex = 7;
            lbTitl.Text = "MAIN MEMORY";
            // 
            // rdoText
            // 
            rdoText.AutoSize = true;
            rdoText.Location = new Point(118, 5);
            rdoText.Name = "rdoText";
            rdoText.Size = new Size(50, 19);
            rdoText.TabIndex = 8;
            rdoText.TabStop = true;
            rdoText.Text = "TEXT";
            rdoText.UseVisualStyleBackColor = true;
            rdoText.CheckedChanged += rdoText_CheckedChanged;
            // 
            // rdoGrid
            // 
            rdoGrid.AutoSize = true;
            rdoGrid.Location = new Point(174, 5);
            rdoGrid.Name = "rdoGrid";
            rdoGrid.Size = new Size(51, 19);
            rdoGrid.TabIndex = 9;
            rdoGrid.TabStop = true;
            rdoGrid.Text = "GRID";
            rdoGrid.UseVisualStyleBackColor = true;
            rdoGrid.CheckedChanged += rdoGrid_CheckedChanged;
            // 
            // dataGrid
            // 
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGrid.Columns.AddRange(new DataGridViewColumn[] { Column4, Column20, Column5, Column6, Column7, Column8, Column9, Column10, Column11, Column12, Column13, Column14, Column15, Column16, Column17, Column18, Column19, S });
            dataGrid.Location = new Point(3, 25);
            dataGrid.Name = "dataGrid";
            dataGrid.ReadOnly = true;
            dataGrid.RowHeadersVisible = false;
            dataGrid.Size = new Size(499, 436);
            dataGrid.TabIndex = 10;
            // 
            // Column4
            // 
            Column4.HeaderText = "BASE";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 80;
            // 
            // Column20
            // 
            Column20.HeaderText = "0";
            Column20.MinimumWidth = 25;
            Column20.Name = "Column20";
            Column20.ReadOnly = true;
            Column20.Width = 25;
            // 
            // Column5
            // 
            Column5.HeaderText = "1";
            Column5.MinimumWidth = 25;
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 25;
            // 
            // Column6
            // 
            Column6.HeaderText = "2";
            Column6.MinimumWidth = 25;
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            Column6.Width = 25;
            // 
            // Column7
            // 
            Column7.HeaderText = "3";
            Column7.MinimumWidth = 25;
            Column7.Name = "Column7";
            Column7.ReadOnly = true;
            Column7.Width = 25;
            // 
            // Column8
            // 
            Column8.HeaderText = "4";
            Column8.MinimumWidth = 25;
            Column8.Name = "Column8";
            Column8.ReadOnly = true;
            Column8.Width = 25;
            // 
            // Column9
            // 
            Column9.HeaderText = "5";
            Column9.MinimumWidth = 25;
            Column9.Name = "Column9";
            Column9.ReadOnly = true;
            Column9.Width = 25;
            // 
            // Column10
            // 
            Column10.HeaderText = "6";
            Column10.MinimumWidth = 25;
            Column10.Name = "Column10";
            Column10.ReadOnly = true;
            Column10.Width = 25;
            // 
            // Column11
            // 
            Column11.HeaderText = "7";
            Column11.MinimumWidth = 25;
            Column11.Name = "Column11";
            Column11.ReadOnly = true;
            Column11.Width = 25;
            // 
            // Column12
            // 
            Column12.HeaderText = "8";
            Column12.MinimumWidth = 25;
            Column12.Name = "Column12";
            Column12.ReadOnly = true;
            Column12.Width = 25;
            // 
            // Column13
            // 
            Column13.HeaderText = "9";
            Column13.MinimumWidth = 25;
            Column13.Name = "Column13";
            Column13.ReadOnly = true;
            Column13.Width = 25;
            // 
            // Column14
            // 
            Column14.HeaderText = "A";
            Column14.MinimumWidth = 25;
            Column14.Name = "Column14";
            Column14.ReadOnly = true;
            Column14.Width = 25;
            // 
            // Column15
            // 
            Column15.HeaderText = "B";
            Column15.MinimumWidth = 25;
            Column15.Name = "Column15";
            Column15.ReadOnly = true;
            Column15.Width = 25;
            // 
            // Column16
            // 
            Column16.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Column16.HeaderText = "C";
            Column16.MinimumWidth = 25;
            Column16.Name = "Column16";
            Column16.ReadOnly = true;
            Column16.Width = 25;
            // 
            // Column17
            // 
            Column17.HeaderText = "D";
            Column17.MinimumWidth = 25;
            Column17.Name = "Column17";
            Column17.ReadOnly = true;
            Column17.Width = 25;
            // 
            // Column18
            // 
            Column18.HeaderText = "E";
            Column18.MinimumWidth = 25;
            Column18.Name = "Column18";
            Column18.ReadOnly = true;
            Column18.Width = 25;
            // 
            // Column19
            // 
            Column19.HeaderText = "F";
            Column19.MinimumWidth = 25;
            Column19.Name = "Column19";
            Column19.ReadOnly = true;
            Column19.Width = 25;
            // 
            // S
            // 
            S.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            S.HeaderText = "STR";
            S.Name = "S";
            S.ReadOnly = true;
            // 
            // MemView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dataGrid);
            Controls.Add(rdoGrid);
            Controls.Add(rdoText);
            Controls.Add(lbTitl);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(richTextBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dataGridWatcher);
            Controls.Add(richTextBox1);
            Name = "MemView";
            Size = new Size(505, 647);
            ((System.ComponentModel.ISupportInitialize)dataGridWatcher).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private DataGridView dataGridWatcher;
        private Label label1;
        private Label label2;
        private RichTextBox richTextBox2;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private Label lbTitl;
        private RadioButton rdoText;
        private RadioButton rdoGrid;
        private DataGridView dataGrid;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column20;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewTextBoxColumn Column8;
        private DataGridViewTextBoxColumn Column9;
        private DataGridViewTextBoxColumn Column10;
        private DataGridViewTextBoxColumn Column11;
        private DataGridViewTextBoxColumn Column12;
        private DataGridViewTextBoxColumn Column13;
        private DataGridViewTextBoxColumn Column14;
        private DataGridViewTextBoxColumn Column15;
        private DataGridViewTextBoxColumn Column16;
        private DataGridViewTextBoxColumn Column17;
        private DataGridViewTextBoxColumn Column18;
        private DataGridViewTextBoxColumn Column19;
        private DataGridViewTextBoxColumn S;
    }
}
