using AdGuard.ApiClient.Client;
using AdGuard.ApiClient.Model;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class AccountMenuService
{
    private readonly ApiClientFactory _apiClientFactory;

    public AccountMenuService(ApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory;
    }

    public async Task ShowAsync()
    {
        try
        {
            var limits = await AnsiConsole.Status()
                .StartAsync("Fetching account information...", async ctx =>
                {
                    using var api = _apiClientFactory.CreateAccountApi();
                    return await api.GetAccountLimitsAsync();
                });

            DisplayAccountLimits(limits);
        }
        catch (ApiException ex)
        {
            AnsiConsole.MarkupLine($"[red]API Error ({ex.ErrorCode}): {ex.Message}[/]");
            AnsiConsole.WriteLine();
        }
    }

    private static void DisplayAccountLimits(AccountLimits limits)
    {
        AnsiConsole.Write(new Rule("[green]Account Limits[/]"));
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Resource[/]")
            .AddColumn("[green]Used[/]")
            .AddColumn("[green]Limit[/]")
            .AddColumn("[green]Usage[/]");

        AddLimitRow(table, "Devices", limits.Devices);
        AddLimitRow(table, "DNS Servers", limits.DnsServers);
        AddLimitRow(table, "User Rules", limits.UserRules);
        AddLimitRow(table, "Access Rules", limits.AccessRules);
        AddLimitRow(table, "Dedicated IPv4", limits.DedicatedIpv4);
        AddLimitRow(table, "Requests", limits.Requests);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private static void AddLimitRow(Table table, string name, Limit? limit)
    {
        if (limit == null)
        {
            table.AddRow(name, "N/A", "N/A", "[grey]N/A[/]");
            return;
        }

        var used = limit.Used ?? 0;
        var max = limit.VarLimit ?? 0;
        var percentage = max > 0 ? (used * 100.0 / max) : 0;

        var usageMarkup = percentage switch
        {
            >= 90 => $"[red]{percentage:F1}%[/]",
            >= 70 => $"[yellow]{percentage:F1}%[/]",
            _ => $"[green]{percentage:F1}%[/]"
        };

        var progressBar = CreateProgressBar(percentage);

        table.AddRow(
            name,
            used.ToString("N0"),
            max.ToString("N0"),
            $"{usageMarkup} {progressBar}");
    }

    private static string CreateProgressBar(double percentage)
    {
        const int width = 10;
        var filled = (int)Math.Round(percentage / 100 * width);
        filled = Math.Min(filled, width);
        var empty = width - filled;

        var color = percentage switch
        {
            >= 90 => "red",
            >= 70 => "yellow",
            _ => "green"
        };

        return $"[{color}]{new string('█', filled)}[/][grey]{new string('░', empty)}[/]";
    }
}
