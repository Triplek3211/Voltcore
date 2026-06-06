using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BatteryGuardian.Core
{
    public class SleepGating
    {
        // Import native Windows power configurations to force a hard sleep/hibernate state if required
        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        public void Initialize()
        {
            // Register an OS event listener for when the laptop changes power state
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            // Check if the OS is actively broadcasting a transition into sleep mode
            if (e.Mode == PowerModes.Suspend)
            {
                // Verify if the system is currently on battery power
                if (System.Windows.Forms.SystemInformation.PowerStatus.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Offline)
                {
                    // Log locally for debugging verification
                    Debug.WriteLine("Battery Guardian intercepted unsafe sleep on battery. Forcing absolute hibernation protocol.");

                    // Force Windows into a true, hard Hibernate (S4) state instead of Modern Standby (S0).
                    // Arguments: (true = Hibernate, false = Force Immediately, false = Disable Wake Events)
                    SetSuspendState(true, false, false);
                }
            }
        }

        public void Shutdown()
        {
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        }
    }
}