namespace MIDP
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            midpDisplay1 = new MIDP.Display.MIDPDisplay();
            panel1 = new Panel();
            panel2 = new Panel();
            panel11 = new Panel();
            label3 = new Label();
            panel10 = new Panel();
            label8 = new Label();
            panel9 = new Panel();
            label7 = new Label();
            panel8 = new Panel();
            label6 = new Label();
            panel7 = new Panel();
            label5 = new Label();
            panel6 = new Panel();
            label4 = new Label();
            panel5 = new Panel();
            panel4 = new Panel();
            label2 = new Label();
            panel3 = new Panel();
            label1 = new Label();
            midpKeyboard1 = new MIDP.Keyboard.MIDPKeyboard();
            panel12 = new Panel();
            fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label9 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel11.SuspendLayout();
            panel10.SuspendLayout();
            panel9.SuspendLayout();
            panel8.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox1).BeginInit();
            SuspendLayout();
            // 
            // midpDisplay1
            // 
            midpDisplay1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            midpDisplay1.BackColor = Color.Black;
            midpDisplay1.Location = new Point(12, 18);
            midpDisplay1.Name = "midpDisplay1";
            midpDisplay1.Size = new Size(290, 305);
            midpDisplay1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.BackColor = Color.Goldenrod;
            panel1.Controls.Add(panel2);
            panel1.Location = new Point(667, 48);
            panel1.Name = "panel1";
            panel1.Size = new Size(270, 442);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.Teal;
            panel2.Controls.Add(panel11);
            panel2.Controls.Add(panel10);
            panel2.Controls.Add(panel9);
            panel2.Controls.Add(panel8);
            panel2.Controls.Add(panel7);
            panel2.Controls.Add(panel6);
            panel2.Controls.Add(panel5);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panel3);
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(264, 436);
            panel2.TabIndex = 2;
            // 
            // panel11
            // 
            panel11.BackColor = Color.Black;
            panel11.Controls.Add(label3);
            panel11.Location = new Point(179, 278);
            panel11.Name = "panel11";
            panel11.Size = new Size(72, 60);
            panel11.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(4, 13);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 0;
            label3.Text = "KEYBOARD";
            // 
            // panel10
            // 
            panel10.BackColor = Color.Black;
            panel10.Controls.Add(label8);
            panel10.Location = new Point(14, 112);
            panel10.Name = "panel10";
            panel10.Size = new Size(237, 52);
            panel10.TabIndex = 3;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.White;
            label8.Location = new Point(13, 13);
            label8.Name = "label8";
            label8.Size = new Size(25, 15);
            label8.TabIndex = 0;
            label8.Text = "I/O";
            // 
            // panel9
            // 
            panel9.BackColor = Color.Black;
            panel9.Controls.Add(label7);
            panel9.Location = new Point(179, 199);
            panel9.Name = "panel9";
            panel9.Size = new Size(72, 60);
            panel9.TabIndex = 6;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.White;
            label7.Location = new Point(15, 13);
            label7.Name = "label7";
            label7.Size = new Size(49, 15);
            label7.TabIndex = 0;
            label7.Text = "SOUND";
            // 
            // panel8
            // 
            panel8.BackColor = Color.Black;
            panel8.Controls.Add(label6);
            panel8.Location = new Point(14, 357);
            panel8.Name = "panel8";
            panel8.Size = new Size(72, 60);
            panel8.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.White;
            label6.Location = new Point(15, 13);
            label6.Name = "label6";
            label6.Size = new Size(33, 15);
            label6.TabIndex = 0;
            label6.Text = "GSM";
            // 
            // panel7
            // 
            panel7.BackColor = Color.Black;
            panel7.Controls.Add(label5);
            panel7.Location = new Point(14, 278);
            panel7.Name = "panel7";
            panel7.Size = new Size(72, 60);
            panel7.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(7, 13);
            label5.Name = "label5";
            label5.Size = new Size(59, 15);
            label5.TabIndex = 0;
            label5.Text = "STORAGE";
            // 
            // panel6
            // 
            panel6.BackColor = Color.Black;
            panel6.Controls.Add(label4);
            panel6.Location = new Point(14, 199);
            panel6.Name = "panel6";
            panel6.Size = new Size(72, 60);
            panel6.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(10, 13);
            label4.Name = "label4";
            label4.Size = new Size(54, 15);
            label4.TabIndex = 0;
            label4.Text = "DISPLAY";
            // 
            // panel5
            // 
            panel5.BackColor = Color.Black;
            panel5.Location = new Point(121, 164);
            panel5.Name = "panel5";
            panel5.Size = new Size(22, 253);
            panel5.TabIndex = 2;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Black;
            panel4.Controls.Add(label2);
            panel4.Location = new Point(152, 15);
            panel4.Name = "panel4";
            panel4.Size = new Size(99, 91);
            panel4.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(13, 13);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 0;
            label2.Text = "MEM";
            // 
            // panel3
            // 
            panel3.BackColor = Color.Black;
            panel3.Controls.Add(label1);
            panel3.Location = new Point(14, 15);
            panel3.Name = "panel3";
            panel3.Size = new Size(99, 91);
            panel3.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(13, 13);
            label1.Name = "label1";
            label1.Size = new Size(29, 15);
            label1.TabIndex = 0;
            label1.Text = "CPU";
            // 
            // midpKeyboard1
            // 
            midpKeyboard1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            midpKeyboard1.BackColor = SystemColors.ButtonShadow;
            midpKeyboard1.Location = new Point(12, 329);
            midpKeyboard1.Name = "midpKeyboard1";
            midpKeyboard1.Size = new Size(290, 233);
            midpKeyboard1.TabIndex = 2;
            // 
            // panel12
            // 
            panel12.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panel12.BackColor = Color.FromArgb(224, 224, 224);
            panel12.Controls.Add(midpKeyboard1);
            panel12.Controls.Add(midpDisplay1);
            panel12.Location = new Point(943, 12);
            panel12.Name = "panel12";
            panel12.Size = new Size(317, 580);
            panel12.TabIndex = 3;
            // 
            // fastColoredTextBox1
            // 
            fastColoredTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fastColoredTextBox1.AutoCompleteBracketsList = new char[]
    {
    '(',
    ')',
    '{',
    '}',
    '[',
    ']',
    '"',
    '"',
    '\'',
    '\''
    };
            fastColoredTextBox1.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\n^\\s*(case|default)\\s*[^:]*(?<range>:)\\s*(?<range>[^;]+);";
            fastColoredTextBox1.AutoScrollMinSize = new Size(179, 14);
            fastColoredTextBox1.BackBrush = null;
            fastColoredTextBox1.BackColor = Color.DimGray;
            fastColoredTextBox1.CharHeight = 14;
            fastColoredTextBox1.CharWidth = 8;
            fastColoredTextBox1.CommentPrefix = ";";
            fastColoredTextBox1.DefaultMarkerSize = 8;
            fastColoredTextBox1.DisabledColor = Color.FromArgb(100, 180, 180, 180);
            fastColoredTextBox1.Font = new Font("Courier New", 9.75F);
            fastColoredTextBox1.ForeColor = Color.White;
            fastColoredTextBox1.Hotkeys = resources.GetString("fastColoredTextBox1.Hotkeys");
            fastColoredTextBox1.IsReplaceMode = false;
            fastColoredTextBox1.Location = new Point(220, 48);
            fastColoredTextBox1.Name = "fastColoredTextBox1";
            fastColoredTextBox1.Paddings = new Padding(0);
            fastColoredTextBox1.SelectionColor = Color.FromArgb(60, 0, 0, 255);
            fastColoredTextBox1.ServiceColors = (FastColoredTextBoxNS.ServiceColors)resources.GetObject("fastColoredTextBox1.ServiceColors");
            fastColoredTextBox1.Size = new Size(441, 442);
            fastColoredTextBox1.TabIndex = 4;
            fastColoredTextBox1.Text = "fastColoredTextBox1";
            fastColoredTextBox1.Zoom = 100;
            fastColoredTextBox1.Load += fastColoredTextBox1_Load;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            flowLayoutPanel1.BackColor = Color.DimGray;
            flowLayoutPanel1.Location = new Point(12, 60);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(202, 430);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = Color.White;
            label9.Location = new Point(12, 42);
            label9.Name = "label9";
            label9.Size = new Size(69, 15);
            label9.TabIndex = 6;
            label9.Text = "Source Files";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(1272, 716);
            Controls.Add(label9);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(fastColoredTextBox1);
            Controls.Add(panel1);
            Controls.Add(panel12);
            Name = "Form1";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel11.ResumeLayout(false);
            panel11.PerformLayout();
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)fastColoredTextBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Display.MIDPDisplay midpDisplay1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel4;
        private Label label2;
        private Panel panel3;
        private Label label1;
        private Panel panel8;
        private Label label6;
        private Panel panel7;
        private Label label5;
        private Panel panel6;
        private Label label4;
        private Panel panel5;
        private Panel panel10;
        private Label label8;
        private Panel panel9;
        private Label label7;
        private Panel panel11;
        private Label label3;
        private Keyboard.MIDPKeyboard midpKeyboard1;
        private Panel panel12;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label9;
    }
}
