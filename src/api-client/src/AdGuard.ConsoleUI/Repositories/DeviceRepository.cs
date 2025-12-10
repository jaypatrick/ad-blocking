using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for device operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<DeviceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public DeviceRepository(IApiClientFactory apiClientFactory, ILogger<DeviceRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("DeviceRepository initialized");
    }

    /// <inheritdoc />
    public async Task<List<Device>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all devices");

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var devices = await api.ListDevicesAsync();

            _logger.LogInformation("Retrieved {Count} devices", devices.Count);
            return devices;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching devices: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("DeviceRepository", "GetAll",
                $"Failed to fetch devices: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to get device with null or empty ID");
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));
        }

        _logger.LogDebug("Fetching device with ID: {DeviceId}", id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.GetDeviceAsync(id);

            _logger.LogInformation("Retrieved device: {DeviceName} (ID: {DeviceId})", device.Name, device.Id);
            return device;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            _logger.LogWarning("Device not found: {DeviceId}", id);
            throw new EntityNotFoundException("Device", id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching device {DeviceId}: {ErrorCode} - {Message}",
                id, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DeviceRepository", "GetById",
                $"Failed to fetch device {id}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(DeviceCreate deviceCreate)
    {
        ArgumentNullException.ThrowIfNull(deviceCreate);

        _logger.LogDebug("Creating device: {DeviceName}", deviceCreate.Name);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            var device = await api.CreateDeviceAsync(deviceCreate);

            _logger.LogInformation("Created device: {DeviceName} (ID: {DeviceId})", device.Name, device.Id);
            return device;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while creating device {DeviceName}: {ErrorCode} - {Message}",
                deviceCreate.Name, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DeviceRepository", "Create",
                $"Failed to create device: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("Attempted to delete device with null or empty ID");
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));
        }

        _logger.LogDebug("Deleting device with ID: {DeviceId}", id);

        try
        {
            using var api = _apiClientFactory.CreateDevicesApi();
            await api.RemoveDeviceAsync(id);

            _logger.LogInformation("Deleted device: {DeviceId}", id);
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            _logger.LogWarning("Device not found for deletion: {DeviceId}", id);
            throw new EntityNotFoundException("Device", id);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while deleting device {DeviceId}: {ErrorCode} - {Message}",
                id, ex.ErrorCode, ex.Message);
            throw new RepositoryException("DeviceRepository", "Delete",
                $"Failed to delete device {id}: {ex.Message}", ex);
        }
    }
}
