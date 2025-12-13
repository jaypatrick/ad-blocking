namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for DNS server operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class DnsServerRepository : IDnsServerRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DnsServerRepository> _logger;

    public DnsServerRepository(
        IApiClientFactory apiClientFactory,
        ILogger<DnsServerRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<DNSServer>> GetAllAsync()
    {
        LogFetchingAllServers();

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync().ConfigureAwait(false);

            LogRetrievedServers(servers.Count);
            return servers;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingServers(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "GetAll",
                $"Failed to fetch DNS servers: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(id));
        }

        LogFetchingServer(id);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync().ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == id);

            if (server == null)
            {
                LogServerNotFound(id);
                throw new EntityNotFoundException("DNSServer", id);
            }

            LogRetrievedServer(server.Name, server.Id);
            return server;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingServer(id, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "GetById",
                $"Failed to fetch DNS server {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> CreateAsync(DNSServerCreate serverCreate)
    {
        ArgumentNullException.ThrowIfNull(serverCreate);

        LogCreatingServer(serverCreate.Name);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var server = await api.CreateDNSServerAsync(serverCreate).ConfigureAwait(false);

            LogServerCreated(server.Name, server.Id);
            return server;
        }
        catch (ApiException ex)
        {
            LogApiErrorCreatingServer(serverCreate.Name, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DnsServerRepository", "Create",
                $"Failed to create DNS server: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// DNS server deletion is not supported by the AdGuard DNS API v1.11.
    /// This method will always throw <see cref="NotSupportedException"/>.
    /// </remarks>
    public Task DeleteAsync(string id)
    {
        LogDeleteNotSupported();
        throw new NotSupportedException("DNS server deletion is not supported by the AdGuard DNS API. DNS servers can only be created and listed.");
    }
}
