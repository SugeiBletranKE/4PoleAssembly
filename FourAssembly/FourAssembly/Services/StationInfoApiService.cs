namespace FourAssembly.Services;

using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class StationInfoApiService
{
    private static string _apiUrl = "http://172.16.47.4:8088/main/system/webdev/Core/odc/CheckSingleSerial";
    private static string _singleSerialApiUrl = _apiUrl;
    private static readonly HttpClient _httpClient = new();
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static bool IsConfigured => !string.IsNullOrWhiteSpace(_apiUrl);
    public static bool IsSingleSerialConfigured => !string.IsNullOrWhiteSpace(_singleSerialApiUrl);

    public static void Initialize(ApiConfig config)
    {
        if (!string.IsNullOrWhiteSpace(config.StationInfoUrl))
            _apiUrl = config.StationInfoUrl.Trim();

        _singleSerialApiUrl = string.IsNullOrWhiteSpace(config.SingleSerialUrl)
            ? _apiUrl
            : config.SingleSerialUrl.Trim();
    }

    // Metodo anterior por lote. Se conserva como respaldo; el flujo principal usa ReviewSerialsIndividuallyAsync.
    public static async Task<StationInfoApiResult> ReviewAsync(
        StationInfoApiRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return new StationInfoApiResult
            {
                Success = false,
                Message = "Configura el URL de la API en settings.json: Api.StationInfoUrl."
            };
        }

        var stopwatch = Stopwatch.StartNew();
        var json = JsonSerializer.Serialize(request, _jsonOptions);

        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_apiUrl, content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = ParseApiResponse(responseBody);
            var success = response.IsSuccessStatusCode
                && (!apiResponse.ResultCode.HasValue || apiResponse.ResultCode.Value == 0);
            stopwatch.Stop();

            LogApiOperation(
                "CHECK",
                _apiUrl,
                request,
                json,
                (int)response.StatusCode,
                success,
                apiResponse.ResultCode,
                apiResponse.ResultText,
                responseBody,
                string.Empty,
                stopwatch.ElapsedMilliseconds);

            return new StationInfoApiResult
            {
                Success = success,
                StatusCode = (int)response.StatusCode,
                ResultCode = apiResponse.ResultCode,
                ResultText = apiResponse.ResultText,
                RejectedSerialNumbers = success ? [] : apiResponse.SerialNumbers,
                Message = BuildResultMessage(response.IsSuccessStatusCode, response.StatusCode, responseBody, apiResponse),
                ResponseBody = responseBody
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogApiOperation(
                "CHECK",
                _apiUrl,
                request,
                json,
                null,
                false,
                null,
                string.Empty,
                string.Empty,
                ex.Message,
                stopwatch.ElapsedMilliseconds);

            LogService.LogError("Station info API review failed", ex);
            return new StationInfoApiResult
            {
                Success = false,
                Message = $"Error consultando API: {ex.Message}"
            };
        }
    }

    public static async Task<StationInfoApiResult> ReviewSerialsIndividuallyAsync(
        StationInfoApiRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!IsSingleSerialConfigured)
        {
            return new StationInfoApiResult
            {
                Success = false,
                Message = "Configura el URL de la API individual en settings.json: Api.SingleSerialUrl."
            };
        }

        var serialResults = new List<StationInfoSerialReview>();
        var rejectedSerials = new List<string>();
        var responseBodies = new List<string>();

        foreach (var serial in request.SerialNumbers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var singleResult = await ReviewSingleSerialAsync(
                request.Station,
                request.PartNumber,
                serial,
                cancellationToken);

            serialResults.Add(singleResult);
            responseBodies.Add($"SERIAL {serial}: {singleResult.ResponseBody}");

            if (!singleResult.Success)
            {
                rejectedSerials.Add(serial);
            }
        }

        var success = rejectedSerials.Count == 0;
        return new StationInfoApiResult
        {
            Success = success,
            ResultCode = success ? 0 : 1,
            ResultText = success ? "OK" : "Seriales con pasos faltantes",
            RejectedSerialNumbers = rejectedSerials,
            SerialResults = serialResults,
            Message = success
                ? "Todos los seriales fueron revisados correctamente."
                : $"Seriales rechazados: {string.Join(", ", rejectedSerials)}",
            ResponseBody = string.Join(Environment.NewLine, responseBodies)
        };
    }

    private static async Task<StationInfoSerialReview> ReviewSingleSerialAsync(
        string station,
        string partNumber,
        string serial,
        CancellationToken cancellationToken)
    {
        var request = new StationInfoSingleSerialApiRequest
        {
            SerialNumber = serial,
            State = 0,
            PartNumber = partNumber,
            Station = station,
            FailureCode = string.Empty,
            Data = []
        };
        var logRequest = new StationInfoApiRequest
        {
            Station = station,
            PartNumber = partNumber,
            SerialNumbers = [serial]
        };
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_singleSerialApiUrl, content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = ParseApiResponse(responseBody);
            var success = response.IsSuccessStatusCode
                && (!apiResponse.ResultCode.HasValue || apiResponse.ResultCode.Value == 0);
            stopwatch.Stop();

            LogApiOperation(
                "CHECK",
                _singleSerialApiUrl,
                logRequest,
                json,
                (int)response.StatusCode,
                success,
                apiResponse.ResultCode,
                apiResponse.ResultText,
                responseBody,
                string.Empty,
                stopwatch.ElapsedMilliseconds);

            return new StationInfoSerialReview
            {
                SerialNumber = serial,
                Success = success,
                StatusCode = (int)response.StatusCode,
                ResultCode = apiResponse.ResultCode,
                ResultText = apiResponse.ResultText,
                Message = BuildResultMessage(response.IsSuccessStatusCode, response.StatusCode, responseBody, apiResponse),
                ResponseBody = responseBody
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogApiOperation(
                "CHECK",
                _singleSerialApiUrl,
                logRequest,
                json,
                null,
                false,
                null,
                string.Empty,
                string.Empty,
                ex.Message,
                stopwatch.ElapsedMilliseconds);

            LogService.LogError($"Station info API serial review failed for {serial}", ex);
            return new StationInfoSerialReview
            {
                SerialNumber = serial,
                Success = false,
                Message = $"Error consultando API para {serial}: {ex.Message}"
            };
        }
    }

    private static StationInfoApiResponse ParseApiResponse(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return new StationInfoApiResponse();

        try
        {
            return JsonSerializer.Deserialize<StationInfoApiResponse>(responseBody, _jsonOptions)
                ?? new StationInfoApiResponse();
        }
        catch (JsonException ex)
        {
            LogService.LogError("Station info API response parse failed", ex);
            return new StationInfoApiResponse();
        }
    }

    private static string BuildResultMessage(
        bool httpSuccess,
        System.Net.HttpStatusCode statusCode,
        string responseBody,
        StationInfoApiResponse apiResponse)
    {
        if (!httpSuccess)
            return $"API respondio {(int)statusCode}: {responseBody}";

        if (apiResponse.ResultCode.HasValue)
            return $"API result: {apiResponse.ResultCode}, {apiResponse.ResultText}";

        return "Informacion revisada correctamente.";
    }

    private static void LogApiOperation(
        string operation,
        string url,
        StationInfoApiRequest request,
        string requestJson,
        int? statusCode,
        bool success,
        int? resultCode,
        string resultText,
        string responseBody,
        string errorMessage,
        long elapsedMilliseconds)
    {
        ApiOperationLogService.LogOperation(new ApiOperationLogEntry
        {
            Operation = operation,
            Url = url,
            Station = request.Station,
            PartNumber = request.PartNumber,
            SerialNumbers = [.. request.SerialNumbers],
            RequestBody = requestJson,
            StatusCode = statusCode,
            Success = success,
            ResultCode = resultCode,
            ResultText = resultText,
            ResponseBody = responseBody,
            ErrorMessage = errorMessage,
            ElapsedMilliseconds = elapsedMilliseconds
        });

        if (!success)
        {
            var station = string.IsNullOrWhiteSpace(request.Station) ? "General" : request.Station;
            var serials = request.SerialNumbers.Count == 0
                ? "NO_SERIAL"
                : string.Join(", ", request.SerialNumbers);
            var reason = !string.IsNullOrWhiteSpace(errorMessage)
                ? errorMessage
                : $"StatusCode={statusCode?.ToString() ?? "N/A"}, ResultCode={resultCode?.ToString() ?? "N/A"}, ResultText={resultText}";

            SystemEventLogService.LogError("API", station, $"{operation} failed for serial(s): {serials}. {reason}");
        }
    }
}

