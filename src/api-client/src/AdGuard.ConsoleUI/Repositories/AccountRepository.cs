using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for account operations.
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public AccountRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<AccountLimits> GetLimitsAsync()
    {
        using var api = _apiClientFactory.CreateAccountApi();
        return await api.GetAccountLimitsAsync();
    }
}
