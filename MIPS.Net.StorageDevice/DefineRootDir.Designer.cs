namespace MIPS.Net.StorageDevice
{
    partial class DefineRootDir
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefineRootDir));
            txPath = new TextBox();
            label1 = new Label();
            liSelPath = new LinkLabel();
            btnOk = new Button();
            SuspendLayout();
            // 
            // txPath
            // 
            txPath.BackColor = Color.White;
            txPath.Location = new Point(13, 30);
            txPath.Multiline = true;
            txPath.Name = "txPath";
            txPath.Size = new Size(467, 51);
            txPath.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 12);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 1;
            label1.Text = "Root path:";
            // 
            // liSelPath
            // 
            liSelPath.AutoSize = true;
            liSelPath.Location = new Point(15, 84);
            liSelPath.Name = "liSelPath";
            liSelPath.Size = new Size(74, 15);
            liSelPath.TabIndex = 2;
            liSelPath.TabStop = true;
            liSelPath.Text = "Select Folder";
            liSelPath.LinkClicked += liSelPath_LinkClicked;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(405, 87);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 3;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // DefineRootDir
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(492, 120);
            Controls.Add(btnOk);
            Controls.Add(liSelPath);
            Controls.Add(label1);
            Controls.Add(txPath);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DefineRootDir";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MIPS.Net :: Storage Simulator - Define Root Directory";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txPath;
        private Label label1;
        private LinkLabel liSelPath;
        private Button btnOk;
    }
}