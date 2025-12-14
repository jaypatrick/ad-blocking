using AdGuard.Repositories.Extensions;

namespace AdGuard.ConsoleUI.Extensions;

/// <summary>
/// Extension methods for registering AdGuard Console UI services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AdGuard Console UI services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// This method registers all display strategies, menu services, and the main application.
    /// It automatically includes the AdGuard repository services.
    /// </remarks>
    public static IServiceCollection AddAdGuardConsoleUI(this IServiceCollection services)
    {
        // Register shared repository services (includes IApiClientFactory and all repositories)
        services.AddAdGuardRepositories();

        // Register Display Strategies
        services.AddDisplayStrategies();

        // Register Menu Services
        services.AddMenuServices();

        // Register Main Application
        services.AddSingleton<ConsoleApplication>();

        return services;
    }

    /// <summary>
    /// Adds display strategy services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDisplayStrategies(this IServiceCollection services)
    {
        // Generic display strategies
        services.AddSingleton<IDisplayStrategy<Device>, DeviceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DNSServer>, DnsServerDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<FilterList>, FilterListDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<WebService>, WebServiceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DedicatedIPv4Address>, DedicatedIPDisplayStrategy>();

        // Specialized display strategies
        services.AddSingleton<AccountLimitsDisplayStrategy>();
        services.AddSingleton<StatisticsDisplayStrategy>();
        services.AddSingleton<QueryLogDisplayStrategy>();
        services.AddSingleton<UserRulesDisplayStrategy>();

        return services;
    }

    /// <summary>
    /// Adds menu services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// Menu services are registered as <see cref="IMenuService"/> to support collection injection
    /// in the main <see cref="ConsoleApplication"/>.
    /// </remarks>
    public static IServiceCollection AddMenuServices(this IServiceCollection services)
    {
        services.AddSingleton<IMenuService, DeviceMenuService>();
        services.AddSingleton<IMenuService, DnsServerMenuService>();
        services.AddSingleton<IMenuService, StatisticsMenuService>();
        services.AddSingleton<IMenuService, AccountMenuService>();
        services.AddSingleton<IMenuService, FilterListMenuService>();
        services.AddSingleton<IMenuService, QueryLogMenuService>();
        services.AddSingleton<IMenuService, WebServiceMenuService>();
        services.AddSingleton<IMenuService, DedicatedIPMenuService>();
        services.AddSingleton<IMenuService, UserRulesMenuService>();

        return services;
    }
}
