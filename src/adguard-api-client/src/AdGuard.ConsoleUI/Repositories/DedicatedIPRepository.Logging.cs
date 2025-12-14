namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Logging extensions for DedicatedIPRepository.
/// Uses LoggerMessage source generation for high-performance logging.
/// </summary>
public partial class DedicatedIPRepository
{
    [LoggerMessage(
        EventId = 6001,
        Level = LogLevel.Information,
        Message = "Fetching all dedicated IP addresses")]
    partial void LogFetchingAllDedicatedIPs();

    [LoggerMessage(
        EventId = 6002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} dedicated IP addresses")]
    partial void LogRetrievedDedicatedIPs(int Count);

    [LoggerMessage(
        EventId = 6003,
        Level = LogLevel.Error,
        Message = "API error fetching dedicated IP addresses. Code: {ErrorCode}, Message: {ErrorMessage}")]
    partial void LogApiErrorFetchingDedicatedIPs(int ErrorCode, string ErrorMessage, Exception ex);

    [LoggerMessage(
        EventId = 6004,
        Level = LogLevel.Information,
        Message = "Allocating new dedicated IP address")]
    partial void LogAllocatingDedicatedIP();

    [LoggerMessage(
        EventId = 6005,
        Level = LogLevel.Information,
        Message = "Allocated dedicated IP address: {IpAddress}")]
    partial void LogAllocatedDedicatedIP(string IpAddress);

    [LoggerMessage(
        EventId = 6006,
        Level = LogLevel.Error,
        Message = "API error allocating dedicated IP address. Code: {ErrorCode}, Message: {ErrorMessage}")]
    partial void LogApiErrorAllocatingDedicatedIP(int ErrorCode, string ErrorMessage, Exception ex);
}
