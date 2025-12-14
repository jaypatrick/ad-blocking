namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for WebService entities.
/// </summary>
public class WebServiceDisplayStrategy : IDisplayStrategy<WebService>
{
    /// <inheritdoc />
    public void Display(IEnumerable<WebService> items)
    {
        var webServices = items.ToList();

        TableBuilderExtensions.DisplayRule("Available Web Services");

        if (webServices.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("web services");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("ID", "Name");

        foreach (var service in webServices)
        {
            table.AddRow(
                service.Id ?? "N/A",
                Markup.Escape(service.Name ?? "N/A"));
        }

        table.Display();
        AnsiConsole.MarkupLine($"[grey]Total: {webServices.Count} web services[/]");
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc />
    public void DisplayDetails(WebService service)
    {
        TableBuilderExtensions.DisplayPanel(
            $"Web Service: {Markup.Escape(service.Name ?? "N/A")}",
            $"[bold]ID:[/] {service.Id ?? "N/A"}",
            $"[bold]Name:[/] {Markup.Escape(service.Name ?? "N/A")}");
    }
}
