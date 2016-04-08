using Microsoft.Win32;

namespace PowerOptionsSysTray
{
    internal class StartUpHandler
    {
        //Registry key for launch on startup
        private RegistryKey launchOnStartupRegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private string applicationName;
        public StartUpHandler(string applicationName)
        {
            this.applicationName = applicationName;
        }
        /// <summary>
        /// Sets the application to startup with windows or not
        /// </summary>
        public void SetStartupValue(bool launchOnStartup)
        {
            if (launchOnStartup)
                launchOnStartupRegistryKey.SetValue(applicationName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                launchOnStartupRegistryKey.DeleteValue(applicationName, false);
        }

        /// <summary>
        /// Tells whether the application launches on startup or not
        /// </summary>
        public bool IsLaunchedOnStartup
        {
            get
            {
                if (launchOnStartupRegistryKey.GetValue(applicationName) == null)
                    return false;

                return true;
            }
        }

    }
}