using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class QueryLogMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public QueryLogMenuService(ApiClientFactory apiClientFactory)
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
                    .Title("[green]Query Log[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(new[]
                    {
                        "View Recent Queries (Last Hour)",
                        "View Today's Queries",
                        "View Custom Time Range",
                        "Clear Query Log",
                        "Back"
                    }));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "View Recent Queries (Last Hour)":
                        await ShowQueryLogAsync(DateTimeExtensions.HoursAgo(1), DateTimeExtensions.Now());
                        break;
                    case "View Today's Queries":
                        await ShowQueryLogAsync(DateTimeExtensions.StartOfToday(), DateTimeExtensions.Now());
                        break;
                    case "View Custom Time Range":
                        await ShowCustomRangeQueryLogAsync();
                        break;
                    case "Clear Query Log":
                        await ClearQueryLogAsync();
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

    private async Task ShowQueryLogAsync(long fromMillis, long toMillis)
    {
        var queryLog = await AnsiConsole.Status()
            .StartAsync("Fetching query log...", async ctx =>
            {
                using var api = _apiClientFactory.CreateQueryLogApi();
                return await api.GetQueryLogAsync(fromMillis, toMillis);
            });

        DisplayQueryLog(queryLog.Items, fromMillis, toMillis);
    }

    private async Task ShowCustomRangeQueryLogAsync()
    {
        var hoursAgo = AnsiConsole.Ask<int>("Hours ago to start from:", 24);
        var fromMillis = DateTimeExtensions.HoursAgo(hoursAgo);
        var toMillis = DateTimeExtensions.Now();

        await ShowQueryLogAsync(fromMillis, toMillis);
    }

    private async Task ClearQueryLogAsync()
    {
        if (!AnsiConsole.Confirm("Are you sure you want to [red]clear all query logs[/]?", false))
        {
            AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        await AnsiConsole.Status()
            .StartAsync("Clearing query log...", async ctx =>
            {
                using var api = _apiClientFactory.CreateQueryLogApi();
                await api.ClearQueryLogAsync();
            });

        AnsiConsole.MarkupLine("[green]Query log cleared successfully![/]");
        AnsiConsole.WriteLine();
    }

    private static void DisplayQueryLog(List<object>? items, long fromMillis, long toMillis)
    {
        var fromDate = DateTimeExtensions.FromUnixMilliseconds(fromMillis);
        var toDate = DateTimeExtensions.FromUnixMilliseconds(toMillis);

        AnsiConsole.Write(new Rule($"[green]Query Log from {fromDate:yyyy-MM-dd HH:mm} to {toDate:yyyy-MM-dd HH:mm} UTC[/]"));
        AnsiConsole.WriteLine();

        if (items == null || items.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No queries found for the selected time range.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Time[/]")
            .AddColumn("[green]Domain[/]")
            .AddColumn("[green]Type[/]")
            .AddColumn("[green]Device[/]")
            .AddColumn("[green]Status[/]");

        var maxItems = Math.Min(items.Count, 50); // Limit to 50 entries for readability
        for (var i = 0; i < maxItems; i++)
        {
            try
            {
                var json = JsonConvert.SerializeObject(items[i]);
                var jObj = JObject.Parse(json);

                var time = jObj["time"]?.Value<long>() ?? 0;
                var domain = jObj["domain"]?.Value<string>() ?? "N/A";
                var queryType = jObj["type"]?.Value<string>() ?? "N/A";
                var deviceName = jObj["device_name"]?.Value<string>() ?? "N/A";
                var status = jObj["status"]?.Value<string>() ?? "N/A";

                var timeStr = DateTimeExtensions.FromUnixMilliseconds(time).ToString("HH:mm:ss");
                var statusMarkup = status.ToLowerInvariant() switch
                {
                    "blocked" => "[red]Blocked[/]",
                    "allowed" => "[green]Allowed[/]",
                    _ => status
                };

                table.AddRow(
                    timeStr,
                    Markup.Escape(domain.Length > 40 ? domain[..37] + "..." : domain),
                    queryType,
                    Markup.Escape(deviceName),
                    statusMarkup);
            }
            catch
            {
                // If we can't parse the item, show raw data
                table.AddRow("N/A", items[i]?.ToString() ?? "N/A", "N/A", "N/A", "N/A");
            }
        }

        AnsiConsole.Write(table);

        if (items.Count > maxItems)
        {
            AnsiConsole.MarkupLine($"[grey]Showing {maxItems} of {items.Count} entries.[/]");
        }

        AnsiConsole.WriteLine();
    }
}
