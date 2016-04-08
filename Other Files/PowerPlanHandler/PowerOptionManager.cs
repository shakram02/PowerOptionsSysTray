using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PowerPlanHandler
{
    // source http://www.fsmpi.uni-bayreuth.de/~dun3/archives/programmatically-change-power-options-using-cshar/519.html

    /// <summary>
    /// Wrapper for managing power plans
    /// </summary>
    public class PowerOptionManager
    {
        static List<PowerPlan> powerPlans;  // List of available power plans
        #region DllImports

        [DllImport("PowrProf.dll")]
        private static extern uint PowerEnumerate(IntPtr RootPowerKey, IntPtr SchemeGuid,
            IntPtr SubGroupOfPowerSettingGuid, uint AcessFlags, uint Index, ref Guid Buffer, ref uint BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern UInt32 PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid,
            IntPtr SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        private static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("PowrProf.dll")]
        private static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid SchemeGuid);

        #endregion

        /// <summary>
        /// List of all available power plans
        /// </summary>
        public List<PowerPlan> PowerPlans => powerPlans;

        /// <summary>
        /// Creates a power option manager which exposes the available power plans and can change the current power plan
        /// </summary>
        public PowerOptionManager()
        {
            powerPlans = GetPowerPlans();
        }

        /// <summary>
        /// Gets all power plans available to system
        /// </summary>
        /// <returns>List of PowerPlan objects</returns>
        private List<PowerPlan> GetPowerPlans()
        {
            powerPlans = new List<PowerPlan>(4);
            List<Guid> powerPlanGuids = GetPowerOptions().ToList();

            foreach (var planGuid in powerPlanGuids)
            {
                string planName = ReadFrienldyName(planGuid);
                powerPlans.Add(new PowerPlan(planName, planGuid));
            }

            return powerPlans;
        }

        /// <summary>
        /// Sets a given PowerPlan to active
        /// </summary>
        /// <param name="plan">Plan to be used</param>
        public void SetActivePowerPlan(PowerPlan plan)
        {
            SetActive(plan.guid);
        }

        /// <summary>
        /// Sets a given PowerPlan to active by its name ignoring character cases
        /// </summary>
        /// <param name="planName">Plan name</param>
        /// <returns>Whether the operation was successful</returns>
        public bool SetActivePowerPlan(string planName)
        {
            try
            {
                // Compares the contents of the list ( lowercased ) and the given name and finds if they're equal
                SetActive(powerPlans.Find(p => p.Name.Equals(planName, StringComparison.InvariantCultureIgnoreCase)).guid);
            }
            catch (Exception exc)
            {
                string error = exc.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns the name of the active power plan
        /// </summary>
        public string GetActivePlan()
        {
            IntPtr pCurrentSchemeGuid = IntPtr.Zero;

            PowerGetActiveScheme(IntPtr.Zero, ref pCurrentSchemeGuid);

            var currentSchemeGuid = (Guid)Marshal.PtrToStructure(pCurrentSchemeGuid, typeof(Guid));

            // Return the active power plan's friendly name
            return ReadFrienldyName(currentSchemeGuid);
        }

        private IEnumerable<Guid> GetPowerOptions()
        {
            Guid schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
                (uint)AccessFlags.ACCESS_SCHEME, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                // Return the list element by element
                yield return schemeGuid;
                schemeIndex++;
            }
        }

        private string ReadFrienldyName(Guid schemeGuid)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            string friendlyName;

            try
            {
                PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                friendlyName = Marshal.PtrToStringUni(pSizeName);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }
            return friendlyName;
        }

        /// <summary>
        /// Activates the GUID that matches a power plan 
        /// </summary>
        /// <param name="powerSchemeId">Target power plan GUID</param>
        private void SetActive(Guid powerSchemeId)
        {
            var schemeGuid = powerSchemeId;

            PowerSetActiveScheme(IntPtr.Zero, ref schemeGuid);
        }

        /// <summary>
        /// Represents a power plan, access the plan by its name
        /// </summary>
        public class PowerPlan
        {
            internal Guid guid;
            private string name;


            /// <summary>
            /// Power plan's name as it's known to the system
            /// </summary>
            public string Name => name;

            private PowerPlan() { }

            /// <summary>
            /// Creates a new instance of the power plan
            /// </summary>
            /// <param name="name"></param>
            /// <param name="guid"></param>
            internal PowerPlan(string name, Guid guid)
            {
                this.name = name;
                this.guid = guid;
            }
        }

        internal enum AccessFlags : uint
        {
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18
        }
    }
}
