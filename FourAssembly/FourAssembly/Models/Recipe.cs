namespace FourAssembly.Models;

public class Recipe
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CableCount { get; set; }
    public bool SharedMode { get; set; }
    public string SharedBG { get; set; } = string.Empty;
    public string ConnectorGender { get; set; } = "A";
    public Dictionary<string, StationConfig> Stations { get; set; } = [];
    public List<Tool> Tools { get; set; } = [];
}

public class StationConfig
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Enable { get; set; } = "FALSE";
    public string BG { get; set; } = string.Empty;
    public string HousingEnable { get; set; } = "FALSE";
    public string Housing { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<int> BgPins { get; set; } = [0, 0, 0, 0];
}
