namespace MIPS.Net.VGA
{
    partial class MainForm
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
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            statusDisplay = new Panel();
            statusKeyBoard = new Panel();
            statusMouse = new Panel();
            screen = new Panel();
            btnPorts = new Button();
            btnTurnOnOff = new Button();
            lbMsKeyboard = new Label();
            lbMSDisplay = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox3.BackgroundImage = Properties.Resources.seta;
            pictureBox3.BackgroundImageLayout = ImageLayout.Center;
            pictureBox3.Location = new Point(219, 414);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(28, 23);
            pictureBox3.TabIndex = 17;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox2.BackgroundImage = Properties.Resources.teclado;
            pictureBox2.BackgroundImageLayout = ImageLayout.Center;
            pictureBox2.Location = new Point(117, 414);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(28, 23);
            pictureBox2.TabIndex = 16;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox1.BackgroundImage = Properties.Resources.monitor;
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(25, 416);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(28, 23);
            pictureBox1.TabIndex = 15;
            pictureBox1.TabStop = false;
            // 
            // statusDisplay
            // 
            statusDisplay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusDisplay.BackColor = SystemColors.ActiveCaptionText;
            statusDisplay.Location = new Point(12, 422);
            statusDisplay.Name = "statusDisplay";
            statusDisplay.Size = new Size(11, 11);
            statusDisplay.TabIndex = 18;
            // 
            // statusKeyBoard
            // 
            statusKeyBoard.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusKeyBoard.BackColor = SystemColors.ActiveCaptionText;
            statusKeyBoard.Location = new Point(104, 422);
            statusKeyBoard.Name = "statusKeyBoard";
            statusKeyBoard.Size = new Size(11, 11);
            statusKeyBoard.TabIndex = 19;
            // 
            // statusMouse
            // 
            statusMouse.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusMouse.BackColor = SystemColors.ActiveCaptionText;
            statusMouse.Location = new Point(207, 422);
            statusMouse.Name = "statusMouse";
            statusMouse.Size = new Size(11, 11);
            statusMouse.TabIndex = 20;
            // 
            // screen
            // 
            screen.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            screen.BackColor = SystemColors.ActiveCaptionText;
            screen.Location = new Point(2, 1);
            screen.Name = "screen";
            screen.Size = new Size(600, 400);
            screen.TabIndex = 21;
            screen.Paint += screen_Paint;
            screen.MouseEnter += screen_MouseEnter;
            screen.MouseLeave += screen_MouseLeave;
            screen.MouseMove += MOUSE_MOVE;
            // 
            // btnPorts
            // 
            btnPorts.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPorts.Location = new Point(425, 414);
            btnPorts.Name = "btnPorts";
            btnPorts.Size = new Size(53, 23);
            btnPorts.TabIndex = 22;
            btnPorts.Text = "Config";
            btnPorts.UseVisualStyleBackColor = true;
            btnPorts.Click += btnPorts_Click;
            // 
            // btnTurnOnOff
            // 
            btnTurnOnOff.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTurnOnOff.BackColor = Color.SeaGreen;
            btnTurnOnOff.FlatStyle = FlatStyle.Flat;
            btnTurnOnOff.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTurnOnOff.ForeColor = Color.White;
            btnTurnOnOff.Location = new Point(511, 414);
            btnTurnOnOff.Name = "btnTurnOnOff";
            btnTurnOnOff.Size = new Size(79, 23);
            btnTurnOnOff.TabIndex = 23;
            btnTurnOnOff.Text = "TURN ON";
            btnTurnOnOff.UseVisualStyleBackColor = false;
            btnTurnOnOff.Click += TURN_ON_OFF;
            btnTurnOnOff.MouseEnter += btnTurnOnOff_MouseEnter;
            btnTurnOnOff.MouseLeave += btnTurnOnOff_MouseLeave;
            // 
            // lbMsKeyboard
            // 
            lbMsKeyboard.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbMsKeyboard.AutoSize = true;
            lbMsKeyboard.Location = new Point(151, 420);
            lbMsKeyboard.Name = "lbMsKeyboard";
            lbMsKeyboard.Size = new Size(29, 15);
            lbMsKeyboard.TabIndex = 24;
            lbMsKeyboard.Text = "0ms";
            // 
            // lbMSDisplay
            // 
            lbMSDisplay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lbMSDisplay.AutoSize = true;
            lbMSDisplay.Location = new Point(56, 420);
            lbMSDisplay.Name = "lbMSDisplay";
            lbMSDisplay.Size = new Size(29, 15);
            lbMSDisplay.TabIndex = 25;
            lbMSDisplay.Text = "0ms";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(602, 443);
            Controls.Add(lbMSDisplay);
            Controls.Add(lbMsKeyboard);
            Controls.Add(btnTurnOnOff);
            Controls.Add(btnPorts);
            Controls.Add(screen);
            Controls.Add(statusMouse);
            Controls.Add(statusKeyBoard);
            Controls.Add(statusDisplay);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIPS.Net Desktop -- All-In-One multi device: display, keyboard and mouse";
            Load += Form1_Load;
            KeyDown += KEYBOARD_INPUT;
            PreviewKeyDown += MainForm_PreviewKeyDown;
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public PictureBox pictureBox3;
        public PictureBox pictureBox2;
        public PictureBox pictureBox1;
        public Panel statusDisplay;
        public Panel statusKeyBoard;
        public Panel statusMouse;
        public Panel screen;
        private Button btnPorts;
        private Button btnTurnOnOff;
        private Label lbMsKeyboard;
        private Label lbMSDisplay;
    }
}
