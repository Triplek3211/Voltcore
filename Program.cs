using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;

namespace BatteryGuardian.Core
{
    static class Program
    {
        // Adding '?' makes these fields nullable, clearing the CS8618 warnings
        private static NotifyIcon? _trayIcon;
        private static System.Threading.Timer? _pollingTimer;
        private static BatteryMetrics? _metrics;
        private static SleepGating? _sleepGating;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _metrics = new BatteryMetrics();
            
            _sleepGating = new SleepGating();
            _sleepGating.Initialize();

        _trayIcon = new NotifyIcon();
            
            // Establish the explicit local runtime execution path
            string iconPath = Path.Combine(AppContext.BaseDirectory, "VoltCore.ico");

            if (File.Exists(iconPath))
            {
                // If our brand asset exists in the active execution folder, load it natively
                _trayIcon.Icon = new Icon(iconPath);
            }
            else
            {
                // Fallback option: prevents the app from vanishing headlessly if the file is missing
                _trayIcon.Icon = SystemIcons.Shield;
                System.Diagnostics.Debug.WriteLine($"Branded asset missing at: {iconPath}. Reverting to fallback system graphic.");
            }

            _trayIcon.Text = "VoltCore - Calibrating...";
            _trayIcon.Visible = true;
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApplication());
            _trayIcon.ContextMenuStrip = contextMenu;

            SystemEvents.PowerModeChanged += OnPowerModeChanged;
            
            // App starting up is completely fine
            _pollingTimer = new System.Threading.Timer(TickMetrics, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            Application.Run();
        }

        // Changing 'object state' to 'object? state' clears the CS8622 warning
        private static void TickMetrics(object? state)
        {
            if (_metrics == null || _trayIcon == null) return;

            _metrics.UpdateMetrics();

            string statusText = _metrics.IsCharging ? "Charging" : "On Battery";
            string summary = $"Battery Health: {100 - _metrics.WearLevel}%\nWear Level: {_metrics.WearLevel}%\nStatus: {statusText}";
            
            if (summary.Length > 63) summary = summary.Substring(0, 60) + "...";

            _trayIcon.Text = summary;
        }

        private static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.StatusChange)
            {
                // Passing a placeholder instead of an outright null literal clears the CS8625 warning
                TickMetrics(state: null);
            }
        }

        private static void ExitApplication()
        {
            _pollingTimer?.Dispose();
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
            
            if (_sleepGating != null)
            {
                _sleepGating.Shutdown();
            }

            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }
            Application.Exit();
        }
    }
}