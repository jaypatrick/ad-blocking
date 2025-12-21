namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Interface for providing retry policies.
/// Enables pluggable retry strategies.
/// </summary>
public interface IRetryPolicyProvider
{
    /// <summary>
    /// Creates a retry policy for API operations.
    /// </summary>
    /// <param name="options">Options for configuring the retry policy.</param>
    /// <returns>An async retry policy.</returns>
    AsyncRetryPolicy CreateRetryPolicy(RetryPolicyOptions? options = null);

    /// <summary>
    /// Creates a typed retry policy for API operations.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="options">Options for configuring the retry policy.</param>
    /// <returns>An async retry policy for operations returning type T.</returns>
    AsyncRetryPolicy<T> CreateRetryPolicy<T>(RetryPolicyOptions? options = null);

    /// <summary>
    /// Creates a retry policy optimized for rate limiting scenarios.
    /// </summary>
    /// <param name="options">Options for configuring the retry policy.</param>
    /// <returns>An async retry policy for rate limiting.</returns>
    AsyncRetryPolicy CreateRateLimitRetryPolicy(RetryPolicyOptions? options = null);

    /// <summary>
    /// Executes an operation with retry policy.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="options">Options for configuring the retry policy.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> ExecuteWithRetryAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        RetryPolicyOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with retry policy.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="options">Options for configuring the retry policy.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ExecuteWithRetryAsync(
        Func<CancellationToken, Task> operation,
        RetryPolicyOptions? options = null,
        CancellationToken cancellationToken = default);
}