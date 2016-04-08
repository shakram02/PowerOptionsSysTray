using System;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (PowerOptionsController powerOptions = new PowerOptionsController())
            {
                //powerOptions.Display();
                Application.Run();
                powerOptions.Dispose();
            }
        }
    }
}