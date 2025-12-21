namespace AdGuard.Repositories.Abstractions;

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