using System;
using System.IO;
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
            try
            {
                using (PowerOptionsController powerOptions = new PowerOptionsController())
                {
                    //powerOptions.Display();
                    Application.Run();
                    powerOptions.Dispose();
                }
            }
            catch (Exception e)
            {
                StreamWriter writer = new StreamWriter(@"d:\log.txt");
                writer.Write(e.Message);
                writer.WriteLine();
                writer.Write(e.StackTrace);
            }


        }
    }
}