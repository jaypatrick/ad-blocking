namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Represents a console table for rendering.
/// </summary>
public class ConsoleTable
{
    /// <summary>
    /// Gets or sets the table title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the table columns.
    /// </summary>
    public IList<ConsoleTableColumn> Columns { get; } = [];

    /// <summary>
    /// Gets the table rows.
    /// </summary>
    public IList<ConsoleTableRow> Rows { get; } = [];

    /// <summary>
    /// Gets or sets whether to show borders.
    /// </summary>
    public bool ShowBorders { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to expand the table to full width.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// Adds a column to the table.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="alignment">The column alignment.</param>
    /// <returns>The table for chaining.</returns>
    public ConsoleTable AddColumn(string header, TextAlignment alignment = TextAlignment.Left)
    {
        Columns.Add(new ConsoleTableColumn { Header = header, Alignment = alignment });
        return this;
    }

    /// <summary>
    /// Adds a row to the table.
    /// </summary>
    /// <param name="values">The row values.</param>
    /// <returns>The table for chaining.</returns>
    public ConsoleTable AddRow(params object?[] values)
    {
        Rows.Add(new ConsoleTableRow { Values = values.Select(v => v?.ToString() ?? string.Empty).ToList() });
        return this;
    }
}