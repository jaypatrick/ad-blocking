namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents the time granularity for statistics aggregation.
/// </summary>
public enum StatisticsGranularity
{
    /// <summary>
    /// Statistics aggregated by hour.
    /// </summary>
    Hourly = 0,

    /// <summary>
    /// Statistics aggregated by day.
    /// </summary>
    Daily = 1,

    /// <summary>
    /// Statistics aggregated by week.
    /// </summary>
    Weekly = 2,

    /// <summary>
    /// Statistics aggregated by month.
    /// </summary>
    Monthly = 3
}