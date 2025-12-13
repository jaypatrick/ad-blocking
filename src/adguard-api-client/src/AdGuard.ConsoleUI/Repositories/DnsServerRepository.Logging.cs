using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Source-generated logging for DnsServerRepository.
/// Uses LoggerMessage.Define for high-performance logging.
/// </summary>
public partial class DnsServerRepository
{
    // Event ID Range: 2001-2099 (DNS Server operations)

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Debug,
        Message = "Fetching all DNS servers")]
    partial void LogFetchingAllServers();

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Retrieved {Count} DNS servers")]
    partial void LogRetrievedServers(int count);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Debug,
        Message = "Fetching DNS server with ID: {ServerId}")]
    partial void LogFetchingServer(string serverId);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Information,
        Message = "Retrieved DNS server: {ServerName} (ID: {ServerId})")]
    partial void LogRetrievedServer(string serverName, string serverId);

    [LoggerMessage(
        EventId = 2005,
        Level = LogLevel.Warning,
        Message = "DNS server not found: {ServerId}")]
    partial void LogServerNotFound(string serverId);

    [LoggerMessage(
        EventId = 2006,
        Level = LogLevel.Warning,
        Message = "Attempted to get DNS server with null or empty ID")]
    partial void LogAttemptedNullServerId();

    [LoggerMessage(
        EventId = 2007,
        Level = LogLevel.Error,
        Message = "API error while fetching DNS servers: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingServers(int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2008,
        Level = LogLevel.Error,
        Message = "API error while fetching DNS server {ServerId}: {ErrorCode} - {Message}")]
    partial void LogApiErrorFetchingServer(string serverId, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2009,
        Level = LogLevel.Debug,
        Message = "Creating DNS server: {ServerName}")]
    partial void LogCreatingServer(string serverName);

    [LoggerMessage(
        EventId = 2010,
        Level = LogLevel.Information,
        Message = "Created DNS server: {ServerName} (ID: {ServerId})")]
    partial void LogServerCreated(string serverName, string serverId);

    [LoggerMessage(
        EventId = 2011,
        Level = LogLevel.Error,
        Message = "API error while creating DNS server {ServerName}: {ErrorCode} - {Message}")]
    partial void LogApiErrorCreatingServer(string serverName, int errorCode, string message, Exception ex);

    [LoggerMessage(
        EventId = 2012,
        Level = LogLevel.Warning,
        Message = "DeleteAsync called but DNS server deletion is not supported by the AdGuard DNS API")]
    partial void LogDeleteNotSupported();
}
