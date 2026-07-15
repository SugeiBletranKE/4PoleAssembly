namespace FourAssembly.Services;

using FourAssembly.Models;

public static class ZebraFlowService
{
    private static string _stationWaitingForZebra = string.Empty;
    private static bool _zebraAvailable = true;

    public static event Action<string>? ZebraReadyForStation;
    public static event Action<string>? ZebraCompletedAtStation;

    public static bool IsZebraAvailable => _zebraAvailable;

    public static void InitializeZebraFlow(CurrentProcess process)
    {
        if (process?.EnabledStations.Count > 0)
        {
            _stationWaitingForZebra = process.EnabledStations[0];
            _zebraAvailable = true;
            ZebraReadyForStation?.Invoke(_stationWaitingForZebra);
        }
    }

    public static bool TryReleaseZebraToStation(string stationCode)
    {
        if (!_zebraAvailable)
            return false;

        var process = ProcessManager.GetCurrentProcess();
        if (process == null)
            return false;

        if (!process.EnabledStations.Contains(stationCode))
            return false;

        _zebraAvailable = false;
        _stationWaitingForZebra = stationCode;
        ProcessManager.ReleaseStationWithZebra(stationCode);

        return true;
    }

    public static void RestartZebraAtStation(string stationCode)
    {
        var process = ProcessManager.GetCurrentProcess();
        if (process == null || !process.EnabledStations.Contains(stationCode))
            return;

        _zebraAvailable = false;
        _stationWaitingForZebra = stationCode;
        ProcessManager.ReleaseStationWithZebra(stationCode);
        ZebraReadyForStation?.Invoke(stationCode);
    }

    public static void CompleteZebraAtCurrentStation(string stationCode)
    {
        var process = ProcessManager.GetCurrentProcess();
        if (process == null)
            return;

        ProcessManager.CompleteZebraAtStation(stationCode);
        System.Threading.Thread.Sleep(300);

        var nextStation = ProcessManager.GetNextStationWaitingZebra();
        if (!string.IsNullOrEmpty(nextStation))
        {
            _zebraAvailable = false;
            _stationWaitingForZebra = nextStation;
            ProcessManager.ReleaseStationWithZebra(nextStation);
            System.Threading.Thread.Sleep(200);
            ZebraReadyForStation?.Invoke(nextStation);
        }
        else
        {
            _zebraAvailable = true;
            _stationWaitingForZebra = string.Empty;
        }

        ZebraCompletedAtStation?.Invoke(stationCode);
    }

    public static string GetStationWaitingForZebra() => _stationWaitingForZebra;

    public static void Reset()
    {
        _stationWaitingForZebra = string.Empty;
        _zebraAvailable = true;
    }
}
