namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="StatisticsRepository"/>.
/// </summary>
public partial class StatisticsRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching statistics from {StartTime} to {EndTime}")]
    private partial void LogFetchingStatistics(long startTime, long endTime);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} statistics entries")]
    private partial void LogRetrievedStatistics(int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
