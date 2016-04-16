using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    /// <summary>
    /// Context menu that hold the power plans
    /// </summary>
    internal class PowerOptionsContextMenu : ContextMenu, IDisposable
    {
        private static PowerOptionsController controller;
        private static StartUpHandler startupHandler;
        private string activePlan;

        /// <summary>
        /// Creates a context menu for the tray
        /// </summary>
        /// <param name="controller">Handles the available power options</param>
        public PowerOptionsContextMenu(PowerOptionsController controller, StartUpHandler startupHandler)
        {
            PowerOptionsContextMenu.controller = controller;
            PowerOptionsContextMenu.startupHandler = startupHandler;
            this.Popup += Menu_Popup;

            PopulateContextMenu();
        }

        private void Menu_Popup(object sender, EventArgs e) => PopulateContextMenu();

        private void AddExit() =>
            this.MenuItems.Add(new MenuItem("Exit", (s, e) => Application.Exit()));

        private void CheckIfStartup(MenuItem startUpMenuItem) =>
            startUpMenuItem.Checked = startupHandler.IsLaunchedOnStartup;

        /// <summary>
        /// Loads a fresh context menu with selected plan on top
        /// </summary>
        private void PopulateContextMenu()
        {
            // Make sure the menu is clean before recreating it
            this.MenuItems.Clear();

            activePlan = controller.GetActivePowerPlan();

            foreach (string planName in controller.GetPlanNames())
            {
                // Selects the current active plan
                if (planName.Equals(activePlan, StringComparison.InvariantCultureIgnoreCase))
                {
                    var selectedPlan = new MenuItem(planName, ItemOnClick);
                    selectedPlan.Checked = true;

                    // Add the selected power plan followed by a separator
                    this.MenuItems.Add(0, selectedPlan);
                    this.MenuItems.Add(1, new MenuItem("-"));
                }
                else
                    this.MenuItems.Add(new MenuItem(planName, ItemOnClick));
            }
            var startUpMenuItem = new MenuItem("Launch On Startup", StartUpOnClick);
            this.MenuItems.Add("-");
            this.MenuItems.Add(startUpMenuItem);

            CheckIfStartup(startUpMenuItem);
            AddExit();


            this.MenuItems.Add("-");
            this.MenuItems.Add(GeneratePowerData());


        }

        private MenuItem GeneratePowerData()
        {
            string pluggedStatus, powerDescription;


            MenuItem powerDataMenuItem = new MenuItem();
            powerDataMenuItem.Enabled = false;
            float batteryPercentage = SystemInformation.PowerStatus.BatteryLifePercent;

            BatteryChargeStatus batteryStatus = SystemInformation.PowerStatus.BatteryChargeStatus;

            if (batteryStatus == BatteryChargeStatus.NoSystemBattery)
            {
                powerDescription = "No battery detected";
                powerDataMenuItem.Text = powerDescription;
                return powerDataMenuItem;
            }

            PowerLineStatus powerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
            pluggedStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? "Plugged in" : "Unplugged";

            // Discharging, time to battery die
            if (powerLineStatus == PowerLineStatus.Offline)
            {
                powerDescription = GenerateBatteryOfflineData(batteryPercentage);
            }
            else
            {
                int secondsToFullCharge = SystemInformation.PowerStatus.BatteryFullLifetime;
                powerDescription = $"{batteryPercentage * 100}% - {pluggedStatus}";
            }
            powerDataMenuItem.Text = powerDescription;

            return powerDataMenuItem;
        }

        private string GenerateBatteryOfflineData(float batteryPercentage)
        {
            TimeSpan timeRemaining;
            int secondsRemaining = SystemInformation.PowerStatus.BatteryLifeRemaining; // Calculated after a while
            if (secondsRemaining != -1)
            {
                timeRemaining = TimeSpan.FromSeconds(secondsRemaining);

                int remainingHours = timeRemaining.Hours;

                // Don't display hours if it's equal 0
                if (remainingHours == 0)
                    return $"{batteryPercentage * 100}% {timeRemaining.Minutes} Minutes - Unplugged";
                else
                    return $"{batteryPercentage * 100}% {timeRemaining.Hours} Hours {timeRemaining.Minutes} Minutes - Unplugged";

            }
            else
                return $"{batteryPercentage * 100}% - Unplugged";
        }

        private void ItemOnClick(object sender, EventArgs e)
        {
            // Uncheck all items, to make only the selected item checked
            foreach (MenuItem item in this.MenuItems)
                item.Checked = false;

            MenuItem clickedItem = (MenuItem)sender;
            clickedItem.Checked = true;
            controller.SetActivePowerPlan(clickedItem.Text);
        }

        private void StartUpOnClick(object sender, EventArgs e)
        {
            // Click handler for startup option
            var menuStartup = (MenuItem)sender;

            // Toggle the state of startup
            startupHandler.SetStartup(!startupHandler.IsLaunchedOnStartup);
            menuStartup.Checked = !startupHandler.IsLaunchedOnStartup;
        }
    }
}