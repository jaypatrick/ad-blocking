using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for filter list operations (read-only).
/// </summary>
public partial class FilterListRepository : BaseRepository<FilterListRepository>, IFilterListRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "FilterListRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<FilterListRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterListRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public FilterListRepository(IApiClientFactory apiClientFactory, ILogger<FilterListRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<FilterList>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingFilterLists();

        var lists = await ExecuteAsync("GetAll", async () =>
        {
            using var api = ApiClientFactory.CreateFilterListsApi();
            return await api.ListFilterListsAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetAll", code, message, ex), cancellationToken);

        LogRetrievedFilterLists(lists.Count);
        return lists;
    }
}
