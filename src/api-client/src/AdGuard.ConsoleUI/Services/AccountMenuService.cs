using AdGuard.ApiClient.Client;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Display;
using AdGuard.ConsoleUI.Helpers;
using AdGuard.ConsoleUI.Repositories;

namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for viewing account information.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class AccountMenuService : IMenuService
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountLimitsDisplayStrategy _displayStrategy;

    public AccountMenuService(
        IAccountRepository accountRepository,
        AccountLimitsDisplayStrategy displayStrategy)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public string Title => "Account Info";

    /// <inheritdoc />
    public async Task ShowAsync()
    {
        try
        {
            var limits = await ConsoleHelpers.WithStatusAsync(
                "Fetching account information...",
                () => _accountRepository.GetLimitsAsync());

            _displayStrategy.Display(limits);
        }
        catch (ApiException ex)
        {
            ConsoleHelpers.ShowApiError(ex);
        }
    }
}
