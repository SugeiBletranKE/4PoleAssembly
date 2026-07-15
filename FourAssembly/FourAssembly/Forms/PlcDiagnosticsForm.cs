namespace FourAssembly.Forms;

using FourAssembly.MultiStation.Services;
using FourAssembly.Services;

public partial class PlcDiagnosticsForm : Form
{
    private readonly PlcService _plc;
    private readonly PlcConfig _config;

    public PlcDiagnosticsForm(PlcService plc, PlcConfig config)
    {
        _plc = plc;
        _config = config;

        InitializeComponent();
        lblEndpoint.Text = $"PLC: {_config.Ip}:{_config.Port}";
        SetStatus("Ready", Color.DimGray);
    }

    private async void BtnTestConnection_Click(object sender, EventArgs e)
    {
        SetStatus("Testing connection...", Color.DarkOrange);
        AddLog("Testing PLC connection...");
        SetBusy(true);

        var connected = await Task.Run(() => _plc.Connect());
        SetBusy(false);
        if (connected)
        {
            SetStatus("Connected", Color.DarkGreen);
            AddLog($"Connected to PLC {_config.Ip}:{_config.Port}");
            SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Connection test OK for {_config.Ip}:{_config.Port}");
            return;
        }

        SetStatus("Connection failed", Color.Red);
        AddLog($"Connection failed to PLC {_config.Ip}:{_config.Port}");
        SystemEventLogService.LogError("PLC", "Diagnostics", $"Connection test failed for {_config.Ip}:{_config.Port}");
    }

    private async void BtnDisconnect_Click(object sender, EventArgs e)
    {
        SetBusy(true);
        var disconnected = await Task.Run(() => _plc.Disconnect());
        SetBusy(false);
        if (disconnected)
        {
            SetStatus("Disconnected", Color.DimGray);
            AddLog("PLC disconnected");
            return;
        }

        SetStatus("Disconnect failed", Color.Red);
        AddLog("PLC disconnect failed");
    }

