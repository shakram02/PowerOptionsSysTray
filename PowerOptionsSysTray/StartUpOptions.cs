using IWshRuntimeLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    internal class StartUpHandler
    {
        //Get the Assembly Name of the application
        private static string appname = Assembly.GetExecutingAssembly().FullName.Remove(Assembly.GetExecutingAssembly().FullName.IndexOf(","));

        //Adds the applications AssemblyName to the Startup folder path and adds the .lnk extension used for shortcuts
        private string StartupPathName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), appname + ".lnk");

        /// <summary>
        /// Tells whether the application launches on startup or not
        /// </summary>
        public bool IsLaunchedOnStartup => System.IO.File.Exists(StartupPathName);

        /// <summary>
        /// Sets the application to startup with windows or not
        /// </summary>
        public void SetStartupValue(bool launchOnStartup)
        {
            if (launchOnStartup)
                CreateStartupLink();
            else
                RemoveStartupLink();
        }

        private void CreateStartupLink()
        {
            CreateShortcut(StartupPathName, true);
        }

        private void RemoveStartupLink()
        {
            CreateShortcut(StartupPathName, false);
        }

        /// <summary>
        /// Creates or removes a shortcut for this application at the specified pathname.
        /// </summary>
        /// <param name="shortcutPathName">
        /// The path where the shortcut is to be created or removed from including the (.lnk) extension.
        /// </param>
        /// <param name="create">True to create a shortcut or False to remove the shortcut.</param>
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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