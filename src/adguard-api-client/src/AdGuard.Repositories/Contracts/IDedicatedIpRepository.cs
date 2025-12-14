using AdGuard.Repositories.Abstractions;

namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for dedicated IP address operations.
/// </summary>
public interface IDedicatedIpRepository : IReadOnlyRepository<DedicatedIPv4Address>
{
    /// <summary>
    /// Allocates a new dedicated IPv4 address.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The allocated IPv4 address.</returns>
    Task<DedicatedIPv4Address> AllocateAsync(CancellationToken cancellationToken = default);
}
