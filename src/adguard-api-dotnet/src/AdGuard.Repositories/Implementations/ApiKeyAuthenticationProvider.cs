namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Default API key authentication provider.
/// </summary>
public class ApiKeyAuthenticationProvider : IAuthenticationProvider
{
    private readonly ILogger<ApiKeyAuthenticationProvider> _logger;
    private readonly Options.AdGuardApiOptions _options;
    private AuthenticationCredentials? _credentials;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The API options.</param>
    public ApiKeyAuthenticationProvider(
        ILogger<ApiKeyAuthenticationProvider> logger,
        IOptions<Options.AdGuardApiOptions>? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new Options.AdGuardApiOptions();

        // Initialize from options if API key is configured
        if (!string.IsNullOrEmpty(_options.ApiKey))
        {
            _credentials = AuthenticationCredentials.FromApiKey(_options.ApiKey);
        }
    }

    /// <inheritdoc/>
    public string Scheme => "ApiKey";

    /// <inheritdoc/>
    public bool IsConfigured => _credentials != null && !string.IsNullOrEmpty(_credentials.ApiKey);

    /// <inheritdoc/>
    public Task<AuthenticationCredentials> GetCredentialsAsync(CancellationToken cancellationToken = default)
    {
        if (_credentials == null)
        {
            throw new InvalidOperationException("Authentication provider is not configured. Call Configure() first.");
        }

        return Task.FromResult(_credentials);
    }

    /// <inheritdoc/>
    public Task<bool> ValidateCredentialsAsync(CancellationToken cancellationToken = default)
    {
        // For API key authentication, we just check if the key exists
        // Actual validation would require making an API call
        var isValid = IsConfigured;
        _logger.LogDebug("Credentials validation result: {IsValid}", isValid);
        return Task.FromResult(isValid);
    }

    /// <inheritdoc/>
    public Task<AuthenticationCredentials?> RefreshCredentialsAsync(CancellationToken cancellationToken = default)
    {
        // API keys don't support refresh
        _logger.LogDebug("API key authentication does not support credential refresh");
        return Task.FromResult<AuthenticationCredentials?>(null);
    }

    /// <inheritdoc/>
    public Task<string> GetAuthenticationHeaderValueAsync(CancellationToken cancellationToken = default)
    {
        if (_credentials?.ApiKey == null)
        {
            throw new InvalidOperationException("Authentication provider is not configured. Call Configure() first.");
        }

        // AdGuard DNS API uses Bearer token format for API key
        return Task.FromResult(_credentials.ApiKey);
    }

    /// <inheritdoc/>
    public void Configure(AuthenticationCredentials credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        if (string.IsNullOrEmpty(credentials.ApiKey))
        {
            throw new ArgumentException("API key is required", nameof(credentials));
        }

        _credentials = credentials;
        _logger.LogInformation("API key authentication configured (key: {MaskedKey})", credentials.MaskedApiKey);
    }

    /// <summary>
    /// Configures the provider with an API key directly.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    public void Configure(string apiKey)
    {
        Configure(AuthenticationCredentials.FromApiKey(apiKey));
    }
}
