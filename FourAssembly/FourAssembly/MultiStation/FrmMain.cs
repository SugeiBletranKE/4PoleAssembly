namespace FourAssembly.MultiStation;

using FourAssembly.Forms;
using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;

public class FrmMain : Form
{
    private readonly MultiAppSettings _settings;
    private readonly PlcService _plc;
    private readonly List<FrmStation> _openStations = [];
    private Controls.StationPanel? _station1Panel;

    public FrmMain(MultiAppSettings settings, PlcService plc)
    {
        _settings = settings;
        _plc = plc;

        this.Text = "FourAssembly - Multi-Station Control";
        this.ClientSize = new Size(1200, 700);
        this.BackColor = Color.White;

        SetupUI();
        this.FormClosing += FrmMain_FormClosing;
    }

    private void SetupUI()
    {
        // Menu panel (60px top)
        var menuPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(40, 40, 40)
        };

        var btnEstaciones = new Button
        {
            Text = "Estaciones",
            Location = new Point(10, 10),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        btnEstaciones.Click += (s, e) => OpenStations();
        menuPanel.Controls.Add(btnEstaciones);

        var btnRecipeEditor = new Button
        {
            Text = "Editor Recetas",
            Location = new Point(140, 10),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        btnRecipeEditor.Click += (s, e) => ShowRecipeEditor();
        menuPanel.Controls.Add(btnRecipeEditor);

        var btnCognex = new Button
        {
            Text = "Cognex Debug",
            Location = new Point(270, 10),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        btnCognex.Click += (s, e) => ShowCognexDebug();
        menuPanel.Controls.Add(btnCognex);

        var btnSettings = new Button
        {
            Text = "Settings",
            Location = new Point(400, 10),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        btnSettings.Click += (s, e) => MessageBox.Show("Settings coming soon");
        menuPanel.Controls.Add(btnSettings);

        this.Controls.Add(menuPanel);

        // Station 1 panel (fill remaining space)
        _station1Panel = new Controls.StationPanel(_settings.Stations[0], _plc)
        {
            Dock = DockStyle.Fill
        };
        this.Controls.Add(_station1Panel);
    }

    private void OpenStations()
    {
        var screens = Screen.AllScreens;

        for (int i = 1; i < _settings.Stations.Count; i++)
        {
            var station = _settings.Stations[i];

            // Check if station is already open
            var existingForm = _openStations.FirstOrDefault(f =>
                !f.IsDisposed && f.Tag?.ToString() == station.StationNumber.ToString());

            if (existingForm != null)
            {
                existingForm.BringToFront();
                existingForm.WindowState = FormWindowState.Normal;
                continue;
            }

            // Create new FrmStation
            var form = new FrmStation(station, _plc);
            form.Tag = station.StationNumber.ToString();
            _openStations.Add(form);

            form.FormClosed += (s, e) => _openStations.Remove(form);

            // Show on correct monitor
            ShowOnMonitor(form, station.MonitorIndex);
            form.Show();
        }
    }

    private void ShowOnMonitor(Form form, int monitorIndex)
    {
        var screens = Screen.AllScreens;
        var screen = screens.Length > monitorIndex ? screens[monitorIndex] : screens[0];
        form.StartPosition = FormStartPosition.Manual;
        form.Location = screen.WorkingArea.Location;
        form.WindowState = FormWindowState.Maximized;
    }

    private void ShowRecipeEditor()
    {
        var editorForm = new RecipeEditorForm();
        editorForm.ShowDialog(this);
    }

    private void ShowCognexDebug()
    {
        var cognexForm = new CognexDiagnosticsForm();
        cognexForm.Show(this);
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Close all open stations
        foreach (var station in _openStations.ToList())
        {
            if (!station.IsDisposed)
                station.Close();
        }

        _station1Panel?.StopCognex();
    }
}
