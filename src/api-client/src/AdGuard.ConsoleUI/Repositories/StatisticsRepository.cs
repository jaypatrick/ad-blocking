namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for statistics operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class StatisticsRepository : IStatisticsRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<StatisticsRepository> _logger;

    public StatisticsRepository(
        IApiClientFactory apiClientFactory,
        ILogger<StatisticsRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long fromMillis, long toMillis)
    {
        LogFetchingStatistics(fromMillis, toMillis);

        try
        {
            using var api = _apiClientFactory.CreateStatisticsApi();
            var stats = await api.GetTimeQueriesStatsAsync(fromMillis, toMillis).ConfigureAwait(false);

            var recordCount = stats.Stats?.Count ?? 0;
            LogRetrievedStatistics(recordCount);

            return stats;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingStatistics(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("StatisticsRepository", "GetTimeQueriesStats",
                $"Failed to fetch statistics: {ex.Message}", ex);
        }
    }
}

