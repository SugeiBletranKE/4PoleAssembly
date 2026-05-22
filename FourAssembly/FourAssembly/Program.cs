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
            ApplicationConfiguration.Initialize();

            // Load settings and initialize services
            var settings = MultiSettingsManager.Load();
            RecipeRepository.Load();
            LogService.Initialize();

            var plc = new PlcService(settings.Plc);

            // Create and show FrmMain on Monitor 0
            var frmMain = new FrmMain(settings, plc);
            ShowOnMonitor(frmMain, settings.Stations[0].MonitorIndex);

            // FrmMain is the main form — closing it closes the app
            Application.Run(frmMain);
        }

        static void ShowOnMonitor(Form form, int monitorIndex)
        {
            var screens = Screen.AllScreens;
            var screen = screens.Length > monitorIndex ? screens[monitorIndex] : screens[0];
            form.StartPosition = FormStartPosition.Manual;
            form.Location = screen.WorkingArea.Location;
            form.WindowState = FormWindowState.Maximized;
        }
    }
}