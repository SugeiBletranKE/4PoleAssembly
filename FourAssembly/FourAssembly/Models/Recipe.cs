namespace FourAssembly.Models;

public class Recipe
{
    public string PartNumber { get; set; } = string.Empty;
    public List<Material> Materials { get; set; } = [];
    public List<Tool> Tools { get; set; } = [];
}
