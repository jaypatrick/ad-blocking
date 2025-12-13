using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for QueryLogRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class QueryLogRepository
{
    // Event ID Range: 6001-6099 (Query log operations)

    [LoggerMessage(
        EventId = 6001,
        Level = LogLevel.Debug,
        Message = "Fetching query log from {FromMillis} to {ToMillis}")]
    partial void LogFetchingQueryLog(long fromMillis, long toMillis);

    [LoggerMessage(
        EventId = 6002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} query log entries")]
    partial void LogRetrievedQueryLog(int count);

    [LoggerMessage(
        EventId = 6003,
        Level = LogLevel.Debug,
        Message = "Clearing query log")]
    partial void LogClearingQueryLog();

    [LoggerMessage(
        EventId = 6004,
        Level = LogLevel.Information,
        Message = "Query log cleared successfully")]
    partial void LogQueryLogCleared();

    [LoggerMessage(
        EventId = 6005,
        Level = LogLevel.Error,
        Message = "API error while fetching query log: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingQueryLog(int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 6006,
        Level = LogLevel.Error,
        Message = "API error while clearing query log: {ErrorCode} - {Message}")]
    partial void LogApiErrorClearingQueryLog(int errorCode, string message, Exception ex);
}
