namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for DNS server operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IDnsServerRepository
{
    /// <summary>
    /// Gets all DNS servers.
    /// </summary>
    Task<List<DNSServer>> GetAllAsync();

    /// <summary>
    /// Gets a DNS server by its ID.
    /// </summary>
    /// <param name="id">The DNS server ID.</param>
    Task<DNSServer> GetByIdAsync(string id);

    /// <summary>
    /// Creates a new DNS server.
    /// </summary>
    /// <param name="serverCreate">The DNS server creation parameters.</param>
    Task<DNSServer> CreateAsync(DNSServerCreate serverCreate);

    /// <summary>
    /// Deletes a DNS server by its ID.
    /// </summary>
    /// <param name="id">The DNS server ID.</param>
    Task DeleteAsync(string id);
}
