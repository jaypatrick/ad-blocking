using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for dedicated IP address operations.
/// </summary>
public partial class DedicatedIpRepository : BaseRepository<DedicatedIpRepository>, IDedicatedIpRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "DedicatedIpRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<DedicatedIpRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DedicatedIpRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DedicatedIpRepository(IApiClientFactory apiClientFactory, ILogger<DedicatedIpRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<DedicatedIPv4Address>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingDedicatedIps();

        var addresses = await ExecuteAsync("GetAll", async () =>
        {
            using var api = ApiClientFactory.CreateDedicatedIpAddressesApi();
            return await api.ListDedicatedIPv4AddressesAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetAll", code, message, ex), cancellationToken);

        LogRetrievedDedicatedIps(addresses.Count);
        return addresses;
    }

    /// <inheritdoc />
    public async Task<DedicatedIPv4Address> AllocateAsync(CancellationToken cancellationToken = default)
    {
        LogAllocatingDedicatedIp();

        var address = await ExecuteAsync("Allocate", async () =>
        {
            using var api = ApiClientFactory.CreateDedicatedIpAddressesApi();
            return await api.AllocateDedicatedIPv4AddressAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("Allocate", code, message, ex), cancellationToken);

        LogDedicatedIpAllocated(address.Ip);
        return address;
    }
}
