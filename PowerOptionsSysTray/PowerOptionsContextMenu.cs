using System;
using System.Windows.Forms;

namespace PowerOptionsSysTray
{
    /// <summary>
    /// Context menu that hold the power plans
    /// </summary>
    internal class PowerOptionsContextMenu : ContextMenu, IDisposable
    {
        private PowerOptionsController controller;
        StartUpHandler startupHandler;

        /// <summary>
        /// Creates a context menu for the tray
        /// </summary>
        /// <param name="controller">Handles the available power options</param>
        public PowerOptionsContextMenu(PowerOptionsController controller, StartUpHandler startupHandler)
        {
            this.controller = controller;
            this.startupHandler = startupHandler;
            this.Popup += Menu_Popup;

            PopulateContextMenu();
        }


        private void Menu_Popup(object sender, EventArgs e)
        {
            PopulateContextMenu();
        }

        private void AddExit()
        {
            this.MenuItems.Add(new MenuItem("Exit", (s, e) => Application.Exit()));
        }

        private void CheckIfStartup(MenuItem startUpMenuItem)
        {
            startUpMenuItem.Checked = startupHandler.IsLaunchedOnStartup;
        }

        /// <summary>
        /// Loads a fresh context menu with selected plan on top
        /// </summary>
        private void PopulateContextMenu()
        {
            // Make sure the menu is clean before recreating it
            this.MenuItems.Clear();

            string activePlan = controller.GetActivePlan();

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
        }

        private void ItemOnClick(object sender, EventArgs e)
        {
            // Uncheck all items, to make only the selected item checked
            foreach (MenuItem item in this.MenuItems)
                item.Checked = false;

            MenuItem clickedItem = (MenuItem)sender;
            clickedItem.Checked = true;
            controller.SetPowerPlan(clickedItem.Text);
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