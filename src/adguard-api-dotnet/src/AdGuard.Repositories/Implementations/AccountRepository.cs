namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for account operations.
/// </summary>
public partial class AccountRepository : BaseRepository<AccountRepository>, IAccountRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "AccountRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<AccountRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public AccountRepository(IApiClientFactory apiClientFactory, ILogger<AccountRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AccountLimits> GetLimitsAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingAccountLimits();

        var limits = await ExecuteAsync("GetLimits", async () =>
        {
            using var api = ApiClientFactory.CreateAccountApi();
            return await api.GetAccountLimitsAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetLimits", code, message, ex), cancellationToken);

        LogRetrievedAccountLimits();
        return limits;
    }
}

