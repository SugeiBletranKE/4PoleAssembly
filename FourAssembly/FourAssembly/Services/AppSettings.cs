namespace FourAssembly.Services;

using System.Text.Json;

public class AppSettings
{
    public string SapStationNumber { get; set; } = string.Empty;
    public string AdminPass { get; set; } = string.Empty;
    public ApiConfig Api { get; set; } = new();
    public PlcConfig Plc { get; set; } = new();
    public PlcConnectorConfig PlcConnector { get; set; } = new();
    public List<Settings> Stations { get; set; } = [];
}

public class ApiConfig
{
    public string StationInfoUrl { get; set; } = "http://172.16.47.4:8088/main/system/webdev/Core/odc/CheckSingleSerial";
    public string SingleSerialUrl { get; set; } = string.Empty;
    public string BookSingleSerialUrl { get; set; } = "http://172.16.47.4:8088/main/system/webdev/Core/odc/BookSingleSerial";
}

public class PlcConfig
{
    public string Ip { get; set; } = "192.168.1.5";
    public int Port { get; set; } = 502;
    public int ActiveStationRegister { get; set; } = 2;
    public int ConnectorSideRegister { get; set; } = 0;
    public int ResetMemory { get; set; }
    public int QualityAuthorizationMemory { get; set; } = 90;
}

public class PlcConnectorConfig
{
    public string Female { get; set; } = "A";
    public string Male { get; set; } = "B";
}

public class Settings
{
    public int StationNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ComPort { get; set; } = string.Empty;
    public int MonitorIndex { get; set; }
    public int PlcButton { get; set; }
    public int? PlcActiveStationRegister { get; set; }
    public int? PlcConnectorSideRegister { get; set; }
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
    public List<int> PlcBgPinMemories { get; set; } = [0, 0, 0, 0];
    public List<int> PlcFemalePinMemories { get; set; } = [0, 0, 0, 0];
    public List<int> PlcMalePinMemories { get; set; } = [0, 0, 0, 0];
}

public static class SettingsManager
{
    private static readonly string _path = @"C:\ke\FourPole\settings.json";

