using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for filter list operations (read-only).
/// </summary>
public partial class FilterListRepository : IFilterListRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<FilterListRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterListRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public FilterListRepository(IApiClientFactory apiClientFactory, ILogger<FilterListRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<FilterList>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingFilterLists();

        try
        {
            using var api = _apiClientFactory.CreateFilterListsApi();
            var lists = await api.ListFilterListsAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedFilterLists(lists.Count);
            return lists;
        }
        catch (ApiException ex)
        {
            LogApiError("GetAll", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("FilterListRepository", "GetAll", $"Failed to fetch filter lists: {ex.Message}", ex);
        }
    }
}
