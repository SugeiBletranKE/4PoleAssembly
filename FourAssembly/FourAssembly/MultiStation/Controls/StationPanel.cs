namespace FourAssembly.MultiStation.Controls;

using FourAssembly.Models;
using FourAssembly.Services;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;

public partial class StationPanel : UserControl
{
    private enum StationState { SCAN_STATION, SCAN_PART, SCAN_MATERIALS, SCAN_TOOLS, SCAN_CABLES, DONE }

    private readonly StationSettings _config;
    private readonly PlcService _plc;

    private StationState _state = StationState.SCAN_STATION;
    private Recipe? _currentRecipe;
    private int _materialIndex = 0;
    private int _toolIndex = 0;
    private List<string> _scannedCables = [];
    private int _cableCount = 0;

    public StationPanel(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;

        InitializeComponent();
        _lblStation.Text = _config.Name;
        Reset();
    }

    private void SubmitScan()
    {
        var barcode = _txtScanInput.Text.Trim();
        _txtScanInput.Clear();
        _txtScanInput.Focus();

        if (string.IsNullOrEmpty(barcode)) return;

        switch (_state)
        {
            case StationState.SCAN_STATION:
                int? stationNum = StationRegistry.Resolve(barcode);
                if (stationNum != _config.StationNumber)
                {
                    MessageBox.Show($"Invalid station. Expected {_config.Name}");
                    return;
                }
                _state = StationState.SCAN_PART;
                UpdateUI();
                break;

            case StationState.SCAN_PART:
                _currentRecipe = RecipeRepository.FindByPartNumber(barcode);
                if (_currentRecipe == null)
                {
                    MessageBox.Show($"Recipe '{barcode}' not found");
                    return;
                }
                _materialIndex = 0;
                _state = _currentRecipe.Materials.Count > 0 ? StationState.SCAN_MATERIALS : StationState.SCAN_TOOLS;
                UpdateUI();
                break;

            case StationState.SCAN_MATERIALS:
                if (_materialIndex >= _currentRecipe!.Materials.Count) return;
                if (_currentRecipe.Materials[_materialIndex].Barcode != barcode)
                {
                    MessageBox.Show($"Expected: {_currentRecipe.Materials[_materialIndex].Name} ({_currentRecipe.Materials[_materialIndex].Barcode})");
                    return;
                }
                _materialIndex++;
                if (_materialIndex >= _currentRecipe.Materials.Count)
                    _state = _currentRecipe.Tools.Count > 0 ? StationState.SCAN_TOOLS : StationState.SCAN_CABLES;
                UpdateUI();
                break;

            case StationState.SCAN_TOOLS:
                if (_toolIndex >= _currentRecipe!.Tools.Count) return;
                if (_currentRecipe.Tools[_toolIndex].Barcode != barcode)
                {
                    MessageBox.Show($"Expected: {_currentRecipe.Tools[_toolIndex].Name} ({_currentRecipe.Tools[_toolIndex].Barcode})");
                    return;
                }
                _toolIndex++;
                if (_toolIndex >= _currentRecipe.Tools.Count)
                    _state = StationState.SCAN_CABLES;
                UpdateUI();
                break;

            case StationState.SCAN_CABLES:
                if (!_scannedCables.Contains(barcode))
                {
                    _scannedCables.Add(barcode);
                    _lstCables.Items.Add(barcode);
                    _cableCount++;
                    _lblCableCount.Text = $"Count: {_cableCount}";
                }
                break;
        }
    }

    private void FinalizeStationCycle()
    {
        if (_state != StationState.SCAN_CABLES)
        {
            MessageBox.Show("Must be in scan cables state");
            return;
        }

        _plc.Connect();

        bool registerOk = _plc.WriteHoldingRegister(_config.PlcCounterRegister, _cableCount);
        if (!registerOk)
        {
            MessageBox.Show("Failed to write counter to PLC");
            return;
        }

        bool pulseOk = _plc.SendPulse(_config.PlcStartCoil, 500);
        if (!pulseOk)
        {
            MessageBox.Show("Failed to send pulse to PLC");
            return;
        }

        MessageBox.Show($"Cycle complete! Scanned {_cableCount} cables.", "Success");
        Reset();
    }

    private void UpdateUI()
    {
        switch (_state)
        {
            case StationState.SCAN_STATION:
                _lblInstruction.Text = "Scan station barcode";
                break;
            case StationState.SCAN_PART:
                _lblInstruction.Text = "Scan part number";
                break;
            case StationState.SCAN_MATERIALS:
                var material = _currentRecipe?.Materials[_materialIndex];
                _lblInstruction.Text = $"Scan material: {material?.Name} ({material?.Barcode})";
                break;
            case StationState.SCAN_TOOLS:
                var tool = _currentRecipe?.Tools[_toolIndex];
                _lblInstruction.Text = $"Scan tool: {tool?.Name} ({tool?.Barcode})";
                break;
            case StationState.SCAN_CABLES:
                _lblInstruction.Text = "Scan cables (press Finalize when done)";
                break;
        }
    }

    public void Reset()
    {
        _state = StationState.SCAN_STATION;
        _currentRecipe = null;
        _materialIndex = 0;
        _toolIndex = 0;
        _scannedCables.Clear();
        _cableCount = 0;
        _lstCables.Items.Clear();
        _lblCableCount.Text = "Count: 0";
        _txtScanInput.Clear();
        _txtScanInput.Focus();
        UpdateUI();

        CognexService.CableReceived += OnCognexDataReceived;
    }

    private void OnCognexDataReceived(int stationNum, string data)
    {
        if (stationNum == _config.StationNumber)
        {
            BeginInvoke(() =>
            {
                if (_state == StationState.SCAN_CABLES)
                {
                    if (!_scannedCables.Contains(data))
                    {
                        _scannedCables.Add(data);
                        _lstCables.Items.Add(data);
                        _cableCount++;
                        _lblCableCount.Text = $"Count: {_cableCount}";
                    }
                }
            });
        }
    }

    public void StopCognex()
    {
        CognexService.CableReceived -= OnCognexDataReceived;
    }

    private void _btnFinalize_Click(object sender, EventArgs e)
    {
        FinalizeStationCycle();
    }

    private void _btnReset_Click(object sender, EventArgs e)
    {

    }

    private void _txtScanInput_TextChanged(object sender, EventArgs e)
    {

    }

    private void _txtScanInput_KeyDown(object sender, KeyEventArgs e)
    {
        SubmitScan();
    }

    public void SetRecipeInfo(Recipe recipe)
    {
        if (recipe?.Stations.TryGetValue(_config.StationNumber.ToString(), out var stationConfig) == true)
        {
            lblBG.Text = stationConfig.BG;
        }
        else
        {
            lblBG.Text = "--";
        }
    }
}
