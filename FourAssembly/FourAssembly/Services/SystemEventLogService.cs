namespace FourAssembly.Services;

public static class SystemEventLogService
{
    private const string BasePath = @"C:\ke\FourPole\Log_System";
    private static readonly object _lockObj = new();

    public static void LogInfo(string category, string station, string message)
    {
        Write("INFO", category, station, message, null);
    }

    public static void LogError(string category, string station, string message, Exception? ex = null)
    {
        Write("ERROR", category, station, message, ex);
    }

    private static void Write(string level, string category, string station, string message, Exception? ex)
    {
        lock (_lockObj)
        {
            try
            {
                Directory.CreateDirectory(BasePath);

                var timestamp = DateTime.Now;
                var path = Path.Combine(BasePath, $"system_{timestamp:yyyy-MM-dd}.log");
                var stationText = string.IsNullOrWhiteSpace(station) ? "General" : station.Trim();
                var line = $"[{timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}] [{stationText}] {message}";

                if (ex != null)
                    line += $" | {ex.GetType().Name}: {ex.Message}";

                File.AppendAllText(path, line + Environment.NewLine);
            }
            catch
            {
                // This logger must never interrupt production flow.
            }
        }
    }
}
