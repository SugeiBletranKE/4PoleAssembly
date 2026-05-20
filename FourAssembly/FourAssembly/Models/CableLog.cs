namespace FourAssembly.Models;

public class CableLog
{
    public string PartNumber { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public List<string> Serials { get; set; } = [];
}
