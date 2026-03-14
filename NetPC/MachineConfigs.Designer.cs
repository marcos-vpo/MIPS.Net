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
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            txMem = new NumericUpDown();
            label3 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            label4 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            button2 = new Button();
            button3 = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)txMem).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(188, 23);
            button1.TabIndex = 0;
            button1.Text = "Open Existing Machine...";
            button1.UseVisualStyleBackColor = true;
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
            // textBox1
            // 
            textBox1.Location = new Point(12, 105);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(324, 23);
            textBox1.TabIndex = 3;
            // 
            // txMem
            // 
            txMem.Location = new Point(15, 155);
            txMem.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            txMem.Name = "txMem";
            txMem.Size = new Size(120, 23);
            txMem.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 137);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 5;
            label3.Text = "Memory";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(141, 159);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(32, 19);
            radioButton1.TabIndex = 6;
            radioButton1.TabStop = true;
            radioButton1.Text = "b";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(179, 159);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(43, 19);
            radioButton2.TabIndex = 7;
            radioButton2.TabStop = true;
            radioButton2.Text = "mb";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(15, 194);
            label4.Name = "label4";
            label4.Size = new Size(71, 15);
            label4.TabIndex = 8;
            label4.Text = "HW Buttons";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Location = new Point(15, 212);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(241, 238);
            flowLayoutPanel1.TabIndex = 9;
            // 
            // button2
            // 
            button2.Location = new Point(262, 212);
            button2.Name = "button2";
            button2.Size = new Size(56, 23);
            button2.TabIndex = 10;
            button2.Text = "add";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(262, 241);
            button3.Name = "button3";
            button3.Size = new Size(56, 23);
            button3.TabIndex = 11;
            button3.Text = "remove";
            button3.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Location = new Point(342, 56);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(155, 401);
            flowLayoutPanel2.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(43, 292);
            label5.Name = "label5";
            label5.Size = new Size(177, 60);
            label5.TabIndex = 12;
            label5.Text = "Hardware buttons appear in the \r\nemulator console and are \r\nprogrammed using \r\nassembly language.";
            // 
            // MachineConfigs
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(733, 469);
            Controls.Add(label5);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(label4);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(label3);
            Controls.Add(txMem);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "MachineConfigs";
            Text = "MachineConfigs";
            ((System.ComponentModel.ISupportInitialize)txMem).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private NumericUpDown txMem;
        private Label label3;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Label label4;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button button2;
        private Button button3;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label5;
    }
}