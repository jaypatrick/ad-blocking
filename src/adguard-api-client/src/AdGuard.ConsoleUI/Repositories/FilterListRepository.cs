namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for filter list operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class FilterListRepository : IFilterListRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<FilterListRepository> _logger;

    public FilterListRepository(
        IApiClientFactory apiClientFactory,
        ILogger<FilterListRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<FilterList>> GetAllAsync()
    {
        LogFetchingAllFilterLists();

        try
        {
            using var api = _apiClientFactory.CreateFilterListsApi();
            var filterLists = await api.ListFilterListsAsync().ConfigureAwait(false);

            LogRetrievedFilterLists(filterLists.Count);
            return filterLists;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingFilterLists(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("FilterListRepository", "GetAll",
                $"Failed to fetch filter lists: {ex.Message}", ex);
        }
    }
}

