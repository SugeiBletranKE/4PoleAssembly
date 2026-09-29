namespace FourAssembly.Forms;

using FourAssembly.MultiStation.Controls;

public sealed partial class StationCountersForm : Form
{
    public sealed record StationCounterAccess(
        int StationNumber,
        string StationName,
        Func<StationPanel.GoodPieceCounterSnapshot> Read,
        Action ResetTotal,
        Action ResetBg,
        Action ResetHour);

    private enum CounterResetType { Total, Bg, Hour }
    private sealed record CounterResetAction(StationCounterAccess Station, CounterResetType Type);

    private readonly IReadOnlyList<StationCounterAccess> _stations;
    private readonly Dictionary<int, Label> _valueLabels = [];
    private readonly System.Windows.Forms.Timer _refreshTimer = new() { Interval = 1000 };

    public StationCountersForm()
    {
        _stations = [];
        InitializeComponent();
    }

    public StationCountersForm(IEnumerable<StationCounterAccess> stations)
    {
        InitializeComponent();
        _stations = stations.OrderBy(station => station.StationNumber).ToList();
        ConfigureStationRows();

        _refreshTimer.Tick += RefreshTimer_Tick;
        Shown += StationCountersForm_Shown;
        FormClosed += StationCountersForm_FormClosed;
    }

    private void ConfigureStationRows()
    {
        var stationLabels = new[] { lblStation1, lblStation2, lblStation3 };
        var valueLabels = new[] { lblCounter1, lblCounter2, lblCounter3 };
        var totalButtons = new[] { btnResetTotal1, btnResetTotal2, btnResetTotal3 };
        var bgButtons = new[] { btnResetBg1, btnResetBg2, btnResetBg3 };
        var hourButtons = new[] { btnResetHour1, btnResetHour2, btnResetHour3 };

        for (var row = 0; row < stationLabels.Length; row++)
        {
            var visible = row < _stations.Count;
            stationLabels[row].Visible = visible;
            valueLabels[row].Visible = visible;
            totalButtons[row].Visible = visible;
            bgButtons[row].Visible = visible;
            hourButtons[row].Visible = visible;

            if (!visible)
                continue;

            var station = _stations[row];
            stationLabels[row].Text = station.StationName;
            totalButtons[row].Tag = new CounterResetAction(station, CounterResetType.Total);
            bgButtons[row].Tag = new CounterResetAction(station, CounterResetType.Bg);
            hourButtons[row].Tag = new CounterResetAction(station, CounterResetType.Hour);
            _valueLabels[station.StationNumber] = valueLabels[row];
        }

        ClientSize = new Size(ClientSize.Width, 90 + (Math.Max(1, _stations.Count) * 110));
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: CounterResetAction reset })
            return;

        var counterName = reset.Type switch
        {
            CounterResetType.Total => "total",
            CounterResetType.Bg => "de BG",
            CounterResetType.Hour => "por hora",
            _ => "seleccionado"
        };

        var confirmation = MessageBox.Show(
            this,
            $"¿Reiniciar únicamente el contador {counterName} de {reset.Station.StationName}?",
            "Confirmar reinicio",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        if (confirmation != DialogResult.Yes)
            return;

        switch (reset.Type)
        {
            case CounterResetType.Total:
                reset.Station.ResetTotal();
                break;
            case CounterResetType.Bg:
                reset.Station.ResetBg();
                break;
            case CounterResetType.Hour:
                reset.Station.ResetHour();
                break;
        }

        RefreshCounters();
    }

    private void RefreshTimer_Tick(object? sender, EventArgs e) => RefreshCounters();

    private void StationCountersForm_Shown(object? sender, EventArgs e)
    {
        RefreshCounters();
        _refreshTimer.Start();
    }

    private void StationCountersForm_FormClosed(object? sender, FormClosedEventArgs e) =>
        _refreshTimer.Dispose();

    private void RefreshCounters()
    {
        foreach (var station in _stations)
        {
            var snapshot = station.Read();
            if (_valueLabels.TryGetValue(station.StationNumber, out var label))
            {
                label.Text =
                    $"Total estación: {snapshot.Total}{Environment.NewLine}" +
                    $"BG {(string.IsNullOrWhiteSpace(snapshot.Bg) ? "--" : snapshot.Bg)}: {snapshot.BgTotal}{Environment.NewLine}" +
                    $"{snapshot.HourStart:HH}:00-{snapshot.HourStart.AddHours(1):HH}:00: {snapshot.HourTotal}";
            }
        }
    }
}
