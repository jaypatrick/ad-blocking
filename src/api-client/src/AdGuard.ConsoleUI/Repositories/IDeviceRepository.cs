using AdGuard.ApiClient.Model;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for device operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IDeviceRepository
{
    /// <summary>
    /// Gets all devices.
    /// </summary>
    Task<List<Device>> GetAllAsync();

    /// <summary>
    /// Gets a device by its ID.
    /// </summary>
    /// <param name="id">The device ID.</param>
    Task<Device> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new device.
    /// </summary>
    /// <param name="deviceCreate">The device creation parameters.</param>
    Task<Device> CreateAsync(DeviceCreate deviceCreate);

    /// <summary>
    /// Deletes a device by its ID.
    /// </summary>
    /// <param name="id">The device ID.</param>
    Task DeleteAsync(string id);
}
