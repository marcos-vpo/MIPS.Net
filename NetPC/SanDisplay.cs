using System;
using System.Drawing;
using System.Windows.Forms;
using MIPS.Net.SoC;
using NetPC.Controllers;

namespace NetPC
{
    public partial class SanDisplay : UserControl, IDisplayAdapter
    {
        // ===============================
        // BACKBUFFER
        // ===============================
        private Bitmap backBuffer;
        private Graphics backGraphics;

        // ===============================
        // STATE
        // ===============================
        private byte[] current_buffer;
        private int current_x = 0;
        private int current_y = 0;

        private Font font;

        public SanDisplay()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            font = this.Font;
        }

        // ===============================
        // INIT
        // ===============================
        private void SanDisplay_Load(object sender, EventArgs e)
        {
            //           InitBackBuffer();
        }

        protected override void OnResize(EventArgs e)
        {
            if (MotherBoard.Instance == null) return;
            if (MotherBoard.Instance.IsOn == false) return;
            base.OnResize(e);
            InitBackBuffer();
            Flush();
        }

        private bool init = false;

        private void InitBackBuffer()
        {
            if (init) return;

            backGraphics?.Dispose();
            backBuffer?.Dispose();

            if (Width <= 0 || Height <= 0)
                return;

            backBuffer = new Bitmap(Width, Height);
            backGraphics = Graphics.FromImage(backBuffer);
            backGraphics.Clear(this.BackColor);

            font ??= new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular);
            init = true;
        }

        // ===============================
        // KNOWLEDGE: DISPLAY STATUS
        // ===============================
        private bool _connected;
        public void DisplayStatus(bool connected)
        {
            _connected = connected;
            if (connected) this.BackColor = Color.FromArgb(25, 25, 25);
            else this.BackColor = Color.Black;

            InitBackBuffer();
        }

        Dictionary<Color, SolidBrush> _cache_colors = new Dictionary<System.Drawing.Color, SolidBrush>();
        private SolidBrush GetColor(Color c)
        {
            if (_cache_colors.ContainsKey(c))
                return _cache_colors[c];
            else
            {
                var sb = new SolidBrush(c);
                _cache_colors.Add(c, sb);
                return sb;
            }
        }


        // ===============================
        // CORE: PRINT CHAR (FAST)
        // ===============================
        public void PrintChar(byte[] raw)
        {
            current_buffer = raw;

            int cursorX = BitConverter.ToInt32(raw[0..4]);
            int cursorY = BitConverter.ToInt32(raw[4..8]);

            int index = 8 + (cursorY * DisplayController.DisplayWidth + cursorX) * 2;
            if (index < 0 || index + 1 >= raw.Length)
                return;

            char c = (char)raw[index];
            byte attr = raw[index + 1];

            Color fg = GetForegroundFromAttr(attr);
            Color bg = this.BackColor; //GetBackgroundFromAttr(attr);

            float x = cursorX * this.font.Size;
            float y = cursorY * this.font.Height;

            lock (this)
            {

                if (cursorX < current_x || cursorY < current_y)
                {
                    backGraphics.FillRectangle(GetColor(this.BackColor), x, y, base.Font.Size, base.Font.Height);
                }

                // desenha o caractere
                backGraphics.DrawString(
                    c.ToString(),
                    font,
                    GetColor(fg),
                    x, y);
            }

            current_x = cursorX;
            current_y = cursorY;
        }

        // ===============================
        // BLIT ÚNICO PRA TELA
        // ===============================
        public void Flush()
        {
            if (!IsHandleCreated || backBuffer == null)
                return;

            BeginInvoke((Action)(() =>
            {
                using Graphics g = CreateGraphics();
                g.DrawImageUnscaled(backBuffer, 0, 0);
            }));
        }

        // ===============================
        // CLEAR
        // ===============================
        public void ClearScreen()
        {
            lock (this)
            {
                if (backGraphics == null) return;
                backGraphics.Clear(this.BackColor);
                current_y = 0;
                current_x = 0;
            }
            Flush();
        }

        // ===============================
        // COLORS
        // ===============================
        private Color GetBackgroundFromAttr(byte attr)
        {
            int bg = (attr >> 4) & 0x07;

            return bg switch
            {
                0x0 => Color.Black,
                0x1 => Color.DarkBlue,
                0x2 => Color.DarkGreen,
                0x3 => Color.DarkCyan,
                0x4 => Color.DarkRed,
                0x5 => Color.DarkMagenta,
                0x6 => Color.Yellow,
                0x7 => Color.Gray,
                _ => Color.Black
            };
        }

        private Color GetForegroundFromAttr(byte attr)
        {
            int fg = attr & 0x0F;

            return fg switch
            {
                0x0 => Color.Black,
                0x1 => Color.DarkBlue,
                0x2 => Color.DarkGreen,
                0x3 => Color.DarkCyan,
                0x4 => Color.DarkRed,
                0x5 => Color.DarkMagenta,
                0x6 => Color.Yellow,
                0x7 => Color.Gray,
                0x8 => Color.DarkGray,
                0x9 => Color.Blue,
                0xA => Color.Green,
                0xB => Color.Cyan,
                0xC => Color.Red,
                0xD => Color.Magenta,
                0xE => Color.Yellow,
                0xF => Color.White,
                _ => Color.Gray
            };
        }

        public void ScrollUp()
        {
            int lineHeight = (int)this.font.Height;

            // copia bitmap inteiro, deslocando 1 linha pra cima
            backGraphics.DrawImage(
                backBuffer,
                new Rectangle(0, 0, Width, Height - lineHeight),
                new Rectangle(0, lineHeight, Width, Height - lineHeight),
                GraphicsUnit.Pixel
            );

            // limpa última linha
            backGraphics.FillRectangle(
                GetColor(this.BackColor),
                0,
                Height - lineHeight,
                Width,
                lineHeight
            );
        }


        // ===============================
        // DIMENSIONS
        // ===============================
        public Tuple<int, int> GetDimensions()
        {
            return new Tuple<int, int>((int)(Width / this.font.Size), (int)(Height / this.font.Height));
        }

        public void PrintTextLine(string line, int x, int y, Color color, object font)
        {
            throw new NotImplementedException();
        }
    }
}
