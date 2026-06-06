VoltCore ⚡

Light-footprint, headless hardware-preservation engine for Windows.

VoltCore is a zero-bloat background system utility designed to maximize lithium-ion battery health and prevent premature hardware degradation. Running headlessly at under 15MB of RAM, it bypasses faulty OS standby loops and provides low-level diagnostics directly from your laptop's hardware management layers.


🚀 Features (Tier 1 - Open Core)

1. Low-Level Hardware Profiling: Queries hardware ACPI and WMI strings directly every 60 seconds to calculate true capacity wear levels rather than relying on basic OS percentage calculations.
2. Off-Flame" Sleep Gating: Actively intercepts OS power state broadcasts. When the laptop lid is closed while on battery, VoltCore overrides bug-prone Windows Modern Standby (S0) states and forces a deep, hardware-safe Hibernation (S4)—preventing backpack overheating and accidental battery drainage.
3. Microscopic Footprint: Built natively in C# on the .NET toolchain, executing silently in the Windows system tray with zero window clutter and near-zero CPU wake cycles.


🛠️ Architecture

VoltCore is structured cleanly across isolated modules:
* BatteryMetrics.cs: Low-level hardware diagnostic communication via WMI `Root\WMI` interfaces.
* SleepGating.cs: Power state interception and native Windows Win32 API suspension overrides.
* Program.cs: Event-driven background worker engine and native notification shell management.


📥 Installation & Running

1. Head over to the **Releases** tab on the right and download the latest compiled `VoltCore_x64.zip`.
2. Extract the contents to a local folder.
3. Ensure `VoltCore.ico` sits in the same folder as the executable, then double-click `VoltCore.exe` to launch it natively. It will initialize silently inside your system tray clock area.
4. **To Exit:** Right-click the system tray icon and select **Exit**.


💬 Community Validation & Hardware Reviews

This project is currently in open beta to map compatibility across various device configurations (Lenovo, Dell, HP, Asus). 

If you are running VoltCore, please open a thread in the **Discussions** tab or file an **Issue** with your laptop model and a copy-paste of your tray metrics! Your feedback directly updates our hardware calibration engine as we prepare the Tier 2 release (Advanced 80% Hard-Stop Charge Gating).

⚠️ A Note on Windows SmartScreen
Because VoltCore is a brand-new binary compiled outside the Microsoft Store, Windows may show a standard "Unrecognized App / SmartScreen" warning on your first launch. 

This is standard behavior for open-source utilities as they accumulate download reputation. You can verify the entire codebase transparently here in this repository. To run the app, click **More Info** -> **Run Anyway**.