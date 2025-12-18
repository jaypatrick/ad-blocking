using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for query log operations.
/// </summary>
public partial class QueryLogRepository : BaseRepository<QueryLogRepository>, IQueryLogRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "QueryLogRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<QueryLogRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryLogRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public QueryLogRepository(IApiClientFactory apiClientFactory, ILogger<QueryLogRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<QueryLogResponse> GetQueryLogAsync(long startTime, long endTime, CancellationToken cancellationToken = default)
    {
        LogFetchingQueryLog(startTime, endTime);

        var log = await ExecuteAsync("GetQueryLog", async () =>
        {
            using var api = ApiClientFactory.CreateQueryLogApi();
            return await api.GetQueryLogAsync(startTime, endTime, cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetQueryLog", code, message, ex), cancellationToken);

        LogRetrievedQueryLog(log.Items?.Count ?? 0);
        return log;
    }

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        LogClearingQueryLog();

        await ExecuteAsync("Clear", async () =>
        {
            using var api = ApiClientFactory.CreateQueryLogApi();
            await api.ClearQueryLogAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("Clear", code, message, ex), cancellationToken);

        LogQueryLogCleared();
    }

    /// <inheritdoc />
    public Task<QueryLogResponse> GetLast24HoursAsync(CancellationToken cancellationToken = default)
    {
        var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
        return GetQueryLogAsync(startTime, endTime, cancellationToken);
    }
}
