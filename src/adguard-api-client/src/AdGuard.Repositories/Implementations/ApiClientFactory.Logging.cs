namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="ApiClientFactory"/>.
/// </summary>
public partial class ApiClientFactory
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Configuring API client with API key")]
    private partial void LogConfiguringApiClient();

    [LoggerMessage(Level = LogLevel.Information, Message = "API client configured successfully")]
    private partial void LogApiClientConfigured();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Attempting to configure API client from settings")]
    private partial void LogConfiguringFromSettings();

    [LoggerMessage(Level = LogLevel.Warning, Message = "No API key found in settings")]
    private partial void LogNoApiKeyInSettings();

    [LoggerMessage(Level = LogLevel.Warning, Message = "API client is not configured")]
    private partial void LogNotConfigured();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Testing API connection")]
    private partial void LogTestingConnection();

    [LoggerMessage(Level = LogLevel.Information, Message = "API connection test successful")]
    private partial void LogConnectionSuccessful();

    [LoggerMessage(Level = LogLevel.Warning, Message = "API connection test failed: {ErrorCode} - {ErrorMessage}")]
    private partial void LogConnectionFailed(int errorCode, string errorMessage, Exception ex);
}
