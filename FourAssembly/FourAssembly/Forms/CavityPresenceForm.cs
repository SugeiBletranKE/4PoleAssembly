namespace FourAssembly.Forms;

using FourAssembly.MultiStation.Services;
using FourAssembly.Services;

/// <summary>
/// Vista de solo lectura para diagnosticar presencia de pines/cavidades en el PLC.
/// No modifica ninguna memoria ni interfiere con el ciclo de produccion.
/// </summary>
public sealed partial class CavityPresenceForm : Form
{
    private static int _openDiagnosticsCount;
    private static readonly object _diagnosticErrorsLock = new();
    private static readonly HashSet<int> _diagnosticErrorMemories = [];

    public static bool IsDiagnosticsActive => Volatile.Read(ref _openDiagnosticsCount) > 0;

    public static bool ShouldSuppressCavityError(int memory, bool active)
    {
        lock (_diagnosticErrorsLock)
        {
            // Cuando el PLC finalmente libera la memoria, termina también la
            // excepción de diagnóstico. Una activación posterior volverá a ser
            // tratada como un error normal de producción.
            if (!active)
            {
                _diagnosticErrorMemories.Remove(memory);
                return false;
            }

            return _diagnosticErrorMemories.Contains(memory);
        }
    }

    private readonly PlcService _plc;
    private readonly IReadOnlyList<Settings> _stations;
    private readonly int _qualityAuthorizationMemory;
    private readonly System.Windows.Forms.Timer _refreshTimer = new() { Interval = 250 };
    private readonly Dictionary<int, StationPresenceControls> _controls = [];
    private bool _reading;

    // Constructor requerido por el diseñador de Windows Forms.
    public CavityPresenceForm()
    {
        _plc = null!;
        _stations = [];
        _qualityAuthorizationMemory = 0;
        InitializeComponent();
    }

    public CavityPresenceForm(PlcService plc, AppSettings settings)
    {
        _plc = plc;
        _stations = settings.Stations.OrderBy(station => station.StationNumber).ToList();
        _qualityAuthorizationMemory = settings.Plc.QualityAuthorizationMemory;

        InitializeComponent();
        lblEndpoint.Text = $"PLC {settings.Plc.Ip}:{settings.Plc.Port}  •  Actualización cada 250 ms  •  Solo lectura";

        foreach (var station in _stations)
            flowStations.Controls.Add(CreateStationCard(station));

        _refreshTimer.Tick += RefreshTimer_Tick;
        Shown += CavityPresenceForm_Shown;
        FormClosed += CavityPresenceForm_FormClosed;
    }

    private void CavityPresenceForm_Shown(object? sender, EventArgs e)
    {
        Interlocked.Increment(ref _openDiagnosticsCount);
        lblEndpoint.Text += "  •  MODO DIAGNÓSTICO: bloqueo de cavidad pausado";
        _refreshTimer.Start();
    }

    private void CavityPresenceForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _refreshTimer.Stop();

        // Toma una última fotografía antes de salir del modo diagnóstico. Esto
        // cubre el caso de que el técnico cierre la ventana inmediatamente después
        // de accionar un pin, antes del siguiente tick de 250 ms.
        foreach (var station in _stations)
        {
            if (station.PlcCavityErrorMemory <= 0)
                continue;

            var active = _plc.ReadCoil(station.PlcCavityErrorMemory);
            if (_plc.IsConnected)
                TrackDiagnosticCavityError(station.PlcCavityErrorMemory, active);
        }

        lock (_diagnosticErrorsLock)
        {
            _diagnosticErrorMemories.Clear();
        }

