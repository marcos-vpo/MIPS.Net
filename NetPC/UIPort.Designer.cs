namespace NetPC
{
    partial class UIPort
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
            img = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)img).BeginInit();
            SuspendLayout();
            // 
            // img
            // 
            img.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            img.BackColor = Color.SeaGreen;
            img.BackgroundImage = Properties.Resources.usb_off;
            img.BackgroundImageLayout = ImageLayout.Center;
            img.Location = new Point(6, 7);
            img.Name = "img";
            img.Size = new Size(39, 36);
            img.TabIndex = 0;
            img.TabStop = false;
            // 
            // UIPort
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Green;
            Controls.Add(img);
            Name = "UIPort";
            Size = new Size(52, 50);
            ((System.ComponentModel.ISupportInitialize)img).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox img;
    }
}
