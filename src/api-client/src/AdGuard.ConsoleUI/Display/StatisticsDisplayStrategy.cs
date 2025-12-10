using AdGuard.ApiClient.Helpers;
using AdGuard.ConsoleUI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for statistics data.
/// </summary>
public class StatisticsDisplayStrategy
{
    /// <summary>
    /// Displays statistics for a time range.
    /// </summary>
    /// <param name="stats">The statistics data.</param>
    /// <param name="fromMillis">Start time in milliseconds.</param>
    /// <param name="toMillis">End time in milliseconds.</param>
    public void Display(List<object>? stats, long fromMillis, long toMillis)
    {
        var fromDate = DateTimeExtensions.FromUnixMilliseconds(fromMillis);
        var toDate = DateTimeExtensions.FromUnixMilliseconds(toMillis);

        TableBuilderExtensions.DisplayRule($"Statistics from {fromDate:yyyy-MM-dd HH:mm} to {toDate:yyyy-MM-dd HH:mm} UTC");

        if (stats == null || stats.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("statistics available for the selected time range");
            return;
        }

        var table = TableBuilderExtensions.CreateStandardTable("Time", "Total Queries", "Blocked", "% Blocked");

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
                AnsiConsole.MarkupLine($"[grey]{stat}[/]");
            }
        }

        table.Display();

        DisplaySummary(totalQueries, totalBlocked);
    }

    private static void DisplaySummary(long totalQueries, long totalBlocked)
    {
        var totalPercentBlocked = totalQueries > 0 ? (totalBlocked * 100.0 / totalQueries) : 0;

        TableBuilderExtensions.DisplayPanel(
            "Summary",
            $"[bold]Total Queries:[/] {totalQueries:N0}",
            $"[bold]Total Blocked:[/] {totalBlocked:N0}",
            $"[bold]Block Rate:[/] {totalPercentBlocked:F1}%");
    }
}
