namespace AdGuard.ConsoleUI.Abstractions;

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