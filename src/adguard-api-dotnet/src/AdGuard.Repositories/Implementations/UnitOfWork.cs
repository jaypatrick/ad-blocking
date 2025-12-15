using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Implementations;

/// <summary>
/// Unit of Work implementation that aggregates all repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly IApiClientFactory _apiClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="devices">The device repository.</param>
    /// <param name="dnsServers">The DNS server repository.</param>
    /// <param name="account">The account repository.</param>
    /// <param name="statistics">The statistics repository.</param>
    /// <param name="queryLog">The query log repository.</param>
    /// <param name="filterLists">The filter list repository.</param>
    /// <param name="webServices">The web service repository.</param>
    /// <param name="dedicatedIps">The dedicated IP repository.</param>
    /// <param name="userRules">The user rules repository.</param>
    public UnitOfWork(
        IApiClientFactory apiClientFactory,
        IDeviceRepository devices,
        IDnsServerRepository dnsServers,
        IAccountRepository account,
        IStatisticsRepository statistics,
        IQueryLogRepository queryLog,
        IFilterListRepository filterLists,
        IWebServiceRepository webServices,
        IDedicatedIpRepository dedicatedIps,
        IUserRulesRepository userRules)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        Devices = devices ?? throw new ArgumentNullException(nameof(devices));
        DnsServers = dnsServers ?? throw new ArgumentNullException(nameof(dnsServers));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        QueryLog = queryLog ?? throw new ArgumentNullException(nameof(queryLog));
        FilterLists = filterLists ?? throw new ArgumentNullException(nameof(filterLists));
        WebServices = webServices ?? throw new ArgumentNullException(nameof(webServices));
        DedicatedIps = dedicatedIps ?? throw new ArgumentNullException(nameof(dedicatedIps));
        UserRules = userRules ?? throw new ArgumentNullException(nameof(userRules));
    }

    /// <inheritdoc />
    public IDeviceRepository Devices { get; }

    /// <inheritdoc />
    public IDnsServerRepository DnsServers { get; }

    /// <inheritdoc />
    public IAccountRepository Account { get; }

    /// <inheritdoc />
    public IStatisticsRepository Statistics { get; }

    /// <inheritdoc />
    public IQueryLogRepository QueryLog { get; }

    /// <inheritdoc />
    public IFilterListRepository FilterLists { get; }

    /// <inheritdoc />
    public IWebServiceRepository WebServices { get; }

    /// <inheritdoc />
    public IDedicatedIpRepository DedicatedIps { get; }

    /// <inheritdoc />
    public IUserRulesRepository UserRules { get; }

    /// <inheritdoc />
    public bool IsConfigured => _apiClientFactory.IsConfigured;

    /// <inheritdoc />
    public void Configure(string apiKey) => _apiClientFactory.Configure(apiKey);

    /// <inheritdoc />
    public void ConfigureFromSettings() => _apiClientFactory.ConfigureFromSettings();

    /// <inheritdoc />
    public Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        => _apiClientFactory.TestConnectionAsync(cancellationToken);
}
