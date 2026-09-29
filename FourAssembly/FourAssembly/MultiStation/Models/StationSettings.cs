namespace FourAssembly.MultiStation.Models;

public class StationSettings
{
    public int StationNumber { get; set; }
    public string SapStationNumber { get; set; } = string.Empty;
    public string AdminPass { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ComPort { get; set; } = string.Empty;
    public int MonitorIndex { get; set; }
    public int PlcButton { get; set; }
    public int PlcActiveStationRegister { get; set; }
    public int PlcConnectorSideRegister { get; set; }
    public int PlcResetMemory { get; set; }
    public int PlcSerialsComplete { get; set; }
    public int PlcStation { get; set; }
    public int PlcCounter { get; set; }
    public int PlcInsertedCountRegister { get; set; }
    public int PlcQuantityProcessMemory { get; set; }
    public int PlcFinalProcessMemory { get; set; }
    public int PlcCounterCompleteMemory { get; set; }
    public int PlcScrapMemory { get; set; }
    public int PlcBtnYellow { get; set; }
    public int PlcMaterialRemovedMemory { get; set; }
    public int PlcCavityErrorMemory { get; set; }
    public int PlcCavityErrorRegister { get; set; }
    public int PlcQualityAuthorizationMemory { get; set; }
    public List<int> PlcFemalePinMemories { get; set; } = [0, 0, 0, 0];
    public List<int> PlcMalePinMemories { get; set; } = [0, 0, 0, 0];
    public string PlcFemale { get; set; } = "A";
    public string PlcMale { get; set; } = "B";
}
