using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for device operations.
/// </summary>
public partial class DeviceRepository : IDeviceRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DeviceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DeviceRepository(IApiClientFactory apiClientFactory, ILogger<DeviceRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<Device>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAllDevices();

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var devices = await api.ListDevicesAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedDevices(devices.Count);
            return devices;
        }
        catch (ApiException ex)
        {
            LogApiError("GetAll", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "GetAll", $"Failed to fetch devices: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        LogFetchingDevice(id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.GetDeviceAsync(id, cancellationToken).ConfigureAwait(false);

            LogRetrievedDevice(device.Name, device.Id);
            return device;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDeviceNotFound(id);
            throw new EntityNotFoundException("Device", id, ex);
        }
        catch (ApiException ex)
        {
            LogApiError("GetById", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "GetById", $"Failed to fetch device {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(DeviceCreate createModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createModel);

        LogCreatingDevice(createModel.Name);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.CreateDeviceAsync(createModel, cancellationToken).ConfigureAwait(false);

            LogDeviceCreated(device.Name, device.Id);
            return device;
        }
        catch (ApiException ex)
        {
            LogApiError("Create", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "Create", $"Failed to create device: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> UpdateAsync(string id, DeviceUpdate updateModel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(updateModel);

        LogUpdatingDevice(id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            await api.UpdateDeviceAsync(id, updateModel, cancellationToken).ConfigureAwait(false);
            
            // Re-fetch the device after update since the API doesn't return it
            var device = await api.GetDeviceAsync(id, cancellationToken).ConfigureAwait(false);

            LogDeviceUpdated(device.Name, device.Id);
            return device;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDeviceNotFound(id);
            throw new EntityNotFoundException("Device", id, ex);
        }
        catch (ApiException ex)
        {
            LogApiError("Update", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "Update", $"Failed to update device {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        LogDeletingDevice(id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            await api.RemoveDeviceAsync(id, cancellationToken).ConfigureAwait(false);

            LogDeviceDeleted(id);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDeviceNotFound(id);
            throw new EntityNotFoundException("Device", id, ex);
        }
        catch (ApiException ex)
        {
            LogApiError("Delete", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("DeviceRepository", "Delete", $"Failed to delete device {id}: {ex.Message}", ex);
        }
    }
}
