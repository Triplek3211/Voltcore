using System;
using System.Management; // Requires System.Management NuGet package

namespace BatteryGuardian.Core
{
    public class BatteryMetrics
    {
        public string DeviceName { get; private set; } = "Unknown";
        public uint DesignCapacity { get; private set; } // mWh
        public uint FullChargeCapacity { get; private set; } // mWh
        public int CurrentPercentage { get; private set; }
        public bool IsCharging { get; private set; }
        public double WearLevel { get; private set; } // Percentage

        public void UpdateMetrics()
        {
            try
            {
                // 1. Query Full Charge and Design Capacities via WMI (Root\WMI)
                using (var searcher = new ManagementObjectSearcher(@"Root\WMI", "SELECT * FROM BatteryFullChargedCapacity"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        FullChargeCapacity = (uint)obj["FullChargedCapacity"];
                    }
                }

                using (var searcher = new ManagementObjectSearcher(@"Root\WMI", "SELECT * FROM BatteryStaticData"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        DesignCapacity = (uint)obj["DesignedCapacity"];
                        DeviceName = obj["DeviceName"]?.ToString() ?? "Generic Laptop Battery";
                    }
                }

                // 2. Query Current Real-time Status via standard Win32 API or WMI
                using (var searcher = new ManagementObjectSearcher(@"Root\CIMV2", "SELECT * FROM Win32_Battery"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        CurrentPercentage = Convert.ToInt32(obj["EstimatedChargeRemaining"]);
                        ushort status = (ushort)obj["BatteryStatus"];
                        IsCharging = (status == 2 || status == 6 || status == 7); // Charging/Maintaining
                    }
                }

                // 3. Calculate Wear Level
                if (DesignCapacity > 0)
                {
                    // Formula: 100 - ((Full / Design) * 100)
                    double health = ((double)FullChargeCapacity / DesignCapacity) * 100;
                   WearLevel = Math.Round(100 - health, 2);
                    if (WearLevel < 0) WearLevel = 0; // Handle calibration overflows safely
                }
            }
            catch (Exception ex)
            {
                // Fail silently or log locally to prevent background app crashes
                System.Diagnostics.Debug.WriteLine($"Hardware Query Error: {ex.Message}");
            }
        }
    }
}