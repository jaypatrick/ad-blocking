using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using AdGuard.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// In-memory implementation of <see cref="ICacheProvider"/>.
/// </summary>
public class InMemoryCacheProvider : ICacheProvider, IDisposable
{
    private readonly ILogger<InMemoryCacheProvider> _logger;
    private readonly Options.CacheOptions _options;
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly Timer _cleanupTimer;
    private long _hits;
    private long _misses;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCacheProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The cache options.</param>
    public InMemoryCacheProvider(
        ILogger<InMemoryCacheProvider> logger,
        IOptions<Options.CacheOptions>? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new Options.CacheOptions();

        // Start cleanup timer to remove expired entries every minute
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <inheritdoc/>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (!entry.IsExpired)
            {
                entry.UpdateSlidingExpiration();
                Interlocked.Increment(ref _hits);
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return Task.FromResult((T?)entry.Value);
            }

            // Entry expired, remove it
            _cache.TryRemove(key, out _);
        }

        Interlocked.Increment(ref _misses);
        _logger.LogDebug("Cache miss for key: {Key}", key);
        return Task.FromResult(default(T?));
    }

    /// <inheritdoc/>
    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        return SetAsync(key, value, CacheEntryOptions.WithAbsoluteExpiration(expiration), cancellationToken);
    }

    /// <inheritdoc/>
    public Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        var entry = new CacheEntry(value, options);
        _cache[key] = entry;
        _logger.LogDebug("Set cache key: {Key}", key);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var value = await factory(cancellationToken);
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    /// <inheritdoc/>
    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var removed = _cache.TryRemove(key, out _);
        if (removed)
        {
            _logger.LogDebug("Removed cache key: {Key}", key);
        }
        return Task.FromResult(removed);
    }

    /// <inheritdoc/>
    public Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        var regex = new Regex(regexPattern, RegexOptions.Compiled);
        var removed = 0;

        foreach (var key in _cache.Keys.Where(k => regex.IsMatch(k)))
        {
            if (_cache.TryRemove(key, out _))
            {
                removed++;
            }
        }

        _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", removed, pattern);
        return Task.FromResult(removed);
    }

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            return Task.FromResult(!entry.IsExpired);
        }
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var count = _cache.Count;
        _cache.Clear();
        _logger.LogInformation("Cleared {Count} cache entries", count);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CacheStatistics
        {
            Hits = Interlocked.Read(ref _hits),
            Misses = Interlocked.Read(ref _misses),
            EntryCount = _cache.Count,
            MemorySizeBytes = EstimateMemorySize()
        });
    }

    private void CleanupExpiredEntries(object? state)
    {
        var expiredKeys = _cache.Where(kvp => kvp.Value.IsExpired).Select(kvp => kvp.Key).ToList();
        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }

        if (expiredKeys.Count > 0)
        {
            _logger.LogDebug("Cleaned up {Count} expired cache entries", expiredKeys.Count);
        }
    }

    private long EstimateMemorySize()
    {
        // Rough estimate - each entry is approximately 100 bytes + key length + value size
        return _cache.Sum(kvp => 100L + (kvp.Key.Length * 2));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _cleanupTimer.Dispose();
            _cache.Clear();
            _disposed = true;
        }
    }

    private class CacheEntry
    {
        public object? Value { get; }
        public DateTimeOffset? AbsoluteExpiration { get; }
        public TimeSpan? SlidingExpiration { get; }
        public DateTimeOffset LastAccessed { get; private set; }

        public CacheEntry(object? value, CacheEntryOptions options)
        {
            Value = value;
            LastAccessed = DateTimeOffset.UtcNow;

            if (options.AbsoluteExpiration.HasValue)
            {
                AbsoluteExpiration = options.AbsoluteExpiration;
            }
            else if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(options.AbsoluteExpirationRelativeToNow.Value);
            }

            SlidingExpiration = options.SlidingExpiration;
        }

        public bool IsExpired
        {
            get
            {
                if (AbsoluteExpiration.HasValue && DateTimeOffset.UtcNow > AbsoluteExpiration.Value)
                {
                    return true;
                }

                if (SlidingExpiration.HasValue && DateTimeOffset.UtcNow > LastAccessed.Add(SlidingExpiration.Value))
                {
                    return true;
                }

                return false;
            }
        }

        public void UpdateSlidingExpiration()
        {
            if (SlidingExpiration.HasValue)
            {
                LastAccessed = DateTimeOffset.UtcNow;
            }
        }
    }
}
