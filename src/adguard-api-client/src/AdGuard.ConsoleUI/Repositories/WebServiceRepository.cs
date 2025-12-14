namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for web service operations.
/// Provides data access abstraction with comprehensive logging.
/// </summary>
public partial class WebServiceRepository : IWebServiceRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<WebServiceRepository> _logger;

    public WebServiceRepository(
        IApiClientFactory apiClientFactory,
        ILogger<WebServiceRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<WebService>> GetAllAsync()
    {
        LogFetchingAllWebServices();

        try
        {
            using var api = _apiClientFactory.CreateWebServicesApi();
            var webServices = await api.ListWebServicesAsync().ConfigureAwait(false);

            LogRetrievedWebServices(webServices.Count);
            return webServices;
        }
        catch (ApiException ex)
        {
            LogApiErrorFetchingWebServices(ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("WebServiceRepository", "GetAll",
                $"Failed to fetch web services: {ex.Message}", ex);
        }
    }
}
