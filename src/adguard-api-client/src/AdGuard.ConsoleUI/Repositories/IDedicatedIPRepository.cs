namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for dedicated IP address operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IDedicatedIPRepository
{
    /// <summary>
    /// Gets all allocated dedicated IPv4 addresses.
    /// </summary>
    Task<List<DedicatedIPv4Address>> GetAllAsync();

    /// <summary>
    /// Allocates a new dedicated IPv4 address.
    /// </summary>
    Task<DedicatedIPv4Address> AllocateAsync();
}
