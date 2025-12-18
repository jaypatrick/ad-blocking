using AdGuard.Repositories.Contracts;

namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Unit of Work interface that aggregates all repositories.
/// Provides a single access point for all data operations.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the device repository.
    /// </summary>
    IDeviceRepository Devices { get; }

    /// <summary>
    /// Gets the DNS server repository.
    /// </summary>
    IDnsServerRepository DnsServers { get; }

    /// <summary>
    /// Gets the account repository.
    /// </summary>
    IAccountRepository Account { get; }

    /// <summary>
    /// Gets the statistics repository.
    /// </summary>
    IStatisticsRepository Statistics { get; }

    /// <summary>
    /// Gets the query log repository.
    /// </summary>
    IQueryLogRepository QueryLog { get; }

    /// <summary>
    /// Gets the filter list repository.
    /// </summary>
    IFilterListRepository FilterLists { get; }

    /// <summary>
    /// Gets the web service repository.
    /// </summary>
    IWebServiceRepository WebServices { get; }

    /// <summary>
    /// Gets the dedicated IP repository.
    /// </summary>
    IDedicatedIpRepository DedicatedIps { get; }

    /// <summary>
    /// Gets the user rules repository.
    /// </summary>
    IUserRulesRepository UserRules { get; }

    /// <summary>
    /// Gets a value indicating whether the API client is configured.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Configures the unit of work with the specified API key.
    /// </summary>
    /// <param name="apiKey">The AdGuard DNS API key.</param>
    void Configure(string apiKey);

    /// <summary>
    /// Attempts to configure from application settings.
    /// </summary>
    void ConfigureFromSettings();

    /// <summary>
    /// Tests the API connection.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the connection is successful; otherwise, false.</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}