    private async void BtnReadCoil_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtCoil.Text.Trim(), out var coil) || coil < 0)
        {
            SetStatus("Invalid coil", Color.Red);
            AddLog("Invalid coil address");
            return;
        }

        SetStatus($"Reading coil {coil}...", Color.DarkOrange);
        SetBusy(true);
        var value = await Task.Run(() => _plc.ReadCoil(coil));
        SetBusy(false);
        if (!_plc.IsConnected)
        {
            SetStatus("PLC not connected", Color.Red);
            AddLog($"PLC not connected. Could not read coil {coil}");
            return;
        }

        SetStatus($"Coil {coil}: {value}", value ? Color.DarkGreen : Color.DimGray);
        AddLog($"Read coil {coil} = {value}");
        SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Read coil {coil} = {value}");
    }

    private async void BtnSendPulse_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtPulseCoil.Text.Trim(), out var coil) || coil < 0)
        {
            SetStatus("Invalid pulse coil", Color.Red);
            AddLog("Invalid pulse coil address");
            return;
        }

        if (!int.TryParse(txtPulseMs.Text.Trim(), out var pulseMs) || pulseMs <= 0)
        {
            SetStatus("Invalid pulse time", Color.Red);
            AddLog("Invalid pulse time");
            return;
        }

        SetStatus($"Sending pulse coil {coil}...", Color.DarkOrange);
        SetBusy(true);
        var ok = await Task.Run(() => _plc.SendPulse(coil, pulseMs));
        SetBusy(false);
        if (ok)
        {
            SetStatus($"Pulse sent to coil {coil}", Color.DarkGreen);
            AddLog($"Sent pulse coil {coil} for {pulseMs} ms");
            SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Sent pulse coil {coil} for {pulseMs} ms");
            return;
        }

        SetStatus($"Pulse failed on coil {coil}", Color.Red);
        AddLog($"Failed to send pulse coil {coil}");
        SystemEventLogService.LogError("PLC", "Diagnostics", $"Failed to send pulse coil {coil}");
    }

    private async void BtnWriteRegister_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtRegister.Text.Trim(), out var register) || register < 0)
        {
            SetStatus("Invalid register", Color.Red);
            AddLog("Invalid register address");
            return;
        }

        if (!int.TryParse(txtRegisterValue.Text.Trim(), out var value) || value < 0 || value > ushort.MaxValue)
        {
            SetStatus("Invalid register value", Color.Red);
            AddLog($"Register value must be between 0 and {ushort.MaxValue}");
            return;
        }

        SetStatus($"Writing register {register}...", Color.DarkOrange);
        SetBusy(true);
        var ok = await Task.Run(() => _plc.WriteHoldingRegister(register, value));
        SetBusy(false);
        if (ok)
        {
            SetStatus($"Register {register} written", Color.DarkGreen);
            AddLog($"Wrote register {register} = {value}");
            SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Wrote register {register} = {value}");
            return;
        }

        SetStatus($"Write failed on register {register}", Color.Red);
        AddLog($"Failed to write register {register}");
        SystemEventLogService.LogError("PLC", "Diagnostics", $"Failed to write register {register}");
    }

    private async void BtnWriteStringRegister_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtStringRegister.Text.Trim(), out var register) || register < 0)
        {
            SetStatus("Invalid string register", Color.Red);
            AddLog("Invalid string register address");
            return;
        }

        var value = txtStringValue.Text.Trim();
        if (string.IsNullOrEmpty(value))
        {
            SetStatus("Invalid string value", Color.Red);
            AddLog("String value cannot be empty");
            return;
        }

        SetStatus($"Writing string register {register}...", Color.DarkOrange);
        SetBusy(true);
        var ok = await Task.Run(() => _plc.WriteHoldingRegisterString(register, value));
        SetBusy(false);
        if (ok)
        {
            SetStatus($"String register {register} written", Color.DarkGreen);
            AddLog($"Wrote string register D{register} = \"{value}\"");
            SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Wrote string register D{register} = \"{value}\"");
            return;
        }

        SetStatus($"String write failed on register {register}", Color.Red);
        AddLog($"Failed to write string register D{register}");
        SystemEventLogService.LogError("PLC", "Diagnostics", $"Failed to write string register D{register}");
    }

    private async void BtnWriteAnyRegister_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtAnyRegister.Text.Trim(), out var register) || register < 0)
        {
            SetStatus("Invalid register", Color.Red);
            AddLog("Invalid register address");
            return;
        }

        var valueText = txtAnyValue.Text.Trim();
        if (string.IsNullOrEmpty(valueText))
        {
            SetStatus("Invalid value", Color.Red);
            AddLog("Value cannot be empty");
            return;
        }

        var writeAsString = cmbAnyType.SelectedIndex == 1;
        SetStatus($"Writing D{register}...", Color.DarkOrange);
        SetBusy(true);

        bool ok;
        if (writeAsString)
        {
            ok = await Task.Run(() => _plc.WriteHoldingRegisterString(register, valueText));
        }
        else
        {
            if (!int.TryParse(valueText, out var value) || value < 0 || value > ushort.MaxValue)
            {
                SetBusy(false);
                SetStatus("Invalid numeric value", Color.Red);
                AddLog($"Numeric value must be between 0 and {ushort.MaxValue}");
                return;
            }

            ok = await Task.Run(() => _plc.WriteHoldingRegister(register, value));
        }

        SetBusy(false);
        if (ok)
        {
            var type = writeAsString ? "string" : "number";
            SetStatus($"D{register} written", Color.DarkGreen);
            AddLog($"Wrote D{register} as {type} = {valueText}");
            SystemEventLogService.LogInfo("PLC", "Diagnostics", $"Wrote D{register} as {type} = {valueText}");
            return;
        }

        SetStatus($"Write failed on D{register}", Color.Red);
        AddLog($"Failed to write D{register} = {valueText}");
        SystemEventLogService.LogError("PLC", "Diagnostics", $"Failed to write D{register} = {valueText}");
    }

    private void SetStatus(string text, Color color)
    {
        lblStatus.Text = text;
        lblStatus.ForeColor = color;
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        lstLog.Items.Add($"[{timestamp}] {message}");
        lstLog.TopIndex = Math.Max(0, lstLog.Items.Count - 1);
    }

    private void SetBusy(bool busy)
    {
        btnTestConnection.Enabled = !busy;
        btnDisconnect.Enabled = !busy;
        btnReadCoil.Enabled = !busy;
        btnSendPulse.Enabled = !busy;
        btnWriteRegister.Enabled = !busy;
        btnWriteStringRegister.Enabled = !busy;
        btnWriteAnyRegister.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void PlcDiagnosticsForm_Load(object sender, EventArgs e)
    {

    }

    private void PlcDiagnosticsForm_Load_1(object sender, EventArgs e)
    {

    }
}
