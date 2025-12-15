using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for query log operations.
/// </summary>
public partial class QueryLogRepository : IQueryLogRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<QueryLogRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryLogRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public QueryLogRepository(IApiClientFactory apiClientFactory, ILogger<QueryLogRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<QueryLogResponse> GetQueryLogAsync(long startTime, long endTime, CancellationToken cancellationToken = default)
    {
        LogFetchingQueryLog(startTime, endTime);

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            var log = await api.GetQueryLogAsync(startTime, endTime, cancellationToken).ConfigureAwait(false);

            LogRetrievedQueryLog(log.Items?.Count ?? 0);
            return log;
        }
        catch (ApiException ex)
        {
            LogApiError("GetQueryLog", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("QueryLogRepository", "GetQueryLog", $"Failed to fetch query log: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        LogClearingQueryLog();

        try
        {
            using var api = _apiClientFactory.CreateQueryLogApi();
            await api.ClearQueryLogAsync(cancellationToken).ConfigureAwait(false);

            LogQueryLogCleared();
        }
        catch (ApiException ex)
        {
            LogApiError("Clear", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("QueryLogRepository", "Clear", $"Failed to clear query log: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public Task<QueryLogResponse> GetLast24HoursAsync(CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
        return GetQueryLogAsync(startTime, endTime, cancellationToken);
    }
}
