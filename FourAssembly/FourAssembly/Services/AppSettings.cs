namespace FourAssembly.Services;

using System.Text.Json;

public class AppSettings
{
    public string Station1 { get; set; } = "COM10";
    public string Station2 { get; set; } = "COM11";
    public string Station3 { get; set; } = "COM12";
}

public static class SettingsManager
{
    private static readonly string _path = @"C:/ke/FourPole/settings.json";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public static AppSettings Load()
    {
        // Crear directorio si no existe
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(_path))
        {
            var defaults = new AppSettings();
            Save(defaults);
            return defaults;
        }

        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_path, json);
    }
}
