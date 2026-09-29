namespace FourAssembly.MultiStation;

using FourAssembly.Models;
using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;
using FourAssembly.Services;

public partial class FrmStation : Form
{
    private readonly StationSettings _config;
    private readonly PlcService _plc;
    private StationPanel? _stationPanel;
    private CurrentProcess? _currentProcess;

    public FrmStation(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;

        InitializeComponent();
        this.Text = config.Name;
        this.WindowState = FormWindowState.Maximized;

        CreateStationPanel();
        SubscribeToEvents();
    }

    private void CreateStationPanel()
    {
        lblDesignerHint.Visible = false;
        _stationPanel = new StationPanel(_config, _plc)
        {
            Dock = DockStyle.Fill
        };
        stationPanelHost.Controls.Add(_stationPanel);
    }

    private void SubscribeToEvents()
    {
        ProcessManager.ProcessStarted += OnProcessStarted;
        ProcessManager.StationStateChanged += OnStationStateChanged;
        FormClosing += FrmStation_FormClosing;
    }

    private void OnProcessStarted(CurrentProcess process)
    {
        _currentProcess = process;
        UpdateStatusPanel();
    }

    private void OnStationStateChanged(string stationCode, StationProcessState newState)
    {
        if (stationCode != _config.StationNumber.ToString())
            return;

        UpdateStatusPanel();
    }

    private void UpdateStatusPanel()
    {
        if (_currentProcess == null)
        {
            _lblPartNumber.Text = "PartNumber: --";
            _lblStatus.Text = "Status: --";
            _lblProcessState.Text = "Esperando...";
            return;
        }

        _lblPartNumber.Text = $"PartNumber: {_currentProcess.PartNumber}";
        _lblStatus.Text = $"Description: {_currentProcess.Description}";

        var stationCode = _config.StationNumber.ToString();
        var state = ProcessManager.GetStationState(stationCode);

        var stateColor = state switch
        {
            StationProcessState.DISABLED => Color.Gray,
            StationProcessState.WAITING_ZEBRA => Color.Orange,
            StationProcessState.ZEBRA_IN_PROGRESS => Color.Blue,
            StationProcessState.COGNEX_ACTIVE => Color.Green,
            StationProcessState.COGNEX_DONE => Color.DarkGreen,
            StationProcessState.PLC_RELEASED => Color.DarkGreen,
            StationProcessState.DONE => Color.Green,
            _ => Color.White
        };

        var stateIcon = state switch
        {
            StationProcessState.DISABLED => "⚫",
            StationProcessState.WAITING_ZEBRA => "⏳",
            StationProcessState.ZEBRA_IN_PROGRESS => "🔵",
            StationProcessState.COGNEX_ACTIVE => "📷",
            StationProcessState.COGNEX_DONE => "✓",
            StationProcessState.PLC_RELEASED => "✓",
            StationProcessState.DONE => "✓",
            _ => "•"
        };

        _lblProcessState.Text = $"{stateIcon} {state}";
        _lblProcessState.ForeColor = stateColor;
    }

    private void FrmStation_FormClosing(object? sender, FormClosingEventArgs e)
    {
        UnsubscribeFromEvents();
        _stationPanel?.StopCognex();
    }

    private void UnsubscribeFromEvents()
    {
        ProcessManager.ProcessStarted -= OnProcessStarted;
        ProcessManager.StationStateChanged -= OnStationStateChanged;
    }

    public void SetRecipeInfo(Recipe recipe)
    {
        _stationPanel?.SetRecipeInfo(recipe);
    }

    public void StartCableScan()
    {
        _stationPanel?.StartCableScan();
    }

    public Task ResetStationFromMainAsync()
    {
        return _stationPanel?.ResetStationFromMainAsync() ?? Task.CompletedTask;
    }

    public StationPanel.GoodPieceCounterSnapshot? GetGoodPieceCounterSnapshot() =>
        _stationPanel?.GetGoodPieceCounterSnapshot();

    public void ResetGoodPieceCountersFromMain() =>
        _stationPanel?.ResetGoodPieceCountersFromMain();

    public void ResetTotalGoodPieceCounterFromMain() =>
        _stationPanel?.ResetTotalGoodPieceCounterFromMain();

    public void ResetBgGoodPieceCounterFromMain() =>
        _stationPanel?.ResetBgGoodPieceCounterFromMain();

    public void ResetHourlyGoodPieceCounterFromMain() =>
        _stationPanel?.ResetHourlyGoodPieceCounterFromMain();
}
