namespace AdGuard.Repositories.Abstractions;

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