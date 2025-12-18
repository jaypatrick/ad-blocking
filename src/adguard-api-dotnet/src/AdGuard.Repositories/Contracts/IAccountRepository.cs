using AdGuard.Repositories.Abstractions;

namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for account operations.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Gets the account limits.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The account limits.</returns>
    Task<AccountLimits> GetLimitsAsync(CancellationToken cancellationToken = default);
}
