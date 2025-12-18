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

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating rules from file '{FilePath}' for DNS server: {DnsServerId}")]
    private partial void LogUpdatingRulesFromFile(string dnsServerId, string filePath);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updated {Count} rules from file for DNS server: {DnsServerId}")]
    private partial void LogRulesUpdatedFromFile(string dnsServerId, int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to read rules file '{FilePath}': {ErrorMessage}")]
    private partial void LogFileReadError(string filePath, string errorMessage, Exception ex);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Setting rules enabled state to {Enabled} for DNS server: {DnsServerId}")]
    private partial void LogSettingRulesEnabled(string dnsServerId, bool enabled);

    [LoggerMessage(Level = LogLevel.Information, Message = "Rules enabled state set to {Enabled} for DNS server: {DnsServerId}")]
    private partial void LogRulesEnabledSet(string dnsServerId, bool enabled);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Adding rule '{Rule}' to DNS server: {DnsServerId}")]
    private partial void LogAddingRule(string dnsServerId, string rule);

    [LoggerMessage(Level = LogLevel.Information, Message = "Rule added to DNS server: {DnsServerId}")]
    private partial void LogRuleAdded(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Clearing all rules for DNS server: {DnsServerId}")]
    private partial void LogClearingRules(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Information, Message = "All rules cleared for DNS server: {DnsServerId}")]
    private partial void LogRulesCleared(string dnsServerId);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
