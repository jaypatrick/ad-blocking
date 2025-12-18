namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for viewing filter lists.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class FilterListMenuService : IMenuService
{
    private readonly IFilterListRepository _filterListRepository;
    private readonly IDisplayStrategy<FilterList> _displayStrategy;

    public FilterListMenuService(
        IFilterListRepository filterListRepository,
        IDisplayStrategy<FilterList> displayStrategy)
    {
        _filterListRepository = filterListRepository ?? throw new ArgumentNullException(nameof(filterListRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public string Title => "Filter Lists";

    /// <inheritdoc />
    public async Task ShowAsync()
    {
        try
        {
            var filterLists = await ConsoleHelpers.WithStatusAsync(
                "Fetching filter lists...",
                () => _filterListRepository.GetAllAsync());

            _displayStrategy.Display(filterLists);
        }
        catch (ApiException ex)
        {
            ConsoleHelpers.ShowApiError(ex);
        }
    }
}
