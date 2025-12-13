namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for account operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class AccountRepository : IAccountRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(
        IApiClientFactory apiClientFactory,
        ILogger<AccountRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<AccountLimits> GetLimitsAsync()
    {
        LogFetchingAccountLimits();

        try
        {
            using var api = _apiClientFactory.CreateAccountApi();
            var limits = await api.GetAccountLimitsAsync().ConfigureAwait(false);

            LogRetrievedAccountLimits();
            return limits;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingAccountLimits(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("AccountRepository", "GetAccountLimits",
                $"Failed to fetch account limits: {ex.Message}", ex);
        }
    }
}

