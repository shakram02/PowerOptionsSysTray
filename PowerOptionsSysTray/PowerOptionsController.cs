using PowerPlanHandler;
using System;
using System.Collections.Generic;
using static PowerPlanHandler.PowerOptionManager;

namespace PowerOptionsSysTray
{
    internal sealed class PowerOptionsController : IDisposable
    {
        private static PowerOptionManager powerManager;
        private PowerNotifyIcon trayIcon;
        private PowerOptionsContextMenu contextMenu;
        private StartUpHandler startupHandler;

        public PowerOptionsController()
        {
            startupHandler = new StartUpHandler();

            powerManager = new PowerOptionManager();
            contextMenu = new PowerOptionsContextMenu(this, startupHandler);
            trayIcon = new PowerNotifyIcon(contextMenu);
        }

        public void SetActivePowerPlan(string planName) => powerManager.SetActivePowerPlan(planName);

        public string GetActivePowerPlan() => powerManager.GetActivePlan();

        public List<string> GetPlanNames()
        {
            List<string> planNames = new List<string>(4);

            foreach (var plan in powerManager.GetPowerPlans())
                planNames.Add(plan.Name);

            return planNames;
        }

        public void Dispose()
        {
            trayIcon.Dispose();
            contextMenu.Dispose();
        }
    }
}