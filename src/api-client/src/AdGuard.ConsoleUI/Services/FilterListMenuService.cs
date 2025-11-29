using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class FilterListMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public FilterListMenuService(ApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task ShowAsync()
    {
        try
        {
            var filterLists = await AnsiConsole.Status()
                .StartAsync("Fetching filter lists...", async ctx =>
                {
                    using var api = _apiClientFactory.CreateFilterListsApi();
                    return await api.ListFilterListsAsync();
                });

            DisplayFilterLists(filterLists);
        }
        catch (ApiException ex)
        {
            AnsiConsole.MarkupLine($"[red]API Error ({ex.ErrorCode}): {ex.Message}[/]");
            AnsiConsole.WriteLine();
        }
    }

    private static void DisplayFilterLists(List<FilterList> filterLists)
    {
        AnsiConsole.Write(new Rule("[green]Available Filter Lists[/]"));
        AnsiConsole.WriteLine();

        if (filterLists.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No filter lists available.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]ID[/]")
            .AddColumn("[green]Name[/]")
            .AddColumn("[green]Description[/]");

        foreach (var filter in filterLists)
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

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[grey]Total: {filterLists.Count} filter lists[/]");
        AnsiConsole.WriteLine();
    }
}
