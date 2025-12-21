namespace AdGuard.Repositories.Abstractions;

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