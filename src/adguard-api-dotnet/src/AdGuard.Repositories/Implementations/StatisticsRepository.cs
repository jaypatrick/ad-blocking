using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for statistics operations.
/// </summary>
public partial class StatisticsRepository : BaseRepository<StatisticsRepository>, IStatisticsRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "StatisticsRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<StatisticsRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public StatisticsRepository(IApiClientFactory apiClientFactory, ILogger<StatisticsRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long startTime, long endTime, CancellationToken cancellationToken = default)
    {
        LogFetchingStatistics(startTime, endTime);

        var stats = await ExecuteAsync("GetTimeQueriesStats", async () =>
        {
            using var api = ApiClientFactory.CreateStatisticsApi();
            return await api.GetTimeQueriesStatsAsync(startTime, endTime, cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetTimeQueriesStats", code, message, ex), cancellationToken);

        LogRetrievedStatistics(stats.Stats?.Count ?? 0);
        return stats;
    }

    /// <inheritdoc />
    public Task<TimeQueriesStatsList> GetLast24HoursStatsAsync(CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
        return GetTimeQueriesStatsAsync(startTime, endTime, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TimeQueriesStatsList> GetLast7DaysStatsAsync(CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeMilliseconds();
        return GetTimeQueriesStatsAsync(startTime, endTime, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TimeQueriesStatsList> GetLast30DaysStatsAsync(CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeMilliseconds();
        return GetTimeQueriesStatsAsync(startTime, endTime, cancellationToken);
    }
}
