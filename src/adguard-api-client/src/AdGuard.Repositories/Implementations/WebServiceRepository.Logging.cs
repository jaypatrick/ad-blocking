namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="WebServiceRepository"/>.
/// </summary>
public partial class WebServiceRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching all web services")]
    private partial void LogFetchingWebServices();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} web services")]
    private partial void LogRetrievedWebServices(int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
