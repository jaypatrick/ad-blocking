using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Extensions;
using RepoImpl = AdGuard.Repositories.Implementations;

namespace AdGuard.ConsoleUI;

/// <summary>
/// Entry point for the AdGuard DNS Console UI application.
/// </summary>
/// <remarks>
/// This application provides a console-based interface for managing AdGuard DNS
/// accounts, devices, DNS servers, and viewing statistics and query logs.
/// </remarks>
public class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code: 0 for success, 1 for failure.</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var configuration = BuildConfiguration();
            var services = ConfigureServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Configure logging for the API client helpers
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            RetryPolicyHelper.SetLogger(loggerFactory.CreateLogger("RetryPolicy"));

            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("AdGuard DNS Console UI starting");

            var app = serviceProvider.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();

            logger.LogInformation("AdGuard DNS Console UI shutting down gracefully");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    /// <summary>
    /// Builds the application configuration from various sources.
    /// </summary>
    /// <returns>The built configuration.</returns>
    /// <remarks>
    /// Configuration sources (in order of precedence):
    /// <list type="number">
    /// <item><description>appsettings.json (optional)</description></item>
    /// <item><description>Environment variables with ADGUARD_ prefix</description></item>
    /// </list>
    /// </remarks>
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("ADGUARD_")
            .Build();
    }

    /// <summary>
    /// Configures the dependency injection container with all required services.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        // Configuration
        services.AddSingleton(configuration);

        // Logging - file-based logging to avoid interfering with Spectre.Console
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register API Client Factory (with interface for DI)
        services.AddSingleton<ApiClientFactory>();
        services.AddSingleton<IApiClientFactory>(sp => sp.GetRequiredService<ApiClientFactory>());

        // Register ConsoleUI-specific Repositories (existing implementation)
        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IDnsServerRepository, DnsServerRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IStatisticsRepository, StatisticsRepository>();
        services.AddSingleton<IFilterListRepository, FilterListRepository>();
        services.AddSingleton<IQueryLogRepository, QueryLogRepository>();
        services.AddSingleton<IWebServiceRepository, WebServiceRepository>();
        services.AddSingleton<IDedicatedIPRepository, DedicatedIPRepository>();
        services.AddSingleton<IUserRulesRepository, UserRulesRepository>();

        // Register shared repository abstractions (Unit of Work pattern)
        // This provides IUnitOfWork for code that wants to use the shared repository pattern
        services.AddSingleton<Repositories.Abstractions.IApiClientFactory, RepoImpl.ApiClientFactory>();
        services.AddSingleton<Repositories.Contracts.IDeviceRepository, RepoImpl.DeviceRepository>();
        services.AddSingleton<Repositories.Contracts.IDnsServerRepository, RepoImpl.DnsServerRepository>();
        services.AddSingleton<Repositories.Contracts.IAccountRepository, RepoImpl.AccountRepository>();
        services.AddSingleton<Repositories.Contracts.IStatisticsRepository, RepoImpl.StatisticsRepository>();
        services.AddSingleton<Repositories.Contracts.IQueryLogRepository, RepoImpl.QueryLogRepository>();
        services.AddSingleton<Repositories.Contracts.IFilterListRepository, RepoImpl.FilterListRepository>();
        services.AddSingleton<Repositories.Contracts.IWebServiceRepository, RepoImpl.WebServiceRepository>();
        services.AddSingleton<Repositories.Contracts.IDedicatedIpRepository, RepoImpl.DedicatedIpRepository>();
        services.AddSingleton<Repositories.Contracts.IUserRulesRepository, RepoImpl.UserRulesRepository>();
        services.AddSingleton<IUnitOfWork, RepoImpl.UnitOfWork>();

        // Register Display Strategies
        services.AddSingleton<IDisplayStrategy<Device>, DeviceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DNSServer>, DnsServerDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<FilterList>, FilterListDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<WebService>, WebServiceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DedicatedIPv4Address>, DedicatedIPDisplayStrategy>();
        services.AddSingleton<AccountLimitsDisplayStrategy>();
        services.AddSingleton<StatisticsDisplayStrategy>();
        services.AddSingleton<QueryLogDisplayStrategy>();
        services.AddSingleton<UserRulesDisplayStrategy>();

        // Register Menu Services
        services.AddSingleton<DeviceMenuService>();
        services.AddSingleton<DnsServerMenuService>();
        services.AddSingleton<StatisticsMenuService>();
        services.AddSingleton<AccountMenuService>();
        services.AddSingleton<FilterListMenuService>();
        services.AddSingleton<QueryLogMenuService>();
        services.AddSingleton<WebServiceMenuService>();
        services.AddSingleton<DedicatedIPMenuService>();
        services.AddSingleton<UserRulesMenuService>();

        // Register Main Application
        services.AddSingleton<ConsoleApplication>();

        return services;
    }
}
