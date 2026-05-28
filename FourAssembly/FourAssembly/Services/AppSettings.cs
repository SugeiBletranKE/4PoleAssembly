namespace FourAssembly.Services;

using System.Text.Json;

public class AppSettings
{
    public PlcConfig Plc { get; set; } = new();
    public List<Settings> Stations { get; set; } = [];
}

public class PlcConfig
{
    public string Ip { get; set; } = "192.168.1.5";
    public int Port { get; set; } = 502;
}

public class Settings
{
    public int StationNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ComPort { get; set; } = string.Empty;
    public int MonitorIndex { get; set; }
    public int PlcStartCoil { get; set; }
    public int PlcCounterRegister { get; set; }
}

public static class SettingsManager
{
    private static readonly string _path = @"C:\ke\FourPole\settings.json";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public static AppSettings Load()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(_path))
        {
            var defaults = CreateDefaults();
            Save(defaults);
            return defaults;
        }

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? CreateDefaults();
    }

    public static void Save(AppSettings settings)
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_path, json);
    }

    private static AppSettings CreateDefaults() => new()
    {
        Plc = new PlcConfig { Ip = "192.168.1.5", Port = 502 },
        Stations = new List<Settings>
        {
            new() { StationNumber = 1, Name = "Station 1", ComPort = "COM10", MonitorIndex = 0, PlcStartCoil = 1, PlcCounterRegister = 10 },
            new() { StationNumber = 2, Name = "Station 2", ComPort = "COM11", MonitorIndex = 1, PlcStartCoil = 2, PlcCounterRegister = 20 },
            new() { StationNumber = 3, Name = "Station 3", ComPort = "COM12", MonitorIndex = 2, PlcStartCoil = 3, PlcCounterRegister = 30 }
        }
    };
}
