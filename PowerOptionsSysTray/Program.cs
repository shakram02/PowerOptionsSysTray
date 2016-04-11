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
                PowerOptionsController powerOptions = new PowerOptionsController();
                Application.Run();
                powerOptions.Dispose();


            }
            catch (Exception e)
            {
                string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Error Log.txt");
                StreamWriter writer = new StreamWriter(logFilePath);
                writer.Write(e.Message);
                writer.WriteLine();
                writer.Write(e.StackTrace);
                writer.Close();
                MessageBox.Show($"An error occured:{Environment.NewLine}{e.Message}{Environment.NewLine}Please check:" + logFilePath);
            }
        }
    }
}