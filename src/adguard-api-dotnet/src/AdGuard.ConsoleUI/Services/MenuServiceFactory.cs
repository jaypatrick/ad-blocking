namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Default implementation of <see cref="IMenuServiceFactory"/>.
/// </summary>
public class MenuServiceFactory : IMenuServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MenuServiceFactory> _logger;
    private readonly ConcurrentDictionary<string, MenuRegistration> _registrations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuServiceFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger instance.</param>
    public MenuServiceFactory(
        IServiceProvider serviceProvider,
        ILogger<MenuServiceFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<string> AvailableMenuIds =>
        _registrations.Keys.ToList().AsReadOnly();

    /// <inheritdoc/>
    public T Create<T>() where T : class, IMenuService
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <inheritdoc/>
    public IMenuService? Create(string menuId)
    {
        if (!_registrations.TryGetValue(menuId, out var registration))
        {
            _logger.LogWarning("Menu service not found: {MenuId}", menuId);
            return null;
        }

        if (registration.Factory != null)
        {
            return registration.Factory();
        }

        if (registration.ServiceType != null)
        {
            return (IMenuService)_serviceProvider.GetRequiredService(registration.ServiceType);
        }

        return null;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<IMenuService> GetAll()
    {
        var services = _serviceProvider.GetServices<IMenuService>();
        return services.ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<IMenuService> GetAllOrdered()
    {
        var services = _serviceProvider.GetServices<IMenuService>().ToList();

        // Sort by order if they implement IMenuServiceEx, otherwise by title
        return services
            .OrderBy(s => s is IMenuServiceEx ex ? ex.Order : 0)
            .ThenBy(s => s.Title)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc/>
    public void Register<T>(string menuId, int order = 0) where T : class, IMenuService
    {
        var registration = new MenuRegistration
        {
            Id = menuId,
            Order = order,
            ServiceType = typeof(T)
        };

        _registrations[menuId] = registration;
        _logger.LogDebug("Registered menu service: {MenuId} -> {Type}", menuId, typeof(T).Name);
    }

    /// <inheritdoc/>
    public void Register(string menuId, Func<IMenuService> factory, int order = 0)
    {
        var registration = new MenuRegistration
        {
            Id = menuId,
            Order = order,
            Factory = factory
        };

        _registrations[menuId] = registration;
        _logger.LogDebug("Registered menu service factory: {MenuId}", menuId);
    }

    /// <inheritdoc/>
    public bool IsRegistered(string menuId)
    {
        return _registrations.ContainsKey(menuId);
    }
}
