using AdGuard.ApiClient.Helpers;
using AdGuard.ConsoleUI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for query log entries.
/// </summary>
public class QueryLogDisplayStrategy
{
    private const int MaxDisplayItems = 50;

    /// <summary>
    /// Displays query log entries for a time range.
    /// </summary>
    /// <param name="items">The query log items.</param>
    /// <param name="fromMillis">Start time in milliseconds.</param>
    /// <param name="toMillis">End time in milliseconds.</param>
    public void Display(List<object>? items, long fromMillis, long toMillis)
    {
        var fromDate = DateTimeExtensions.FromUnixMilliseconds(fromMillis);
        var toDate = DateTimeExtensions.FromUnixMilliseconds(toMillis);

        TableBuilderExtensions.DisplayRule($"Query Log from {fromDate:yyyy-MM-dd HH:mm} to {toDate:yyyy-MM-dd HH:mm} UTC");

        if (items == null || items.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("queries found for the selected time range");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("Time", "Domain", "Type", "Device", "Status");

        var maxItems = Math.Min(items.Count, MaxDisplayItems);
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
                var statusMarkup = GetStatusMarkup(status);
                var truncatedDomain = TruncateDomain(domain, 40);

                table.AddRow(
                    timeStr,
                    Markup.Escape(truncatedDomain),
                    queryType,
                    Markup.Escape(deviceName),
                    statusMarkup);
            }
            catch
            {
                table.AddRow("N/A", items[i]?.ToString() ?? "N/A", "N/A", "N/A", "N/A");
            }
        }

        table.Display();

        if (items.Count > maxItems)
        {
            AnsiConsole.MarkupLine($"[grey]Showing {maxItems} of {items.Count} entries.[/]");
            AnsiConsole.WriteLine();
        }
    }

    private static string GetStatusMarkup(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "blocked" => "[red]Blocked[/]",
            "allowed" => "[green]Allowed[/]",
            _ => status
        };
    }

    private static string TruncateDomain(string domain, int maxLength)
    {
        return domain.Length > maxLength
            ? domain[..(maxLength - 3)] + "..."
            : domain;
    }
}
