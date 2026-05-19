namespace NetPC
{
    partial class NetPCForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetPCForm));
            btTurnOnOff = new Button();
            sanDisplay1 = new SanDisplay();
            menuStrip1 = new MenuStrip();
            machineToolStripMenuItem = new ToolStripMenuItem();
            newMachineToolStripMenuItem = new ToolStripMenuItem();
            editMachinesToolStripMenuItem = new ToolStripMenuItem();
            panel1 = new Panel();
            panel7 = new Panel();
            panelIntr = new Panel();
            label7 = new Label();
            panelFFI = new Panel();
            panelClock = new Panel();
            panel6 = new Panel();
            panelMMU = new Panel();
            label6 = new Label();
            label3 = new Label();
            panel4 = new Panel();
            label4 = new Label();
            panel2 = new Panel();
            label2 = new Label();
            panel3 = new Panel();
            panel_io_interruption = new Panel();
            label1 = new Label();
            panel_mem = new Panel();
            panel5 = new Panel();
            label5 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            lbInstEnabled = new CheckBox();
            lbKeyboardPing = new Label();
            panel_io = new Panel();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel5.SuspendLayout();
            SuspendLayout();
            // 
            // btTurnOnOff
            // 
            btTurnOnOff.BackColor = SystemColors.ControlDark;
            btTurnOnOff.BackgroundImage = Properties.Resources.power_off;
            btTurnOnOff.BackgroundImageLayout = ImageLayout.Center;
            btTurnOnOff.FlatAppearance.BorderSize = 0;
            btTurnOnOff.FlatStyle = FlatStyle.Flat;
            btTurnOnOff.Location = new Point(698, 36);
            btTurnOnOff.Name = "btTurnOnOff";
            btTurnOnOff.Size = new Size(43, 42);
            btTurnOnOff.TabIndex = 1;
            btTurnOnOff.UseVisualStyleBackColor = false;
            btTurnOnOff.Click += btTurnOnOff_Click;
            // 
            // sanDisplay1
            // 
            sanDisplay1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            sanDisplay1.BackColor = Color.Black;
            sanDisplay1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            sanDisplay1.Location = new Point(47, 36);
            sanDisplay1.Name = "sanDisplay1";
            sanDisplay1.Size = new Size(645, 509);
            sanDisplay1.TabIndex = 2;
            sanDisplay1.KeyDown += sanDisplay1_KeyDown;
            sanDisplay1.KeyPress += sanDisplay1_KeyPress;
            sanDisplay1.MouseEnter += sanDisplay1_MouseEnter;
            sanDisplay1.MouseLeave += sanDisplay1_MouseLeave;
            sanDisplay1.PreviewKeyDown += sanDisplay1_PreviewKeyDown;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { machineToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1096, 24);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // machineToolStripMenuItem
            // 
            machineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newMachineToolStripMenuItem, editMachinesToolStripMenuItem });
            machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            machineToolStripMenuItem.Size = new Size(65, 20);
            machineToolStripMenuItem.Text = "Machine";
            // 
            // newMachineToolStripMenuItem
            // 
            newMachineToolStripMenuItem.Name = "newMachineToolStripMenuItem";
            newMachineToolStripMenuItem.Size = new Size(156, 22);
            newMachineToolStripMenuItem.Text = "New Machine...";
            newMachineToolStripMenuItem.Click += newMachineToolStripMenuItem_Click;
            // 
            // editMachinesToolStripMenuItem
            // 
            editMachinesToolStripMenuItem.Name = "editMachinesToolStripMenuItem";
            editMachinesToolStripMenuItem.Size = new Size(156, 22);
            editMachinesToolStripMenuItem.Text = "Edit Machines";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(panel7);
            panel1.Controls.Add(panelFFI);
            panel1.Controls.Add(panelClock);
            panel1.Controls.Add(panel6);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(panel4);
            panel1.Location = new Point(890, 163);
            panel1.Name = "panel1";
            panel1.Size = new Size(196, 165);
            panel1.TabIndex = 8;
            // 
            // panel7
            // 
            panel7.BackColor = Color.FromArgb(64, 64, 64);
            panel7.Controls.Add(panelIntr);
            panel7.Controls.Add(label7);
            panel7.Location = new Point(93, 3);
            panel7.Name = "panel7";
            panel7.Size = new Size(38, 39);
            panel7.TabIndex = 17;
            // 
            // panelIntr
            // 
            panelIntr.BackColor = Color.DimGray;
            panelIntr.Location = new Point(2, 30);
            panelIntr.Name = "panelIntr";
            panelIntr.Size = new Size(34, 6);
            panelIntr.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.ForeColor = SystemColors.ButtonFace;
            label7.Location = new Point(4, 6);
            label7.Name = "label7";
            label7.Size = new Size(30, 19);
            label7.TabIndex = 3;
            label7.Text = "INT";
            // 
            // panelFFI
            // 
            panelFFI.BackColor = Color.DimGray;
            panelFFI.Location = new Point(7, 33);
            panelFFI.Name = "panelFFI";
            panelFFI.Size = new Size(34, 6);
            panelFFI.TabIndex = 15;
            // 
            // panelClock
            // 
            panelClock.Location = new Point(57, 70);
            panelClock.Name = "panelClock";
            panelClock.Size = new Size(17, 17);
            panelClock.TabIndex = 5;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(64, 64, 64);
            panel6.Controls.Add(panelMMU);
            panel6.Controls.Add(label6);
            panel6.Location = new Point(49, 3);
            panel6.Name = "panel6";
            panel6.Size = new Size(38, 39);
            panel6.TabIndex = 4;
            // 
            // panelMMU
            // 
            panelMMU.BackColor = Color.DimGray;
            panelMMU.Location = new Point(2, 30);
            panelMMU.Name = "panelMMU";
            panelMMU.Size = new Size(34, 6);
            panelMMU.TabIndex = 16;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 8F);
            label6.ForeColor = SystemColors.ButtonFace;
            label6.Location = new Point(1, 8);
            label6.Name = "label6";
            label6.Size = new Size(35, 13);
            label6.TabIndex = 3;
            label6.Text = "MMU";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(77, 66);
            label3.Name = "label3";
            label3.Size = new Size(40, 21);
            label3.TabIndex = 2;
            label3.Text = "CPU";
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(64, 64, 64);
            panel4.Controls.Add(label4);
            panel4.Location = new Point(5, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(38, 39);
            panel4.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.ForeColor = SystemColors.ButtonFace;
            label4.Location = new Point(6, 7);
            label4.Name = "label4";
            label4.Size = new Size(27, 19);
            label4.TabIndex = 3;
            label4.Text = "FFI";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Black;
            panel2.Controls.Add(label2);
            panel2.Location = new Point(890, 385);
            panel2.Name = "panel2";
            panel2.Size = new Size(196, 83);
            panel2.TabIndex = 9;
            panel2.Paint += panel2_Paint;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(72, 29);
            label2.Name = "label2";
            label2.Size = new Size(54, 21);
            label2.TabIndex = 1;
            label2.Text = "R A M";
            // 
            // panel3
            // 
            panel3.BackColor = Color.Black;
            panel3.Controls.Add(panel_io_interruption);
            panel3.Controls.Add(label1);
            panel3.Location = new Point(776, 84);
            panel3.Name = "panel3";
            panel3.Size = new Size(62, 384);
            panel3.TabIndex = 10;
            // 
            // panel_io_interruption
            // 
            panel_io_interruption.BackColor = Color.Goldenrod;
            panel_io_interruption.Location = new Point(12, 180);
            panel_io_interruption.Name = "panel_io_interruption";
            panel_io_interruption.Size = new Size(31, 7);
            panel_io_interruption.TabIndex = 3;
            panel_io_interruption.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(12, 49);
            label1.Name = "label1";
            label1.Size = new Size(34, 105);
            label1.TabIndex = 0;
            label1.Text = "I/O\r\n\r\n B\r\n U\r\n S";
            // 
            // panel_mem
            // 
            panel_mem.BackColor = Color.SeaGreen;
            panel_mem.Location = new Point(890, 328);
            panel_mem.Name = "panel_mem";
            panel_mem.Size = new Size(196, 56);
            panel_mem.TabIndex = 11;
            panel_mem.Paint += panel_mem_Paint;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Black;
            panel5.Controls.Add(label5);
            panel5.Location = new Point(890, 84);
            panel5.Name = "panel5";
            panel5.Size = new Size(196, 50);
            panel5.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ButtonFace;
            label5.Location = new Point(74, 14);
            label5.Name = "label5";
            label5.Size = new Size(41, 21);
            label5.TabIndex = 2;
            label5.Text = "GPU";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Location = new Point(698, 84);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(72, 428);
            flowLayoutPanel1.TabIndex = 13;
            // 
            // lbInstEnabled
            // 
            lbInstEnabled.AutoSize = true;
            lbInstEnabled.ForeColor = Color.White;
            lbInstEnabled.Location = new Point(890, 27);
            lbInstEnabled.Name = "lbInstEnabled";
            lbInstEnabled.Size = new Size(178, 19);
            lbInstEnabled.TabIndex = 14;
            lbInstEnabled.Text = "ENABLE INSTRUMENTATION";
            lbInstEnabled.UseVisualStyleBackColor = true;
            lbInstEnabled.CheckedChanged += lbInstEnabled_CheckedChanged;
            // 
            // lbKeyboardPing
            // 
            lbKeyboardPing.AutoSize = true;
            lbKeyboardPing.Location = new Point(776, 480);
            lbKeyboardPing.Name = "lbKeyboardPing";
            lbKeyboardPing.Size = new Size(12, 15);
            lbKeyboardPing.TabIndex = 15;
            lbKeyboardPing.Text = "-";
            // 
            // panel_io
            // 
            panel_io.BackColor = Color.SeaGreen;
            panel_io.Location = new Point(839, 163);
            panel_io.Name = "panel_io";
            panel_io.Size = new Size(50, 305);
            panel_io.TabIndex = 12;
            panel_io.Paint += panel_io_Paint;
            // 
            // NetPCForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.SeaGreen;
            ClientSize = new Size(1096, 557);
            Controls.Add(panel_io);
            Controls.Add(lbKeyboardPing);
            Controls.Add(lbInstEnabled);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(panel5);
            Controls.Add(panel_mem);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(sanDisplay1);
            Controls.Add(btTurnOnOff);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NetPCForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Activated += Form1_Activated;
            FormClosing += Form1_FormClosing;
            Load += NetPCForm_Load;
            Move += Form1_Move;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btTurnOnOff;
        private SanDisplay sanDisplay1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem machineToolStripMenuItem;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel_mem;
        private Panel panel4;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label2;
        private Panel panel5;
        private Label label5;
        private FlowLayoutPanel flowLayoutPanel1;
        private Panel panel6;
        private Label label6;
        private CheckBox lbInstEnabled;
        private Panel panelClock;
        private Panel panelFFI;
        private Label lbKeyboardPing;
        private Panel panel_io_interruption;
        private Panel panel_io;
        private Panel panelMMU;
        private Panel panel7;
        private Panel panelIntr;
        private Label label7;
        private ToolStripMenuItem newMachineToolStripMenuItem;
        private ToolStripMenuItem editMachinesToolStripMenuItem;
    }
}
