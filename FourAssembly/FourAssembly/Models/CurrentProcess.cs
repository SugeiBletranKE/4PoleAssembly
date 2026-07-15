namespace FourAssembly.Models;

public class CurrentProcess
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ConnectorGender { get; set; } = "A";
    public List<Tool> Tools { get; set; } = [];

    public List<string> EnabledStations { get; set; } = [];
    public Dictionary<string, StationProcessState> StationStates { get; set; } = [];

    public int CurrentStationIndex { get; set; } = 0;
    public string CurrentStationCode =>
        CurrentStationIndex < EnabledStations.Count
            ? EnabledStations[CurrentStationIndex]
            : string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public void ResetProcess()
    {
        CurrentStationIndex = 0;
        foreach (var station in EnabledStations)
        {
            StationStates[station] = StationProcessState.WAITING_ZEBRA;
        }
    }

    public bool MoveToNextStation()
    {
        if (CurrentStationIndex < EnabledStations.Count - 1)
        {
            CurrentStationIndex++;
            return true;
        }
        return false;
    }

    public bool AllStationsDone()
    {
        return EnabledStations.All(s =>
            StationStates[s] == StationProcessState.DONE);
    }
}
