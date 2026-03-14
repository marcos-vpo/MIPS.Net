namespace NetPC
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.EnableVisualStyles();
            Application.ThreadException += (sender, e) =>
            {
                int i = 0;

            };

            // Registra tratamento de exceptions em threads não-UI
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                int i = 0;
            };

            Application.Run(new NetPCForm());
        }
    }

    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Panel p, bool enable)
        {
            var prop = typeof(Control)
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(p, enable, null);
        }
    }
}