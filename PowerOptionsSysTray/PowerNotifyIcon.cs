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
        private Timer timer;

        public PowerNotifyIcon(ContextMenu powerContextMenu)
        {
            notificationIcon = new NotifyIcon();
            notificationIcon.Icon = Resources.Icon;
            notificationIcon.Text = "Power Tray";   // TODO Display battery capcacity

            notificationIcon.MouseMove += NotificationIcon_MouseMove;

            timer = new Timer();
            timer.Interval = 50;

            timer.Tick += Timer_Tick;
            notificationIcon.ContextMenu = powerContextMenu;
            notificationIcon.Visible = true;
        }

        public void Dispose()
        {
            notificationIcon.Visible = false;
            notificationIcon.Dispose();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Don't display battery information if it doesn't exist
            if (SystemInformation.PowerStatus.BatteryChargeStatus != BatteryChargeStatus.NoSystemBattery)
                notificationIcon.Text = $"Battery Level {(SystemInformation.PowerStatus.BatteryLifePercent * 100)}%";

            timer.Stop();
        }

        private void NotificationIcon_MouseMove(object sender, MouseEventArgs e)
        {
            timer.Start();  // Start the timer to show the tooltip data
        }
    }
}