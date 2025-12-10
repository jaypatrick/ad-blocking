using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for filter list operations.
/// </summary>
public class FilterListRepository : IFilterListRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public FilterListRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<List<FilterList>> GetAllAsync()
    {
        using var api = _apiClientFactory.CreateFilterListsApi();
        return await api.ListFilterListsAsync();
    }
}
