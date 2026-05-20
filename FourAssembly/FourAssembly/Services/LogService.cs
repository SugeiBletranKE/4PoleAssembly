namespace FourAssembly.Services;

using System.IO;

public static class LogService
{
    private static readonly string _logPath = @"C:/ke/FourPole/app.log";
    private static readonly object _lockObj = new();

    public static void Initialize()
    {
        var dir = Path.GetDirectoryName(_logPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Log("=== Application Started ===");
    }

    public static void Log(string message)
    {
        lock (_lockObj)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logLine = $"[{timestamp}] {message}";
                File.AppendAllText(_logPath, logLine + Environment.NewLine);
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
            if (File.Exists(_logPath))
                File.Delete(_logPath);
        }
        catch { }
    }
}
