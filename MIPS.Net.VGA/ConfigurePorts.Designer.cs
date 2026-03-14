namespace MIPS.Net.VGA
{
    partial class ConfigurePorts
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
            label1 = new Label();
            txDisplay = new TextBox();
            txKeyboard = new TextBox();
            label2 = new Label();
            txMouse = new TextBox();
            label3 = new Label();
            btOk = new Button();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            domainUpDown1 = new DomainUpDown();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(68, 34);
            label1.Name = "label1";
            label1.Size = new Size(73, 15);
            label1.TabIndex = 0;
            label1.Text = "Display Port:";
            // 
            // txDisplay
            // 
            txDisplay.Location = new Point(147, 29);
            txDisplay.Name = "txDisplay";
            txDisplay.Size = new Size(100, 23);
            txDisplay.TabIndex = 6;
            // 
            // txKeyboard
            // 
            txKeyboard.Location = new Point(147, 158);
            txKeyboard.Name = "txKeyboard";
            txKeyboard.Size = new Size(100, 23);
            txKeyboard.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(56, 162);
            label2.Name = "label2";
            label2.Size = new Size(85, 15);
            label2.TabIndex = 7;
            label2.Text = "Keyboard Port:";
            // 
            // txMouse
            // 
            txMouse.Location = new Point(147, 199);
            txMouse.Name = "txMouse";
            txMouse.Size = new Size(100, 23);
            txMouse.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(70, 204);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 9;
            label3.Text = "Mouse Port:";
            // 
            // btOk
            // 
            btOk.Location = new Point(180, 262);
            btOk.Name = "btOk";
            btOk.Size = new Size(67, 23);
            btOk.TabIndex = 11;
            btOk.Text = "Ok";
            btOk.UseVisualStyleBackColor = true;
            btOk.Click += btOk_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.monitor;
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(22, 26);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(28, 23);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = Properties.Resources.teclado;
            pictureBox2.BackgroundImageLayout = ImageLayout.Center;
            pictureBox2.Location = new Point(22, 159);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(28, 23);
            pictureBox2.TabIndex = 13;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = Properties.Resources.seta;
            pictureBox3.BackgroundImageLayout = ImageLayout.Center;
            pictureBox3.Location = new Point(22, 199);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(28, 23);
            pictureBox3.TabIndex = 14;
            pictureBox3.TabStop = false;
            // 
            // domainUpDown1
            // 
            domainUpDown1.Location = new Point(137, 75);
            domainUpDown1.Name = "domainUpDown1";
            domainUpDown1.Size = new Size(120, 23);
            domainUpDown1.TabIndex = 15;
            domainUpDown1.Text = "0";
            // 
            // ConfigurePorts
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(259, 297);
            Controls.Add(domainUpDown1);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(btOk);
            Controls.Add(txMouse);
            Controls.Add(label3);
            Controls.Add(txKeyboard);
            Controls.Add(label2);
            Controls.Add(txDisplay);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfigurePorts";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configure Ports";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txDisplay;
        private TextBox txKeyboard;
        private Label label2;
        private TextBox txMouse;
        private Label label3;
        private Button btOk;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private DomainUpDown domainUpDown1;
    }
}