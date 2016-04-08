using PowerOptionsSysTray.Properties;
using System;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    /// <summary>
    /// Notification tray handler class
    /// </summary>
    internal class PowerNotifyIcon : IDisposable
    {
        private NotifyIcon notificationIcon;

        public PowerNotifyIcon(ContextMenu powerContextMenu)
        {
            notificationIcon = new NotifyIcon();
            notificationIcon.Icon = new System.Drawing.Icon("Icon.ico");
            notificationIcon.Text = "Power Options";

            notificationIcon.ContextMenu = powerContextMenu;
            notificationIcon.Visible = true;
        }

        public void Dispose()
        {
            notificationIcon.Visible = false;
            notificationIcon.Dispose();
        }
    }
}