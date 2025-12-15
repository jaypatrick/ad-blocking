namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Strategy interface for displaying collections of items.
/// Enables flexible display formats (table, list, cards, etc.).
/// </summary>
/// <typeparam name="T">The type of items to display.</typeparam>
public interface IDisplayStrategy<in T>
{
    /// <summary>
    /// Displays a collection of items.
    /// </summary>
    /// <param name="items">The items to display.</param>
    void Display(IEnumerable<T> items);

    /// <summary>
    /// Displays a single item with detailed information.
    /// </summary>
    /// <param name="item">The item to display.</param>
    void DisplayDetails(T item);
}
