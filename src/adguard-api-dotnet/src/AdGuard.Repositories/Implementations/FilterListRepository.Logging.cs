namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="FilterListRepository"/>.
/// </summary>
public partial class FilterListRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching all filter lists")]
    private partial void LogFetchingFilterLists();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} filter lists")]
    private partial void LogRetrievedFilterLists(int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
