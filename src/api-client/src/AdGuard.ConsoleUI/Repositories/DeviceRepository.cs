namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for device operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class DeviceRepository : IDeviceRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DeviceRepository> _logger;

    public DeviceRepository(
        IApiClientFactory apiClientFactory,
        ILogger<DeviceRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<Device>> GetAllAsync()
    {
        LogFetchingAllDevices();

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var devices = await api.ListDevicesAsync().ConfigureAwait(false);

            LogRetrievedDevices(devices.Count);
            return devices;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingDevices(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "GetAll",
                $"Failed to fetch devices: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            LogAttemptedNullDeviceId();
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));
        }

        LogFetchingDevice(id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.GetDeviceAsync(id).ConfigureAwait(false);

            LogRetrievedDevice(device.Name, device.Id);
            return device;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDeviceNotFound(id);
            throw new EntityNotFoundException("Device", id);
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingDevice(id, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "GetById",
                $"Failed to fetch device {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(DeviceCreate deviceCreate)
    {
        ArgumentNullException.ThrowIfNull(deviceCreate);

        LogCreatingDevice(deviceCreate.Name);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.CreateDeviceAsync(deviceCreate).ConfigureAwait(false);

            LogDeviceCreated(device.Name, device.Id);
            return device;
        }
        catch (ApiException ex)
        {
            LogApiErrorCreatingDevice(deviceCreate.Name, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "Create",
                $"Failed to create device: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            LogAttemptedNullDeleteDeviceId();
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));
        }

        LogDeletingDevice(id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            await api.RemoveDeviceAsync(id).ConfigureAwait(false);

            LogDeviceDeleted(id);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDeviceNotFoundForDeletion(id);
            throw new EntityNotFoundException("Device", id);
        }
        catch (ApiException ex)
        {
            LogApiErrorDeletingDevice(id, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "Delete",
                $"Failed to delete device {id}: {ex.Message}", ex);
        }
    }
}
