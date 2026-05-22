namespace FourAssembly.MultiStation.Services;

using FourAssembly.MultiStation.Models;
using FourAssembly.Services;
using System.Reflection;

public class PlcService
{
    private readonly PlcConfig _config;
    private object? _client;
    private bool _connected = false;
    private Type? _modbusClientType;

    public PlcService(PlcConfig config)
    {
        _config = config;
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
            }
        }
        catch
        {
            LogService.Log("Warning: EasyModbus not found. PLC operations will be simulated.");
            _modbusClientType = null;
            _client = null;
        }
    }

    public bool Connect()
    {
        try
        {
            if (_connected) return true;

            if (_modbusClientType == null)
            {
                _connected = true;
                LogService.Log("PLC connected (simulated - EasyModbus not available)");
                return true;
            }

            var connectMethod = _modbusClientType.GetMethod("Connect", Type.EmptyTypes);
            connectMethod?.Invoke(_client, null);
            _connected = true;
            LogService.Log("PLC connected successfully");
            return true;
        }
        catch (Exception ex)
        {
            _connected = false;
            LogService.LogError("Failed to connect to PLC", ex);
            return false;
        }
    }

    public bool Disconnect()
    {
        try
        {
            if (_modbusClientType == null)
            {
                _connected = false;
                LogService.Log("PLC disconnected (simulated)");
                return true;
            }

            var disconnectMethod = _modbusClientType.GetMethod("Disconnect", Type.EmptyTypes);
            disconnectMethod?.Invoke(_client, null);
            _connected = false;
            LogService.Log("PLC disconnected");
            return true;
        }
        catch (Exception ex)
        {
            LogService.LogError("Error disconnecting from PLC", ex);
            return false;
        }
    }

    public bool WriteCoil(int coilAddress, bool value)
    {
        try
        {
            if (!_connected && !Connect()) return false;

            if (_modbusClientType == null)
            {
                LogService.Log($"Wrote coil {coilAddress} = {value} (simulated)");
                return true;
            }

            var method = _modbusClientType.GetMethod("WriteSingleCoil", new[] { typeof(int), typeof(bool) });
            method?.Invoke(_client, new object[] { coilAddress, value });
            LogService.Log($"Wrote coil {coilAddress} = {value}");
            return true;
        }
        catch (Exception ex)
        {
            LogService.LogError($"Failed to write coil {coilAddress}", ex);
            return false;
        }
    }

    public bool WriteHoldingRegister(int register, int value)
    {
        try
        {
            if (!_connected && !Connect()) return false;

            if (_modbusClientType == null)
            {
                LogService.Log($"Wrote register {register} = {value} (simulated)");
                return true;
            }

            var method = _modbusClientType.GetMethod("WriteSingleRegister", new[] { typeof(int), typeof(ushort) });
            method?.Invoke(_client, new object[] { register, (ushort)value });
            LogService.Log($"Wrote register {register} = {value}");
            return true;
        }
        catch (Exception ex)
        {
            LogService.LogError($"Failed to write register {register}", ex);
            return false;
        }
    }

    public bool SendPulse(int coilAddress, int ms = 500)
    {
        try
        {
            if (!WriteCoil(coilAddress, true)) return false;
            Thread.Sleep(ms);
            if (!WriteCoil(coilAddress, false)) return false;
            LogService.Log($"Sent pulse on coil {coilAddress} ({ms}ms)");
            return true;
        }
        catch (Exception ex)
        {
            LogService.LogError($"Failed to send pulse on coil {coilAddress}", ex);
            return false;
        }
    }
}
