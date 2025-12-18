namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Interface for authentication providers.
/// Enables pluggable authentication mechanisms.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Gets the authentication scheme (e.g., "Bearer", "ApiKey", "Basic").
    /// </summary>
    string Scheme { get; }

    /// <summary>
    /// Gets the authentication credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authentication credentials.</returns>
    Task<AuthenticationCredentials> GetCredentialsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the current credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if credentials are valid; otherwise, false.</returns>
    Task<bool> ValidateCredentialsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the authentication credentials if supported.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed credentials, or null if refresh is not supported.</returns>
    Task<AuthenticationCredentials?> RefreshCredentialsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authentication header value.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The header value to use for authentication.</returns>
    Task<string> GetAuthenticationHeaderValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures the authentication provider with credentials.
    /// </summary>
    /// <param name="credentials">The credentials to configure.</param>
    void Configure(AuthenticationCredentials credentials);

    /// <summary>
    /// Gets a value indicating whether the provider is configured with valid credentials.
    /// </summary>
    bool IsConfigured { get; }
}

/// <summary>
/// Represents authentication credentials.
/// </summary>
public class AuthenticationCredentials
{
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the username for basic authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for basic authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets a value indicating whether the credentials are expired.
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTimeOffset.UtcNow >= ExpiresAt.Value;

    /// <summary>
    /// Gets a value indicating whether the credentials can be refreshed.
    /// </summary>
    public bool CanRefresh => !string.IsNullOrEmpty(RefreshToken);

    /// <summary>
    /// Gets a masked representation of the API key for logging.
    /// </summary>
    public string? MaskedApiKey =>
        string.IsNullOrEmpty(ApiKey) ? null :
        ApiKey.Length > 8 ? $"{ApiKey[..4]}...{ApiKey[^4..]}" : "***";

    /// <summary>
    /// Creates credentials from an API key.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <returns>Authentication credentials.</returns>
    public static AuthenticationCredentials FromApiKey(string apiKey) => new() { ApiKey = apiKey };

    /// <summary>
    /// Creates credentials from an access token.
    /// </summary>
    /// <param name="accessToken">The access token.</param>
    /// <param name="expiresAt">Optional expiration time.</param>
    /// <param name="refreshToken">Optional refresh token.</param>
    /// <returns>Authentication credentials.</returns>
    public static AuthenticationCredentials FromToken(
        string accessToken,
        DateTimeOffset? expiresAt = null,
        string? refreshToken = null) => new()
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken
        };

    /// <summary>
    /// Creates credentials from username and password.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>Authentication credentials.</returns>
    public static AuthenticationCredentials FromBasicAuth(string username, string password) => new()
    {
        Username = username,
        Password = password
    };
}

/// <summary>
/// Event arguments for authentication events.
/// </summary>
public class AuthenticationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the authentication scheme.
    /// </summary>
    public string Scheme { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets whether to cancel the operation.
    /// </summary>
    public bool Cancel { get; set; }
}

/// <summary>
/// Event arguments for authentication failure events.
/// </summary>
public class AuthenticationFailedEventArgs : AuthenticationEventArgs
{
    /// <summary>
    /// Gets the exception that caused the failure.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the failure reason.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the failure was handled.
    /// </summary>
    public bool Handled { get; set; }
}

/// <summary>
/// Interface for authentication provider factory.
/// </summary>
public interface IAuthenticationProviderFactory
{
    /// <summary>
    /// Creates an authentication provider for the specified scheme.
    /// </summary>
    /// <param name="scheme">The authentication scheme.</param>
    /// <returns>The authentication provider.</returns>
    IAuthenticationProvider Create(string scheme);

    /// <summary>
    /// Gets the available authentication schemes.
    /// </summary>
    IReadOnlyCollection<string> AvailableSchemes { get; }

    /// <summary>
    /// Registers an authentication provider for a scheme.
    /// </summary>
    /// <param name="scheme">The authentication scheme.</param>
    /// <param name="providerFactory">Factory function to create the provider.</param>
    void Register(string scheme, Func<IAuthenticationProvider> providerFactory);
}
