namespace FourAssembly.Forms;
using FourAssembly.Services;

public partial class CognexDiagnosticsForm : Form
{
    private int _totalScans = 0;

    public CognexDiagnosticsForm()
    {
        InitializeComponent();
        SetupDiagnostics();
    }

    private void SetupDiagnostics()
    {
        CognexService.CableReceived += OnCableReceived;
        Text = "Cognex Diagnostics - Real-time Data";
    }

    private void OnCableReceived(int stationNum, string serial)
    {
        BeginInvoke(() =>
        {
            _totalScans++;
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var message = $"[{timestamp}] Station {stationNum}: {serial}";

            lstCognexData.Items.Add(message);
            lblTotalScans.Text = $"Total Scans: {_totalScans}";

            // Auto-scroll to bottom
            lstCognexData.TopIndex = Math.Max(0, lstCognexData.Items.Count - 1);
        });
    }

    private void BtnClear_Click(object sender, EventArgs e)
    {
        lstCognexData.Items.Clear();
        _totalScans = 0;
        lblTotalScans.Text = "Total Scans: 0";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        CognexService.CableReceived -= OnCableReceived;
        base.OnFormClosing(e);
    }
}
