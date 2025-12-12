namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Main console application that orchestrates menu navigation.
/// Uses dependency injection for all menu services.
/// </summary>
public class ConsoleApplication
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IMenuService _deviceMenu;
    private readonly IMenuService _dnsServerMenu;
    private readonly IMenuService _statisticsMenu;
    private readonly IMenuService _accountMenu;
    private readonly IMenuService _filterListMenu;
    private readonly IMenuService _queryLogMenu;
    private readonly Dictionary<string, IMenuService> _menuServices;

    public ConsoleApplication(
        IApiClientFactory apiClientFactory,
        DeviceMenuService deviceMenu,
        DnsServerMenuService dnsServerMenu,
        StatisticsMenuService statisticsMenu,
        AccountMenuService accountMenu,
        FilterListMenuService filterListMenu,
        QueryLogMenuService queryLogMenu)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        _deviceMenu = deviceMenu ?? throw new ArgumentNullException(nameof(deviceMenu));
        _dnsServerMenu = dnsServerMenu ?? throw new ArgumentNullException(nameof(dnsServerMenu));
        _statisticsMenu = statisticsMenu ?? throw new ArgumentNullException(nameof(statisticsMenu));
        _accountMenu = accountMenu ?? throw new ArgumentNullException(nameof(accountMenu));
        _filterListMenu = filterListMenu ?? throw new ArgumentNullException(nameof(filterListMenu));
        _queryLogMenu = queryLogMenu ?? throw new ArgumentNullException(nameof(queryLogMenu));

        // Register menu services for the Open/Closed principle
        _menuServices = new Dictionary<string, IMenuService>
        {
            { "Devices", _deviceMenu },
            { "DNS Servers", _dnsServerMenu },
            { "Statistics", _statisticsMenu },
            { "Query Log", _queryLogMenu },
            { "Filter Lists", _filterListMenu },
            { "Account Info", _accountMenu }
        };
    }

    public async Task RunAsync()
    {
        DisplayWelcomeBanner();

        // Try to load API key from configuration
        _apiClientFactory.ConfigureFromSettings();

        // If not configured, prompt for API key
        if (!_apiClientFactory.IsConfigured)
        {
            await ConfigureApiKeyAsync();
        }

        await MainMenuLoopAsync();

        AnsiConsole.MarkupLine("[grey]Goodbye![/]");
    }

    private static void DisplayWelcomeBanner()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("AdGuard DNS")
                .LeftJustified()
                .Color(Color.Green));

        AnsiConsole.Write(new Rule("[green]Console UI for AdGuard DNS API[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
    }

    private async Task ConfigureApiKeyAsync()
    {
        AnsiConsole.MarkupLine("[yellow]No API key configured.[/]");
        AnsiConsole.MarkupLine("[grey]Get your API key from: https://adguard-dns.io/dashboard/#/settings/api[/]");
        AnsiConsole.WriteLine();

        var apiKey = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter your [green]API Key[/]:")
                .PromptStyle("green")
                .Secret());

        _apiClientFactory.Configure(apiKey);

        await ConsoleHelpers.WithStatusAsync("Testing connection...", async () =>
        {
            var success = await _apiClientFactory.TestConnectionAsync();
            if (success)
            {
                AnsiConsole.MarkupLine("[green]Connection successful![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Connection failed. Please check your API key.[/]");
            }
        });

        AnsiConsole.WriteLine();
    }

    private async Task MainMenuLoopAsync()
    {
        var running = true;

        while (running)
        {
            var choices = _menuServices.Keys.Concat(new[] { "Settings", "Exit" }).ToArray();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Main Menu[/]")
                    .PageSize(12)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(choices));

            AnsiConsole.WriteLine();

            try
            {
                if (choice == "Exit")
                {
                    running = false;
                }
                else if (choice == "Settings")
                {
                    await ShowSettingsMenuAsync();
                }
                else if (_menuServices.TryGetValue(choice, out var menuService))
                {
                    await menuService.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                AnsiConsole.WriteLine();
            }
        }
    }

    private async Task ShowSettingsMenuAsync()
    {
        var choice = ConsoleHelpers.SelectChoice(
            "[green]Settings[/]",
            "Change API Key",
            "Test Connection",
            "Back");

        switch (choice)
        {
            case "Change API Key":
                await ConfigureApiKeyAsync();
                break;
            case "Test Connection":
                await TestConnectionAsync();
                break;
        }
    }

    private async Task TestConnectionAsync()
    {
        await ConsoleHelpers.WithStatusAsync("Testing connection...", async () =>
        {
            var success = await _apiClientFactory.TestConnectionAsync();
            if (success)
            {
                AnsiConsole.MarkupLine("[green]Connection is working![/]");
            }
        });
        AnsiConsole.WriteLine();
    }
}