        Interlocked.Decrement(ref _openDiagnosticsCount);
        _refreshTimer.Dispose();
    }

    private Control CreateStationCard(Settings station)
    {
        var card = new Panel
        {
            Width = 1180,
            Height = 218,
            Margin = new Padding(0, 0, 0, 14),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        var title = new Label
        {
            Text = $"ESTACIÓN {station.StationNumber}  —  {station.Name}",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = Color.FromArgb(20, 55, 100),
            AutoSize = true,
            Location = new Point(16, 10)
        };
        card.Controls.Add(title);

        var connection = CreateValueLabel("Esperando lectura…", new Point(910, 10), new Size(245, 30));
        connection.TextAlign = ContentAlignment.MiddleCenter;
        card.Controls.Add(connection);

        card.Controls.Add(CreateCaption("FEMALE / A", new Point(16, 48)));
        card.Controls.Add(CreateCaption("MALE / B", new Point(16, 101)));

        var femaleIndicators = CreatePinIndicators(card, station.PlcFemalePinMemories, 145, 43);
        var maleIndicators = CreatePinIndicators(card, station.PlcMalePinMemories, 145, 96);

        var inserted = CreateValueLabel($"Insertadas D{station.PlcInsertedCountRegister}: --", new Point(585, 48), new Size(260, 40));
        var error = CreateValueLabel($"Error M{station.PlcCavityErrorMemory}: --", new Point(855, 48), new Size(300, 40));
        var authorization = CreateValueLabel($"Autorización M{_qualityAuthorizationMemory}: --", new Point(585, 101), new Size(260, 40));
        var detail = CreateValueLabel($"Detalle D{station.PlcCavityErrorRegister}: --", new Point(855, 101), new Size(300, 40));
        var handshake = CreateValueLabel("Estado autorización: esperando lectura…", new Point(16, 154), new Size(1139, 42));
        card.Controls.Add(inserted);
        card.Controls.Add(error);
        card.Controls.Add(authorization);
        card.Controls.Add(detail);
        card.Controls.Add(handshake);

        _controls[station.StationNumber] = new StationPresenceControls(
            connection, femaleIndicators, maleIndicators, inserted, error, authorization, detail, handshake);
        return card;
    }

    private static Label CreateCaption(string text, Point location) => new()
    {
        Text = text,
        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor = Color.DimGray,
        AutoSize = true,
        Location = location
    };

    private static Label CreateValueLabel(string text, Point location, Size size) => new()
    {
        Text = text,
        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor = Color.DimGray,
        BackColor = Color.FromArgb(240, 243, 247),
        BorderStyle = BorderStyle.FixedSingle,
        Location = location,
        Size = size,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(8, 0, 4, 0)
    };

    private static List<Label> CreatePinIndicators(Control parent, IReadOnlyList<int>? registers, int left, int top)
    {
        var result = new List<Label>();
        for (var index = 0; index < 4; index++)
        {
            var register = registers is { Count: > 0 } && index < registers.Count ? registers[index] : 0;
            var label = CreateValueLabel($"P{index + 1}  D{register}: --", new Point(left + (index * 108), top), new Size(100, 40));
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Padding = Padding.Empty;
            parent.Controls.Add(label);
            result.Add(label);
        }

        return result;
    }

    private async void RefreshTimer_Tick(object? sender, EventArgs e)
    {
        await RefreshAllStationsAsync();
    }

    private async void BtnRefresh_Click(object? sender, EventArgs e)
    {
        btnRefresh.Enabled = false;
        btnRefresh.Text = "Actualizando…";
        try
        {
            await RefreshAllStationsAsync();
        }
        finally
        {
            btnRefresh.Text = "↻ Refrescar";
            btnRefresh.Enabled = true;
        }
    }

    private async Task RefreshAllStationsAsync()
    {
        if (_reading)
            return;

        _reading = true;
        try
        {
            foreach (var station in _stations)
                await RefreshStationAsync(station);
        }
        finally
        {
            _reading = false;
        }
    }

    private async Task RefreshStationAsync(Settings station)
    {
        var ui = _controls[station.StationNumber];
        var female = await ReadCoilsAsync(station.PlcFemalePinMemories);
        var male = await ReadRegistersAsync(station.PlcMalePinMemories);
        var inserted = await ReadRegisterAsync(station.PlcInsertedCountRegister);
        var errorActive = station.PlcCavityErrorMemory > 0
            && await Task.Run(() => _plc.ReadCoil(station.PlcCavityErrorMemory));
        TrackDiagnosticCavityError(station.PlcCavityErrorMemory, errorActive);
        var authorizationActive = _qualityAuthorizationMemory > 0
            && await Task.Run(() => _plc.ReadCoil(_qualityAuthorizationMemory));
        var detail = await ReadRegisterAsync(station.PlcCavityErrorRegister);

        var connected = _plc.IsConnected;
        ui.Connection.Text = connected ? "● PLC conectado" : "● PLC desconectado";
        ui.Connection.ForeColor = connected ? Color.DarkGreen : Color.Red;

        UpdatePinIndicators(ui.FemalePins, station.PlcFemalePinMemories, female, connected);
        UpdatePinIndicators(ui.MalePins, station.PlcMalePinMemories, male, connected);

        ui.Inserted.Text = $"Insertadas D{station.PlcInsertedCountRegister}: {(connected ? inserted : "--")}";
        ui.Error.Text = $"Error M{station.PlcCavityErrorMemory}: {(connected ? (errorActive ? "ACTIVO" : "Normal") : "--")}";
        ui.Error.BackColor = errorActive && connected ? Color.MistyRose : Color.FromArgb(240, 243, 247);
        ui.Error.ForeColor = errorActive && connected ? Color.DarkRed : Color.DimGray;

        ui.Authorization.Text = $"Autorización M{_qualityAuthorizationMemory}: " +
            (connected ? (authorizationActive ? "ACTIVA" : "OFF") : "--");
        ui.Authorization.BackColor = authorizationActive && connected ? Color.LightGoldenrodYellow : Color.FromArgb(240, 243, 247);
        ui.Authorization.ForeColor = authorizationActive && connected ? Color.DarkOrange : Color.DimGray;

        var detected = connected ? GetCavityDetailText(detail) : "";
        ui.Detail.Text = connected
            ? $"Detalle D{station.PlcCavityErrorRegister}: {detail} | {detected}"
            : $"Detalle D{station.PlcCavityErrorRegister}: --";

        UpdateAuthorizationHandshake(ui, station, connected, errorActive, authorizationActive);
    }

    private static void TrackDiagnosticCavityError(int memory, bool active)
    {
        if (memory <= 0 || !IsDiagnosticsActive)
            return;

        lock (_diagnosticErrorsLock)
        {
            if (active)
                _diagnosticErrorMemories.Add(memory);
            else
                _diagnosticErrorMemories.Remove(memory);
        }
    }

    private void UpdateAuthorizationHandshake(
        StationPresenceControls ui,
        Settings station,
        bool connected,
        bool errorActive,
        bool authorizationActive)
    {
        if (!connected)
        {
            ui.Handshake.Text = "SIN CONEXIÓN: no es posible comprobar la autorización.";
            ui.Handshake.BackColor = Color.Gainsboro;
            ui.Handshake.ForeColor = Color.DimGray;
            return;
        }

        if (!errorActive)
        {
            ui.Handshake.Text = $"LIBERADO: M{station.PlcCavityErrorMemory}=0. La estación puede quitar el bloqueo.";
            ui.Handshake.BackColor = Color.Honeydew;
            ui.Handshake.ForeColor = Color.DarkGreen;
            return;
        }

        if (authorizationActive)
        {
            ui.Handshake.Text = $"AUTORIZACIÓN ENVIADA: M{_qualityAuthorizationMemory}=1; esperando que el PLC cambie M{station.PlcCavityErrorMemory} a 0.";
            ui.Handshake.BackColor = Color.LightGoldenrodYellow;
            ui.Handshake.ForeColor = Color.DarkOrange;
            return;
        }

        ui.Handshake.Text = $"BLOQUEADO: M{station.PlcCavityErrorMemory}=1. Requiere llave; si ya terminó el pulso, el PLC no liberó el error.";
        ui.Handshake.BackColor = Color.MistyRose;
        ui.Handshake.ForeColor = Color.DarkRed;
    }

    private async Task<List<int>> ReadCoilsAsync(IReadOnlyList<int>? coils)
    {
        var values = new List<int>(4);
        for (var index = 0; index < 4; index++)
        {
            var address = coils is { Count: > 0 } && index < coils.Count ? coils[index] : 0;
            values.Add(await ReadCoilAsync(address));
        }
        return values;
    }

    private async Task<int> ReadCoilAsync(int address)
    {
        if (address <= 0)
            return 0;
        return await Task.Run(() => _plc.ReadCoil(address) ? 1 : 0);
    }

    private async Task<List<int>> ReadRegistersAsync(IReadOnlyList<int>? registers)
    {
        var values = new List<int>(4);
        for (var index = 0; index < 4; index++)
        {
            var address = registers is { Count: > 0 } && index < registers.Count ? registers[index] : 0;
            values.Add(await ReadRegisterAsync(address));
        }
        return values;
    }

    private async Task<int> ReadRegisterAsync(int address)
    {
        if (address <= 0)
            return 0;
        return await Task.Run(() => _plc.ReadHoldingRegister(address));
    }

    private static void UpdatePinIndicators(
        IReadOnlyList<Label> labels,
        IReadOnlyList<int>? registers,
        IReadOnlyList<int> values,
        bool connected)
    {
        for (var index = 0; index < labels.Count; index++)
        {
            var address = registers is { Count: > 0 } && index < registers.Count ? registers[index] : 0;
            var enabled = connected && address > 0 && values[index] != 0;
            labels[index].Text = address > 0
                ? $"P{index + 1}  D{address}: {(connected ? values[index] : "--") }"
                : $"P{index + 1}  sin D";
            labels[index].BackColor = enabled ? Color.LightGreen : Color.FromArgb(240, 243, 247);
            labels[index].ForeColor = enabled ? Color.DarkGreen : Color.DimGray;
        }
    }

    private static string GetCavityDetailText(int value)
    {
        return value switch
        {
            0 => "Sin error de cavidad",
            >= 1 and <= 4 => $"Cavidad {value}",
            _ => "Valor no reconocido"
        };
    }

    private sealed record StationPresenceControls(
        Label Connection,
        List<Label> FemalePins,
        List<Label> MalePins,
        Label Inserted,
        Label Error,
        Label Authorization,
        Label Detail,
        Label Handshake);
}
