namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="QueryLogRepository"/>.
/// </summary>
public partial class QueryLogRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching query log from {StartTime} to {EndTime}")]
    private partial void LogFetchingQueryLog(long startTime, long endTime);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} query log entries")]
    private partial void LogRetrievedQueryLog(int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Clearing query log")]
    private partial void LogClearingQueryLog();

    [LoggerMessage(Level = LogLevel.Information, Message = "Query log cleared")]
    private partial void LogQueryLogCleared();

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
