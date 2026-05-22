namespace FourAssembly.MultiStation.Controls;

using FourAssembly.Models;
using FourAssembly.Services;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;

public class StationPanel : UserControl
{
    private enum StationState { SCAN_STATION, SCAN_PART, SCAN_MATERIALS, SCAN_TOOLS, SCAN_CABLES, DONE }

    private readonly StationSettings _config;
    private readonly PlcService _plc;
    private StationCognexReader _cognex = null!;

    private StationState _state = StationState.SCAN_STATION;
    private Recipe? _currentRecipe;
    private int _materialIndex = 0;
    private int _toolIndex = 0;
    private List<string> _scannedCables = [];
    private int _cableCount = 0;

    // UI Controls
    private Label _lblStation = null!;
    private Label _lblInstruction = null!;
    private TextBox _txtScanInput = null!;
    private ListView _lstCables = null!;
    private Label _lblCableCount = null!;
    private Button _btnFinalize = null!;
    private Button _btnReset = null!;
    private Label _lblStatus = null!;

    public StationPanel(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;
        _cognex = new StationCognexReader(config.ComPort);

        this.BackColor = Color.White;
        SetupUI();
        Reset();
    }

    private void SetupUI()
    {
        // Header
        _lblStation = new Label { Text = _config.Name, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
        this.Controls.Add(_lblStation);

        _lblInstruction = new Label { Location = new Point(20, 60), AutoSize = true, MaximumSize = new Size(850, 0) };
        this.Controls.Add(_lblInstruction);

        // Scan input
        var lblInput = new Label { Text = "Scan:", Location = new Point(20, 110), AutoSize = true };
        _txtScanInput = new TextBox { Location = new Point(80, 108), Size = new Size(300, 24), Font = new Font("Consolas", 11) };
        _txtScanInput.KeyDown += (s, e) => { if (e.KeyCode == Keys.Return) SubmitScan(); };
        this.Controls.Add(lblInput);
        this.Controls.Add(_txtScanInput);

        // Cables ListView
        var lblCables = new Label { Text = "Scanned Cables:", Location = new Point(20, 150), AutoSize = true };
        _lstCables = new ListView { Location = new Point(20, 170), Size = new Size(450, 350), View = View.List };
        _cableCount = 0;
        this.Controls.Add(lblCables);
        this.Controls.Add(_lstCables);

        _lblCableCount = new Label { Text = "Count: 0", Location = new Point(500, 170), AutoSize = true };
        this.Controls.Add(_lblCableCount);

        // Buttons
        _btnFinalize = new Button { Text = "Finalize", Location = new Point(500, 250), Size = new Size(100, 40), BackColor = Color.LimeGreen };
        _btnFinalize.Click += (s, e) => FinalizeStationCycle();
        this.Controls.Add(_btnFinalize);

        _btnReset = new Button { Text = "Reset", Location = new Point(620, 250), Size = new Size(100, 40) };
        _btnReset.Click += (s, e) => Reset();
        this.Controls.Add(_btnReset);

        // Status
        _lblStatus = new Label { Location = new Point(20, 540), AutoSize = true, ForeColor = Color.Blue };
        this.Controls.Add(_lblStatus);
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

        if (!_cognex.Start())
            MessageBox.Show($"Warning: Could not open COM {_config.ComPort}");
    }

    public void StopCognex()
    {
        _cognex?.Stop();
    }
}
