namespace FourAssembly.MultiStation.Services;

using FourAssembly.Services;
using System.Reflection;

public class PlcService
{
    private const int ConnectRetryCooldownMs = 3000;
    private const int ConnectionTimeoutMs = 1000;

    private readonly PlcConfig _config;
    private readonly object _syncRoot = new();
    private object? _client;
    private bool _connected;
    private Type? _modbusClientType;
    private DateTime _nextConnectAttemptUtc = DateTime.MinValue;

    public bool IsConnected => _connected;

    public PlcService(PlcConfig config)
    {
        _config = config;
        SystemEventLogService.LogInfo("PLC", "General", $"PLC service created for {_config.Ip}:{_config.Port}");
        TryLoadModbusClient();
    }

    private void TryLoadModbusClient()
    {
        try
        {
            var assembly = Assembly.Load("EasyModbus");
            _modbusClientType = assembly.GetType("EasyModbus.ModbusClient");
            if (_modbusClientType != null)
            {
                _client = Activator.CreateInstance(_modbusClientType, _config.Ip, _config.Port);
                ConfigureConnectionTimeout();
            }
        }
        catch (Exception ex)
        {
            LogService.LogError("EasyModbus not found. PLC operations are disabled.", ex);
            SystemEventLogService.LogError("PLC", "General", "EasyModbus not found. PLC operations are disabled.", ex);
            _modbusClientType = null;
            _client = null;
        }
    }

