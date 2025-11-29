using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class StatisticsMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public StatisticsMenuService(ApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task ShowAsync()
    {
        var running = true;

        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Statistics[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(new[]
                    {
                        "Last 24 Hours",
                        "Last 7 Days",
                        "Last 30 Days",
                        "Custom Time Range",
                        "Back"
                    }));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "Last 24 Hours":
                        await ShowStatisticsAsync(DateTimeExtensions.HoursAgo(24), DateTimeExtensions.Now());
                        break;
                    case "Last 7 Days":
                        await ShowStatisticsAsync(DateTimeExtensions.DaysAgo(7), DateTimeExtensions.Now());
                        break;
                    case "Last 30 Days":
                        await ShowStatisticsAsync(DateTimeExtensions.DaysAgo(30), DateTimeExtensions.Now());
                        break;
                    case "Custom Time Range":
                        await ShowCustomRangeStatisticsAsync();
                        break;
                    case "Back":
                        running = false;
                        break;
                }
            }
            catch (ApiException ex)
            {
                AnsiConsole.MarkupLine($"[red]API Error ({ex.ErrorCode}): {ex.Message}[/]");
                AnsiConsole.WriteLine();
            }
        }
    }

    private async Task ShowStatisticsAsync(long fromMillis, long toMillis)
    {
        var stats = await AnsiConsole.Status()
            .StartAsync("Fetching statistics...", async ctx =>
            {
                using var api = _apiClientFactory.CreateStatisticsApi();
                return await api.GetTimeQueriesStatsAsync(fromMillis, toMillis);
            });

        DisplayStatistics(stats.Stats, fromMillis, toMillis);
    }

    private async Task ShowCustomRangeStatisticsAsync()
    {
        var daysAgo = AnsiConsole.Ask<int>("Days ago to start from:", 7);
        var fromMillis = DateTimeExtensions.DaysAgo(daysAgo);
        var toMillis = DateTimeExtensions.Now();

        await ShowStatisticsAsync(fromMillis, toMillis);
    }

    private static void DisplayStatistics(List<object>? stats, long fromMillis, long toMillis)
    {
        var fromDate = DateTimeExtensions.FromUnixMilliseconds(fromMillis);
        var toDate = DateTimeExtensions.FromUnixMilliseconds(toMillis);

        AnsiConsole.Write(new Rule($"[green]Statistics from {fromDate:yyyy-MM-dd HH:mm} to {toDate:yyyy-MM-dd HH:mm} UTC[/]"));
        AnsiConsole.WriteLine();

        if (stats == null || stats.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No statistics available for the selected time range.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        // Try to parse and display the statistics
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Time[/]")
            .AddColumn("[green]Total Queries[/]")
            .AddColumn("[green]Blocked[/]")
            .AddColumn("[green]% Blocked[/]");

        long totalQueries = 0;
        long totalBlocked = 0;

        foreach (var stat in stats)
        {
            try
            {
                var json = JsonConvert.SerializeObject(stat);
                var jObj = JObject.Parse(json);

                var time = jObj["time"]?.Value<long>() ?? 0;
                var queries = jObj["queries"]?.Value<long>() ?? 0;
                var blocked = jObj["blocked"]?.Value<long>() ?? 0;
                var percentBlocked = queries > 0 ? (blocked * 100.0 / queries) : 0;

                totalQueries += queries;
                totalBlocked += blocked;

                var timeStr = DateTimeExtensions.FromUnixMilliseconds(time).ToString("yyyy-MM-dd HH:mm");
                table.AddRow(
                    timeStr,
                    queries.ToString("N0"),
                    blocked.ToString("N0"),
                    $"{percentBlocked:F1}%");
            }
            catch
            {
                // If we can't parse the stat, just show it as JSON
                AnsiConsole.MarkupLine($"[grey]{stat}[/]");
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        // Summary
        var totalPercentBlocked = totalQueries > 0 ? (totalBlocked * 100.0 / totalQueries) : 0;
        var summaryPanel = new Panel(new Rows(
            new Markup($"[bold]Total Queries:[/] {totalQueries:N0}"),
            new Markup($"[bold]Total Blocked:[/] {totalBlocked:N0}"),
            new Markup($"[bold]Block Rate:[/] {totalPercentBlocked:F1}%")))
        {
            Header = new PanelHeader("[green]Summary[/]"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(summaryPanel);
        AnsiConsole.WriteLine();
    }
}
