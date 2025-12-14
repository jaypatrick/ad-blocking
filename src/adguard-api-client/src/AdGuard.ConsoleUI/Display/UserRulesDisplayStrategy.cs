namespace AdGuard.ConsoleUI.Display;

/// <summary>
/// Display strategy for user rules settings.
/// Renders rules information using Spectre.Console tables and panels.
/// </summary>
public class UserRulesDisplayStrategy : IDisplayStrategy<UserRulesSettings>
{
    /// <inheritdoc />
    public void Display(IEnumerable<UserRulesSettings> items)
    {
        var itemList = items.ToList();
        
        if (itemList.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("user rules");
            return;
        }

        foreach (var settings in itemList)
        {
            DisplayDetails(settings);
        }
    }

    /// <inheritdoc />
    public void DisplayDetails(UserRulesSettings settings)
    {
        var statusColor = settings.Enabled ? "green" : "yellow";
        var statusText = settings.Enabled ? "Enabled" : "Disabled";

        var panel = new Panel(
            new Rows(
                new Markup($"[bold]Status:[/] [{statusColor}]{statusText}[/]"),
                new Markup($"[bold]Total Rules:[/] {settings.RulesCount}")))
        {
            Header = new PanelHeader("[bold blue]User Rules Settings[/]"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        if (settings.Rules.Count > 0)
        {
            DisplayRulesTable(settings.Rules);
        }
        else
        {
            AnsiConsole.MarkupLine("[grey]No rules configured.[/]");
            AnsiConsole.WriteLine();
        }
    }

    /// <summary>
    /// Displays a table of rules with pagination info.
    /// </summary>
    /// <param name="rules">The list of rules to display.</param>
    /// <param name="maxDisplay">Maximum number of rules to display.</param>
    public void DisplayRulesTable(IReadOnlyList<string> rules, int maxDisplay = 20)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn("[bold]#[/]").RightAligned())
            .AddColumn("[bold]Rule[/]");

        var displayCount = Math.Min(rules.Count, maxDisplay);

        for (int i = 0; i < displayCount; i++)
        {
            var rule = rules[i];
            var ruleColor = GetRuleColor(rule);
            table.AddRow(
                $"[grey]{i + 1}[/]",
                $"[{ruleColor}]{Markup.Escape(TruncateRule(rule))}[/]");
        }

        AnsiConsole.Write(table);

        if (rules.Count > maxDisplay)
        {
            AnsiConsole.MarkupLine($"[grey]... and {rules.Count - maxDisplay} more rules[/]");
        }

        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Gets a color for the rule based on its type.
    /// </summary>
    private static string GetRuleColor(string rule)
    {
        if (string.IsNullOrWhiteSpace(rule)) return "grey";

        var trimmed = rule.Trim();

        // Block rules (domain blocking)
        if (trimmed.StartsWith("||") && trimmed.EndsWith("^"))
            return "red";

        // Exception rules (whitelist)
        if (trimmed.StartsWith("@@"))
            return "green";

        // Important rules
        if (trimmed.Contains("$important"))
            return "yellow";

        // Host-style blocking
        if (trimmed.StartsWith("0.0.0.0 ") || trimmed.StartsWith("127.0.0.1 "))
            return "red";

        // Comments
        if (trimmed.StartsWith('!') || trimmed.StartsWith('#'))
            return "grey";

        return "white";
    }

    /// <summary>
    /// Truncates a rule for display.
    /// </summary>
    private static string TruncateRule(string rule, int maxLength = 80)
    {
        if (string.IsNullOrEmpty(rule)) return string.Empty;
        return rule.Length <= maxLength ? rule : rule[..(maxLength - 3)] + "...";
    }

    /// <summary>
    /// Displays a summary of rules by type.
    /// </summary>
    public void DisplayRulesSummary(IReadOnlyList<string> rules)
    {
        var blockRules = rules.Count(r => r.StartsWith("||") || r.StartsWith("0.0.0.0") || r.StartsWith("127.0.0.1"));
        var exceptionRules = rules.Count(r => r.StartsWith("@@"));
        var commentLines = rules.Count(r => r.StartsWith('!') || r.StartsWith('#'));
        var otherRules = rules.Count - blockRules - exceptionRules - commentLines;

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Rule Type[/]")
            .AddColumn(new TableColumn("[bold]Count[/]").RightAligned());

        table.AddRow("[red]Block Rules[/]", blockRules.ToString());
        table.AddRow("[green]Exception Rules[/]", exceptionRules.ToString());
        table.AddRow("[grey]Comments[/]", commentLines.ToString());
        table.AddRow("[white]Other Rules[/]", otherRules.ToString());
        table.AddRow(new Rule(), new Rule());
        table.AddRow("[bold]Total[/]", $"[bold]{rules.Count}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}
