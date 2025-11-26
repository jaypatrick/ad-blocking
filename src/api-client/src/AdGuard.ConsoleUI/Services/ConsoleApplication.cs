using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

public class ConsoleApplication
{
    private readonly ApiClientFactory _apiClientFactory;
    private readonly DeviceMenuService _deviceMenu;
    private readonly DnsServerMenuService _dnsServerMenu;
    private readonly StatisticsMenuService _statisticsMenu;
    private readonly AccountMenuService _accountMenu;
    private readonly FilterListMenuService _filterListMenu;
    private readonly QueryLogMenuService _queryLogMenu;

    public ConsoleApplication(
        ApiClientFactory apiClientFactory,
        DeviceMenuService deviceMenu,
        DnsServerMenuService dnsServerMenu,
        StatisticsMenuService statisticsMenu,
        AccountMenuService accountMenu,
        FilterListMenuService filterListMenu,
        QueryLogMenuService queryLogMenu)
    {
        _apiClientFactory = apiClientFactory;
        _deviceMenu = deviceMenu;
        _dnsServerMenu = dnsServerMenu;
        _statisticsMenu = statisticsMenu;
        _accountMenu = accountMenu;
        _filterListMenu = filterListMenu;
        _queryLogMenu = queryLogMenu;
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

        await AnsiConsole.Status()
            .StartAsync("Testing connection...", async ctx =>
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
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Main Menu[/]")
                    .PageSize(12)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(new[]
                    {
                        "Devices",
                        "DNS Servers",
                        "Statistics",
                        "Query Log",
                        "Filter Lists",
                        "Account Info",
                        "Settings",
                        "Exit"
                    }));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "Devices":
                        await _deviceMenu.ShowAsync();
                        break;
                    case "DNS Servers":
                        await _dnsServerMenu.ShowAsync();
                        break;
                    case "Statistics":
                        await _statisticsMenu.ShowAsync();
                        break;
                    case "Query Log":
                        await _queryLogMenu.ShowAsync();
                        break;
                    case "Filter Lists":
                        await _filterListMenu.ShowAsync();
                        break;
                    case "Account Info":
                        await _accountMenu.ShowAsync();
                        break;
                    case "Settings":
                        await ShowSettingsMenuAsync();
                        break;
                    case "Exit":
                        running = false;
                        break;
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
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Settings[/]")
                .PageSize(6)
                .HighlightStyle(new Style(Color.Green))
                .AddChoices(new[]
                {
                    "Change API Key",
                    "Test Connection",
                    "Back"
                }));

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
        await AnsiConsole.Status()
            .StartAsync("Testing connection...", async ctx =>
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
