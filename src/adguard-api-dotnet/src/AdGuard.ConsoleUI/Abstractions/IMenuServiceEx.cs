namespace AdGuard.ConsoleUI.Abstractions;

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