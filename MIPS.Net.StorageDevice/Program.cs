namespace MIPS.Net.StorageDevice
{
    internal static class Program
    {
        public static int SYSTEM_PORT_AUTO = 0;

        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 0)
                SYSTEM_PORT_AUTO = int.Parse(args[0]);
       //     SYSTEM_PORT_AUTO = 13555;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}