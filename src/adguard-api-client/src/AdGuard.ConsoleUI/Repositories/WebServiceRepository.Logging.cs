namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Logging extensions for WebServiceRepository.
/// Uses LoggerMessage source generation for high-performance logging.
/// </summary>
public partial class WebServiceRepository
{
    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Information,
        Message = "Fetching all web services")]
    partial void LogFetchingAllWebServices();

    [LoggerMessage(
        EventId = 5002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} web services")]
    partial void LogRetrievedWebServices(int Count);

    [LoggerMessage(
        EventId = 5003,
        Level = LogLevel.Error,
        Message = "API error fetching web services. Code: {ErrorCode}, Message: {ErrorMessage}")]
    partial void LogApiErrorFetchingWebServices(int ErrorCode, string ErrorMessage, Exception ex);
}
