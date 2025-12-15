namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Logging methods for <see cref="DnsServerRepository"/>.
/// </summary>
public partial class DnsServerRepository
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching all DNS servers")]
    private partial void LogFetchingAllDnsServers();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} DNS servers")]
    private partial void LogRetrievedDnsServers(int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Fetching DNS server with ID: {Id}")]
    private partial void LogFetchingDnsServer(string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved DNS server: {Name} ({Id})")]
    private partial void LogRetrievedDnsServer(string name, string id);

    [LoggerMessage(Level = LogLevel.Warning, Message = "DNS server not found with ID: {Id}")]
    private partial void LogDnsServerNotFound(string id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating DNS server: {Name}")]
    private partial void LogCreatingDnsServer(string name);

    [LoggerMessage(Level = LogLevel.Information, Message = "DNS server created: {Name} ({Id})")]
    private partial void LogDnsServerCreated(string name, string id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating DNS server with ID: {Id}")]
    private partial void LogUpdatingDnsServer(string id);

    [LoggerMessage(Level = LogLevel.Information, Message = "DNS server updated: {Name} ({Id})")]
    private partial void LogDnsServerUpdated(string name, string id);

    [LoggerMessage(Level = LogLevel.Error, Message = "API error in {Operation}: {ErrorCode} - {ErrorMessage}")]
    private partial void LogApiError(string operation, int errorCode, string errorMessage, Exception ex);
}
