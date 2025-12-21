namespace AdGuard.ConsoleUI.Extensions;

/// <summary>
/// Extension methods for registering AdGuard Console UI services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Summary
    /// </summary>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds AdGuard Console UI services to the service collection.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        /// <remarks>
        /// This method registers all display strategies, menu services, and the main application.
        /// It automatically includes the AdGuard repository services.
        /// </remarks>
        public IServiceCollection AddAdGuardConsoleUI()
        {
            // Register shared repository services (includes IApiClientFactory and all repositories)
            services.AddAdGuardRepositories();

            // Register rendering abstractions (default: Spectre.Console)
            services.TryAddSingleton<IConsoleRenderer, SpectreConsoleRenderer>();
            services.TryAddSingleton<IConsolePrompter, SpectreConsolePrompter>();

            // Register menu service factory
            services.TryAddSingleton<IMenuServiceFactory, MenuServiceFactory>();

            // Register Display Strategies
            services.AddDisplayStrategies();

            // Register Menu Services
            services.AddMenuServices();

            // Register Main Application
            services.TryAddSingleton<ConsoleApplication>();

            return services;
        }

        /// <summary>
        /// Adds AdGuard Console UI with custom renderer.
        /// </summary>
        /// <typeparam name="TRenderer">The renderer type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddAdGuardConsoleUIWithRenderer<TRenderer>()
            where TRenderer : class, IConsoleRenderer
        {
            services.AddSingleton<IConsoleRenderer, TRenderer>();
            return services.AddAdGuardConsoleUI();
        }

        /// <summary>
        /// Adds AdGuard Console UI with custom prompter.
        /// </summary>
        /// <typeparam name="TPrompter">The prompter type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddAdGuardConsoleUIWithPrompter<TPrompter>()
            where TPrompter : class, IConsolePrompter
        {
            services.AddSingleton<IConsolePrompter, TPrompter>();
            return services.AddAdGuardConsoleUI();
        }

        /// <summary>
        /// Adds a custom console renderer.
        /// </summary>
        /// <typeparam name="T">The renderer type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddConsoleRenderer<T>()
            where T : class, IConsoleRenderer
        {
            services.AddSingleton<IConsoleRenderer, T>();
            return services;
        }

        /// <summary>
        /// Adds a custom console prompter.
        /// </summary>
        /// <typeparam name="T">The prompter type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddConsolePrompter<T>()
            where T : class, IConsolePrompter
        {
            services.AddSingleton<IConsolePrompter, T>();
            return services;
        }

        /// <summary>
        /// Adds display strategy services to the service collection.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddDisplayStrategies()
        {
            // Generic display strategies
            services.TryAddSingleton<IDisplayStrategy<Device>, DeviceDisplayStrategy>();
            services.TryAddSingleton<IDisplayStrategy<DNSServer>, DnsServerDisplayStrategy>();
            services.TryAddSingleton<IDisplayStrategy<FilterList>, FilterListDisplayStrategy>();
            services.TryAddSingleton<IDisplayStrategy<WebService>, WebServiceDisplayStrategy>();
            services.TryAddSingleton<IDisplayStrategy<DedicatedIPv4Address>, DedicatedIPDisplayStrategy>();

            // Specialized display strategies
            services.TryAddSingleton<AccountLimitsDisplayStrategy>();
            services.TryAddSingleton<StatisticsDisplayStrategy>();
            services.TryAddSingleton<QueryLogDisplayStrategy>();
            services.TryAddSingleton<UserRulesDisplayStrategy>();

            return services;
        }

        /// <summary>
        /// Adds a custom display strategy.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TStrategy">The strategy type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddDisplayStrategy<TEntity, TStrategy>()
            where TStrategy : class, IDisplayStrategy<TEntity>
        {
            services.AddSingleton<IDisplayStrategy<TEntity>, TStrategy>();
            return services;
        }

        /// <summary>
        /// Adds menu services to the service collection.
        /// </summary>
        /// <returns>The service collection for chaining.</returns>
        /// <remarks>
        /// Menu services are registered as <see cref="IMenuService"/> to support collection injection
        /// in the main <see cref="ConsoleApplication"/>.
        /// </remarks>
        public IServiceCollection AddMenuServices()
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

        /// <summary>
        /// Adds a custom menu service.
        /// </summary>
        /// <typeparam name="T">The menu service type.</typeparam>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddMenuService<T>()
            where T : class, IMenuService
        {
            services.AddSingleton<IMenuService, T>();
            return services;
        }
    }
}
