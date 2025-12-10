using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for account operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<AccountRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public AccountRepository(IApiClientFactory apiClientFactory, ILogger<AccountRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("AccountRepository initialized");
    }

    /// <inheritdoc />
    public async Task<AccountLimits> GetLimitsAsync()
    {
        _logger.LogDebug("Fetching account limits");

        try
        {
            using var api = _apiClientFactory.CreateAccountApi();
            var limits = await api.GetAccountLimitsAsync();

            _logger.LogInformation("Retrieved account limits successfully");
            _logger.LogDebug("Devices: {Used}/{Limit}, DNS Servers: {DnsUsed}/{DnsLimit}",
                limits.Devices?.Used, limits.Devices?.VarLimit,
                limits.DnsServers?.Used, limits.DnsServers?.VarLimit);

            return limits;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching account limits: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("AccountRepository", "GetLimits",
                $"Failed to fetch account limits: {ex.Message}", ex);
        }
    }
}
