namespace FourAssembly.MultiStation.Services;

using System.IO.Ports;
using FourAssembly.Services;

public class StationCognexReader
{
    private SerialPort? _port;
    private Thread? _readerThread;
    private bool _running = false;
    private byte[] _buffer = new byte[1024];
    private int _bufferIndex = 0;

    public event Action<string>? CableReceived;

    public StationCognexReader(string comPort)
    {
        _port = new SerialPort(comPort, 115200, Parity.None, 8, StopBits.One)
        {
            RtsEnable = true,
            DtrEnable = true,
            ReadTimeout = 1000,
            WriteTimeout = 1000
        };
    }

    public bool Start()
    {
        try
        {
            if (_port?.IsOpen == true) return true;
            _port?.Open();
            _running = true;
            _readerThread = new Thread(ReaderLoop) { IsBackground = true, Name = $"CognexReader-{_port?.PortName}" };
            _readerThread.Start();
            return true;
        }
        catch (Exception ex)
        {
            LogService.LogError($"Failed to start CognexReader on {_port?.PortName}", ex);
            return false;
        }
    }

    public void Stop()
    {
        _running = false;
        try
        {
            _readerThread?.Join(1000);
            _port?.Close();
            _port?.Dispose();
        }
        catch (Exception ex)
        {
            LogService.LogError("Error stopping CognexReader", ex);
        }
    }

    private void ReaderLoop()
    {
        while (_running && _port?.IsOpen == true)
        {
            try
            {
                if (_port.BytesToRead > 0)
                {
                    int data = _port.ReadByte();
                    if (data == 13 || data == 10) // \r or \n
                    {
                        if (_bufferIndex > 0)
                        {
                            var cable = System.Text.Encoding.ASCII.GetString(_buffer, 0, _bufferIndex).Trim();
                            if (!string.IsNullOrWhiteSpace(cable))
                                CableReceived?.Invoke(cable);
                            _bufferIndex = 0;
                        }
                    }
                    else
                    {
                        _buffer[_bufferIndex++] = (byte)data;
                        if (_bufferIndex >= _buffer.Length) _bufferIndex = 0;
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
            catch (TimeoutException)
            {
                // Normal timeout, continue
            }
            catch (Exception ex)
            {
                LogService.LogError("Error in CognexReader loop", ex);
                _running = false;
            }
        }
    }
}
