namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="DedicatedIpRepository"/>.
/// </summary>
public partial class DedicatedIpRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching all dedicated IP addresses")]
    private partial void LogFetchingDedicatedIps();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} dedicated IP addresses")]
    private partial void LogRetrievedDedicatedIps(int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Allocating new dedicated IP address")]
    private partial void LogAllocatingDedicatedIp();

    [LoggerMessage(Level = LogLevel.Information, Message = "Dedicated IP allocated: {Address}")]
    private partial void LogDedicatedIpAllocated(string address);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
