using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for query log operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class QueryLogRepository : IQueryLogRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<QueryLogRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryLogRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public QueryLogRepository(IApiClientFactory apiClientFactory, ILogger<QueryLogRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("QueryLogRepository initialized");
    }

    /// <inheritdoc />
    public async Task<QueryLogResponse> GetQueryLogAsync(long fromMillis, long toMillis)
    {
        _logger.LogDebug("Fetching query log from {FromMillis} to {ToMillis}", fromMillis, toMillis);

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            var queryLog = await api.GetQueryLogAsync(fromMillis, toMillis);

            var itemCount = queryLog.Items?.Count ?? 0;
            _logger.LogInformation("Retrieved {Count} query log entries", itemCount);

            return queryLog;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching query log: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("QueryLogRepository", "GetQueryLog",
                $"Failed to fetch query log: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        _logger.LogDebug("Clearing query log");

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            await api.ClearQueryLogAsync();

            _logger.LogInformation("Query log cleared successfully");
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while clearing query log: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("QueryLogRepository", "Clear",
                $"Failed to clear query log: {ex.Message}", ex);
        }
    }
}
