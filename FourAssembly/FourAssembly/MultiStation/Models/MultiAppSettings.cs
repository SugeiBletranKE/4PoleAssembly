namespace FourAssembly.MultiStation.Models;

public class MultiAppSettings
{
    public PlcConfig Plc { get; set; } = new();
    public List<StationSettings> Stations { get; set; } = [];
}

public class PlcConfig
{
    public string Ip { get; set; } = "192.168.1.5";
    public int Port { get; set; } = 502;
}
