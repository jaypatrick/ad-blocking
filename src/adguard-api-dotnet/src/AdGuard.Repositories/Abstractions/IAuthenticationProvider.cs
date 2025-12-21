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