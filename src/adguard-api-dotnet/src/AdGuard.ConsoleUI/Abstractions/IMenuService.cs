namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Interface for menu services that can be displayed to the user.
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Gets the display title for this menu.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Shows the menu and handles user interaction.
    /// </summary>
    Task ShowAsync();
}
