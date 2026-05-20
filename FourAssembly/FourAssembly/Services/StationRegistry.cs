namespace FourAssembly.Services;

public static class StationRegistry
{
    private static readonly Dictionary<string, int> _stations = new()
    {
        { "S01", 1 },
        { "S02", 2 },
        { "S03", 3 },
    };

    public static int? Resolve(string barcode) =>
        _stations.TryGetValue(barcode.Trim(), out var stationNum) ? stationNum : null;
}
