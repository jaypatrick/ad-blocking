namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Cache statistics.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Gets or sets the number of cache hits.
    /// </summary>
    public long Hits { get; set; }

    /// <summary>
    /// Gets or sets the number of cache misses.
    /// </summary>
    public long Misses { get; set; }

    /// <summary>
    /// Gets or sets the current number of entries.
    /// </summary>
    public long EntryCount { get; set; }

    /// <summary>
    /// Gets or sets the estimated memory size in bytes.
    /// </summary>
    public long MemorySizeBytes { get; set; }

    /// <summary>
    /// Gets the cache hit ratio.
    /// </summary>
    public double HitRatio => Hits + Misses > 0 ? (double)Hits / (Hits + Misses) : 0;
}