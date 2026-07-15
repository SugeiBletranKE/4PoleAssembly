namespace FourAssembly.Models;

public enum StationProcessState
{
    DISABLED,
    WAITING_ZEBRA,
    ZEBRA_IN_PROGRESS,
    COGNEX_ACTIVE,
    COGNEX_DONE,
    PLC_RELEASED,
    DONE
}
