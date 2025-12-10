using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for device operations.
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public DeviceRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<List<Device>> GetAllAsync()
    {
        using var api = _apiClientFactory.CreateDevicesApi();
        return await api.ListDevicesAsync();
    }

    /// <inheritdoc />
    public async Task<Device> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));

        using var api = _apiClientFactory.CreateDevicesApi();
        return await api.GetDeviceAsync(id);
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(DeviceCreate deviceCreate)
    {
        ArgumentNullException.ThrowIfNull(deviceCreate);

        using var api = _apiClientFactory.CreateDevicesApi();
        return await api.CreateDeviceAsync(deviceCreate);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));

        using var api = _apiClientFactory.CreateDevicesApi();
        await api.RemoveDeviceAsync(id);
    }
}
