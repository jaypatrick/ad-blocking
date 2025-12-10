using AdGuard.ApiClient.Model;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for account operations.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Gets the account limits.
    /// </summary>
    Task<AccountLimits> GetLimitsAsync();
}
