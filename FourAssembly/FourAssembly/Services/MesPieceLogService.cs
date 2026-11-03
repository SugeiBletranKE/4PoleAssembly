namespace FourAssembly.Services;

using System.Text.Json;

public static class MesPieceLogService
{
    private const string BasePath = @"C:\ke\FourPole\Log_MES";
    private static readonly object _lockObj = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public static void LogPieceReview(MesPieceLogEntry entry)
    {
        // No genera un archivo por cada pieza correcta. Conserva rechazos,
        // errores técnicos y cualquier clasificación diferente de OK.
        if (entry.Success
            && string.Equals(entry.Classification, "OK", StringComparison.OrdinalIgnoreCase))
            return;

        lock (_lockObj)
        {
            try
            {
                var timestamp = DateTime.Now;
                var station = SanitizeFileName(entry.Station);
                var serial = SanitizeFileName(entry.SerialNumber);
                var result = string.IsNullOrWhiteSpace(entry.Classification)
                    ? (entry.Success ? "OK" : "API_ERROR")
                    : SanitizeFileName(entry.Classification);

                // Cambio solicitado: trazabilidad MES por pieza revisada, un archivo por serial.
                var dir = Path.Combine(BasePath, timestamp.ToString("yyyy-MM-dd"), station);
                Directory.CreateDirectory(dir);

                var baseFileName = $"{timestamp:yyyyMMdd}_{serial}_{result}";
                var path = Path.Combine(dir, $"{baseFileName}.json");
                var duplicateNumber = 2;
                while (File.Exists(path))
                {
                    path = Path.Combine(dir, $"{baseFileName}_{duplicateNumber}.json");
                    duplicateNumber++;
                }

                entry.Timestamp = timestamp;
                File.WriteAllText(path, JsonSerializer.Serialize(entry, _jsonOptions));
            }
            catch (Exception ex)
            {
                LogService.LogError("Failed to write MES piece log", ex);
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

public class MesPieceLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Station { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public int CableCount { get; set; }
    public bool Success { get; set; }
    public bool IsScrap { get; set; }
    public string Classification { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public int? ResultCode { get; set; }
    public string ResultText { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
}
