namespace FourAssembly.Controls;

using System.Media;
using FourAssembly.Models;
using FourAssembly.Services;

public enum StationState
{
    SCAN_STATION,
    SCAN_PART,
    SCAN_MATERIALS,
    SCAN_TOOLS,
    SCAN_CABLES,
    DONE
}

public partial class StationControl : UserControl
{
    public event Action<int>? StationScanned;

    private StationState _state = StationState.SCAN_STATION;
    private Recipe? _recipe;
    private int _itemIndex;
    private int _currentStationNumber;
    private readonly List<string> _cablesScanned = [];

    public int StationNumber { get; init; }

    public StationControl()
    {
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        txtScanInput.KeyDown += TxtScanInput_KeyDown;
    }

    private void TxtScanInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            SubmitScan(txtScanInput.Text);
            txtScanInput.Clear();
        }
    }

    public void SubmitScan(string rawBarcode)
    {
        var barcode = rawBarcode.Trim();
        switch (_state)
        {
            case StationState.SCAN_STATION:
                HandleScanStation(barcode);
                break;
            case StationState.SCAN_PART:
                HandleScanPart(barcode);
                break;
            case StationState.SCAN_MATERIALS:
                HandleScanMaterial(barcode);
                break;
            case StationState.SCAN_TOOLS:
                HandleScanTool(barcode);
                break;
        }
    }

    private void HandleScanStation(string barcode)
    {
        var stationNum = StationRegistry.Resolve(barcode);
        if (stationNum is null)
        {
            ShowError($"Unknown station: {barcode}");
            return;
        }

        _currentStationNumber = stationNum.Value;
        lblStationTitle!.Text = $"Estación {_currentStationNumber:D2}";
        ShowSuccess($"Estación identificada: {_currentStationNumber:D2}");
        StationScanned?.Invoke(_currentStationNumber);
        TransitionTo(StationState.SCAN_PART);
    }

    private void HandleScanPart(string barcode)
    {
        var recipe = RecipeRepository.FindByPartNumber(barcode);
        if (recipe is null)
        {
            ShowError($"No recipe for: {barcode}");
            return;
        }
        _recipe = recipe;
        _itemIndex = 0;
        ShowSuccess($"Recipe loaded: {recipe.PartNumber}");
        TransitionTo(StationState.SCAN_MATERIALS);
    }

    private void HandleScanMaterial(string barcode)
    {
        var expected = _recipe!.Materials[_itemIndex];
        if (!string.Equals(barcode, expected.Barcode, StringComparison.OrdinalIgnoreCase))
        {
            ShowError($"Wrong material. Expected: {expected.Name}");
            return;
        }

        ShowSuccess($"Material OK: {expected.Name}");
        _itemIndex++;

        if (_itemIndex >= _recipe.Materials.Count)
        {
            _itemIndex = 0;
            TransitionTo(StationState.SCAN_TOOLS);
        }
        else
        {
            PromptNextItem();
        }
    }

    private void HandleScanTool(string barcode)
    {
        var expected = _recipe!.Tools[_itemIndex];
        if (!string.Equals(barcode, expected.Barcode, StringComparison.OrdinalIgnoreCase))
        {
            ShowError($"Wrong tool. Expected: {expected.Name}");
            return;
        }

        ShowSuccess($"Tool OK: {expected.Name}");
        _itemIndex++;

        if (_itemIndex >= _recipe.Tools.Count)
        {
            TransitionTo(StationState.SCAN_CABLES);
        }
        else
        {
            PromptNextItem();
        }
    }

    private void ShowError(string message)
    {
        panelStatus.BackColor = Color.Red;
        lblStatus.ForeColor = Color.White;
        lblStatus.Text = message;
        SystemSounds.Beep.Play();
    }

    private void ShowSuccess(string message)
    {
        panelStatus.BackColor = Color.LimeGreen;
        lblStatus.ForeColor = Color.Black;
        lblStatus.Text = message;
    }

    private void TransitionTo(StationState next)
    {
        if (next == StationState.SCAN_MATERIALS && _recipe!.Materials.Count == 0)
            next = StationState.SCAN_TOOLS;
        if (next == StationState.SCAN_TOOLS && _recipe!.Tools.Count == 0)
            next = StationState.DONE;

        _state = next;
        UpdatePromptUI();
    }

    private void UpdatePromptUI()
    {
        panelStatus.BackColor = Color.LightGray;
        lblStatus.Text = string.Empty;

        lblInstruction.Text = _state switch
        {
            StationState.SCAN_STATION => "Scan the STATION barcode on this PC",
            StationState.SCAN_PART => "Scan the PART NUMBER barcode",
            StationState.SCAN_MATERIALS => $"Scan MATERIAL {_itemIndex + 1}/{_recipe!.Materials.Count}: {_recipe.Materials[_itemIndex].Name}",
            StationState.SCAN_TOOLS => $"Scan TOOL {_itemIndex + 1}/{_recipe!.Tools.Count}: {_recipe.Tools[_itemIndex].Name}",
            StationState.SCAN_CABLES => $"Scan CABLES with Cognex ({_cablesScanned.Count} registered)",
            StationState.DONE => "ALL CHECKS PASSED - Ready for next cycle",
            _ => string.Empty
        };

        if (_state == StationState.DONE)
        {
            panelStatus.BackColor = Color.DodgerBlue;
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = "COMPLETE";
        }

        // Visibilidad de controles Cognex
        var showCables = _state == StationState.SCAN_CABLES;
        lstCables!.Visible = showCables;
        lblCableCount!.Visible = showCables;
        btnFinalizarCables!.Visible = showCables;
    }

    private void PromptNextItem() => UpdatePromptUI();

    public void ReceiveCableScan(string serial)
    {
        if (_state != StationState.SCAN_CABLES) return;

        _cablesScanned.Add(serial);
        BeginInvoke(() =>
        {
            lstCables?.Items.Add(serial);
            if (lblCableCount != null)
                lblCableCount.Text = $"Cables: {_cablesScanned.Count}";
            ShowSuccess($"Cable OK: {serial}");
        });
    }

    private void BtnReset_Click(object sender, EventArgs e) => Reset();

    private void BtnFinalizarCables_Click(object sender, EventArgs e)
    {
        if (_cablesScanned.Count == 0)
        {
            ShowError("No cables scanned.");
            return;
        }

        ShowSuccess($"Finalized: {_cablesScanned.Count} cables registered.");
        TransitionTo(StationState.DONE);
    }

    public void Reset()
    {
        _state = StationState.SCAN_STATION;
        _recipe = null;
        _itemIndex = 0;
        _cablesScanned.Clear();
        lstCables?.Items.Clear();
        if (lblCableCount != null) lblCableCount.Text = "Cables: 0";
        UpdatePromptUI();
        txtScanInput.Focus();
    }

    public void FocusScanInput() => txtScanInput.Focus();

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible) txtScanInput.Focus();
    }

    private void txtScanInput_TextChanged(object sender, EventArgs e)
    {

    }
}
