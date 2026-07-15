namespace FourAssembly
{
    using FourAssembly.MultiStation;
    using FourAssembly.MultiStation.Services;
    using FourAssembly.Services;

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ApplicationConfiguration.Initialize();

                var settings = SettingsManager.Load();
                StationInfoApiService.Initialize(settings.Api);
                RecipeRepository.Load();
                LogService.Initialize();
                SystemEventLogService.LogInfo("APP", "General", "Application started");

                var plc = new PlcService(settings.Plc);

                var frmMain = new FrmMain(settings, plc);
                var masterStation = settings.Stations.FirstOrDefault(station => station.StationNumber == 3)
                    ?? settings.Stations.FirstOrDefault();
                ShowOnMonitor(frmMain, masterStation?.MonitorIndex ?? 0);

                Application.Run(frmMain);
            }
            catch (Exception ex)
            {
                WriteStartupError(ex);
                MessageBox.Show(
                    $"No se pudo iniciar el sistema.{Environment.NewLine}{Environment.NewLine}{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{Environment.NewLine}Detalle: C:\\ke\\FourPole\\startup_error.log",
                    "FourAssembly",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        static void WriteStartupError(Exception ex)
        {
            try
            {
                var dir = @"C:\ke\FourPole";
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, "startup_error.log");
                File.AppendAllText(
                    path,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {ex}{Environment.NewLine}");
            }
            catch
            {
                // Startup error reporting must not throw another startup error.
            }
        }

        static void ShowOnMonitor(Form form, int monitorIndex)
        {
            var screens = Screen.AllScreens;
            var screen = screens.Length > monitorIndex ? screens[monitorIndex] : screens[0];
            var fallback = screens.Length <= monitorIndex;
            var message = fallback
                ? $"Requested monitor {monitorIndex}, but only {screens.Length} monitor(s) detected. Using monitor 0."
                : $"Using monitor {monitorIndex} of {screens.Length} detected monitor(s).";

            SystemEventLogService.LogInfo("MONITOR", form.Name, message);
            form.StartPosition = FormStartPosition.Manual;
            form.Location = screen.WorkingArea.Location;
            form.WindowState = FormWindowState.Maximized;
        }
    }
}
