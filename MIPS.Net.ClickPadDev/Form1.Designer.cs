namespace MIPS.Net.ClickPadDev
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
            button1 = new Button();
            button2 = new Button();
            panelPad = new Panel();
            label1 = new Label();
            lbOut = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Location = new Point(1, 237);
            button1.Name = "button1";
            button1.Size = new Size(137, 43);
            button1.TabIndex = 0;
            button1.Text = "Right Button";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.White;
            button2.Location = new Point(140, 237);
            button2.Name = "button2";
            button2.Size = new Size(138, 43);
            button2.TabIndex = 1;
            button2.Text = "Left Button";
            button2.UseVisualStyleBackColor = true;
            // 
            // panelPad
            // 
            panelPad.BackColor = Color.FromArgb(64, 64, 64);
            panelPad.Cursor = Cursors.Hand;
            panelPad.Location = new Point(12, 25);
            panelPad.Name = "panelPad";
            panelPad.Size = new Size(255, 206);
            panelPad.TabIndex = 2;
            panelPad.MouseClick += panelPad_MouseClick;
            panelPad.MouseDoubleClick += panelPad_MouseDoubleClick;
            panelPad.MouseMove += panelPad_MouseMove;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 7F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(12, 5);
            label1.Name = "label1";
            label1.Size = new Size(36, 12);
            label1.TabIndex = 3;
            label1.Text = "IN 0ms";
            // 
            // lbOut
            // 
            lbOut.AutoSize = true;
            lbOut.Font = new Font("Segoe UI", 7F);
            lbOut.ForeColor = Color.White;
            lbOut.Location = new Point(221, 6);
            lbOut.Name = "lbOut";
            lbOut.Size = new Size(46, 12);
            lbOut.TabIndex = 4;
            lbOut.Text = "OUT 0ms";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(279, 281);
            Controls.Add(lbOut);
            Controls.Add(label1);
            Controls.Add(panelPad);
            Controls.Add(button2);
            Controls.Add(button1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIPS.Net Desktop - ClickPad device";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Panel panelPad;
        private Label label1;
        private Label lbOut;
    }
}
