using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for dedicated IP address operations.
/// </summary>
public partial class DedicatedIpRepository : IDedicatedIpRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DedicatedIpRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DedicatedIpRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DedicatedIpRepository(IApiClientFactory apiClientFactory, ILogger<DedicatedIpRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<DedicatedIPv4Address>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingDedicatedIps();

        try
        {
            using var api = _apiClientFactory.CreateDedicatedIpAddressesApi();
            var addresses = await api.ListDedicatedIPv4AddressesAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedDedicatedIps(addresses.Count);
            return addresses;
        }
        catch (ApiException ex)
        {
            LogApiError("GetAll", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DedicatedIpRepository", "GetAll", $"Failed to fetch dedicated IPs: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DedicatedIPv4Address> AllocateAsync(CancellationToken cancellationToken = default)
    {
        LogAllocatingDedicatedIp();

        try
        {
            using var api = _apiClientFactory.CreateDedicatedIpAddressesApi();
            var address = await api.AllocateDedicatedIPv4AddressAsync(cancellationToken).ConfigureAwait(false);

            LogDedicatedIpAllocated(address.Ip);
            return address;
        }
        catch (ApiException ex)
        {
            LogApiError("Allocate", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DedicatedIpRepository", "Allocate", $"Failed to allocate dedicated IP: {ex.Message}", ex);
        }
    }
}