    public bool Connect()
    {
        lock (_syncRoot)
        {
            try
            {
                if (_connected)
                    return true;

                if (DateTime.UtcNow < _nextConnectAttemptUtc)
                    return false;

                if (_modbusClientType == null || _client == null)
                {
                    _connected = false;
                    _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                    LogService.LogError("Cannot connect to PLC because EasyModbus is not loaded");
                    SystemEventLogService.LogError("PLC", "General", "Cannot connect to PLC because EasyModbus is not loaded");
                    return false;
                }

                var connectMethod = _modbusClientType.GetMethod("Connect", Type.EmptyTypes);
                connectMethod?.Invoke(_client, null);
                _connected = true;
                _nextConnectAttemptUtc = DateTime.MinValue;

                LogService.Log("PLC connected successfully");
                SystemEventLogService.LogInfo("PLC", "General", $"PLC connected successfully to {_config.Ip}:{_config.Port}");
                return true;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError("Failed to connect to PLC", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to connect to PLC {_config.Ip}:{_config.Port}", ex);
                return false;
            }
        }
    }

    public bool Disconnect()
    {
        lock (_syncRoot)
        {
            try
            {
                if (_modbusClientType == null || _client == null)
                {
                    _connected = false;
                    LogService.Log("PLC disconnected (EasyModbus not loaded)");
                    SystemEventLogService.LogInfo("PLC", "General", "PLC disconnected (EasyModbus not loaded)");
                    return true;
                }

                var disconnectMethod = _modbusClientType.GetMethod("Disconnect", Type.EmptyTypes);
                disconnectMethod?.Invoke(_client, null);
                _connected = false;

                LogService.Log("PLC disconnected");
                SystemEventLogService.LogInfo("PLC", "General", "PLC disconnected");
                return true;
            }
            catch (Exception ex)
            {
                _connected = false;
                LogService.LogError("Error disconnecting from PLC", ex);
                SystemEventLogService.LogError("PLC", "General", "Error disconnecting from PLC", ex);
                return false;
            }
        }
    }

    public bool WriteCoil(int coilAddress, bool value)
    {
        lock (_syncRoot)
        {
            try
            {
                // Envia una senal digital al PLC escribiendo ON/OFF en la bobina indicada.
                // Si no hay conexion activa, intenta reconectar antes de mandar la senal.
                if (!_connected && !Connect())
                    return false;

                if (_modbusClientType == null || _client == null)
                {
                    LogService.LogError($"Cannot write coil {coilAddress} because EasyModbus is not loaded");
                    return false;
                }

                var method = _modbusClientType.GetMethod("WriteSingleCoil", new[] { typeof(int), typeof(bool) });
                method?.Invoke(_client, new object[] { coilAddress, value });
                LogService.Log($"Wrote coil {coilAddress} = {value}");
                return true;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError($"Failed to write coil {coilAddress}", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to write coil {coilAddress}", ex);
                return false;
            }
        }
    }

    public bool ReadCoil(int coilAddress)
    {
        lock (_syncRoot)
        {
            try
            {
                if (!_connected && !Connect())
                    return false;

                if (_modbusClientType == null || _client == null)
                {
                    LogService.LogError($"Cannot read coil {coilAddress} because EasyModbus is not loaded");
                    return false;
                }

                var method = _modbusClientType.GetMethod("ReadCoils", new[] { typeof(int), typeof(int) });
                if (method?.Invoke(_client, new object[] { coilAddress, 1 }) is bool[] values && values.Length > 0)
                    return values[0];

                return false;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError($"Failed to read coil {coilAddress}", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to read coil {coilAddress}", ex);
                return false;
            }
        }
    }

    public bool WriteHoldingRegister(int register, int value)
    {
        lock (_syncRoot)
        {
            try
            {
                // Envia un valor numerico al PLC escribiendolo en el registro configurado.
                // Se usa para mandar datos de proceso, como el contador de cables.
                if (!_connected && !Connect())
                    return false;

                if (_modbusClientType == null || _client == null)
                {
                    LogService.LogError($"Cannot write register {register} because EasyModbus is not loaded");
                    return false;
                }

                var method = _modbusClientType.GetMethod("WriteSingleRegister", new[] { typeof(int), typeof(ushort) });
                method?.Invoke(_client, new object[] { register, (ushort)value });
                LogService.Log($"Wrote register {register} = {value}");
                return true;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError($"Failed to write register {register}", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to write register {register}", ex);
                return false;
            }
        }
    }

    public int ReadHoldingRegister(int register)
    {
        lock (_syncRoot)
        {
            try
            {
                if (!_connected && !Connect())
                    return 0;

                if (_modbusClientType == null || _client == null)
                {
                    LogService.LogError($"Cannot read register {register} because EasyModbus is not loaded");
                    return 0;
                }

                var method = _modbusClientType.GetMethod("ReadHoldingRegisters", new[] { typeof(int), typeof(int) });
                if (method?.Invoke(_client, new object[] { register, 1 }) is int[] values && values.Length > 0)
                    return values[0];

                return 0;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError($"Failed to read register {register}", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to read register {register}", ex);
                return 0;
            }
        }
    }

    public bool WriteHoldingRegisterString(int register, string value)
    {
        lock (_syncRoot)
        {
            try
            {
                if (!_connected && !Connect())
                    return false;

                if (_modbusClientType == null || _client == null)
                {
                    LogService.LogError($"Cannot write string register {register} because EasyModbus is not loaded");
                    return false;
                }

                var registers = ConvertStringToRegisters(value);
                var method = _modbusClientType.GetMethod("WriteMultipleRegisters", new[] { typeof(int), typeof(int[]) });
                method?.Invoke(_client, new object[] { register, registers });
                LogService.Log($"Wrote string register {register} = {value}");
                return true;
            }
            catch (Exception ex)
            {
                _connected = false;
                _nextConnectAttemptUtc = DateTime.UtcNow.AddMilliseconds(ConnectRetryCooldownMs);
                LogService.LogError($"Failed to write string register {register}", ex);
                SystemEventLogService.LogError("PLC", "General", $"Failed to write string register {register}", ex);
                return false;
            }
        }
    }

    public bool SendPulse(int coilAddress, int ms = 500)
    {
        try
        {
            // Genera una senal momentanea: prende la bobina, espera el tiempo indicado
            // y la apaga para que el PLC detecte el flanco de liberacion.
            if (!WriteCoil(coilAddress, true))
                return false;

            Thread.Sleep(ms);

            if (!WriteCoil(coilAddress, false))
                return false;

            LogService.Log($"Sent pulse on coil {coilAddress} ({ms}ms)");
            return true;
        }
        catch (Exception ex)
        {
            _connected = false;
            LogService.LogError($"Failed to send pulse on coil {coilAddress}", ex);
            SystemEventLogService.LogError("PLC", "General", $"Failed to send pulse on coil {coilAddress}", ex);
            return false;
        }
    }

    private void ConfigureConnectionTimeout()
    {
        if (_client == null || _modbusClientType == null)
            return;

        var timeoutProperty = _modbusClientType.GetProperty("ConnectionTimeout");
        if (timeoutProperty?.CanWrite == true)
            timeoutProperty.SetValue(_client, ConnectionTimeoutMs);
    }

    private static int[] ConvertStringToRegisters(string value)
    {
        var text = string.IsNullOrEmpty(value) ? "\0" : value;
        if (text.Length % 2 != 0)
            text += "\0";

        var registers = new int[text.Length / 2];
        for (var i = 0; i < registers.Length; i++)
        {
            // El PLC/HMI almacena strings en D como byte bajo primero.
            // Para "A" en D0 debe quedar 0x0041, no 0x4100.
            registers[i] = text[i * 2] + (text[(i * 2) + 1] << 8);
        }

        return registers;
    }
}
