using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for DNS server operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class DnsServerRepository : IDnsServerRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DnsServerRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsServerRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public DnsServerRepository(IApiClientFactory apiClientFactory, ILogger<DnsServerRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("DnsServerRepository initialized");
    }

    /// <inheritdoc />
    public async Task<List<DNSServer>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all DNS servers");

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync();

            _logger.LogInformation("Retrieved {Count} DNS servers", servers.Count);
            return servers;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching DNS servers: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("DnsServerRepository", "GetAll",
                $"Failed to fetch DNS servers: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to get DNS server with null or empty ID");
            throw new ArgumentException("DNS Server ID cannot be null or empty.", nameof(id));
        }

        _logger.LogDebug("Fetching DNS server with ID: {ServerId}", id);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync();
            var server = servers.FirstOrDefault(s => s.Id == id);

            if (server == null)
            {
                _logger.LogWarning("DNS server not found: {ServerId}", id);
                throw new EntityNotFoundException("DNSServer", id);
            }

            _logger.LogInformation("Retrieved DNS server: {ServerName} (ID: {ServerId})", server.Name, server.Id);
            return server;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching DNS server {ServerId}: {ErrorCode} - {Message}",
                id, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DnsServerRepository", "GetById",
                $"Failed to fetch DNS server {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> CreateAsync(DNSServerCreate serverCreate)
    {
        ArgumentNullException.ThrowIfNull(serverCreate);

        _logger.LogDebug("Creating DNS server: {ServerName}", serverCreate.Name);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var server = await api.CreateDNSServerAsync(serverCreate);

            _logger.LogInformation("Created DNS server: {ServerName} (ID: {ServerId})", server.Name, server.Id);
            return server;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while creating DNS server {ServerName}: {ErrorCode} - {Message}",
                serverCreate.Name, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DnsServerRepository", "Create",
                $"Failed to create DNS server: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to delete DNS server with null or empty ID");
            throw new ArgumentException("DNS Server ID cannot be null or empty.", nameof(id));
        }

        _logger.LogDebug("Deleting DNS server with ID: {ServerId}", id);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            await api.RemoveDNSServerAsync(id);

            _logger.LogInformation("Deleted DNS server: {ServerId}", id);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            _logger.LogWarning("DNS server not found for deletion: {ServerId}", id);
            throw new EntityNotFoundException("DNSServer", id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while deleting DNS server {ServerId}: {ErrorCode} - {Message}",
                id, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DnsServerRepository", "Delete",
                $"Failed to delete DNS server {id}: {ex.Message}", ex);
        }
    }
}
