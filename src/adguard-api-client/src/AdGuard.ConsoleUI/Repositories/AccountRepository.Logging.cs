using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for AccountRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class AccountRepository
{
    // Event ID Range: 3001-3099 (Account operations)

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Debug,
        Message = "Fetching account limits")]
    partial void LogFetchingAccountLimits();

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Information,
        Message = "Retrieved account limits")]
    partial void LogRetrievedAccountLimits();

    [LoggerMessage(
        EventId = 3003,
        Level = LogLevel.Error,
        Message = "API error while fetching account limits: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingAccountLimits(int errorCode, string message, Exception ex);
}
