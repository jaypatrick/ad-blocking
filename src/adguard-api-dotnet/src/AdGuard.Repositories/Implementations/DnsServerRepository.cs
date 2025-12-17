using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for DNS server operations.
/// </summary>
public partial class DnsServerRepository : BaseRepository<DnsServerRepository>, IDnsServerRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "DnsServerRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<DnsServerRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DnsServerRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DnsServerRepository(IApiClientFactory apiClientFactory, ILogger<DnsServerRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<DNSServer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAllDnsServers();

        var servers = await ExecuteAsync("GetAll", async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            return await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetAll", code, message, ex), cancellationToken);

        LogRetrievedDnsServers(servers.Count);
        return servers;
    }

    /// <inheritdoc />
    public async Task<DNSServer> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateId(id);
        LogFetchingDnsServer(id);

        DNSServer? server = null;
        await ExecuteAsync("GetById", async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            server = servers.FirstOrDefault(s => s.Id == id);
            
            if (server == null)
            {
                LogDnsServerNotFound(id);
                throw new EntityNotFoundException("DnsServer", id);
            }
        }, (code, message, ex) => LogApiError("GetById", code, message, ex), cancellationToken);

        LogRetrievedDnsServer(server!.Name, server.Id);
        return server;
    }

    /// <inheritdoc />
    public async Task<DNSServer> CreateAsync(DNSServerCreate createModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createModel);
        LogCreatingDnsServer(createModel.Name);

        var server = await ExecuteAsync("Create", async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            return await api.CreateDNSServerAsync(createModel, cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("Create", code, message, ex), cancellationToken);

        LogDnsServerCreated(server.Name, server.Id);
        return server;
    }

    /// <inheritdoc />
    public async Task<DNSServer> UpdateAsync(string id, DNSServerSettingsUpdate updateModel, CancellationToken cancellationToken = default)
    {
        ValidateId(id);
        ArgumentNullException.ThrowIfNull(updateModel);
        LogUpdatingDnsServer(id);

        var server = await ExecuteWithEntityCheckAsync("Update", "DnsServer", id, async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            await api.UpdateDNSServerSettingsAsync(id, updateModel, cancellationToken).ConfigureAwait(false);
            
            // Re-fetch the server after update since the API doesn't return it
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == id);
            
            if (server == null)
            {
                throw new EntityNotFoundException("DnsServer", id);
            }

            return server;
        }, serverId => LogDnsServerNotFound(serverId), (code, message, ex) => LogApiError("Update", code, message, ex), cancellationToken);

        LogDnsServerUpdated(server.Name, server.Id);
        return server;
    }

    /// <inheritdoc />
    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // Note: AdGuard DNS API doesn't support deleting DNS servers
        // This is implemented to satisfy the interface but will throw
        throw new NotSupportedException("AdGuard DNS API does not support deleting DNS servers.");
    }
}
