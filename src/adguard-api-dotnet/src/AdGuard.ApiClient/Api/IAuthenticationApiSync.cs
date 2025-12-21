namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAuthenticationApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Generates Access and Refresh token
    /// </summary>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="mfaToken"> (optional)</param>
    /// <param name="password"> (optional)</param>
    /// <param name="refreshToken"> (optional)</param>
    /// <param name="username"> (optional)</param>
    /// <returns>AccessTokenResponse</returns>
    AccessTokenResponse AccessToken(string? mfaToken = default, string? password = default, string? refreshToken = default, string? username = default);

    /// <summary>
    /// Generates Access and Refresh token
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <exception cref="AdGuard.ApiClient.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="mfaToken"> (optional)</param>
    /// <param name="password"> (optional)</param>
    /// <param name="refreshToken"> (optional)</param>
    /// <param name="username"> (optional)</param>
    /// <returns>ApiResponse of AccessTokenResponse</returns>
    ApiResponse<AccessTokenResponse> AccessTokenWithHttpInfo(string? mfaToken = default, string? password = default, string? refreshToken = default, string? username = default);
    #endregion Synchronous Operations
}