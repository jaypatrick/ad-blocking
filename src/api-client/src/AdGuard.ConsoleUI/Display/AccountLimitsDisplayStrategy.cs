using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Helpers;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for AccountLimits entity.
/// </summary>
public class AccountLimitsDisplayStrategy
{
    /// <summary>
    /// Displays account limits information.
    /// </summary>
    /// <param name="limits">The account limits to display.</param>
    public void Display(AccountLimits limits)
    {
        TableBuilderExtensions.DisplayRule("Account Limits");

        var table = TableBuilderExtensions.CreateStandardTable("Resource", "Used", "Limit", "Usage");

        AddLimitRow(table, "Devices", limits.Devices);
        AddLimitRow(table, "DNS Servers", limits.DnsServers);
        AddLimitRow(table, "User Rules", limits.UserRules);
        AddLimitRow(table, "Access Rules", limits.AccessRules);
        AddLimitRow(table, "Dedicated IPv4", limits.DedicatedIpv4);
        AddLimitRow(table, "Requests", limits.Requests);

        table.Display();
    }

    private static void AddLimitRow(Table table, string name, Limit? limit)
    {
        if (limit == null)
        {
            table.AddRow(name, "N/A", "N/A", "[grey]N/A[/]");
            return;
        }

        var used = limit.Used;
        var max = limit.VarLimit;
        var percentage = max > 0 ? (used * 100.0 / max) : 0;

        var usageMarkup = ConsoleHelpers.GetPercentageMarkup(percentage);
        var progressBar = ConsoleHelpers.CreateProgressBar(percentage);

        table.AddRow(
            name,
            used.ToString("N0"),
            max.ToString("N0"),
            $"{usageMarkup} {progressBar}");
    }
}
