namespace FourAssembly.MultiStation;

using FourAssembly.Models;
using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;

public partial class FrmStation : Form
{
    private readonly StationSettings _config;
    private readonly PlcService _plc;
    private StationPanel? _stationPanel;


    public FrmStation(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;

        InitializeComponent();
        this.Text = config.Name;

        _stationPanel = new StationPanel(config, plc) { Dock = DockStyle.Fill };
        this.Controls.Add(_stationPanel);

        this.FormClosing += (s, e) => _stationPanel?.StopCognex();
    }

    public void SetRecipeInfo(Recipe recipe)
    {
        _stationPanel?.SetRecipeInfo(recipe);
    }
}
