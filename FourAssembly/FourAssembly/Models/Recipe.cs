namespace FourAssembly.Models;

public class Recipe
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, StationConfig> Stations { get; set; } = [];
    public List<Material> Materials { get; set; } = [];
    public List<Tool> Tools { get; set; } = [];
}

public class StationConfig
{
    public string Name { get; set; } = string.Empty;
    public string Enable { get; set; } = "FALSE";
    public string BG { get; set; } = string.Empty;
    public string HousingEnable { get; set; } = "FALSE";
    public string Housing { get; set; } = string.Empty;
}
