namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="AccountRepository"/>.
/// </summary>
public partial class AccountRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching account limits")]
    private partial void LogFetchingAccountLimits();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved account limits")]
    private partial void LogRetrievedAccountLimits();

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
