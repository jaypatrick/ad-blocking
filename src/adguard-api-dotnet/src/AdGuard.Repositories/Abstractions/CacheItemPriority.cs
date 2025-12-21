namespace AdGuard.Repositories.Abstractions;

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