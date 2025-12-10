using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for query log operations.
/// </summary>
public class QueryLogRepository : IQueryLogRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public QueryLogRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<QueryLogResponse> GetQueryLogAsync(long fromMillis, long toMillis)
    {
        using var api = _apiClientFactory.CreateQueryLogApi();
        return await api.GetQueryLogAsync(fromMillis, toMillis);
    }

    /// <inheritdoc />
    public async Task ClearAsync()
    {
        using var api = _apiClientFactory.CreateQueryLogApi();
        await api.ClearQueryLogAsync();
    }
}
