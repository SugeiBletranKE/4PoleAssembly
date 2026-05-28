namespace FourAssembly.MultiStation;

using FourAssembly.Forms;
using FourAssembly.Models;
using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;
using FourAssembly.Services;

public partial class FrmMain : Form
{
    private enum ScanState { Idle, Station, BG, Material, Tool, Ready }

    private readonly AppSettings _settings;
    private readonly PlcService _plc;
    private readonly List<FrmStation> _openStations = [];
    private Controls.StationPanel? _station1Panel;

    private ScanState _scanState = ScanState.Idle;
    private Recipe? _currentRecipe;
    private string _scannedStation = "";
    private StationConfig? _scannedStationConfig;
    private int _materialIndex = 0;
    private int _toolIndex = 0;
    public FrmMain(AppSettings settings, PlcService plc)
    {
        _settings = settings;
        _plc = plc;

        InitializeComponent();

        if (_settings.Stations.Count >= 3)
        {
            CognexService.Initialize(
                _settings.Stations[0].ComPort,
                _settings.Stations[1].ComPort,
                _settings.Stations[2].ComPort);
        }

        SetupStationPanel();
        LoadPartNumbers();
        OpenStations();
        _txtScanInput.KeyDown += _txtScanInput_KeyDown;
        this.FormClosing += FrmMain_FormClosing;
    }

    private void SetupStationPanel()
    {
        // Station 1 panel (fill remaining space)
        var stationConfig = new StationSettings
        {
            StationNumber = _settings.Stations[0].StationNumber,
            Name = _settings.Stations[0].Name,
            ComPort = _settings.Stations[0].ComPort,
            MonitorIndex = _settings.Stations[0].MonitorIndex,
            PlcStartCoil = _settings.Stations[0].PlcStartCoil,
            PlcCounterRegister = _settings.Stations[0].PlcCounterRegister
        };
        _station1Panel = new Controls.StationPanel(stationConfig, _plc)
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
            var appStation = _settings.Stations[i];
            var station = new StationSettings
            {
                StationNumber = appStation.StationNumber,
                Name = appStation.Name,
                ComPort = appStation.ComPort,
                MonitorIndex = appStation.MonitorIndex,
                PlcStartCoil = appStation.PlcStartCoil,
                PlcCounterRegister = appStation.PlcCounterRegister
            };

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

    private void btnEstaciones_Click(object sender, EventArgs e)
    {
        OpenStations();
    }

    private void btnRecipeEditor_Click(object sender, EventArgs e)
    {
        ShowRecipeEditor();
    }

    private void btnCognex_Click(object sender, EventArgs e)
    {
        ShowCognexDebug();
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {

    }

    private void LoadPartNumbers()
    {
        var partNumbers = RecipeRepository.GetAll()
            .Select(r => r.PartNumber)
            .OrderBy(pn => pn)
            .ToList();

        cmbVariante.DataSource = partNumbers;
    }

    private void cmbVariante_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbVariante.SelectedItem is string partNumber)
        {
            var recipe = RecipeRepository.FindByPartNumber(partNumber);
            if (recipe != null)
            {
                _currentRecipe = recipe;
                DisplayRecipeInfo(recipe);
                ResetScan();
            }
        }
    }

    private void DisplayRecipeInfo(Recipe recipe)
    {
        if (_station1Panel != null)
        {
            _station1Panel.SetRecipeInfo(recipe);
        }

        foreach (var station in _openStations)
        {
            if (!station.IsDisposed)
            {
                station.SetRecipeInfo(recipe);
            }
        }
    }

    private void ResetScan()
    {
        _scanState = ScanState.Station;
        _scannedStation = "";
        _scannedStationConfig = null;
        _materialIndex = 0;
        _toolIndex = 0;
        _txtScanInput.Clear();
        _txtScanInput.Focus();
        UpdateScanInstruction();
    }

    private void _txtScanInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            e.Handled = true;
            SubmitScan();
        }
    }

    private void SubmitScan()
    {
        var barcode = _txtScanInput.Text.Trim();
        _txtScanInput.Clear();
        _txtScanInput.Focus();

        if (string.IsNullOrEmpty(barcode) || _currentRecipe == null)
            return;

        switch (_scanState)
        {
            case ScanState.Station:
                ValidateStation(barcode);
                break;
            case ScanState.BG:
                ValidateBG(barcode);
                break;
            case ScanState.Material:
                ValidateMaterial(barcode);
                break;
            case ScanState.Tool:
                ValidateTool(barcode);
                break;
        }
    }

    private void ValidateStation(string barcode)
    {
        if (_currentRecipe.Stations.TryGetValue(barcode, out var stationConfig))
        {
            _scannedStation = barcode;
            _scannedStationConfig = stationConfig;
            _scanState = ScanState.BG;
            UpdateScanInstruction();
        }
        else
        {
            ShowError("Estación no válida");
        }
    }

    private void ValidateBG(string barcode)
    {
        if (_scannedStationConfig?.BG == barcode)
        {
            _scanState = ScanState.Material;
            _materialIndex = 0;
            UpdateScanInstruction();
        }
        else
        {
            ShowError($"BG incorrecto. Esperado: {_scannedStationConfig?.BG}");
        }
    }

    private void ValidateMaterial(string barcode)
    {
        if (_materialIndex < _currentRecipe.Materials.Count)
        {
            if (_currentRecipe.Materials[_materialIndex].Barcode == barcode)
            {
                _materialIndex++;
                if (_materialIndex >= _currentRecipe.Materials.Count)
                {
                    _scanState = ScanState.Tool;
                    _toolIndex = 0;
                }
                UpdateScanInstruction();
            }
            else
            {
                ShowError($"Material incorrecto. Esperado: {_currentRecipe.Materials[_materialIndex].Name}");
            }
        }
    }

    private void ValidateTool(string barcode)
    {
        if (_toolIndex < _currentRecipe.Tools.Count)
        {
            if (_currentRecipe.Tools[_toolIndex].Barcode == barcode)
            {
                _toolIndex++;
                if (_toolIndex >= _currentRecipe.Tools.Count)
                {
                    _scanState = ScanState.Ready;
                }
                UpdateScanInstruction();
            }
            else
            {
                ShowError($"Herramienta incorrecta. Esperada: {_currentRecipe.Tools[_toolIndex].Name}");
            }
        }
    }

    private void UpdateScanInstruction()
    {
        switch (_scanState)
        {
            case ScanState.Station:
                lblInfo.Text = "Escanear Estación";
                lblInfo.ForeColor = Color.Black;
                break;
            case ScanState.BG:
                lblInfo.Text = $"Escanear BG: {_scannedStationConfig?.BG}";
                lblInfo.ForeColor = Color.Black;
                break;
            case ScanState.Material:
                var matCount = $"{_materialIndex + 1}/{_currentRecipe?.Materials.Count}";
                lblInfo.Text = $"Escanear Material {matCount}";
                lblInfo.ForeColor = Color.Black;
                break;
            case ScanState.Tool:
                var toolCount = $"{_toolIndex + 1}/{_currentRecipe?.Tools.Count}";
                lblInfo.Text = $"Escanear Herramienta {toolCount}";
                lblInfo.ForeColor = Color.Black;
                break;
            case ScanState.Ready:
                lblInfo.Text = "✓ ¡Listo! Puedes abrir las estaciones";
                lblInfo.ForeColor = Color.Green;
                break;
        }
    }

    private void ShowError(string message)
    {
        lblInfo.Text = $"✗ {message}";
        lblInfo.ForeColor = Color.Red;
    }
}
