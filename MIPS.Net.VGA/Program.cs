namespace MIPS.Net.VGA
{
    internal static class Program
    {
        public static int SYSTEM_PORT_AUTO { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
                SYSTEM_PORT_AUTO = int.Parse(args[0]);

            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {

            }
        }
    }
}