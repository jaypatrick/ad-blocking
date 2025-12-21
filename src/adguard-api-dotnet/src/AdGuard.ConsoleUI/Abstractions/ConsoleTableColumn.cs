namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Represents a table column.
/// </summary>
public class ConsoleTableColumn
{
    /// <summary>
    /// Gets or sets the column header.
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column alignment.
    /// </summary>
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
}