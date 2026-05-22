namespace FourAssembly.MultiStation.Services;

using System.Text.Json;
using FourAssembly.MultiStation.Models;

public static class MultiSettingsManager
{
    private static readonly string _path = @"C:\ke\FourPole\multistation_settings.json";
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static MultiAppSettings Load()
    {
        EnsureDirectoryExists();

        if (!File.Exists(_path))
        {
            var defaults = CreateDefaults();
            Save(defaults);
            return defaults;
        }

        try
        {
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<MultiAppSettings>(json, _jsonOptions) ?? CreateDefaults();
        }
        catch
        {
            var defaults = CreateDefaults();
            Save(defaults);
            return defaults;
        }
    }

    public static void Save(MultiAppSettings settings)
    {
        EnsureDirectoryExists();
        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_path, json);
    }

    private static void EnsureDirectoryExists()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    private static MultiAppSettings CreateDefaults() => new()
    {
        Plc = new PlcConfig { Ip = "192.168.1.5", Port = 502 },
        Stations = new List<StationSettings>
        {
            new() { StationNumber = 1, Name = "Station 1", ComPort = "COM10", MonitorIndex = 0, PlcStartCoil = 1, PlcCounterRegister = 10 },
            new() { StationNumber = 2, Name = "Station 2", ComPort = "COM11", MonitorIndex = 1, PlcStartCoil = 2, PlcCounterRegister = 20 },
            new() { StationNumber = 3, Name = "Station 3", ComPort = "COM12", MonitorIndex = 2, PlcStartCoil = 3, PlcCounterRegister = 30 }
        }
    };
}
