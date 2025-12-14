using AdGuard.Repositories.Abstractions;
using AdGuard.Repositories.Contracts;
using AdGuard.Repositories.Implementations;

namespace AdGuard.Repositories.Extensions;

/// <summary>
/// Extension methods for registering AdGuard repositories with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AdGuard repository services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositories(this IServiceCollection services)
    {
        // Register API Client Factory
        services.AddSingleton<IApiClientFactory, ApiClientFactory>();

        // Register Repositories
        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IDnsServerRepository, DnsServerRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IStatisticsRepository, StatisticsRepository>();
        services.AddSingleton<IQueryLogRepository, QueryLogRepository>();
        services.AddSingleton<IFilterListRepository, FilterListRepository>();
        services.AddSingleton<IWebServiceRepository, WebServiceRepository>();
        services.AddSingleton<IDedicatedIpRepository, DedicatedIpRepository>();
        services.AddSingleton<IUserRulesRepository, UserRulesRepository>();

        // Register Unit of Work
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds AdGuard repository services to the service collection with scoped lifetime.
    /// Use this when you need per-request isolation (e.g., in web applications).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositoriesScoped(this IServiceCollection services)
    {
        // Register API Client Factory as singleton (shared configuration)
        services.AddSingleton<IApiClientFactory, ApiClientFactory>();

        // Register Repositories as scoped
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IDnsServerRepository, DnsServerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        services.AddScoped<IQueryLogRepository, QueryLogRepository>();
        services.AddScoped<IFilterListRepository, FilterListRepository>();
        services.AddScoped<IWebServiceRepository, WebServiceRepository>();
        services.AddScoped<IDedicatedIpRepository, DedicatedIpRepository>();
        services.AddScoped<IUserRulesRepository, UserRulesRepository>();

        // Register Unit of Work as scoped
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
