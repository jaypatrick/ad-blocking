namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for user rules operations on DNS servers.
/// </summary>
public partial class UserRulesRepository : BaseRepository<UserRulesRepository>, IUserRulesRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "UserRulesRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<UserRulesRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRulesRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public UserRulesRepository(IApiClientFactory apiClientFactory, ILogger<UserRulesRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserRulesSettings> GetAsync(string dnsServerId, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
        LogFetchingUserRules(dnsServerId);

        UserRulesSettings? userRules = null;
        await ExecuteAsync("GetAsync", async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == dnsServerId);

            if (server == null)
            {
                LogDnsServerNotFound(dnsServerId);
                throw new EntityNotFoundException("DnsServer", dnsServerId);
            }

            // Settings is typed as Object in the API, so we need to deserialize it
            if (server.Settings != null)
            {
                try
                {
                    var settingsJson = System.Text.Json.JsonSerializer.Serialize(server.Settings);
                    var settings = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(settingsJson);
                    
                    if (settings.TryGetProperty("user_rules_settings", out var userRulesElement))
                    {
                        userRules = System.Text.Json.JsonSerializer.Deserialize<UserRulesSettings>(userRulesElement.GetRawText()) 
                            ?? new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
                    }
                    else
                    {
                        userRules = new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
                    }
                }
                catch
                {
                    userRules = new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
                }
            }
            else
            {
                userRules = new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
            }
        }, (code, message, ex) => LogApiError("GetAsync", code, message, ex), cancellationToken);

        LogRetrievedUserRules(dnsServerId, userRules!.Rules?.Count ?? 0);
        return userRules;
    }

    /// <inheritdoc />
    public async Task<DNSServer> UpdateAsync(string dnsServerId, UserRulesSettingsUpdate userRules, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
        ArgumentNullException.ThrowIfNull(userRules);
        LogUpdatingUserRules(dnsServerId);

        var server = await ExecuteWithEntityCheckAsync("Update", "DnsServer", dnsServerId, async () =>
        {
            using var api = ApiClientFactory.CreateDnsServersApi();
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: userRules);
            await api.UpdateDNSServerSettingsAsync(dnsServerId, settingsUpdate, cancellationToken).ConfigureAwait(false);
            
            // Re-fetch the server after update since the API doesn't return it
            var servers = await api.ListDNSServersAsync(cancellationToken).ConfigureAwait(false);
            var server = servers.FirstOrDefault(s => s.Id == dnsServerId);
            
            if (server == null)
            {
                throw new EntityNotFoundException("DnsServer", dnsServerId);
            }

            return server;
        }, serverId => LogDnsServerNotFound(serverId), (code, message, ex) => LogApiError("Update", code, message, ex), cancellationToken);

        LogUserRulesUpdated(dnsServerId);
        return server;
    }

    /// <inheritdoc />
    public async Task<int> UpdateFromFileAsync(string dnsServerId, string filePath, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));
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
            throw new RepositoryException(RepositoryName, "UpdateFromFile", $"Failed to read rules file: {ex.Message}", ex);
        }
        catch (ApiException ex)
        {
            LogApiError("UpdateFromFile", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException(RepositoryName, "UpdateFromFile", $"Failed to update rules from file: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetEnabledAsync(string dnsServerId, bool enabled, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
        LogSettingRulesEnabled(dnsServerId, enabled);

        try
        {
            var existingRules = await GetAsync(dnsServerId, cancellationToken).ConfigureAwait(false);
            var userRulesUpdate = new UserRulesSettingsUpdate(enabled: enabled, rules: existingRules.Rules?.ToList() ?? new List<string>());
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
            throw new RepositoryException(RepositoryName, "SetEnabled", $"Failed to set rules enabled state: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddRuleAsync(string dnsServerId, string rule, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
        ArgumentException.ThrowIfNullOrWhiteSpace(rule, nameof(rule));
        LogAddingRule(dnsServerId, rule);

        await AppendRulesAsync(dnsServerId, [rule], cancellationToken).ConfigureAwait(false);

        LogRuleAdded(dnsServerId);
    }

    /// <inheritdoc />
    public async Task<DNSServer> AppendRulesAsync(string dnsServerId, IEnumerable<string> rulesToAdd, CancellationToken cancellationToken = default)
    {
        ValidateId(dnsServerId, nameof(dnsServerId));
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
        ValidateId(dnsServerId, nameof(dnsServerId));
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
            throw new RepositoryException(RepositoryName, "ClearRules", $"Failed to clear rules: {ex.Message}", ex);
        }
    }
}
