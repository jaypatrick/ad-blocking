using Spectre.Console;

namespace AdGuard.ConsoleUI.Helpers;

/// <summary>
/// Extension methods for building Spectre.Console tables.
/// </summary>
public static class TableBuilderExtensions
{
    /// <summary>
    /// Creates a standard table with rounded border and green column headers.
    /// </summary>
    /// <param name="columns">The column headers.</param>
    /// <returns>A configured table.</returns>
    public static Table CreateStandardTable(params string[] columns)
    {
        var table = new Table().Border(TableBorder.Rounded);

        foreach (var column in columns)
        {
            table.AddColumn($"[green]{column}[/]");
        }

        return table;
    }

    /// <summary>
    /// Displays the table with an empty line after.
    /// </summary>
    /// <param name="table">The table to display.</param>
    public static void Display(this Table table)
    {
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a panel with standard formatting.
    /// </summary>
    /// <param name="title">The panel title.</param>
    /// <param name="content">The panel content rows.</param>
    public static void DisplayPanel(string title, params string[] content)
    {
        var rows = content.Select(c => new Markup(c)).ToArray();

        var panel = new Panel(new Rows(rows))
        {
            Header = new PanelHeader($"[green]{title}[/]"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a rule/divider with a title.
    /// </summary>
    /// <param name="title">The rule title.</param>
    public static void DisplayRule(string title)
    {
        AnsiConsole.Write(new Rule($"[green]{title}[/]"));
        AnsiConsole.WriteLine();
    }
}
