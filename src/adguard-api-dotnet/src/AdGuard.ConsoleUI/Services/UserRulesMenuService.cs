namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for managing user rules on DNS servers.
/// Provides UI for viewing, adding, uploading, and managing DNS blocking rules.
/// </summary>
public class UserRulesMenuService : BaseMenuService
{
    private readonly IUserRulesRepository _userRulesRepository;
    private readonly IDnsServerRepository _dnsServerRepository;
    private readonly UserRulesDisplayStrategy _displayStrategy;

    public UserRulesMenuService(
        IUserRulesRepository userRulesRepository,
        IDnsServerRepository dnsServerRepository,
        UserRulesDisplayStrategy displayStrategy)
    {
        _userRulesRepository = userRulesRepository ?? throw new ArgumentNullException(nameof(userRulesRepository));
        _dnsServerRepository = dnsServerRepository ?? throw new ArgumentNullException(nameof(dnsServerRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public override string Title => "User Rules Management";

    /// <inheritdoc />
    protected override Dictionary<string, Func<Task>> GetMenuActions() => new()
    {
        { "View Current Rules", ViewRulesAsync },
        { "Upload Rules from File", UploadRulesFromFileAsync },
        { "Add Single Rule", AddRuleAsync },
        { "Enable/Disable Rules", ToggleRulesAsync },
        { "Clear All Rules", ClearRulesAsync }
    };

    private async Task ViewRulesAsync()
    {
        var server = await SelectDnsServerAsync();
        if (server == null) return;

        var settings = await ConsoleHelpers.WithStatusAsync(
            "Fetching user rules...",
            () => _userRulesRepository.GetAsync(server.Id));

        _displayStrategy.DisplayDetails(settings);

        if (settings.Rules.Count > 0)
        {
            var showSummary = ConsoleHelpers.ConfirmAction("Show rules summary?");
            if (showSummary)
            {
                _displayStrategy.DisplayRulesSummary(settings.Rules);
            }
        }
    }

    private async Task UploadRulesFromFileAsync()
    {
        var server = await SelectDnsServerAsync();
        if (server == null) return;

        // Get file path from user
        var filePath = AnsiConsole.Ask<string>(
            "Enter path to [green]rules file[/] (or 'default' for rules/adguard_user_filter.txt):");

        if (filePath.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            // Use the default rules file path
            filePath = GetDefaultRulesFilePath();
        }

        // Expand environment variables and resolve path
        filePath = Environment.ExpandEnvironmentVariables(filePath);
        filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
        {
            ConsoleHelpers.ShowError($"File not found: {filePath}");
            return;
        }

        // Show file info
        var fileInfo = new FileInfo(filePath);
        AnsiConsole.MarkupLine($"[grey]File: {Markup.Escape(filePath)}[/]");
        AnsiConsole.MarkupLine($"[grey]Size: {fileInfo.Length:N0} bytes[/]");
        AnsiConsole.WriteLine();

        // Get current rules count for comparison
        var currentSettings = await ConsoleHelpers.WithStatusAsync(
            "Fetching current rules...",
            () => _userRulesRepository.GetAsync(server.Id));

        if (currentSettings.RulesCount > 0)
        {
            ConsoleHelpers.ShowWarning($"This will replace {currentSettings.RulesCount} existing rules.");
        }

        if (!ConsoleHelpers.ConfirmAction("Proceed with upload?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        var rulesCount = await ConsoleHelpers.WithStatusAsync(
            "Uploading rules...",
            () => _userRulesRepository.UpdateFromFileAsync(server.Id, filePath));

        ConsoleHelpers.ShowSuccess($"Successfully uploaded {rulesCount} rules to DNS server '{server.Name}'!");
    }

    private async Task AddRuleAsync()
    {
        var server = await SelectDnsServerAsync();
        if (server == null) return;

        AnsiConsole.MarkupLine("[grey]Enter a rule in AdGuard filter list format.[/]");
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("[grey]  ||example.com^     - Block domain[/]");
        AnsiConsole.MarkupLine("[grey]  @@||example.com^   - Allow domain[/]");
        AnsiConsole.MarkupLine("[grey]  ||ads.*^           - Block with wildcard[/]");
        AnsiConsole.WriteLine();

        var rule = AnsiConsole.Ask<string>("Enter [green]rule[/]:");

        if (string.IsNullOrWhiteSpace(rule))
        {
            ConsoleHelpers.ShowError("Rule cannot be empty.");
            return;
        }

        if (rule.Length > 1024)
        {
            ConsoleHelpers.ShowError("Rule exceeds maximum length of 1024 characters.");
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            "Adding rule...",
            () => _userRulesRepository.AddRuleAsync(server.Id, rule));

        ConsoleHelpers.ShowSuccess($"Rule added to DNS server '{server.Name}'!");
    }

    private async Task ToggleRulesAsync()
    {
        var server = await SelectDnsServerAsync();
        if (server == null) return;

        var currentSettings = await ConsoleHelpers.WithStatusAsync(
            "Fetching current settings...",
            () => _userRulesRepository.GetAsync(server.Id));

        var currentStatus = currentSettings.Enabled ? "enabled" : "disabled";
        var newStatus = !currentSettings.Enabled;
        var newStatusText = newStatus ? "enable" : "disable";

        AnsiConsole.MarkupLine($"[grey]Rules are currently {currentStatus}.[/]");

        if (!ConsoleHelpers.ConfirmAction($"Do you want to {newStatusText} user rules?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            $"{(newStatus ? "Enabling" : "Disabling")} rules...",
            () => _userRulesRepository.SetEnabledAsync(server.Id, newStatus));

        var resultText = newStatus ? "enabled" : "disabled";
        ConsoleHelpers.ShowSuccess($"User rules {resultText} for DNS server '{server.Name}'!");
    }

    private async Task ClearRulesAsync()
    {
        var server = await SelectDnsServerAsync();
        if (server == null) return;

        var currentSettings = await ConsoleHelpers.WithStatusAsync(
            "Fetching current rules...",
            () => _userRulesRepository.GetAsync(server.Id));

        if (currentSettings.RulesCount == 0)
        {
            ConsoleHelpers.ShowInfo("No rules to clear.");
            return;
        }

        ConsoleHelpers.ShowWarning($"This will permanently delete {currentSettings.RulesCount} rules.");

        if (!ConsoleHelpers.ConfirmAction("Are you sure you want to [red]clear all rules[/]?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            "Clearing rules...",
            () => _userRulesRepository.ClearRulesAsync(server.Id));

        ConsoleHelpers.ShowSuccess($"All rules cleared from DNS server '{server.Name}'!");
    }

    private async Task<DNSServer?> SelectDnsServerAsync()
    {
        var servers = await ConsoleHelpers.WithStatusAsync(
            "Fetching DNS servers...",
            () => _dnsServerRepository.GetAllAsync());

        if (servers.Count == 0)
        {
            ConsoleHelpers.ShowNoItemsMessage("DNS servers");
            return null;
        }

        return ConsoleHelpers.SelectItem(
            "Select a [green]DNS server[/]:",
            servers,
            s => $"{s.Name} ({s.Id}){(s.Default ? " [default]" : "")}");
    }

    private static string GetDefaultRulesFilePath()
    {
        // Try to find the default rules file relative to the current directory
        var candidates = new[]
        {
            "rules/adguard_user_filter.txt",
            "../rules/adguard_user_filter.txt",
            "../../rules/adguard_user_filter.txt",
            "../../../rules/adguard_user_filter.txt",
            "../../../../rules/adguard_user_filter.txt"
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        // Return the most likely path even if it doesn't exist
        return Path.GetFullPath("rules/adguard_user_filter.txt");
    }
}
