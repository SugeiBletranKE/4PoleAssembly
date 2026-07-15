namespace FourAssembly.Services;

using System.Text.Json;

public static class ApiOperationLogService
{
    private const string BasePath = @"C:\ke\FourPole\Log_API";
    private static readonly object _lockObj = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public static void LogOperation(ApiOperationLogEntry entry)
    {
        lock (_lockObj)
        {
            try
            {
                var timestamp = DateTime.Now;
                var operation = SanitizeFileName(entry.Operation);
                var station = SanitizeFileName(entry.Station);
                var serials = entry.SerialNumbers.Count == 0
                    ? "NO_SERIAL"
                    : SanitizeFileName(string.Join("-", entry.SerialNumbers));
                var result = entry.Success ? "OK" : "NOK";

                var dir = Path.Combine(BasePath, operation, timestamp.ToString("yyyy-MM-dd"));
                Directory.CreateDirectory(dir);

                var fileName = $"{timestamp:yyyyMMdd_HHmmss_fff}_{station}_{serials}_{result}_{Guid.NewGuid():N}.json";
                var path = Path.Combine(dir, fileName);

                File.WriteAllText(path, JsonSerializer.Serialize(entry, _jsonOptions));
            }
            catch (Exception ex)
            {
                LogService.LogError("Failed to write API operation log", ex);
            }
        }
    }

    private static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "UNKNOWN";

        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(value
            .Trim()
            .Select(c => invalidChars.Contains(c) ? '_' : c)
            .ToArray());

        return sanitized.Length <= 80 ? sanitized : sanitized[..80];
    }
}

public class ApiOperationLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Operation { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Station { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public List<string> SerialNumbers { get; set; } = [];
    public string RequestBody { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public bool Success { get; set; }
    public int? ResultCode { get; set; }
    public string ResultText { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public long ElapsedMilliseconds { get; set; }
}
