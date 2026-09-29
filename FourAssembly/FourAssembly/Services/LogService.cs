namespace FourAssembly.Services;

using System.IO;

public static class LogService
{
    private static readonly string _logDirectory = @"C:\ke\FourPole";
    private static readonly object _lockObj = new();

    private static string GetLogPath(DateTime date) =>
        Path.Combine(_logDirectory, $"app_{date:yyyy-MM-dd}.log");

    public static void Initialize()
    {
        if (!Directory.Exists(_logDirectory))
            Directory.CreateDirectory(_logDirectory);

        Log("=== Application Started ===");
    }

    public static void Log(string message)
    {
        lock (_lockObj)
        {
            try
            {
                var now = DateTime.Now;
                var timestamp = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logLine = $"[{timestamp}] {message}";
                File.AppendAllText(GetLogPath(now), logLine + Environment.NewLine);
                Console.WriteLine(logLine);
            }
            catch { }
        }
    }

    public static void LogError(string message, Exception? ex = null)
    {
        var msg = ex != null ? $"ERROR: {message} | {ex.GetType().Name}: {ex.Message}" : $"ERROR: {message}";
        Log(msg);
    }

    public static void ClearLog()
    {
        try
        {
            var logPath = GetLogPath(DateTime.Now);
            if (File.Exists(logPath))
                File.Delete(logPath);
        }
        catch { }
    }
}
