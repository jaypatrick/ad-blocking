namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for FilterList entities.
/// </summary>
public class FilterListDisplayStrategy : IDisplayStrategy<FilterList>
{
    /// <inheritdoc />
    public void Display(IEnumerable<FilterList> items)
    {
        var filterList = items.ToList();

        TableBuilderExtensions.DisplayRule("Available Filter Lists");

        if (filterList.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("filter lists");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("ID", "Name", "Description");

        foreach (var filter in filterList)
        {
            var description = filter.Description ?? "";
            if (description.Length > 60)
            {
                description = description[..57] + "...";
            }

            table.AddRow(
                filter.FilterId ?? "N/A",
                Markup.Escape(filter.Name ?? "N/A"),
                Markup.Escape(description));
        }

        table.Display();
        AnsiConsole.MarkupLine($"[grey]Total: {filterList.Count} filter lists[/]");
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc />
    public void DisplayDetails(FilterList filter)
    {
        TableBuilderExtensions.DisplayPanel(
            $"Filter: {Markup.Escape(filter.Name ?? "N/A")}",
            $"[bold]ID:[/] {filter.FilterId ?? "N/A"}",
            $"[bold]Name:[/] {Markup.Escape(filter.Name ?? "N/A")}",
            $"[bold]Description:[/] {Markup.Escape(filter.Description ?? "N/A")}");
    }
}
