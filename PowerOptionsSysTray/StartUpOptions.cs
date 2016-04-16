using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    internal class StartUpHandler
    {
        //Get the Assembly Name of the application
        private static string appname = Assembly.GetExecutingAssembly().FullName.Remove(Assembly.GetExecutingAssembly().FullName.IndexOf(","));

        // The path to the key where Windows looks for startup applications
        private static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private static string applicationName = Assembly.GetExecutingAssembly().GetName().Name;

        //Adds the applications AssemblyName to the Startup folder path and adds the .lnk extension used for shortcuts
        private static string StartupPathName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appname + ".lnk");

        /// <summary>
        /// Tells whether the application launches on startup or not
        /// </summary>
        public bool IsLaunchedOnStartup => rkApp.GetValue(applicationName) == null ? false : true;

        /// <summary>
        /// Sets the application to startup with windows or not
        /// </summary>
        public void SetStartup(bool launchOnStartup)
        {
            //CreateShortcut(StartupPathName, launchOnStartup);
            if (launchOnStartup && rkApp.GetValue(applicationName) == null)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue(applicationName, Application.ExecutablePath);
            }
            else if (!launchOnStartup && rkApp.GetValue(applicationName) != null)
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue(applicationName, false);
            }
        }

        /// <summary>
        /// Creates or removes a shortcut for this application at the specified pathname.
        /// </summary>
        /// <param name="shortcutPathName">
        /// The path where the shortcut is to be created or removed from including the (.lnk) extension.
        /// </param>
        /// <param name="create">          True to create a shortcut or False to remove the shortcut.</param>
        private void CreateShortcut(string shortcutPathName, bool create)
        {
            /// Source https://code.msdn.microsoft.com/windowsdesktop/Create-a-shortcut-for-your-ad3d9cb3
            if (create)
            {
                try
                {
                    string shortcutTarget = System.IO.Path.Combine(Application.StartupPath, appname + ".exe");
                    WshShell myShell = new WshShell();
                    WshShortcut myShortcut = (WshShortcut)myShell.CreateShortcut(shortcutPathName);
                    myShortcut.TargetPath = shortcutTarget; //The exe file this shortcut executes when double clicked
                    myShortcut.IconLocation = shortcutTarget + ",0"; //Sets the icon of the shortcut to the exe`s icon
                    myShortcut.WorkingDirectory = Application.StartupPath; //The working directory for the exe
                    myShortcut.Arguments = ""; //The arguments used when executing the exe
                    myShortcut.Save(); //Creates the shortcut
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    if (System.IO.File.Exists(shortcutPathName))
                        System.IO.File.Delete(shortcutPathName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}