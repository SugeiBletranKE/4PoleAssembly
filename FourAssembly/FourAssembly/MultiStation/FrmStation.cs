namespace FourAssembly.MultiStation;

using FourAssembly.MultiStation.Controls;
using FourAssembly.MultiStation.Models;
using FourAssembly.MultiStation.Services;

public class FrmStation : Form
{
    private readonly StationSettings _config;
    private readonly PlcService _plc;
    private StationPanel? _stationPanel;

    public FrmStation(StationSettings config, PlcService plc)
    {
        _config = config;
        _plc = plc;

        this.Text = config.Name;
        this.ClientSize = new Size(900, 600);
        this.BackColor = Color.White;

        _stationPanel = new StationPanel(config, plc) { Dock = DockStyle.Fill };
        this.Controls.Add(_stationPanel);

        this.FormClosing += (s, e) => _stationPanel?.StopCognex();
    }
}
