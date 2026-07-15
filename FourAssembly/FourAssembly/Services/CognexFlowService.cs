namespace FourAssembly.Services;

using FourAssembly.Models;

public static class CognexFlowService
{
    private static HashSet<string> _stationsInCognex = [];
    private static HashSet<string> _cognexCompleted = [];

    public static event Action<string>? CognexStarted;
    public static event Action<string>? CognexCompleted;

    public static void StartCognexAtStation(string stationCode)
    {
        var process = ProcessManager.GetCurrentProcess();
        if (process == null || !process.EnabledStations.Contains(stationCode))
            return;

        _stationsInCognex.Add(stationCode);
        CognexStarted?.Invoke(stationCode);
    }

    public static void CompleteCognexAtStation(string stationCode)
    {
        if (!_stationsInCognex.Contains(stationCode))
            return;

        _stationsInCognex.Remove(stationCode);
        _cognexCompleted.Add(stationCode);
        ProcessManager.CompleteCognexAtStation(stationCode);
        CognexCompleted?.Invoke(stationCode);
    }

    public static bool IsCognexActiveAtStation(string stationCode) =>
        _stationsInCognex.Contains(stationCode);

    public static bool IsCognexCompletedAtStation(string stationCode) =>
        _cognexCompleted.Contains(stationCode);

    public static void ReleaseToPLC(string stationCode)
    {
        ProcessManager.ReleaseToPLC(stationCode);
    }

    public static void FinishStation(string stationCode)
    {
        _stationsInCognex.Remove(stationCode);
        _cognexCompleted.Remove(stationCode);
        ProcessManager.FinishStation(stationCode);
    }

    public static void Reset()
    {
        _stationsInCognex.Clear();
        _cognexCompleted.Clear();
    }
}
