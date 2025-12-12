using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for StatisticsRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class StatisticsRepository
{
    // Event ID Range: 4001-4099 (Statistics operations)

    [LoggerMessage(
        EventId = 4001,
        Level = LogLevel.Debug,
        Message = "Fetching statistics from {FromMillis} to {ToMillis}")]
    partial void LogFetchingStatistics(long fromMillis, long toMillis);

    [LoggerMessage(
        EventId = 4002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} statistics records")]
    partial void LogRetrievedStatistics(int count);

    [LoggerMessage(
        EventId = 4003,
        Level = LogLevel.Error,
        Message = "API error while fetching statistics: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingStatistics(int errorCode, string message, Exception ex);
}
