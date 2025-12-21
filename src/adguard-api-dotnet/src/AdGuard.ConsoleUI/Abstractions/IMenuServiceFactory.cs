namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Factory interface for creating menu services dynamically.
/// </summary>
public interface IMenuServiceFactory
{
    /// <summary>
    /// Creates a menu service by type.
    /// </summary>
    /// <typeparam name="T">The menu service type.</typeparam>
    /// <returns>The created menu service.</returns>
    T Create<T>() where T : class, IMenuService;

    /// <summary>
    /// Creates a menu service by identifier.
    /// </summary>
    /// <param name="menuId">The menu identifier.</param>
    /// <returns>The created menu service, or null if not found.</returns>
    IMenuService? Create(string menuId);

    /// <summary>
    /// Gets all available menu service identifiers.
    /// </summary>
    IReadOnlyCollection<string> AvailableMenuIds { get; }

    /// <summary>
    /// Gets all registered menu services.
    /// </summary>
    IReadOnlyCollection<IMenuService> GetAll();

    /// <summary>
    /// Gets all registered menu services ordered by display order.
    /// </summary>
    IReadOnlyCollection<IMenuService> GetAllOrdered();

    /// <summary>
    /// Registers a menu service type.
    /// </summary>
    /// <typeparam name="T">The menu service type.</typeparam>
    /// <param name="menuId">The menu identifier.</param>
    /// <param name="order">The display order.</param>
    void Register<T>(string menuId, int order = 0) where T : class, IMenuService;

    /// <summary>
    /// Registers a menu service factory function.
    /// </summary>
    /// <param name="menuId">The menu identifier.</param>
    /// <param name="factory">The factory function.</param>
    /// <param name="order">The display order.</param>
    void Register(string menuId, Func<IMenuService> factory, int order = 0);

    /// <summary>
    /// Checks if a menu service is registered.
    /// </summary>
    /// <param name="menuId">The menu identifier.</param>
    /// <returns>True if registered; otherwise, false.</returns>
    bool IsRegistered(string menuId);
}