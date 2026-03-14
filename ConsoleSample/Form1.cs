using System.Drawing.Text;

namespace ConsoleSample
{
    public partial class Form1 : Form
    {
        private byte[] buffer;
        private int screenWidth = 3;
        private int screenHeight = 3;
        public Form1()
        {
            InitializeComponent();

            buffer = new byte[screenWidth * screenHeight * 8];
            fontSize = (byte)Font.Size;
        }


        private int cursorX = 0;
        private int cursorY = 0;
        private ConsoleColor foregroundColor = ConsoleColor.White; // Cor padrão
        private ConsoleColor backgroundColor = ConsoleColor.Black;   // Fundo padrão
        private FontStyle attributes = FontStyle.Regular; // Sem atributos
        private byte fontSize = 14;

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 1. Calcula o índice no buffer
            int index = (cursorY * screenWidth + cursorX) * 8;

            // 2. Obtém o caractere (byte)
            byte charCode = (byte)e.KeyChar;

            // 3. Insere o caractere no buffer
            buffer[index + 0] = charCode;

            // 4. Insere os atributos
            buffer[index + 1] = (byte)(int)foregroundColor;
            buffer[index + 2] = (byte)(int)backgroundColor;
            buffer[index + 3] = (byte)(int)attributes;
            buffer[index + 4] = (byte)(int)fontSize;

            // 5. Preenche com zero os bytes nao utilizados
            buffer[index + 5] = 0;
            buffer[index + 6] = 0;
            buffer[index + 7] = 0;


            e.Handled = true; // Indica que o evento foi tratado
            OnPaint(null, null); // Força a atualização da tela
        }


        private Color GetColor(ConsoleColor consoleColor)
        {
            switch (consoleColor)
            {
                case ConsoleColor.Black: return Color.Black;
                case ConsoleColor.DarkBlue: return Color.DarkBlue;
                case ConsoleColor.DarkGreen: return Color.DarkGreen;
                case ConsoleColor.DarkCyan: return Color.DarkCyan;
                case ConsoleColor.DarkRed: return Color.DarkRed;
                case ConsoleColor.DarkMagenta: return Color.DarkMagenta;
                case ConsoleColor.DarkYellow: return Color.DarkGoldenrod;
                case ConsoleColor.Gray: return Color.Gray;
                case ConsoleColor.DarkGray: return Color.DarkGray;
                case ConsoleColor.Blue: return Color.Blue;
                case ConsoleColor.Green: return Color.Green;
                case ConsoleColor.Cyan: return Color.Cyan;
                case ConsoleColor.Red: return Color.Red;
                case ConsoleColor.Magenta: return Color.Magenta;
                case ConsoleColor.Yellow: return Color.Yellow;
                case ConsoleColor.White: return Color.White;
                default: return Color.Black; // Cor padrão
            }
        }

        bool first = true;
        private Graphics g;
        private Graphics G
        {
            get
            {
                try
                {
                    if (g == null) g = this.CreateGraphics();
                    return g;
                }
                catch { return null; }
            }
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (first)
            {
                first = false;
                return;
            }

            Graphics g = G;
            if (e != null) return;

            //      g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            float charWidth = g.MeasureString("M", Font).Width;
            float charHeight = Font.GetHeight(g);

            // Calculate the rectangle to invalidate around the cursor position
            RectangleF invalidRect = new RectangleF(
                cursorX * charWidth,
                cursorY * charHeight,
                charWidth,
                charHeight
            );

            // Only draw if the invalid rectangle intersects with the area to paint
            //    if (e.ClipRectangle.IntersectsWith(Rectangle.Round(invalidRect)))
            {
                int index = ((cursorY * screenWidth + cursorX) * 8);
                if (index < 0) return;

                byte charCode = buffer[index + 0];
                ConsoleColor foreground = (ConsoleColor)buffer[index + 1];
                ConsoleColor background = (ConsoleColor)buffer[index + 2];
                FontStyle attributes = (FontStyle)buffer[index + 3];
                byte fontSize = buffer[index + 4];

                Font font = new Font(Font.FontFamily, fontSize > 0 ? fontSize : Font.Size, attributes);
                string character = ((char)charCode).ToString();

                // Desenha o fundo
                g.FillRectangle(new SolidBrush(Color.Orange), cursorX * charWidth, cursorY * charHeight, charWidth, charHeight);

                // Desenha o texto
                g.DrawString(character, font, new SolidBrush(GetColor(foreground)), cursorX * charWidth, cursorY * charHeight);
            }


            // 6. Avança o cursor
            cursorX++;
       //     globalCursor++;
            if (cursorX >= screenWidth)
            {
                cursorX = 0;
                cursorY++;
                if (cursorY >= screenHeight)
                {
                    cursorY = 0; // Volta para o início
                }
            }

        }

   //     int globalCursor = 0;

        private void Form1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            return;
            if (first) return;

            Task.Run(() =>
            {
                Invoke(() =>
                {
              //      cursorX = 0;
              //      cursorY = 0;
                    //         int gc = globalCursor;
                    //         globalCursor = 0;
                    int index = ((cursorY * screenWidth + cursorX) * 8);
                    int gc = (index / 8);
                    cursorX = 0;
                    cursorY = 0;
                    for (int i = 0; i < gc; i++)
                    {
                        OnPaint(null, null);
                    }
                });
            });
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            return;
            if (first) return;

            Task.Run(() =>
            {
                Invoke(() =>
                {
                    //      cursorX = 0;
                    //      cursorY = 0;
                    //         int gc = globalCursor;
                    //         globalCursor = 0;
                    int index = ((cursorY * screenWidth + cursorX) * 8);
                    int gc = (index / 8);
                    cursorX = 0;
                    cursorY = 0;
                    for (int i = 0; i < gc; i++)
                    {
                        OnPaint(null, null);
                    }
                });
            });
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (first) return;

            Task.Run(() =>
            {
                Invoke(() =>
                {
                    //      cursorX = 0;
                    //      cursorY = 0;
                    //         int gc = globalCursor;
                    //         globalCursor = 0;
                    int index = ((cursorY * screenWidth + cursorX) * 8);
                    int gc = (index / 8);
                    cursorX = 0;
                    cursorY = 0;
                    for (int i = 0; i < gc; i++)
                    {
                        OnPaint(null, null);
                    }
                });
            });
        }
    }
}
