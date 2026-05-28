namespace FourAssembly.Forms;

using FourAssembly.Controls;
using FourAssembly.Services;

public partial class FrmPanel : Form
{
    private StationControl[]? _stationControls;

    public FrmPanel()
    {
        InitializeComponent();

        LogService.Initialize();
        LogService.Log("FrmPanel initializing...");

        // Load Cognex settings and initialize
        var settings = SettingsManager.Load();
        var stations = string.Join(", ", settings.Stations.Select(s => s.ComPort));
        LogService.Log($"Settings loaded: {stations}");
        var ports = settings.Stations.Select(s => s.ComPort).ToArray();
        CognexService.Initialize(ports[0], ports[1], ports[2]);

        ShowStations();
        LogService.Log("FrmPanel ready");
    }

    private void BtnStations_Click(object sender, EventArgs e) => ShowStations();

    private void BtnRecipeEditor_Click(object sender, EventArgs e) => ShowRecipeEditor();

    private void BtnCognexDiagnostics_Click(object sender, EventArgs e)
    {
        var diagnosticsForm = new CognexDiagnosticsForm();
        diagnosticsForm.Show();
    }

    private void ShowStations()
    {
        pnlContent.Controls.Clear();

        // Create a wrapper panel with tabs
        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            TabIndex = 0,
            SizeMode = TabSizeMode.Fixed
        };

        _stationControls = new StationControl[3];
        string[] stationNames = { "Station 1", "Station 2", "Station 3" };

        for (int i = 0; i < 3; i++)
        {
            var tabPage = new TabPage
            {
                Text = stationNames[i],
                Name = $"tabPage{i + 1}",
                Padding = new Padding(0)
            };

            var stationControl = new StationControl
            {
                StationNumber = i + 1,
                Dock = DockStyle.Fill
            };

            tabPage.Controls.Add(stationControl);
            tabControl.TabPages.Add(tabPage);
            _stationControls[i] = stationControl;

            // Suscribir al evento de escaneo de estación
            var tabIdx = i;
            stationControl.StationScanned += (stationNum) =>
            {
                if (stationNum >= 1 && stationNum <= 3)
                    tabControl.SelectedIndex = stationNum - 1;
            };
        }

        tabControl.SelectedIndexChanged += (s, e) =>
        {
            int idx = tabControl.SelectedIndex;
            if (idx >= 0 && idx < _stationControls.Length)
                _stationControls[idx].FocusScanInput();
        };

        // Suscribir al evento de Cognex para recibir cables
        CognexService.CableReceived += (stationNum, serial) =>
        {
            if (stationNum >= 1 && stationNum <= 3)
                _stationControls![stationNum - 1].ReceiveCableScan(serial);
        };

        pnlContent.Controls.Add(tabControl);
    }

    private void ShowRecipeEditor()
    {
        pnlContent.Controls.Clear();

        // Create RecipeEditorForm as a borderless form in the panel
        var editorPanel = new Panel { Dock = DockStyle.Fill };

        // Embed the recipe editor
        var editorForm = new RecipeEditorForm();
        editorForm.TopLevel = false;
        editorForm.FormBorderStyle = FormBorderStyle.None;
        editorForm.Dock = DockStyle.Fill;

        editorPanel.Controls.Add(editorForm);
        pnlContent.Controls.Add(editorPanel);

        editorForm.Show();
    }
}
