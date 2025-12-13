namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for query log operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class QueryLogRepository : IQueryLogRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<QueryLogRepository> _logger;

    public QueryLogRepository(
        IApiClientFactory apiClientFactory,
        ILogger<QueryLogRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<QueryLogResponse> GetQueryLogAsync(long fromMillis, long toMillis)
    {
        LogFetchingQueryLog(fromMillis, toMillis);

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            var queryLog = await api.GetQueryLogAsync(fromMillis, toMillis).ConfigureAwait(false);

            var itemCount = queryLog.Items?.Count ?? 0;
            LogRetrievedQueryLog(itemCount);

            return queryLog;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingQueryLog(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("QueryLogRepository", "GetQueryLog",
                $"Failed to fetch query log: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        LogClearingQueryLog();

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            await api.ClearQueryLogAsync().ConfigureAwait(false);

            LogQueryLogCleared();
        }
        catch (ApiException ex)
        {
            LogApiErrorClearingQueryLog(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("QueryLogRepository", "Clear",
                $"Failed to clear query log: {ex.Message}", ex);
        }
    }
}

