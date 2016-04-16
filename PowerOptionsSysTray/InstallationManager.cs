using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{

    public sealed class InstallationManager
    {

        public void CleanOldProgramData()
        {
            KillOldRunningProcess();
            RemoveOldApplicationStartupEntry();
        }

        public void RemoveOldApplicationStartupEntry()
        {
            // Check for the old entries of the program at startup file
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            List<string> startupFiles = Directory.GetFiles(startupFolder).Select(Path.GetFileName).ToList();

            try
            {
                string file = startupFiles.Where(fileName =>
            fileName.StartsWith("Power Tray") == true).First();
                File.Delete(Path.Combine(startupFolder, file));
            }
            catch { /*Startup entry not found*/ }
        }

        public void KillOldRunningProcess()
        {
            StreamWriter writer = new StreamWriter("D:\\killingOld.txt");
            MessageBox.Show("Killing old process");
            // Kill the process if it's running
            var processes = Process.GetProcessesByName("PowerOptionsSysTray");
            writer.WriteLine("Killing old stuff");
            foreach (var item in processes)
            {
                writer.WriteLine(item.ProcessName);
            }
            if (processes.Length > 0)
                processes[0].Kill();

            writer.Close();
        }

        public void AfterUninstallEventHandler()
        {
            //MessageBox.Show("After uninstall");
            // Remove the entries in registry
            StartUpHandler stH = new StartUpHandler();

            stH.SetStartup(false);
            RemoveApplicationFiles();
        }

        public void RemoveApplicationFiles()
        {
            StreamWriter writer = new StreamWriter("d:\\deleteerror.txt");

            try
            {
                var appFolder = Directory.GetCurrentDirectory();    // TODO ???
                writer.WriteLine("Running remove application folder:" + appFolder);

                var fileNames = Directory.GetFiles(appFolder);

                // Delete everything related to the application on the pc
                Directory.Delete(appFolder, true);
            }
            catch (Exception exc)
            {
                var appFolder = Directory.GetCurrentDirectory();
                // TODO remove
                writer.Write(exc.Message);
                string[] remainingFiles = Directory.GetFiles(appFolder);
                writer.WriteLine("remaining files");

                foreach (var item in remainingFiles)
                    writer.WriteLine(item);

                writer.WriteLine("\n" + exc.StackTrace);
            }
            finally
            {
                writer.WriteLine("An exception occured, now at finally");
                writer.Close();
            }
        }
    }
}