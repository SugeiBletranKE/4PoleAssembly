namespace FourAssembly.MultiStation.Models;

public class StationSettings
{
    public int StationNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ComPort { get; set; } = string.Empty;
    public int MonitorIndex { get; set; }
    public int PlcStartCoil { get; set; }
    public int PlcCounterRegister { get; set; }
}
