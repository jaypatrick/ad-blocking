using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for statistics operations.
/// </summary>
public class StatisticsRepository : IStatisticsRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public StatisticsRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<StatsResponse> GetTimeQueriesStatsAsync(long fromMillis, long toMillis)
    {
        using var api = _apiClientFactory.CreateStatisticsApi();
        return await api.GetTimeQueriesStatsAsync(fromMillis, toMillis);
    }
}
