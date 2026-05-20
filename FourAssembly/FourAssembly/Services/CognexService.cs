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
        LogService.Log("CognexService.Initialize() starting...");
        var portNames = new[] { com1, com2, com3 };

        for (int i = 0; i < 3; i++)
        {
            try
            {
                LogService.Log($"Attempting to open Station {i + 1} on {portNames[i]}...");
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
            }
        }
    }

    private static void ReadPortData(int stationNumber, SerialPort port)
    {
        LogService.Log($"[ReadThread-{stationNumber}] Started reader thread");
        var buffer = new System.Text.StringBuilder();

        while (_running && port.IsOpen)
        {
            try
            {
                if (port.BytesToRead > 0)
                {
                    int byteRead = port.ReadByte();
                    LogService.Log($"[ReadThread-{stationNumber}] Byte: {byteRead} ('{(char)byteRead}')");

                    // Carriage return (\r = 13) o Line feed (\n = 10)
                    if (byteRead == 13 || byteRead == 10)
                    {
                        var line = buffer.ToString().Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            LogService.Log($"[ReadThread-{stationNumber}] Complete line: '{line}'");
                            ProcessData(stationNumber, line);
                        }
                        buffer.Clear();
                    }
                    else
                    {
                        buffer.Append((char)byteRead);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (TimeoutException)
            {
            }
            catch (Exception ex)
            {
                LogService.LogError($"[ReadThread-{stationNumber}] Error", ex);
                Thread.Sleep(100);
            }
        }

        LogService.Log($"[ReadThread-{stationNumber}] Exited");
    }

    private static void ProcessData(int stationNumber, string line)
    {
        try
        {
            var data = line.Trim();

            if (string.IsNullOrWhiteSpace(data))
            {
                LogService.Log($"[ProcessData-{stationNumber}] Empty data received");
                return;
            }

            LogService.Log($"✓ [COM Station {stationNumber}] Data received: {data}");

            CableReceived?.Invoke(stationNumber, data);
        }
        catch (Exception ex)
        {
            LogService.LogError($"[ProcessData-{stationNumber}] Error", ex);
        }
    }

    public static void Stop()
    {
        LogService.Log("CognexService.Stop() called");
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
    }
}
