namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Represents a table row.
/// </summary>
public class ConsoleTableRow
{
    /// <summary>
    /// Gets or sets the row values.
    /// </summary>
    public IList<string> Values { get; set; } = [];
}