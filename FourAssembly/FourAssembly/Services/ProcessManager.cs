namespace FourAssembly.Services;

using FourAssembly.Models;

public static class ProcessManager
{
    private static CurrentProcess? _currentProcess;
    private static Dictionary<string, StationProcessState> _stationStates = [];

    public static event Action<CurrentProcess>? ProcessStarted;
    public static event Action<string, StationProcessState>? StationStateChanged;
    public static event Action? ProcessCompleted;
    public static event Action? ProcessReset;

    public static CurrentProcess? GetCurrentProcess() => _currentProcess;

    public static StationProcessState GetStationState(string stationCode)
    {
        return _stationStates.TryGetValue(stationCode, out var state)
            ? state
            : StationProcessState.DISABLED;
    }

    public static void StartProcess(Recipe recipe)
    {
        _currentProcess = new CurrentProcess
        {
            PartNumber = recipe.PartNumber,
            Description = recipe.Description,
            ConnectorGender = recipe.ConnectorGender,
            Tools = new List<Tool>(recipe.Tools)
        };

        var enabledStations = recipe.Stations
            .Where(kvp => kvp.Value.Enable == "TRUE")
            .Select(kvp => kvp.Key)
            .OrderBy(k => int.Parse(k))
            .ToList();

        _currentProcess.EnabledStations = enabledStations;

        _stationStates.Clear();
        foreach (var stationCode in enabledStations)
        {
            _currentProcess.StationStates[stationCode] = StationProcessState.WAITING_ZEBRA;
            _stationStates[stationCode] = StationProcessState.WAITING_ZEBRA;
        }

        for (int i = 1; i <= 4; i++)
        {
            var code = i.ToString();
            if (!enabledStations.Contains(code))
            {
                _stationStates[code] = StationProcessState.DISABLED;
            }
        }

        ProcessStarted?.Invoke(_currentProcess);
    }

    public static void ReleaseStationWithZebra(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.ZEBRA_IN_PROGRESS);
    }

    public static void RestartStationWaitingZebra(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.WAITING_ZEBRA);
    }

    public static void RestartStationCognexActive(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.COGNEX_ACTIVE);
    }

    public static void CompleteZebraAtStation(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.COGNEX_ACTIVE);

        if (MoveToNextStation())
        {
            var nextStation = _currentProcess.CurrentStationCode;
            if (!string.IsNullOrEmpty(nextStation) && nextStation != stationCode)
            {
                SetStationState(nextStation, StationProcessState.WAITING_ZEBRA);
            }
        }
    }

    public static void CompleteCognexAtStation(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.COGNEX_DONE);
    }

    public static void ReleaseToPLC(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.PLC_RELEASED);
    }

    public static void FinishStation(string stationCode)
    {
        if (_currentProcess == null)
            return;

        SetStationState(stationCode, StationProcessState.DONE);

        if (_currentProcess.AllStationsDone())
        {
            ProcessCompleted?.Invoke();
            _currentProcess = null;
        }
    }

    private static void SetStationState(string stationCode, StationProcessState newState)
    {
        if (_currentProcess == null)
            return;

        if (_stationStates.ContainsKey(stationCode))
        {
            _stationStates[stationCode] = newState;
        }

        if (_currentProcess.StationStates.ContainsKey(stationCode))
        {
            _currentProcess.StationStates[stationCode] = newState;
        }

        StationStateChanged?.Invoke(stationCode, newState);
    }

    private static bool MoveToNextStation()
    {
        if (_currentProcess == null)
            return false;

        return _currentProcess.MoveToNextStation();
    }

    public static string GetNextStationWaitingZebra()
    {
        if (_currentProcess == null)
            return string.Empty;

        for (int i = _currentProcess.CurrentStationIndex; i < _currentProcess.EnabledStations.Count; i++)
        {
            var station = _currentProcess.EnabledStations[i];
            if (_stationStates.TryGetValue(station, out var state) && state == StationProcessState.WAITING_ZEBRA)
            {
                return station;
            }
        }
        return string.Empty;
    }

    public static void ResetProcess()
    {
        _currentProcess = null;
        _stationStates.Clear();
        ProcessReset?.Invoke();
    }
}
