namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for dedicated IP address operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class DedicatedIPRepository : IDedicatedIPRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DedicatedIPRepository> _logger;

    public DedicatedIPRepository(
        IApiClientFactory apiClientFactory,
        ILogger<DedicatedIPRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<DedicatedIPv4Address>> GetAllAsync()
    {
        LogFetchingAllDedicatedIPs();

        try
        {
            using var api = _apiClientFactory.CreateDedicatedIpAddressesApi();
            var addresses = await api.ListDedicatedIPv4AddressesAsync().ConfigureAwait(false);

            LogRetrievedDedicatedIPs(addresses.Count);
            return addresses;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingDedicatedIPs(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DedicatedIPRepository", "GetAll",
                $"Failed to fetch dedicated IP addresses: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DedicatedIPv4Address> AllocateAsync()
    {
        LogAllocatingDedicatedIP();

        try
        {
            using var api = _apiClientFactory.CreateDedicatedIpAddressesApi();
            var address = await api.AllocateDedicatedIPv4AddressAsync().ConfigureAwait(false);

            LogAllocatedDedicatedIP(address.Ip);
            return address;
        }
        catch (ApiException ex)
        {
            LogApiErrorAllocatingDedicatedIP(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DedicatedIPRepository", "Allocate",
                $"Failed to allocate dedicated IP address: {ex.Message}", ex);
        }
    }
}