public class StationInfoApiRequest
{
    [JsonPropertyName("STATION")]
    public string Station { get; set; } = string.Empty;

    [JsonPropertyName("PARTNUMBER")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("SERIAL_NUMBERS")]
    public List<string> SerialNumbers { get; set; } = [];
}

public class StationInfoSingleSerialApiRequest
{
    [JsonPropertyName("SERIAL_NUMBER")]
    public string SerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("STATE")]
    public int State { get; set; }

    [JsonPropertyName("PARTNUMBER")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("STATION")]
    public string Station { get; set; } = string.Empty;

    [JsonPropertyName("FAILURE_CODE")]
    public string FailureCode { get; set; } = string.Empty;

    [JsonPropertyName("DATA")]
    public List<object> Data { get; set; } = [];
}

public class StationInfoApiResult
{
    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public int? ResultCode { get; set; }
    public string ResultText { get; set; } = string.Empty;
    public List<string> RejectedSerialNumbers { get; set; } = [];
    public List<StationInfoSerialReview> SerialResults { get; set; } = [];
    public string Message { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
}

public class StationInfoSerialReview
{
    public string SerialNumber { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public int? ResultCode { get; set; }
    public string ResultText { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ResponseBody { get; set; } = string.Empty;
}

public class StationInfoApiResponse
{
    [JsonPropertyName("RESULT_CODE")]
    public int? ResultCode { get; set; }

    [JsonPropertyName("RESULT_TEXT")]
    public string ResultText { get; set; } = string.Empty;

    [JsonPropertyName("SERIAL_NUMBERS")]
    public List<string> SerialNumbers { get; set; } = [];
}
