using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for FilterListRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class FilterListRepository
{
    // Event ID Range: 5001-5099 (Filter list operations)

    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Debug,
        Message = "Fetching all filter lists")]
    partial void LogFetchingAllFilterLists();

    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} filter lists")]
    partial void LogRetrievedFilterLists(int count);

    [LoggerMessage(
        EventId = 5003,
        Level = LogLevel.Error,
        Message = "API error while fetching filter lists: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingFilterLists(int errorCode, string message, Exception ex);
}
