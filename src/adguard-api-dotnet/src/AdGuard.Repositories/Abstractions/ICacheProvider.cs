namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Interface for cache providers.
/// Enables pluggable caching backends (in-memory, Redis, etc.).
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached value if found; otherwise, default.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a cached value with the specified expiration.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The expiration time span.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a cached value with the specified options.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="options">The cache entry options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a cached value or creates it using the factory if not found.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Factory function to create the value if not cached.</param>
    /// <param name="expiration">The expiration time span.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cached or newly created value.</returns>
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the value was removed; otherwise, false.</returns>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all cached values matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The key pattern (supports * wildcard).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entries removed.</returns>
    Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached values.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cache statistics.</returns>
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for cache entries.
/// </summary>
public class CacheEntryOptions
{
    /// <summary>
    /// Gets or sets the absolute expiration time.
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the absolute expiration relative to now.
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration time.
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Gets or sets the priority of the cache entry.
    /// </summary>
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

    /// <summary>
    /// Gets or sets the cache entry tags for group invalidation.
    /// </summary>
    public IList<string> Tags { get; set; } = [];

    /// <summary>
    /// Creates options with absolute expiration relative to now.
    /// </summary>
    /// <param name="expiration">The expiration time span.</param>
    /// <returns>The cache entry options.</returns>
    public static CacheEntryOptions WithAbsoluteExpiration(TimeSpan expiration)
    {
        return new CacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
    }

    /// <summary>
    /// Creates options with sliding expiration.
    /// </summary>
    /// <param name="expiration">The sliding expiration time span.</param>
    /// <returns>The cache entry options.</returns>
    public static CacheEntryOptions WithSlidingExpiration(TimeSpan expiration)
    {
        return new CacheEntryOptions { SlidingExpiration = expiration };
    }
}

/// <summary>
/// Cache item priority levels.
/// </summary>
public enum CacheItemPriority
{
    /// <summary>
    /// Low priority - removed first during memory pressure.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority - removed last during memory pressure.
    /// </summary>
    High = 2,

    /// <summary>
    /// Never removed automatically.
    /// </summary>
    NeverRemove = 3
}

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
