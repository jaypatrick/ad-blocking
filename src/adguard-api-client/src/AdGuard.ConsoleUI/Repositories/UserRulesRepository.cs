namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for user rules operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class UserRulesRepository : IUserRulesRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IDnsServerRepository _dnsServerRepository;
    private readonly ILogger<UserRulesRepository> _logger;

    public UserRulesRepository(
        IApiClientFactory apiClientFactory,
        IDnsServerRepository dnsServerRepository,
        ILogger<UserRulesRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _dnsServerRepository = dnsServerRepository ?? throw new ArgumentNullException(nameof(dnsServerRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<UserRulesSettings> GetAsync(string dnsServerId)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        LogFetchingUserRules(dnsServerId);

        try
        {
            // Get the DNS server to access its settings
            var server = await _dnsServerRepository.GetByIdAsync(dnsServerId).ConfigureAwait(false);

            // Parse the settings object to extract user rules
            var userRulesSettings = ParseUserRulesFromSettings(server.Settings);

            LogRetrievedUserRules(dnsServerId, userRulesSettings.RulesCount);
            return userRulesSettings;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingUserRules(dnsServerId, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "Get",
                $"Failed to fetch user rules for DNS server {dnsServerId}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string dnsServerId, UserRulesSettingsUpdate update)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        ArgumentNullException.ThrowIfNull(update);

        LogUpdatingUserRules(dnsServerId, update.Rules?.Count ?? 0);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: update);
            await api.UpdateDNSServerSettingsAsync(dnsServerId, settingsUpdate).ConfigureAwait(false);

            LogUserRulesUpdated(dnsServerId, update.Rules?.Count ?? 0);
        }
        catch (ApiException ex)
        {
            LogApiErrorUpdatingUserRules(dnsServerId, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "Update",
                $"Failed to update user rules for DNS server {dnsServerId}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<int> UpdateFromFileAsync(string dnsServerId, string filePath)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            LogFileNotFound(filePath);
            throw new FileNotFoundException($"Rules file not found: {filePath}", filePath);
        }

        LogLoadingRulesFromFile(dnsServerId, filePath);

        try
        {
            // Read and parse the rules file
            var lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);

            // Filter out comments and empty lines
            var rules = lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Where(line => !line.TrimStart().StartsWith('!'))
                .Where(line => !line.TrimStart().StartsWith('#'))
                .Select(line => line.Trim())
                .Where(line => line.Length <= 1024) // Max rule length per API spec
                .Distinct()
                .ToList();

            LogParsedRulesFromFile(filePath, rules.Count, lines.Length);

            // Update the rules on the server
            var update = new UserRulesSettingsUpdate(enabled: true, rules: rules);
            await UpdateAsync(dnsServerId, update).ConfigureAwait(false);

            LogRulesUploadedFromFile(dnsServerId, rules.Count);
            return rules.Count;
        }
        catch (IOException ex)
        {
            LogFileReadError(filePath, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "UpdateFromFile",
                $"Failed to read rules file: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetEnabledAsync(string dnsServerId, bool enabled)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        LogSettingRulesEnabled(dnsServerId, enabled);

        try
        {
            using var api = _apiClientFactory.CreateDnsServersApi();
            var update = new UserRulesSettingsUpdate(enabled: enabled);
            var settingsUpdate = new DNSServerSettingsUpdate(userRulesSettings: update);
            await api.UpdateDNSServerSettingsAsync(dnsServerId, settingsUpdate).ConfigureAwait(false);

            LogRulesEnabledSet(dnsServerId, enabled);
        }
        catch (ApiException ex)
        {
            LogApiErrorSettingEnabled(dnsServerId, enabled, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "SetEnabled",
                $"Failed to set rules enabled state for DNS server {dnsServerId}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddRuleAsync(string dnsServerId, string rule)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        if (string.IsNullOrWhiteSpace(rule))
        {
            throw new ArgumentException("Rule cannot be null or empty.", nameof(rule));
        }

        LogAddingRule(dnsServerId, rule);

        try
        {
            // Get current rules
            var currentSettings = await GetAsync(dnsServerId).ConfigureAwait(false);
            var rules = currentSettings.Rules.ToList();

            // Add new rule if not already present
            if (!rules.Contains(rule))
            {
                rules.Add(rule);
                var update = new UserRulesSettingsUpdate(rules: rules);
                await UpdateAsync(dnsServerId, update).ConfigureAwait(false);
                LogRuleAdded(dnsServerId, rule);
            }
            else
            {
                LogRuleAlreadyExists(dnsServerId, rule);
            }
        }
        catch (ApiException ex)
        {
            LogApiErrorAddingRule(dnsServerId, rule, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "AddRule",
                $"Failed to add rule to DNS server {dnsServerId}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearRulesAsync(string dnsServerId)
    {
        if (string.IsNullOrWhiteSpace(dnsServerId))
        {
            LogAttemptedNullServerId();
            throw new ArgumentException("DNS server ID cannot be null or empty.", nameof(dnsServerId));
        }

        LogClearingRules(dnsServerId);

        try
        {
            var update = new UserRulesSettingsUpdate(rules: new List<string>());
            await UpdateAsync(dnsServerId, update).ConfigureAwait(false);
            LogRulesCleared(dnsServerId);
        }
        catch (ApiException ex)
        {
            LogApiErrorClearingRules(dnsServerId, ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("UserRulesRepository", "ClearRules",
                $"Failed to clear rules for DNS server {dnsServerId}: {ex.Message}", ex);
        }
    }

    private static UserRulesSettings ParseUserRulesFromSettings(object settings)
    {
        if (settings == null)
        {
            return new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
        }

        try
        {
            // The settings object is a JObject, parse it
            if (settings is JObject jSettings)
            {
                var userRulesToken = jSettings["user_rules_settings"];
                if (userRulesToken != null)
                {
                    var enabled = userRulesToken["enabled"]?.Value<bool>() ?? false;
                    var rules = userRulesToken["rules"]?.ToObject<List<string>>() ?? new List<string>();
                    var rulesCount = userRulesToken["rules_count"]?.Value<int>() ?? rules.Count;
                    return new UserRulesSettings(enabled, rules, rulesCount);
                }
            }

            // Try direct deserialization
            var json = JsonConvert.SerializeObject(settings);
            var parsed = JsonConvert.DeserializeObject<JObject>(json);
            if (parsed != null)
            {
                var userRulesToken = parsed["user_rules_settings"];
                if (userRulesToken != null)
                {
                    var enabled = userRulesToken["enabled"]?.Value<bool>() ?? false;
                    var rules = userRulesToken["rules"]?.ToObject<List<string>>() ?? new List<string>();
                    var rulesCount = userRulesToken["rules_count"]?.Value<int>() ?? rules.Count;
                    return new UserRulesSettings(enabled, rules, rulesCount);
                }
            }
        }
        catch (JsonException)
        {
            // Fall through to default
        }

        return new UserRulesSettings(enabled: false, rules: new List<string>(), rulesCount: 0);
    }
}
