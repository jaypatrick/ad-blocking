using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for user rules operations on DNS servers.
/// </summary>
public partial class UserRulesRepository : IUserRulesRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<UserRulesRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRulesRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public UserRulesRepository(IApiClientFactory apiClientFactory, ILogger<UserRulesRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<UserRulesSettings> GetByDnsServerIdAsync(string dnsServerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);

        LogFetchingUserRules(dnsServerId);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == dnsServerId);

            if (server == null)
            {
                LogDnsServerNotFound(dnsServerId);
                throw new EntityNotFoundException("DnsServer", dnsServerId);
            }

            var userRules = server.Settings?.UserRules ?? new UserRulesSettings();
            LogRetrievedUserRules(dnsServerId, userRules.Rules?.Count ?? 0);
            return userRules;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiError("GetByDnsServerId", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "GetByDnsServerId", $"Failed to fetch user rules: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> UpdateAsync(string dnsServerId, UserRulesSettingsUpdate userRules, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);
        ArgumentNullException.ThrowIfNull(userRules);

        LogUpdatingUserRules(dnsServerId);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var settingsUpdate = new DNSServerSettingsUpdate(userRules: userRules);
            var server = await api.UpdateDNSServerSettingsAsync(dnsServerId, settingsUpdate, cancellationToken).ConfigureAwait(false);

            LogUserRulesUpdated(dnsServerId);
            return server;
        }
        catch (ApiException ex) when (ex.ErrorCode == 404)
        {
            LogDnsServerNotFound(dnsServerId);
            throw new EntityNotFoundException("DnsServer", dnsServerId, ex);
        }
        catch (ApiException ex)
        {
            LogApiError("Update", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "Update", $"Failed to update user rules: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<DNSServer> AppendRulesAsync(string dnsServerId, IEnumerable<string> rulesToAdd, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);
        ArgumentNullException.ThrowIfNull(rulesToAdd);

        var rulesList = rulesToAdd.ToList();
        LogAppendingUserRules(dnsServerId, rulesList.Count);

        // Get existing rules
        var existingRules = await GetByDnsServerIdAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
        var currentRules = existingRules.Rules?.ToList() ?? [];

        // Append new rules
        currentRules.AddRange(rulesList);

        // Update with combined rules
        var userRulesUpdate = new UserRulesSettingsUpdate(enabled: existingRules.Enabled, rules: currentRules);
        return await UpdateAsync(dnsServerId, userRulesUpdate, cancellationToken).ConfigureAwait(false);
    }
}
