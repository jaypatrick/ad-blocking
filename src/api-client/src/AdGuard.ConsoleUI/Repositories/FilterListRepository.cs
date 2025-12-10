using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using Microsoft.Extensions.Logging;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for filter list operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public class FilterListRepository : IFilterListRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<FilterListRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterListRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger instance.</param>
    public FilterListRepository(IApiClientFactory apiClientFactory, ILogger<FilterListRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FilterListRepository initialized");
    }

    /// <inheritdoc />
    public async Task<List<FilterList>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all filter lists");

        try
        {
            using var api = _apiClientFactory.CreateFilterListsApi();
            var filterLists = await api.ListFilterListsAsync();

            _logger.LogInformation("Retrieved {Count} filter lists", filterLists.Count);
            return filterLists;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "API error while fetching filter lists: {ErrorCode} - {Message}",
                ex.ErrorCode, ex.Message);
            throw new RepositoryException("FilterListRepository", "GetAll",
                $"Failed to fetch filter lists: {ex.Message}", ex);
        }
    }
}