    public static string FilePath => _path;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
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
        var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? CreateDefaults();
        NormalizeApi(settings);
        NormalizeMonitorIndexes(settings);
        NormalizePlcMap(settings);
        return settings;
    }

    public static void Save(AppSettings settings)
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_path, json);
    }

    public static string ReadRawJson()
    {
        if (!File.Exists(_path))
            Save(CreateDefaults());

        return File.ReadAllText(_path);
    }

    public static void SaveRawJson(string json)
    {
        _ = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions)
            ?? throw new JsonException("El contenido de settings.json esta vacio.");

        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (File.Exists(_path))
            File.Copy(_path, _path + ".bak", overwrite: true);

        File.WriteAllText(_path, json);
    }

    private static AppSettings CreateDefaults() => new()
    {
        SapStationNumber = string.Empty,
        Api = new ApiConfig
        {
            StationInfoUrl = "http://172.16.47.4:8088/main/system/webdev/Core/odc/CheckSingleSerial",
            SingleSerialUrl = string.Empty,
            BookSingleSerialUrl = "http://172.16.47.4:8088/main/system/webdev/Core/odc/BookSingleSerial"
        },
            Plc = new PlcConfig { Ip = "192.168.1.5", Port = 502, ActiveStationRegister = 2, ConnectorSideRegister = 0, ResetMemory = 0, QualityAuthorizationMemory = 90 },
        // Mapa del tipo de conector para PLC:
        // Female = A
        // Male = B
        PlcConnector = new PlcConnectorConfig { Female = "A", Male = "B" },
        // Mapa de comunicacion con el PLC por estacion:
        // plcButton = bobina que la app lee desde el PLC.
        //   0 = boton fisico suelto / no presionado
        //   1 = boton fisico presionado
        // plcSerialsComplete = bobina donde la app manda un pulso al terminar de escanear seriales.
        // plcQuantityProcessMemory = memoria M que se prende cuando el boton manda el contador final.
        // plcFinalProcessMemory = memoria M que se prende cuando la estacion termina el proceso.
        // plcCounterCompleteMemory = memoria M que la app lee para confirmar que PLC recibio N=N.
        // plcScrapMemory = memoria M que se prende cuando hay scrap; 0 = deshabilitado hasta definir memoria.
        // plcStation = bobina donde la app manda el pulso para liberar/arrancar el ciclo.
        // plcCounter = registro donde la app escribe el total de cables aprobados.
        Stations = new List<Settings>
        {
            new() { StationNumber = 1, Name = "Station 1", ComPort = "COM10", MonitorIndex = 0, PlcButton = 24, PlcActiveStationRegister = 91, PlcConnectorSideRegister = 95, PlcSerialsComplete = 201, PlcStation = 1, PlcCounter = 1, PlcInsertedCountRegister = 5, PlcQuantityProcessMemory = 61, PlcFinalProcessMemory = 55, PlcCounterCompleteMemory = 44, PlcScrapMemory = 0, PlcMaterialRemovedMemory = 0, PlcCavityErrorMemory = 80, PlcCavityErrorRegister = 70 },
            new() { StationNumber = 2, Name = "Station 2", ComPort = "COM11", MonitorIndex = 1, PlcButton = 25, PlcActiveStationRegister = 90, PlcConnectorSideRegister = 94, PlcSerialsComplete = 202, PlcStation = 2, PlcCounter = 9, PlcInsertedCountRegister = 7, PlcQuantityProcessMemory = 62, PlcFinalProcessMemory = 56, PlcCounterCompleteMemory = 49, PlcScrapMemory = 0, PlcMaterialRemovedMemory = 0, PlcCavityErrorMemory = 81, PlcCavityErrorRegister = 71 },
            new() { StationNumber = 3, Name = "Station 3", ComPort = "COM12", MonitorIndex = 2, PlcButton = 26, PlcActiveStationRegister = 2, PlcConnectorSideRegister = 0, PlcSerialsComplete = 203, PlcStation = 3, PlcCounter = 12, PlcInsertedCountRegister = 10, PlcQuantityProcessMemory = 63, PlcFinalProcessMemory = 57, PlcCounterCompleteMemory = 54, PlcScrapMemory = 0, PlcMaterialRemovedMemory = 0, PlcCavityErrorMemory = 82, PlcCavityErrorRegister = 72 }
        }
    };

    private static void NormalizeApi(AppSettings settings)
    {
        settings.Api ??= new ApiConfig();
        if (string.IsNullOrWhiteSpace(settings.Api.StationInfoUrl))
            settings.Api.StationInfoUrl = CreateDefaults().Api.StationInfoUrl;

        if (string.IsNullOrWhiteSpace(settings.Api.BookSingleSerialUrl))
            settings.Api.BookSingleSerialUrl = CreateDefaults().Api.BookSingleSerialUrl;
    }

    private static void NormalizeMonitorIndexes(AppSettings settings)
    {
        if (settings.Stations.Count == 0)
            return;

        var monitorIndexes = settings.Stations.Select(station => station.MonitorIndex).ToList();
        var looksOneBased = !monitorIndexes.Contains(0)
            && monitorIndexes.Min() == 1
            && monitorIndexes.Max() == settings.Stations.Count;

        if (!looksOneBased)
            return;

        foreach (var station in settings.Stations)
            station.MonitorIndex--;
    }
    //? Revisar metodo
    private static void NormalizePlcMap(AppSettings settings)
    {
        settings.Plc ??= new PlcConfig();
        settings.PlcConnector ??= new PlcConnectorConfig();

        if (settings.Stations.Count == 0)
            settings.Stations = CreateDefaults().Stations;

        if (settings.Plc.ActiveStationRegister <= 0)
            settings.Plc.ActiveStationRegister = 2;

        if (settings.Plc.ConnectorSideRegister < 0)
            settings.Plc.ConnectorSideRegister = 0;

        if (settings.Plc.ResetMemory < 0)
            settings.Plc.ResetMemory = 0;

        if (settings.Plc.QualityAuthorizationMemory <= 0)
            settings.Plc.QualityAuthorizationMemory = 90;

        if (string.IsNullOrWhiteSpace(settings.PlcConnector.Female))
            settings.PlcConnector.Female = "A";

        if (string.IsNullOrWhiteSpace(settings.PlcConnector.Male))
            settings.PlcConnector.Male = "B";

        foreach (var station in settings.Stations)
        {
            // Si el archivo settings.json viejo no tiene los nombres nuevos,
            // se cargan los valores por defecto segun el numero de estacion.
            if (station.PlcButton <= 0 || station.PlcButton == 100 + station.StationNumber)
                station.PlcButton = GetDefaultButtonMemory(station.StationNumber);

            if (!station.PlcActiveStationRegister.HasValue || station.PlcActiveStationRegister <= 0)
                station.PlcActiveStationRegister = GetDefaultActiveStationRegister(station.StationNumber);

            if (!station.PlcConnectorSideRegister.HasValue || station.PlcConnectorSideRegister < 0)
                station.PlcConnectorSideRegister = GetDefaultConnectorSideRegister(station.StationNumber);

            if (station.PlcSerialsComplete <= 0)
                station.PlcSerialsComplete = 200 + station.StationNumber;

            if (station.PlcStation <= 0)
                station.PlcStation = station.StationNumber;

            if (station.PlcCounter <= 0 || station.PlcCounter == station.StationNumber * 10)
                station.PlcCounter = GetDefaultCounterRegister(station.StationNumber);

            if (station.PlcInsertedCountRegister < 0)
                station.PlcInsertedCountRegister = 0;

            if (station.PlcQuantityProcessMemory <= 0)
                station.PlcQuantityProcessMemory = 60 + station.StationNumber;

            if (station.PlcFinalProcessMemory <= 0)
                station.PlcFinalProcessMemory = 54 + station.StationNumber;

            if (station.PlcCounterCompleteMemory <= 0)
                station.PlcCounterCompleteMemory = GetDefaultCounterCompleteMemory(station.StationNumber);

            if (station.PlcScrapMemory < 0)
                station.PlcScrapMemory = 0;

            if (station.PlcBtnYellow < 0)
                station.PlcBtnYellow = 0;

            if (station.PlcMaterialRemovedMemory < 0)
                station.PlcMaterialRemovedMemory = 0;

            if (station.PlcCavityErrorMemory <= 0)
                station.PlcCavityErrorMemory = 79 + station.StationNumber;

            if (station.PlcCavityErrorRegister <= 0)
                station.PlcCavityErrorRegister = 69 + station.StationNumber;

            station.PlcBgPinMemories ??= [0, 0, 0, 0];
            while (station.PlcBgPinMemories.Count < 4)
                station.PlcBgPinMemories.Add(0);

            station.PlcFemalePinMemories ??= [0, 0, 0, 0];
            while (station.PlcFemalePinMemories.Count < 4)
                station.PlcFemalePinMemories.Add(0);

            station.PlcMalePinMemories ??= [0, 0, 0, 0];
            while (station.PlcMalePinMemories.Count < 4)
                station.PlcMalePinMemories.Add(0);
        }
    }

    private static int GetDefaultButtonMemory(int stationNumber) => stationNumber switch
    {
        1 => 24,
        2 => 25,
        3 => 26,
        _ => 23 + stationNumber
    };

    private static int GetDefaultActiveStationRegister(int stationNumber) => stationNumber switch
    {
        1 => 91,
        2 => 90,
        3 => 2,
        _ => 2
    };

    private static int GetDefaultConnectorSideRegister(int stationNumber) => stationNumber switch
    {
        1 => 95,
        2 => 94,
        3 => 0,
        _ => 0
    };

    private static int GetDefaultCounterRegister(int stationNumber) => stationNumber switch
    {
        1 => 1,
        2 => 9,
        3 => 12,
        _ => stationNumber * 10
    };

    private static int GetDefaultCounterCompleteMemory(int stationNumber) => stationNumber switch
    {
        1 => 44,
        2 => 49,
        3 => 54,
        _ => 0
    };
}
