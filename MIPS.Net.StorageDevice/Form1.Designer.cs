namespace MIPS.Net.StorageDevice
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
            txRoot = new TextBox();
            label1 = new Label();
            liSelPath = new LinkLabel();
            label2 = new Label();
            txPort = new TextBox();
            txDevType = new TextBox();
            label3 = new Label();
            button1 = new Button();
            label4 = new Label();
            txBuffer = new TextBox();
            panelConn = new Panel();
            listBox1 = new ListBox();
            SuspendLayout();
            // 
            // txRoot
            // 
            txRoot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txRoot.Location = new Point(12, 26);
            txRoot.Multiline = true;
            txRoot.Name = "txRoot";
            txRoot.ReadOnly = true;
            txRoot.Size = new Size(319, 46);
            txRoot.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 8);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 1;
            label1.Text = "Root path";
            // 
            // liSelPath
            // 
            liSelPath.AutoSize = true;
            liSelPath.Location = new Point(274, 9);
            liSelPath.Name = "liSelPath";
            liSelPath.Size = new Size(57, 15);
            liSelPath.TabIndex = 3;
            liSelPath.TabStop = true;
            liSelPath.Text = "Change...";
            liSelPath.LinkClicked += liSelPath_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 81);
            label2.Name = "label2";
            label2.Size = new Size(73, 15);
            label2.TabIndex = 4;
            label2.Text = "System port:";
            // 
            // txPort
            // 
            txPort.Location = new Point(92, 78);
            txPort.Name = "txPort";
            txPort.Size = new Size(62, 23);
            txPort.TabIndex = 5;
            // 
            // txDevType
            // 
            txDevType.Location = new Point(90, 104);
            txDevType.Name = "txDevType";
            txDevType.Size = new Size(41, 23);
            txDevType.TabIndex = 7;
            txDevType.Text = "0x2D";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 107);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 6;
            label3.Text = "Device type:";
            // 
            // button1
            // 
            button1.Location = new Point(256, 81);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 9;
            button1.Text = "Connect";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 132);
            label4.Name = "label4";
            label4.Size = new Size(45, 15);
            label4.TabIndex = 10;
            label4.Text = "History";
            label4.Click += label4_Click;
            // 
            // txBuffer
            // 
            txBuffer.Location = new Point(231, 345);
            txBuffer.Name = "txBuffer";
            txBuffer.ReadOnly = true;
            txBuffer.Size = new Size(100, 23);
            txBuffer.TabIndex = 16;
            // 
            // panelConn
            // 
            panelConn.Location = new Point(243, 87);
            panelConn.Name = "panelConn";
            panelConn.Size = new Size(12, 12);
            panelConn.TabIndex = 17;
            // 
            // listBox1
            // 
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(11, 150);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(320, 184);
            listBox1.TabIndex = 18;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(343, 380);
            Controls.Add(listBox1);
            Controls.Add(panelConn);
            Controls.Add(txBuffer);
            Controls.Add(label4);
            Controls.Add(button1);
            Controls.Add(txDevType);
            Controls.Add(label3);
            Controls.Add(txPort);
            Controls.Add(label2);
            Controls.Add(liSelPath);
            Controls.Add(label1);
            Controls.Add(txRoot);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIPS.Net - Mocked Storage Device";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txRoot;
        private Label label1;
        private LinkLabel liSelPath;
        private Label label2;
        private TextBox txPort;
        private TextBox txDevType;
        private Label label3;
        private Button button1;
        private Label label4;
        private TextBox txBuffer;
        private Panel panelConn;
        private ListBox listBox1;
    }
}
