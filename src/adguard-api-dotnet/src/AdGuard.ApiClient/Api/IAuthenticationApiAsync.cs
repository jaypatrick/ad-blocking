namespace AdGuard.ApiClient.Api;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAuthenticationApiAsync : IApiAccessor
{
    #region Asynchronous Operations
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
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of AccessTokenResponse</returns>
    System.Threading.Tasks.Task<AccessTokenResponse> AccessTokenAsync(string? mfaToken = default, string? password = default, string? refreshToken = default, string? username = default, System.Threading.CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (AccessTokenResponse)</returns>
    System.Threading.Tasks.Task<ApiResponse<AccessTokenResponse>> AccessTokenWithHttpInfoAsync(string? mfaToken = default, string? password = default, string? refreshToken = default, string? username = default, System.Threading.CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}