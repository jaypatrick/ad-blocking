using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Exceptions;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for web service operations (read-only).
/// </summary>
public partial class WebServiceRepository : IWebServiceRepository
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly ILogger<WebServiceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebServiceRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public WebServiceRepository(IApiClientFactory apiClientFactory, ILogger<WebServiceRepository> logger)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<WebService>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingWebServices();

        try
        {
            using var api = _apiClientFactory.CreateWebServicesApi();
            var services = await api.ListWebServicesAsync(cancellationToken).ConfigureAwait(false);

            LogRetrievedWebServices(services.Count);
            return services;
        }
        catch (ApiException ex)
        {
            LogApiError("GetAll", ex.ErrorCode, ex.Message, ex);
            throw new RepositoryException("WebServiceRepository", "GetAll", $"Failed to fetch web services: {ex.Message}", ex);
        }
    }
}
