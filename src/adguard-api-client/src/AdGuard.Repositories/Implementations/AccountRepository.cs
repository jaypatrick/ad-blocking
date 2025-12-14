using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for account operations.
/// </summary>
public partial class AccountRepository : IAccountRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<AccountRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public AccountRepository(IApiClientFactory apiClientFactory, ILogger<AccountRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<AccountLimits> GetLimitsAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAccountLimits();

        try
        {
            using var api = _apiClientFactory.CreateAccountApi();
            var limits = await api.GetAccountLimitsAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedAccountLimits();
            return limits;
        }
        catch (ApiException ex)
        {
            LogApiError("GetLimits", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("AccountRepository", "GetLimits", $"Failed to fetch account limits: {ex.Message}", ex);
        }
    }
}
