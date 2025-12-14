namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="UserRulesRepository"/>.
/// </summary>
public partial class UserRulesRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching user rules for DNS server: {DnsServerId}")]
    private partial void LogFetchingUserRules(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} user rules for DNS server: {DnsServerId}")]
    private partial void LogRetrievedUserRules(string dnsServerId, int count);

    [LoggerMessage(Level = LogLevel.Warning, Message = "DNS server not found with ID: {DnsServerId}")]
    private partial void LogDnsServerNotFound(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating user rules for DNS server: {DnsServerId}")]
    private partial void LogUpdatingUserRules(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Information, Message = "User rules updated for DNS server: {DnsServerId}")]
    private partial void LogUserRulesUpdated(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Appending {Count} rules to DNS server: {DnsServerId}")]
    private partial void LogAppendingUserRules(string dnsServerId, int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
