using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AdGuard.ApiClient.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
public class LoggingBenchmarks
{
    private readonly ILogger<LoggingBenchmarks> _logger = NullLogger<LoggingBenchmarks>.Instance;
    private readonly string _deviceId = "test-device-123";
    private readonly string _deviceName = "Test Device";
    private readonly int _count = 100;

    [Benchmark(Description = "Standard LogDebug with interpolation")]
    public void StandardLogDebug()
    {
        for (int i = 0; i < _count; i++)
        {
            _logger.LogDebug("Fetching device with ID: {DeviceId}", _deviceId);
        }
    }

    [Benchmark(Description = "LogDebug with IsEnabled check")]
    public void LogDebugWithCheck()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Fetching device with ID: {DeviceId}", _deviceId);
            }
        }
    }

    [Benchmark(Description = "Standard LogInformation")]
    public void StandardLogInformation()
    {
        for (int i = 0; i < _count; i++)
        {
            _logger.LogInformation("Retrieved device: {DeviceName} (ID: {DeviceId})", _deviceName, _deviceId);
        }
    }

    [Benchmark(Description = "LogInformation with IsEnabled check")]
    public void LogInformationWithCheck()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Retrieved device: {DeviceName} (ID: {DeviceId})", _deviceName, _deviceId);
            }
        }
    }

    [Benchmark(Description = "String interpolation")]
    public void StringInterpolation()
    {
        for (int i = 0; i < _count; i++)
        {
            var message = $"Fetching device with ID: {_deviceId}";
            // Simulate logging overhead without actual logging
            _ = message;
        }
    }

    [Benchmark(Description = "String.Format")]
    public void StringFormat()
    {
        for (int i = 0; i < _count; i++)
        {
            var message = string.Format("Fetching device with ID: {0}", _deviceId);
            _ = message;
        }
    }
}
