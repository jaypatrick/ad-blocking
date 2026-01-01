namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Represents aggregate statistics totals.
/// </summary>
public record StatisticsTotals
{
    /// <summary>
    /// Gets the total number of queries.
    /// </summary>
    public long TotalQueries { get; init; }

    /// <summary>
    /// Gets the total number of blocked queries.
    /// </summary>
    public long BlockedQueries { get; init; }

    /// <summary>
    /// Gets the total number of allowed queries.
    /// </summary>
    public long AllowedQueries { get; init; }

    /// <summary>
    /// Gets the total number of cached queries.
    /// </summary>
    public long CachedQueries { get; init; }

    /// <summary>
    /// Gets the average response time in milliseconds.
    /// </summary>
    public double AverageResponseTimeMs { get; init; }

    /// <summary>
    /// Gets the block rate as a percentage.
    /// </summary>
    public double BlockRate => TotalQueries > 0 ? (double)BlockedQueries / TotalQueries * 100 : 0;
}