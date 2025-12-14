using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for UserRulesRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class UserRulesRepository
{
    // Event ID Range: 2100-2199 (User Rules operations)

    [LoggerMessage(
        EventId = 2100,
        Level = LogLevel.Warning,
        Message = "Attempted to access user rules with null or empty DNS server ID")]
    partial void LogAttemptedNullServerId();

    [LoggerMessage(
        EventId = 2101,
        Level = LogLevel.Debug,
        Message = "Fetching user rules for DNS server: {DnsServerId}")]
    partial void LogFetchingUserRules(string dnsServerId);

    [LoggerMessage(
        EventId = 2102,
        Level = LogLevel.Information,
        Message = "Retrieved {RulesCount} user rules for DNS server: {DnsServerId}")]
    partial void LogRetrievedUserRules(string dnsServerId, int rulesCount);

    [LoggerMessage(
        EventId = 2103,
        Level = LogLevel.Error,
        Message = "API error while fetching user rules for DNS server {DnsServerId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingUserRules(string dnsServerId, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2104,
        Level = LogLevel.Debug,
        Message = "Updating user rules for DNS server: {DnsServerId} with {RulesCount} rules")]
    partial void LogUpdatingUserRules(string dnsServerId, int rulesCount);

    [LoggerMessage(
        EventId = 2105,
        Level = LogLevel.Information,
        Message = "Updated user rules for DNS server: {DnsServerId} with {RulesCount} rules")]
    partial void LogUserRulesUpdated(string dnsServerId, int rulesCount);

    [LoggerMessage(
        EventId = 2106,
        Level = LogLevel.Error,
        Message = "API error while updating user rules for DNS server {DnsServerId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorUpdatingUserRules(string dnsServerId, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2107,
        Level = LogLevel.Warning,
        Message = "Rules file not found: {FilePath}")]
    partial void LogFileNotFound(string filePath);

    [LoggerMessage(
        EventId = 2108,
        Level = LogLevel.Debug,
        Message = "Loading rules from file for DNS server {DnsServerId}: {FilePath}")]
    partial void LogLoadingRulesFromFile(string dnsServerId, string filePath);

    [LoggerMessage(
        EventId = 2109,
        Level = LogLevel.Debug,
        Message = "Parsed {RulesCount} rules from file {FilePath} (total lines: {TotalLines})")]
    partial void LogParsedRulesFromFile(string filePath, int rulesCount, int totalLines);

    [LoggerMessage(
        EventId = 2110,
        Level = LogLevel.Information,
        Message = "Uploaded {RulesCount} rules from file to DNS server: {DnsServerId}")]
    partial void LogRulesUploadedFromFile(string dnsServerId, int rulesCount);

    [LoggerMessage(
        EventId = 2111,
        Level = LogLevel.Error,
        Message = "Error reading rules file {FilePath}: {Message}")]
    partial void LogFileReadError(string filePath, string message, Exception ex);

    [LoggerMessage(
        EventId = 2112,
        Level = LogLevel.Debug,
        Message = "Setting rules enabled state for DNS server {DnsServerId} to: {Enabled}")]
    partial void LogSettingRulesEnabled(string dnsServerId, bool enabled);

    [LoggerMessage(
        EventId = 2113,
        Level = LogLevel.Information,
        Message = "Set rules enabled state for DNS server {DnsServerId} to: {Enabled}")]
    partial void LogRulesEnabledSet(string dnsServerId, bool enabled);

    [LoggerMessage(
        EventId = 2114,
        Level = LogLevel.Error,
        Message = "API error while setting rules enabled state for DNS server {DnsServerId} to {Enabled}: {ErrorCode} - {Message}")]
    partial void LogApiErrorSettingEnabled(string dnsServerId, bool enabled, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2115,
        Level = LogLevel.Debug,
        Message = "Adding rule to DNS server {DnsServerId}: {Rule}")]
    partial void LogAddingRule(string dnsServerId, string rule);

    [LoggerMessage(
        EventId = 2116,
        Level = LogLevel.Information,
        Message = "Added rule to DNS server {DnsServerId}: {Rule}")]
    partial void LogRuleAdded(string dnsServerId, string rule);

    [LoggerMessage(
        EventId = 2117,
        Level = LogLevel.Debug,
        Message = "Rule already exists for DNS server {DnsServerId}: {Rule}")]
    partial void LogRuleAlreadyExists(string dnsServerId, string rule);

    [LoggerMessage(
        EventId = 2118,
        Level = LogLevel.Error,
        Message = "API error while adding rule to DNS server {DnsServerId} ({Rule}): {ErrorCode} - {Message}")]
    partial void LogApiErrorAddingRule(string dnsServerId, string rule, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2119,
        Level = LogLevel.Debug,
        Message = "Clearing all rules for DNS server: {DnsServerId}")]
    partial void LogClearingRules(string dnsServerId);

    [LoggerMessage(
        EventId = 2120,
        Level = LogLevel.Information,
        Message = "Cleared all rules for DNS server: {DnsServerId}")]
    partial void LogRulesCleared(string dnsServerId);

    [LoggerMessage(
        EventId = 2121,
        Level = LogLevel.Error,
        Message = "API error while clearing rules for DNS server {DnsServerId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorClearingRules(string dnsServerId, int errorCode, string message, Exception ex);
}
