using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Common;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Repository implementation for web service operations (read-only).
/// </summary>
public partial class WebServiceRepository : BaseRepository<WebServiceRepository>, IWebServiceRepository
{
    /// <inheritdoc />
    protected override string RepositoryName => "WebServiceRepository";

    // Required for LoggerMessage source generator
    private readonly ILogger<WebServiceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebServiceRepository"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="logger">The logger.</param>
    public WebServiceRepository(IApiClientFactory apiClientFactory, ILogger<WebServiceRepository> logger)
        : base(apiClientFactory, logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<WebService>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        LogFetchingWebServices();

        var services = await ExecuteAsync("GetAll", async () =>
        {
            using var api = ApiClientFactory.CreateWebServicesApi();
            return await api.ListWebServicesAsync(cancellationToken).ConfigureAwait(false);
        }, (code, message, ex) => LogApiError("GetAll", code, message, ex), cancellationToken);

        LogRetrievedWebServices(services.Count);
        return services;
    }
}
