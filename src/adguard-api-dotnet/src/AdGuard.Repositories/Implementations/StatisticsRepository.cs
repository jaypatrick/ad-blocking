using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for statistics operations.
/// </summary>
public partial class StatisticsRepository : IStatisticsRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<StatisticsRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public StatisticsRepository(IApiClientFactory apiClientFactory, ILogger<StatisticsRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long startTime, long endTime, CancellationToken cancellationToken = default)
    {
        LogFetchingStatistics(startTime, endTime);

        try
        {
            using var api = _apiClientFactory.CreateStatisticsApi();
            var stats = await api.GetTimeQueriesStatsAsync(startTime, endTime, cancellationToken).ConfigureAwait(false);

            LogRetrievedStatistics(stats.Data?.Count ?? 0);
            return stats;
        }
        catch (ApiException ex)
        {
            LogApiError("GetTimeQueriesStats", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("StatisticsRepository", "GetTimeQueriesStats", $"Failed to fetch statistics: {ex.Message}", ex);
        }
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
