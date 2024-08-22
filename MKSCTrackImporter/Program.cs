namespace MKSCTrackImporter
{
    internal static class Program
    {
        public static bool loggingEnabled = false;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                if (args[0] == "-l")
                {
                    loggingEnabled = true;
                }
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}