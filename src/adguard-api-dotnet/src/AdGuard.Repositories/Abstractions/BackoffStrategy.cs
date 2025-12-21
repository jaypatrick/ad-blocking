namespace AdGuard.Repositories.Abstractions;

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