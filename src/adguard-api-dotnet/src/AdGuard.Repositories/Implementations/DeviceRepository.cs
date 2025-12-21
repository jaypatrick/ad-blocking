namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for device operations.
/// </summary>
public partial class DeviceRepository : BaseRepository<DeviceRepository>, IDeviceRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "DeviceRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<DeviceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public DeviceRepository(IApiClientFactory apiClientFactory, ILogger<DeviceRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Device>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAllDevices();

        var devices = await ExecuteAsync("GetAll", async () =>
        {
            using var api = ApiClientFactory.CreateDevicesApi();
            return await api.ListDevicesAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetAll", code, message, ex), cancellationToken);

        LogRetrievedDevices(devices.Count);
        return devices;
    }

    /// <inheritdoc />
    public async Task<Device> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateId(id);
        LogFetchingDevice(id);

        var device = await ExecuteWithEntityCheckAsync("GetById", "Device", id, async () =>
        {
            using var api = ApiClientFactory.CreateDevicesApi();
            return await api.GetDeviceAsync(id, cancellationToken).ConfigureAwait(false);
        }, deviceId => LogDeviceNotFound(deviceId), (code, message, ex) => LogApiError("GetById", code, message, ex), cancellationToken);

        LogRetrievedDevice(device.Name, device.Id);
        return device;
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(DeviceCreate createModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createModel);
        LogCreatingDevice(createModel.Name);

        var device = await ExecuteAsync("Create", async () =>
        {
            using var api = ApiClientFactory.CreateDevicesApi();
            return await api.CreateDeviceAsync(createModel, cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("Create", code, message, ex), cancellationToken);

        LogDeviceCreated(device.Name, device.Id);
        return device;
    }

    /// <inheritdoc />
    public async Task<Device> UpdateAsync(string id, DeviceUpdate updateModel, CancellationToken cancellationToken = default)
    {
        ValidateId(id);
        ArgumentNullException.ThrowIfNull(updateModel);
        LogUpdatingDevice(id);

        var device = await ExecuteWithEntityCheckAsync("Update", "Device", id, async () =>
        {
            using var api = ApiClientFactory.CreateDevicesApi();
            await api.UpdateDeviceAsync(id, updateModel, cancellationToken).ConfigureAwait(false);
            
            // Re-fetch the device after update since the API doesn't return it
            return await api.GetDeviceAsync(id, cancellationToken).ConfigureAwait(false);
        }, deviceId => LogDeviceNotFound(deviceId), (code, message, ex) => LogApiError("Update", code, message, ex), cancellationToken);

        LogDeviceUpdated(device.Name, device.Id);
        return device;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateId(id);
        LogDeletingDevice(id);

        await ExecuteWithEntityCheckAsync("Delete", "Device", id, async () =>
        {
            using var api = ApiClientFactory.CreateDevicesApi();
            await api.RemoveDeviceAsync(id, cancellationToken).ConfigureAwait(false);
        }, deviceId => LogDeviceNotFound(deviceId), (code, message, ex) => LogApiError("Delete", code, message, ex), cancellationToken);

        LogDeviceDeleted(id);
    }
}
