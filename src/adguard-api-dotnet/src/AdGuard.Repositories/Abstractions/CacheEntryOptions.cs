namespace AdGuard.Repositories.Abstractions;

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