namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Main console application that orchestrates menu navigation.
/// Uses dependency injection for all menu services.
/// </summary>
public class ConsoleApplication
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly Dictionary<string, IMenuService> _menuServices;

    /// <summary>
    /// Defines the display order for menu items.
    /// Services not in this list will be appended alphabetically at the end.
    /// </summary>
    private static readonly string[] MenuOrder =
    [
        "Devices",
        "DNS Servers",
        "User Rules",
        "Statistics",
        "Query Log",
        "Filter Lists",
        "Web Services",
        "Dedicated IP Addresses",
        "Account Info"
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
    /// </summary>
    /// <param name="apiClientFactory">The API client factory.</param>
    /// <param name="menuServices">Collection of menu services to display.</param>
    /// <exception cref="ArgumentNullException">Thrown when apiClientFactory or menuServices is null.</exception>
    public ConsoleApplication(
        IApiClientFactory apiClientFactory,
        IEnumerable<IMenuService> menuServices)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
        ArgumentNullException.ThrowIfNull(menuServices);

        // Build menu dictionary from Title property, maintaining desired order
        var servicesByTitle = menuServices.ToDictionary(s => s.Title, s => s);
        _menuServices = new Dictionary<string, IMenuService>();

        // Add services in defined order
        foreach (var title in MenuOrder)
        {
            if (servicesByTitle.TryGetValue(title, out var service))
            {
                _menuServices[title] = service;
                servicesByTitle.Remove(title);
            }
        }

        // Add any remaining services alphabetically
        foreach (var kvp in servicesByTitle.OrderBy(kvp => kvp.Key))
        {
            _menuServices[kvp.Key] = kvp.Value;
        }
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
