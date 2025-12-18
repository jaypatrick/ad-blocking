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

/// <summary>
/// Extended menu service interface with additional metadata.
/// </summary>
public interface IMenuServiceEx : IMenuService
{
    /// <summary>
    /// Gets the unique identifier for this menu.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display order for this menu (lower values appear first).
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Gets a description of what this menu does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the icon or emoji for this menu (optional).
    /// </summary>
    string? Icon { get; }

    /// <summary>
    /// Gets a value indicating whether this menu is enabled.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether this menu requires authentication.
    /// </summary>
    bool RequiresAuthentication { get; }
}

/// <summary>
/// Base class for extended menu services.
/// </summary>
public abstract class MenuServiceBase : IMenuServiceEx
{
    /// <inheritdoc/>
    public abstract string Id { get; }

    /// <inheritdoc/>
    public abstract string Title { get; }

    /// <inheritdoc/>
    public virtual int Order => 0;

    /// <inheritdoc/>
    public virtual string Description => string.Empty;

    /// <inheritdoc/>
    public virtual string? Icon => null;

    /// <inheritdoc/>
    public virtual bool IsEnabled => true;

    /// <inheritdoc/>
    public virtual bool RequiresAuthentication => true;

    /// <inheritdoc/>
    public abstract Task ShowAsync();
}

/// <summary>
/// Menu registration information.
/// </summary>
public class MenuRegistration
{
    /// <summary>
    /// Gets or sets the menu identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the menu service type.
    /// </summary>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Gets or sets the factory function.
    /// </summary>
    public Func<IMenuService>? Factory { get; set; }
}

/// <summary>
/// Interface for menu service builders (fluent configuration).
/// </summary>
public interface IMenuServiceBuilder
{
    /// <summary>
    /// Configures the menu identifier.
    /// </summary>
    /// <param name="id">The menu identifier.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder WithId(string id);

    /// <summary>
    /// Configures the display order.
    /// </summary>
    /// <param name="order">The display order.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder WithOrder(int order);

    /// <summary>
    /// Configures the menu as requiring authentication.
    /// </summary>
    /// <param name="required">Whether authentication is required.</param>
    /// <returns>The builder for chaining.</returns>
    IMenuServiceBuilder RequiresAuth(bool required = true);

    /// <summary>
    /// Builds and registers the menu service.
    /// </summary>
    void Build();
}
