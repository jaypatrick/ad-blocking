using AdGuard.Repositories.Abstractions;

namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for device operations.
/// </summary>
public interface IDeviceRepository : IRepository<Device, string, DeviceCreate, DeviceUpdate>
{
}
