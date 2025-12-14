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
    public async Task<UserRulesSettings> GetAsync(string dnsServerId, CancellationToken cancellationToken = default)
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
            LogApiError("GetAsync", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "GetAsync", $"Failed to fetch user rules: {ex.Message}", ex);
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
    public async Task<int> UpdateFromFileAsync(string dnsServerId, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        LogUpdatingRulesFromFile(dnsServerId, filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Rules file not found: {filePath}", filePath);
        }

        try
        {
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken).ConfigureAwait(false);
            var rules = lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            var existingRules = await GetAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: existingRules.Enabled, rules: rules);
            await UpdateAsync(dnsServerId, userRulesUpdate, cancellationToken).ConfigureAwait(false);

            LogRulesUpdatedFromFile(dnsServerId, rules.Count);
            return rules.Count;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (IOException ex)
        {
            LogFileReadError(filePath, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "UpdateFromFile", $"Failed to read rules file: {ex.Message}", ex);
        }
        catch (ApiException ex)
        {
            LogApiError("UpdateFromFile", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "UpdateFromFile", $"Failed to update rules from file: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetEnabledAsync(string dnsServerId, bool enabled, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);

        LogSettingRulesEnabled(dnsServerId, enabled);

        try
        {
            var existingRules = await GetAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: enabled, rules: existingRules.Rules?.ToList());
            await UpdateAsync(dnsServerId, userRulesUpdate, cancellationToken).ConfigureAwait(false);

            LogRulesEnabledSet(dnsServerId, enabled);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiError("SetEnabled", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "SetEnabled", $"Failed to set rules enabled state: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddRuleAsync(string dnsServerId, string rule, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);

        LogAddingRule(dnsServerId, rule);

        await AppendRulesAsync(dnsServerId, [rule], cancellationToken).ConfigureAwait(false);

        LogRuleAdded(dnsServerId);
    }

    /// <inheritdoc />
    public async Task<DNSServer> AppendRulesAsync(string dnsServerId, IEnumerable<string> rulesToAdd, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);
        ArgumentNullException.ThrowIfNull(rulesToAdd);

        var rulesList = rulesToAdd.ToList();
        LogAppendingUserRules(dnsServerId, rulesList.Count);

        // Get existing rules
        var existingRules = await GetAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
        var currentRules = existingRules.Rules?.ToList() ?? [];

        // Append new rules
        currentRules.AddRange(rulesList);

        // Update with combined rules
        var userRulesUpdate = new UserRulesSettingsUpdate(enabled: existingRules.Enabled, rules: currentRules);
        return await UpdateAsync(dnsServerId, userRulesUpdate, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task ClearRulesAsync(string dnsServerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dnsServerId);

        LogClearingRules(dnsServerId);

        try
        {
            var existingRules = await GetAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: existingRules.Enabled, rules: []);
            await UpdateAsync(dnsServerId, userRulesUpdate, cancellationToken).ConfigureAwait(false);

            LogRulesCleared(dnsServerId);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiError("ClearRules", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "ClearRules", $"Failed to clear rules: {ex.Message}", ex);
        }
    }
}
