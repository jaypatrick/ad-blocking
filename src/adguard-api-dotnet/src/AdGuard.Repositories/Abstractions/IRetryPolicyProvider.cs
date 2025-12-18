using Polly.Retry;

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

/// <summary>
/// Options for configuring retry policies.
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries in seconds.
    /// </summary>
    public int InitialDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum delay between retries in seconds.
    /// </summary>
    public int MaxDelaySeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the backoff strategy.
    /// </summary>
    public BackoffStrategy Strategy { get; set; } = BackoffStrategy.Exponential;

    /// <summary>
    /// Gets or sets whether to add jitter to retry delays.
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Gets or sets the HTTP status codes to retry on.
    /// </summary>
    public IList<int> RetryableStatusCodes { get; set; } = [408, 429, 500, 502, 503, 504];

    /// <summary>
    /// Gets or sets a callback invoked before each retry.
    /// </summary>
    public Action<RetryAttemptContext>? OnRetry { get; set; }

    /// <summary>
    /// Creates default options for general API operations.
    /// </summary>
    public static RetryPolicyOptions Default() => new();

    /// <summary>
    /// Creates options optimized for rate limiting scenarios.
    /// </summary>
    public static RetryPolicyOptions ForRateLimiting() => new()
    {
        MaxRetries = 5,
        InitialDelaySeconds = 5,
        MaxDelaySeconds = 60,
        Strategy = BackoffStrategy.Linear,
        RetryableStatusCodes = [429]
    };

    /// <summary>
    /// Creates options for aggressive retry with minimal delays.
    /// </summary>
    public static RetryPolicyOptions Aggressive() => new()
    {
        MaxRetries = 5,
        InitialDelaySeconds = 1,
        MaxDelaySeconds = 10,
        Strategy = BackoffStrategy.Exponential,
        UseJitter = true
    };

    /// <summary>
    /// Creates options with no retry (fail fast).
    /// </summary>
    public static RetryPolicyOptions NoRetry() => new()
    {
        MaxRetries = 0
    };
}

/// <summary>
/// Backoff strategy for retry delays.
/// </summary>
public enum BackoffStrategy
{
    /// <summary>
    /// Constant delay between retries.
    /// </summary>
    Constant,

    /// <summary>
    /// Linear increase in delay: delay = initialDelay * retryAttempt.
    /// </summary>
    Linear,

    /// <summary>
    /// Exponential increase in delay: delay = initialDelay * 2^(retryAttempt-1).
    /// </summary>
    Exponential
}

/// <summary>
/// Context provided to retry callbacks.
/// </summary>
public class RetryAttemptContext
{
    /// <summary>
    /// Gets the current retry attempt number (1-based).
    /// </summary>
    public int RetryAttempt { get; init; }

    /// <summary>
    /// Gets the total maximum retries configured.
    /// </summary>
    public int MaxRetries { get; init; }

    /// <summary>
    /// Gets the exception that triggered the retry.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the delay before the next retry attempt.
    /// </summary>
    public TimeSpan Delay { get; init; }

    /// <summary>
    /// Gets the HTTP status code if available.
    /// </summary>
    public int? StatusCode { get; init; }
}
