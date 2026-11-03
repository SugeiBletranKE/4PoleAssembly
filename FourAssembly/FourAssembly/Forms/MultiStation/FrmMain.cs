namespace FourAssembly.MultiStation;

using FourAssembly.Forms;
using FourAssembly.Models;
using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;
using FourAssembly.Services;
using System.Runtime.InteropServices;

public partial class FrmMain : Form
{
    private const int MasterStationNumber = 3;
    private readonly AppSettings _settings;
    private readonly PlcService _plc;
    private readonly List<FrmStation> _openStations = [];
    private Controls.StationPanel? _masterStationPanel;
    private CurrentProcess? _currentProcess;
    private bool _systemMinimizedFromMenu;

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
        SubscribeToEvents();
        _txtScanInput.Visible = true;
        pnlMasterScan.Visible = true;
        pnlBottomArea.BringToFront();
        pnlMasterScan.BringToFront();
        _txtScanInput.KeyDown += _txtScanInput_KeyDown;
        ZebraFlowService.ZebraReadyForStation += OnZebraReadyForMasterStation;
        this.FormClosing += FrmMain_FormClosing;
        this.Resize += FrmMain_Resize;
    }

    private void SetupStationPanel()
    {
        var stationConfig = CreateStationSettings(GetMasterAppStationSettings());
        _masterStationPanel = new Controls.StationPanel(stationConfig, _plc)
        {
            Dock = DockStyle.Fill
        };
        this.Controls.Add(_masterStationPanel);
    }

    private void OpenStations()
    {
        var screens = Screen.AllScreens;
        SystemEventLogService.LogInfo("MONITOR", "General", $"{screens.Length} monitor(s) detected before opening station windows");
        var occupiedMonitorIndexes = new HashSet<int>();
        var masterStation = GetMasterAppStationSettings();
        occupiedMonitorIndexes.Add(GetValidMonitorIndex(masterStation.MonitorIndex));

        foreach (var appStation in _settings.Stations.Where(station => station.StationNumber != MasterStationNumber))
        {
            var station = CreateStationSettings(appStation);

            var existingForm = _openStations.FirstOrDefault(f =>
                !f.IsDisposed && f.Tag?.ToString() == station.StationNumber.ToString());

            if (existingForm != null)
            {
                if (existingForm.WindowState == FormWindowState.Minimized)
                    RestoreStationWindow(existingForm);

                existingForm.BringToFront();
                continue;
            }

            var form = new FrmStation(station, _plc);
            form.Tag = station.StationNumber.ToString();
            _openStations.Add(form);

            form.FormClosed += (s, e) => _openStations.Remove(form);

            ShowOnMonitor(form, station, occupiedMonitorIndexes);
            form.Show();
        }
    }

    private FourAssembly.Services.Settings GetMasterAppStationSettings()
    {
        return _settings.Stations.FirstOrDefault(station => station.StationNumber == MasterStationNumber)
            ?? _settings.Stations.First();
    }

    private StationSettings CreateStationSettings(FourAssembly.Services.Settings appStation)
    {
        return new StationSettings
        {
            StationNumber = appStation.StationNumber,
            SapStationNumber = _settings.SapStationNumber,
            AdminPass = _settings.AdminPass,
            Name = appStation.Name,
            ComPort = appStation.ComPort,
            MonitorIndex = appStation.MonitorIndex,
            PlcButton = appStation.PlcButton,
            PlcActiveStationRegister = appStation.PlcActiveStationRegister ?? _settings.Plc.ActiveStationRegister,
            PlcConnectorSideRegister = appStation.PlcConnectorSideRegister ?? _settings.Plc.ConnectorSideRegister,
            PlcResetMemory = _settings.Plc.ResetMemory,
            PlcSerialsComplete = appStation.PlcSerialsComplete,
            PlcStation = appStation.PlcStation,
            PlcCounter = appStation.PlcCounter,
            PlcInsertedCountRegister = appStation.PlcInsertedCountRegister,
            PlcQuantityProcessMemory = appStation.PlcQuantityProcessMemory,
            PlcFinalProcessMemory = appStation.PlcFinalProcessMemory,
            PlcCounterCompleteMemory = appStation.PlcCounterCompleteMemory,
            PlcScrapMemory = appStation.PlcScrapMemory,
            PlcBtnYellow = appStation.PlcBtnYellow,
            PlcMaterialRemovedMemory = appStation.PlcMaterialRemovedMemory,
            PlcCavityErrorMemory = appStation.PlcCavityErrorMemory,
            PlcCavityErrorRegister = appStation.PlcCavityErrorRegister,
            PlcQualityAuthorizationMemory = _settings.Plc.QualityAuthorizationMemory,
            PlcFemalePinMemories = new List<int>(appStation.PlcFemalePinMemories),
            PlcMalePinMemories = new List<int>(appStation.PlcMalePinMemories),
            PlcFemale = _settings.PlcConnector.Female,
            PlcMale = _settings.PlcConnector.Male
        };
    }

    private void ShowOnMonitor(Form form, StationSettings station, HashSet<int> occupiedMonitorIndexes)
    {
        var screens = Screen.AllScreens;
        var monitorIndex = ResolveStationMonitorIndex(station, occupiedMonitorIndexes);
        var screen = screens[monitorIndex];
        var stationName = $"Station {station.StationNumber}";

        SystemEventLogService.LogInfo(
            "MONITOR",
            stationName,
            $"Using monitor {monitorIndex} of {screens.Length} detected monitor(s). Configured monitor was {station.MonitorIndex}.");

        form.StartPosition = FormStartPosition.Manual;
        form.Location = screen.WorkingArea.Location;
        form.WindowState = FormWindowState.Maximized;
    }

    private int ResolveStationMonitorIndex(StationSettings station, HashSet<int> occupiedMonitorIndexes)
    {
        var screens = Screen.AllScreens;
        if (screens.Length == 0)
            return 0;

        var requestedMonitorIndex = station.MonitorIndex;
        if (requestedMonitorIndex >= 0
            && requestedMonitorIndex < screens.Length
            && !occupiedMonitorIndexes.Contains(requestedMonitorIndex))
        {
            occupiedMonitorIndexes.Add(requestedMonitorIndex);
            return requestedMonitorIndex;
        }

        var availableMonitorIndex = Enumerable
            .Range(0, screens.Length)
            .FirstOrDefault(index => !occupiedMonitorIndexes.Contains(index), -1);

        if (availableMonitorIndex >= 0)
        {
            occupiedMonitorIndexes.Add(availableMonitorIndex);
            SystemEventLogService.LogInfo(
                "MONITOR",
                $"Station {station.StationNumber}",
                $"Configured monitor {requestedMonitorIndex} is unavailable or already used. Using monitor {availableMonitorIndex}.");
            return availableMonitorIndex;
        }

        var mainMonitorIndex = GetValidMonitorIndex(GetMasterAppStationSettings().MonitorIndex);
        var sharedMonitorIndex = Enumerable
            .Range(0, screens.Length)
            .FirstOrDefault(index => index != mainMonitorIndex, mainMonitorIndex);

        SystemEventLogService.LogError(
            "MONITOR",
            $"Station {station.StationNumber}",
            $"No free monitor available for configured monitor {requestedMonitorIndex}. Sharing monitor {sharedMonitorIndex}.");
        return sharedMonitorIndex;
    }

    private static int GetValidMonitorIndex(int monitorIndex)
    {
        var screens = Screen.AllScreens;
        return monitorIndex >= 0 && monitorIndex < screens.Length
            ? monitorIndex
            : 0;
    }

    private void SubscribeToEvents()
    {
        ProcessManager.ProcessStarted += OnProcessStarted;
        ProcessManager.StationStateChanged += OnStationStateChanged;
        ProcessManager.ProcessCompleted += OnProcessCompleted;
        ProcessManager.ProcessReset += OnProcessReset;
    }

    private void OnProcessStarted(CurrentProcess process)
    {
        _currentProcess = process;
        UpdateProcessDisplay();
    }

    private void OnStationStateChanged(string stationCode, StationProcessState newState)
    {
        UpdateProcessDisplay();
    }

    private void OnProcessCompleted()
    {
        lblInfo.Text = "✓ ¡Proceso completado!";
        lblInfo.ForeColor = Color.Green;
        _currentProcess = null;
    }

    private void OnProcessReset()
    {
        _currentProcess = null;
        lblInfo.Text = "Proceso reiniciado.";
        lblInfo.ForeColor = Color.DarkOrange;
    }

    private void UpdateProcessDisplay()
    {
        if (_currentProcess == null)
            return;

        var statusText = $"PartNumber: {_currentProcess.PartNumber} | {_currentProcess.Description}";

        var statesText = "\nEstaciones:\n";
        foreach (var station in _currentProcess.EnabledStations)
        {
            var state = ProcessManager.GetStationState(station);
            var icon = state switch
            {
                StationProcessState.WAITING_ZEBRA => "⏳",
                StationProcessState.ZEBRA_IN_PROGRESS => "🔵",
                StationProcessState.COGNEX_ACTIVE => "📷",
                StationProcessState.COGNEX_DONE => "✓",
                StationProcessState.PLC_RELEASED => "✓",
                StationProcessState.DONE => "✓",
                _ => "•"
            };
            statesText += $"  Station {station}: {icon} {state}\n";
        }

        lblInfo.Text = statusText + statesText;
        lblInfo.ForeColor = Color.Black;
    }

    private void LoadPartNumbers()
    {
        RecipeRepository.Load();
        var partNumbers = RecipeRepository.GetAll()
            .Select(r => r.PartNumber)
            .OrderBy(pn => pn)
            .ToList();

        cmbVariante.DataSource = partNumbers;
    }

    private void cmbVariante_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Solo actualiza cuando cambias la selección, sin mostrar mensajes
    }

    private void DisplayRecipeInfo(Recipe recipe)
    {
        _masterStationPanel?.SetRecipeInfo(recipe);
        foreach (var station in _openStations)
        {
            if (!station.IsDisposed)
            {
                station.SetRecipeInfo(recipe);
            }
        }
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        ZebraFlowService.ZebraReadyForStation -= OnZebraReadyForMasterStation;
        foreach (var station in _openStations.ToList())
        {
            if (!station.IsDisposed)
                station.Close();
        }

        _masterStationPanel?.StopCognex();
        ProcessManager.ProcessStarted -= OnProcessStarted;
        ProcessManager.StationStateChanged -= OnStationStateChanged;
        ProcessManager.ProcessCompleted -= OnProcessCompleted;
        ProcessManager.ProcessReset -= OnProcessReset;
        ProcessManager.ResetProcess();
        ZebraFlowService.Reset();
        CognexFlowService.Reset();
    }

    private void btnEstaciones_Click(object sender, EventArgs e)
    {
        MinimizeSystem();
    }

    private void MinimizeSystem()
    {
        var openStations = _openStations
            .Where(station => !station.IsDisposed)
            .ToList();

        foreach (var station in openStations)
            MinimizeStationWindow(station);

        _systemMinimizedFromMenu = true;
        ShowWindow(this.Handle, SW_FORCEMINIMIZE);
    }

    private void FrmMain_Resize(object? sender, EventArgs e)
    {
        if (!_systemMinimizedFromMenu || WindowState == FormWindowState.Minimized)
            return;

        _systemMinimizedFromMenu = false;
        RestoreStationWindows();
    }

    private static void MinimizeStationWindow(Form station)
    {
        ShowWindow(station.Handle, SW_FORCEMINIMIZE);
    }

    private void RestoreStationWindows()
    {
        var openStations = _openStations
            .Where(station => !station.IsDisposed)
            .ToList();

        if (openStations.Count == 0)
        {
            OpenStations();
            return;
        }

        foreach (var station in openStations)
            RestoreStationWindow(station);
    }

    private static void RestoreStationWindow(Form station)
    {
        ShowWindow(station.Handle, SW_RESTORE);
        station.WindowState = FormWindowState.Maximized;
    }

    private void btnRecipeEditor_Click(object sender, EventArgs e)
    {
        ShowRecipeEditor();
    }

    private void ShowRecipeEditor()
    {
        var editorForm = new RecipeEditorForm();
        editorForm.ShowDialog(this);
        LoadPartNumbers();
    }

    private void btnCognex_Click(object sender, EventArgs e)
    {
        ShowCognexDebug();
    }

    private void btnPlc_Click(object sender, EventArgs e)
    {
        ShowPlcDebug();
    }

    private void btnPresence_Click(object sender, EventArgs e)
    {
        var presenceForm = new CavityPresenceForm(_plc, _settings);
        presenceForm.Show(this);
    }

    private void ShowCognexDebug()
    {
        var cognexForm = new CognexDiagnosticsForm();
        cognexForm.Show(this);
    }

    private void ShowPlcDebug()
    {
        var plcForm = new PlcDiagnosticsForm(_plc, _settings.Plc);
        plcForm.Show(this);
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        using var settingsForm = new SettingsEditorForm(ResetFromSettingsAsync);
        settingsForm.ShowDialog(this);
    }

    private void btnCounters_Click(object sender, EventArgs e)
    {
        var counters = new List<StationCountersForm.StationCounterAccess>();

        if (_masterStationPanel is not null)
        {
            var snapshot = _masterStationPanel.GetGoodPieceCounterSnapshot();
            counters.Add(new StationCountersForm.StationCounterAccess(
                snapshot.StationNumber,
                snapshot.StationName,
                _masterStationPanel.GetGoodPieceCounterSnapshot,
                _masterStationPanel.ResetTotalGoodPieceCounterFromMain,
                _masterStationPanel.ResetBgGoodPieceCounterFromMain,
                _masterStationPanel.ResetHourlyGoodPieceCounterFromMain));
        }

        foreach (var stationForm in _openStations.Where(form => !form.IsDisposed))
        {
            var snapshot = stationForm.GetGoodPieceCounterSnapshot();
            if (snapshot is null || counters.Any(item => item.StationNumber == snapshot.StationNumber))
                continue;

            counters.Add(new StationCountersForm.StationCounterAccess(
                snapshot.StationNumber,
                snapshot.StationName,
                () => stationForm.GetGoodPieceCounterSnapshot() ?? snapshot,
                stationForm.ResetTotalGoodPieceCounterFromMain,
                stationForm.ResetBgGoodPieceCounterFromMain,
                stationForm.ResetHourlyGoodPieceCounterFromMain));
        }

        if (counters.Count == 0)
        {
            MessageBox.Show(this, "No hay estaciones abiertas.", "Contadores",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new StationCountersForm(counters);
        form.ShowDialog(this);
    }

    private void btnToggleSelector_Click(object sender, EventArgs e)
    {
        var showSelector = !pnlCmb.Visible;
        label1.Visible = showSelector;
        pnlCmb.Visible = showSelector;
        btnToggleSelector.Text = showSelector ? "Ocultar selector" : "Mostrar selector";
        PerformLayout();
    }

    private async Task ResetFromSettingsAsync(int? stationNumber)
    {
        if (stationNumber.HasValue)
        {
            await ResetStationFromMainAsync(stationNumber.Value);
            return;
        }

        if (ConfirmReset("¿Reiniciar todas las estaciones?") && _masterStationPanel != null)
            await _masterStationPanel.ResetAllStationsFromMainAsync();
    }

    private async Task ResetStationFromMainAsync(int stationNumber)
    {
        if (!ConfirmReset($"¿Reiniciar únicamente la estación {stationNumber}?"))
            return;

        if (stationNumber == MasterStationNumber)
        {
            if (_masterStationPanel != null)
                await _masterStationPanel.ResetStationFromMainAsync();
            return;
        }

        var stationForm = _openStations.FirstOrDefault(form =>
            !form.IsDisposed && form.Tag?.ToString() == stationNumber.ToString());

        if (stationForm == null)
        {
            MessageBox.Show(this, $"La estación {stationNumber} no está abierta.", "Reset de estación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await stationForm.ResetStationFromMainAsync();
    }

    private bool ConfirmReset(string message)
    {
        return MessageBox.Show(this, message, "Confirmar reset",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
    }

    private void _txtScanInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            var barcode = _txtScanInput.Text.Trim();
            _txtScanInput.Clear();
            _masterStationPanel?.SubmitExternalScan(barcode);
            _txtScanInput.Focus();
        }
    }

    private void OnZebraReadyForMasterStation(string stationCode)
    {
        if (stationCode != MasterStationNumber.ToString() || IsDisposed)
            return;

        if (InvokeRequired)
        {
            BeginInvoke(() => OnZebraReadyForMasterStation(stationCode));
            return;
        }

        _txtScanInput.Visible = true;
        _txtScanInput.BringToFront();
        _txtScanInput.Focus();
    }

    private void ShowError(string message)
    {
        lblInfo.Text = $"✗ {message}";
        lblInfo.ForeColor = Color.Red;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void pnMenu_MouseDown(object sender, MouseEventArgs e)
    {
        ReleaseCapture();
        SendMessage(this.Handle, 0x112, 0xf012, 0);
    }

    [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
    private extern static void ReleaseCapture();

    [DllImport("user32.DLL", EntryPoint = "SendMessage")]
    private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

    private const int SW_RESTORE = 9;
    private const int SW_FORCEMINIMIZE = 11;

    [DllImport("user32.DLL")]
    private extern static bool ShowWindow(System.IntPtr hWnd, int nCmdShow);

    private async void btnActive_Click(object sender, EventArgs e)
    {
        if (cmbVariante.SelectedItem is string partNumber)
        {
            var activeProcess = ProcessManager.GetCurrentProcess();
            if (activeProcess != null
                && string.Equals(
                    activeProcess.PartNumber,
                    partNumber,
                    StringComparison.OrdinalIgnoreCase))
            {
                lblInfo.Text = $"PartNumber {partNumber} ya esta activo.";
                lblInfo.ForeColor = Color.DodgerBlue;
                return;
            }

            var recipe = RecipeRepository.FindByPartNumber(partNumber);
            if (recipe != null)
            {
                if (activeProcess != null)
                    await ResetForPartNumberChangeAsync(activeProcess.PartNumber, partNumber);

                ProcessManager.StartProcess(recipe);
                var process = ProcessManager.GetCurrentProcess();
                if (process != null)
                {
                    ZebraFlowService.InitializeZebraFlow(process);

                    if (process.EnabledStations.Count > 0)
                    {
                        var firstStation = process.EnabledStations[0];
                        lblInfo.Text = $"⬇️ Dirígete a STATION {firstStation} - Escanea S{firstStation:00}";
                        lblInfo.ForeColor = Color.DodgerBlue;

                        ZebraFlowService.TryReleaseZebraToStation(firstStation);
                        if (firstStation == MasterStationNumber.ToString())
                            _txtScanInput.Focus();
                    }
                    else
                    {
                        lblInfo.Text = "❌ No hay estaciones habilitadas";
                        lblInfo.ForeColor = Color.Red;
                    }
                }
            }

            else
            {
                lblInfo.Text = "❌ PartNumber no encontrado";
                lblInfo.ForeColor = Color.Red;
            }
        }
        else
        {
            lblInfo.Text = "❌ Selecciona un PartNumber primero";
            lblInfo.ForeColor = Color.Red;
        }
    }

    private async Task ResetForPartNumberChangeAsync(string previousPartNumber, string nextPartNumber)
    {
        LogService.Log($"PartNumber change reset started: {previousPartNumber} -> {nextPartNumber}");

        ZebraFlowService.Reset();
        CognexFlowService.Reset();
        ProcessManager.ResetProcess();

        if (_settings.Plc.ResetMemory > 0)
        {
            var resetOk = await Task.Run(() => _plc.SendPulse(_settings.Plc.ResetMemory, 500));
            LogService.Log(resetOk
                ? $"PartNumber change global PLC reset pulse sent on M{_settings.Plc.ResetMemory}"
                : $"PartNumber change global PLC reset pulse FAILED on M{_settings.Plc.ResetMemory}");
        }

        var pinRegisters = _settings.Stations
            .SelectMany(station => station.PlcFemalePinMemories.Concat(station.PlcMalePinMemories))
            .Where(register => register > 0)
            .Distinct()
            .ToList();

        foreach (var register in pinRegisters)
        {
            var ok = await Task.Run(() => _plc.WriteHoldingRegister(register, 0));
            LogService.Log(ok
                ? $"PartNumber change reset BG pin register D{register} = 0"
                : $"PartNumber change failed to reset BG pin register D{register}");
        }

        LogService.Log($"PartNumber change reset completed: {previousPartNumber} -> {nextPartNumber}");
    }

}
