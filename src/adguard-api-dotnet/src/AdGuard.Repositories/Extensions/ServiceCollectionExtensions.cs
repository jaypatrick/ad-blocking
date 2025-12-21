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
        services.TryAddSingleton<IApiClientFactory, ApiClientFactory>();

        // Register extensibility services
        services.TryAddSingleton<ICacheProvider, InMemoryCacheProvider>();
        services.TryAddSingleton<IAuthenticationProvider, ApiKeyAuthenticationProvider>();

        // Register Repositories
        services.TryAddSingleton<IDeviceRepository, DeviceRepository>();
        services.TryAddSingleton<IDnsServerRepository, DnsServerRepository>();
        services.TryAddSingleton<IAccountRepository, AccountRepository>();
        services.TryAddSingleton<IStatisticsRepository, StatisticsRepository>();
        services.TryAddSingleton<IQueryLogRepository, QueryLogRepository>();
        services.TryAddSingleton<IFilterListRepository, FilterListRepository>();
        services.TryAddSingleton<IWebServiceRepository, WebServiceRepository>();
        services.TryAddSingleton<IDedicatedIpRepository, DedicatedIpRepository>();
        services.TryAddSingleton<IUserRulesRepository, UserRulesRepository>();

        // Register Unit of Work
        services.TryAddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds AdGuard repository services to the service collection with configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind options from.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// This method binds <see cref="AdGuardApiOptions"/> and <see cref="CacheOptions"/>
    /// from the configuration and registers them with the Options pattern.
    /// </remarks>
    public static IServiceCollection AddAdGuardRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options from configuration
        services.Configure<AdGuardApiOptions>(
            configuration.GetSection(AdGuardApiOptions.SectionName));
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SectionName));

        // Register repositories
        return services.AddAdGuardRepositories();
    }

    /// <summary>
    /// Adds AdGuard repository services with custom options configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure API options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositories(
        this IServiceCollection services,
        Action<AdGuardApiOptions> configureOptions)
    {
        // Configure options via action
        services.Configure(configureOptions);

        // Register repositories
        return services.AddAdGuardRepositories();
    }

    /// <summary>
    /// Adds AdGuard repository services with custom cache provider.
    /// </summary>
    /// <typeparam name="TCacheProvider">The cache provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositoriesWithCache<TCacheProvider>(
        this IServiceCollection services)
        where TCacheProvider : class, ICacheProvider
    {
        services.AddSingleton<ICacheProvider, TCacheProvider>();
        return services.AddAdGuardRepositories();
    }

    /// <summary>
    /// Adds AdGuard repository services with custom authentication provider.
    /// </summary>
    /// <typeparam name="TAuthProvider">The authentication provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositoriesWithAuth<TAuthProvider>(
        this IServiceCollection services)
        where TAuthProvider : class, IAuthenticationProvider
    {
        services.AddSingleton<IAuthenticationProvider, TAuthProvider>();
        return services.AddAdGuardRepositories();
    }

    /// <summary>
    /// Adds a custom cache provider.
    /// </summary>
    /// <typeparam name="T">The cache provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCacheProvider<T>(this IServiceCollection services)
        where T : class, ICacheProvider
    {
        services.AddSingleton<ICacheProvider, T>();
        return services;
    }

    /// <summary>
    /// Adds a custom authentication provider.
    /// </summary>
    /// <typeparam name="T">The authentication provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthenticationProvider<T>(this IServiceCollection services)
        where T : class, IAuthenticationProvider
    {
        services.AddSingleton<IAuthenticationProvider, T>();
        return services;
    }

    /// <summary>
    /// Adds a custom retry policy provider.
    /// </summary>
    /// <typeparam name="T">The retry policy provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRetryPolicyProvider<T>(this IServiceCollection services)
        where T : class, IRetryPolicyProvider
    {
        services.AddSingleton<IRetryPolicyProvider, T>();
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
        services.TryAddSingleton<IApiClientFactory, ApiClientFactory>();

        // Register extensibility services as singleton
        services.TryAddSingleton<ICacheProvider, InMemoryCacheProvider>();
        services.TryAddSingleton<IAuthenticationProvider, ApiKeyAuthenticationProvider>();

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

    /// <summary>
    /// Adds AdGuard repository services with scoped lifetime and configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration to bind options from.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAdGuardRepositoriesScoped(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options from configuration
        services.Configure<AdGuardApiOptions>(
            configuration.GetSection(AdGuardApiOptions.SectionName));
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SectionName));

        // Register repositories with scoped lifetime
        return services.AddAdGuardRepositoriesScoped();
    }
}
