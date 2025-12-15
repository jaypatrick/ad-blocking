using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for DNS server operations.
/// </summary>
public partial class DnsServerRepository : IDnsServerRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DnsServerRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsServerRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DnsServerRepository(IApiClientFactory apiClientFactory, ILogger<DnsServerRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<DNSServer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAllDnsServers();

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedDnsServers(servers.Count);
            return servers;
        }
        catch (ApiException ex)
        {
            LogApiError("GetAll", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "GetAll", $"Failed to fetch DNS servers: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        LogFetchingDnsServer(id);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == id);

            if (server == null)
            {
                LogDnsServerNotFound(id);
                throw new EntityNotFoundException("DnsServer", id);
            }

            LogRetrievedDnsServer(server.Name, server.Id);
            return server;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiError("GetById", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "GetById", $"Failed to fetch DNS server {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> CreateAsync(DNSServerCreate createModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createModel);

        LogCreatingDnsServer(createModel.Name);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var server = await api.CreateDNSServerAsync(createModel, cancellationToken).ConfigureAwait(false);

            LogDnsServerCreated(server.Name, server.Id);
            return server;
        }
        catch (ApiException ex)
        {
            LogApiError("Create", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "Create", $"Failed to create DNS server: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> UpdateAsync(string id, DNSServerSettingsUpdate updateModel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(updateModel);

        LogUpdatingDnsServer(id);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            await api.UpdateDNSServerSettingsAsync(id, updateModel, cancellationToken).ConfigureAwait(false);
            
            // Re-fetch the server after update since the API doesn't return it
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == id);
            
            if (server == null)
            {
                throw new EntityNotFoundException("DnsServer", id);
            }

            LogDnsServerUpdated(server.Name, server.Id);
            return server;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDnsServerNotFound(id);
            throw new EntityNotFoundException("DnsServer", id, ex);
        }
        catch (ApiException ex)
        {
            LogApiError("Update", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "Update", $"Failed to update DNS server {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // Note: AdGuard DNS API doesn't support deleting DNS servers
        // This is implemented to satisfy the interface but will throw
        throw new NotSupportedException("AdGuard DNS API does not support deleting DNS servers.");
    }
}
