namespace FourAssembly.Services;

using System.IO.Ports;

public static class CognexService
{
    public static event Action<int, string>? CableReceived;

    private static readonly SerialPort?[] _ports = new SerialPort?[3];
    private static readonly object _lockObj = new();
    private static readonly Thread?[] _readThreads = new Thread?[3];
    private static volatile bool _running = true;

    public static void Initialize(string com1, string com2, string com3)
    {
        _running = true;
        LogService.Log("CognexService.Initialize() starting...");
        SystemEventLogService.LogInfo("COGNEX", "General", "CognexService.Initialize() starting");
        var availablePorts = SerialPort.GetPortNames().OrderBy(name => name).ToArray();
        LogService.Log($"Available COM ports: {(availablePorts.Length == 0 ? "NONE" : string.Join(", ", availablePorts))}");
        var portNames = new[] { com1, com2, com3 };

        for (int i = 0; i < 3; i++)
        {
            var station = $"Station {i + 1}";
            try
            {
                LogService.Log($"Attempting to open Station {i + 1} on {portNames[i]}...");
                SystemEventLogService.LogInfo("COGNEX", station, $"Attempting to open port {portNames[i]}");
                var port = new SerialPort(portNames[i])
                {
                    BaudRate = 115200,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ReadTimeout = 100,
                    WriteTimeout = 1000,
                    NewLine = "\r\n",
                    RtsEnable = true,
                    DtrEnable = true
                };

                port.Open();
                _ports[i] = port;
                SystemEventLogService.LogInfo("COGNEX", station, $"Connected on {portNames[i]}");

                LogService.Log($"✓ Cognex Station {i + 1} connected on {portNames[i]}");

                // Start reader thread for this port
                int stationNum = i + 1;
                var readerThread = new Thread(() => ReadPortData(stationNum, port))
                {
                    IsBackground = true,
                    Name = $"CognexReader-Station{stationNum}"
                };
                readerThread.Start();
                _readThreads[i] = readerThread;
            }
            catch (Exception ex)
            {
                LogService.LogError($"Failed to open {portNames[i]}", ex);
                SystemEventLogService.LogError("COGNEX", station, $"Failed to open port {portNames[i]}", ex);
            }
        }
    }

    private static void ReadPortData(int stationNumber, SerialPort port)
    {
        LogService.Log($"[ReadThread-{stationNumber}] Started reader thread");
        SystemEventLogService.LogInfo("SCANNER", $"Station {stationNumber}", $"Started reader thread on {port.PortName}");
        var buffer = new System.Text.StringBuilder();
        var lastByteReceivedAt = DateTime.MinValue;

        void EmitBufferedData(string completionReason)
        {
            var line = buffer.ToString();
            buffer.Clear();

            if (string.IsNullOrWhiteSpace(line))
                return;

            LogService.Log($"[ReadThread-{stationNumber}] Complete data ({completionReason}): '{line}'");
            SystemEventLogService.LogInfo("SCANNER", $"Station {stationNumber}", $"Complete data ({completionReason}): '{line}'");
            ProcessData(stationNumber, line);
        }

        while (_running && port.IsOpen)
        {
            try
            {
                if (port.BytesToRead > 0)
                {
                    int byteRead = port.ReadByte();

                    // Carriage return (\r = 13) o Line feed (\n = 10)
                    if (byteRead == 13 || byteRead == 10)
                    {
                        EmitBufferedData("terminator");
                    }
                    else
                    {
                        buffer.Append((char)byteRead);
                        lastByteReceivedAt = DateTime.Now;
                    }
                }
                else
                {
                    // Algunos Cognex no agregan CR/LF. Si dejaron de llegar bytes,
                    // se considera completa la lectura después de una pausa corta.
                    if (buffer.Length > 0
                        && lastByteReceivedAt != DateTime.MinValue
                        && DateTime.Now - lastByteReceivedAt >= TimeSpan.FromMilliseconds(150))
                    {
                        EmitBufferedData("idle timeout");
                    }

                    Thread.Sleep(10);
                }
            }
            catch (TimeoutException)
            {
            }
            catch (Exception ex)
            {
                LogService.LogError($"[ReadThread-{stationNumber}] Error", ex);
                SystemEventLogService.LogError("SCANNER", $"Station {stationNumber}", "Reader thread error", ex);
                Thread.Sleep(100);
            }
        }

        LogService.Log($"[ReadThread-{stationNumber}] Exited");
        SystemEventLogService.LogInfo("SCANNER", $"Station {stationNumber}", "Reader thread exited");
    }

    private static void ProcessData(int stationNumber, string line)
    {
        try
        {
            // Quita espacios/terminadores y los delimitadores STX/ETX que algunos
            // lectores seriales agregan alrededor del código decodificado.
            var data = line.Trim()
                .Trim('\0', '\x02', '\x03')
                .Trim();

            if (string.IsNullOrWhiteSpace(data))
            {
                LogService.Log($"[ProcessData-{stationNumber}] Empty data received");
                return;
            }

            LogService.Log($"✓ [COM Station {stationNumber}] Data received: {data}");

            SystemEventLogService.LogInfo("SCANNER", $"Station {stationNumber}", $"Data received: {data}");
            CableReceived?.Invoke(stationNumber, data);
        }
        catch (Exception ex)
        {
            LogService.LogError($"[ProcessData-{stationNumber}] Error", ex);
            SystemEventLogService.LogError("SCANNER", $"Station {stationNumber}", "Error processing scanner data", ex);
        }
    }

    public static bool IsStationConnected(int stationNumber)
    {
        var index = stationNumber - 1;
        return index >= 0
            && index < _ports.Length
            && _ports[index]?.IsOpen == true;
    }

    public static void Stop()
    {
        LogService.Log("CognexService.Stop() called");
        SystemEventLogService.LogInfo("COGNEX", "General", "CognexService.Stop() called");
        _running = false;

        lock (_lockObj)
        {
            foreach (var port in _ports)
            {
                if (port?.IsOpen == true)
                {
                    try
                    {
                        port.Close();
                        port.Dispose();
                    }
                    catch { }
                }
            }
        }

        foreach (var thread in _readThreads)
        {
            thread?.Join(TimeSpan.FromSeconds(1));
        }

        LogService.Log("CognexService stopped");
        SystemEventLogService.LogInfo("COGNEX", "General", "CognexService stopped");
    }
}
