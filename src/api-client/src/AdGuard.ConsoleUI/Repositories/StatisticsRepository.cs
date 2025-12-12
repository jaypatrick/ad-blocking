using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for statistics operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class StatisticsRepository : IStatisticsRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<StatisticsRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public StatisticsRepository(IApiClientFactory apiClientFactory, ILogger<StatisticsRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("StatisticsRepository initialized");
    }

    /// <inheritdoc />
    public async Task<TimeQueriesStatsList> GetTimeQueriesStatsAsync(long fromMillis, long toMillis)
    {
        _logger.LogDebug("Fetching statistics from {FromMillis} to {ToMillis}", fromMillis, toMillis);

        try
        {
            using var api = _apiClientFactory.CreateStatisticsApi();
            var stats = await api.GetTimeQueriesStatsAsync(fromMillis, toMillis);

            var recordCount = stats.Stats?.Count ?? 0;
            _logger.LogInformation("Retrieved {Count} statistics records", recordCount);

            return stats;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching statistics: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("StatisticsRepository", "GetTimeQueriesStats",
                $"Failed to fetch statistics: {ex.Message}", ex);
        }
    }
}
