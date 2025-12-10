using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RulesCompiler.Abstractions;
using RulesCompiler.Console.Helpers;
using RulesCompiler.Models;
using Spectre.Console;

namespace RulesCompiler.Console.Services;

/// <summary>
/// Main console application that provides both interactive and command-line modes.
/// </summary>
public class ConsoleApplication
{
    private readonly ILogger<ConsoleApplication> _logger;
    private readonly IRulesCompilerService _compilerService;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleApplication"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="compilerService">The compiler service.</param>
    /// <param name="configuration">The application configuration.</param>
    public ConsoleApplication(
        ILogger<ConsoleApplication> logger,
        IRulesCompilerService compilerService,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _compilerService = compilerService ?? throw new ArgumentNullException(nameof(compilerService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Runs the console application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    public async Task<int> RunAsync(string[] args)
    {
        // Check for command-line mode
        var configPath = _configuration["config"] ?? _configuration["c"];
        var compileOnly = _configuration.GetValue<bool>("compile");
        var copyToRules = _configuration.GetValue<bool>("copy") || _configuration.GetValue<bool>("CopyToRules");
        var showVersion = _configuration.GetValue<bool>("version") || _configuration.GetValue<bool>("v");

        if (showVersion)
        {
            await ShowVersionInfoAsync();
            return 0;
        }

        if (compileOnly || !string.IsNullOrEmpty(configPath))
        {
            // Command-line mode
            return await RunCompilationAsync(configPath, copyToRules);
        }

        // Interactive mode
        return await RunInteractiveAsync();
    }

    private async Task<int> RunInteractiveAsync()
    {
        DisplayWelcomeBanner();

        var running = true;
        var exitCode = 0;

        while (running)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]What would you like to do?[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(
                        "View Configuration",
                        "Compile Rules",
                        "Compile and Copy to Rules",
                        "Version Info",
                        "Exit"));

            AnsiConsole.WriteLine();

            try
            {
                switch (choice)
                {
                    case "View Configuration":
                        await ViewConfigurationAsync();
                        break;
                    case "Compile Rules":
                        await CompileRulesAsync(copyToRules: false);
                        break;
                    case "Compile and Copy to Rules":
                        await CompileRulesAsync(copyToRules: true);
                        break;
                    case "Version Info":
                        await ShowVersionInfoAsync();
                        break;
                    case "Exit":
                        running = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing menu option");
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                exitCode = 1;
            }

            AnsiConsole.WriteLine();
        }

        AnsiConsole.MarkupLine("[grey]Goodbye![/]");
        return exitCode;
    }

    private static void DisplayWelcomeBanner()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Rules Compiler")
                .LeftJustified()
                .Color(Color.Green));

        AnsiConsole.Write(new Rule("[green]AdGuard Filter Rules Compiler[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
    }

    private async Task ViewConfigurationAsync()
    {
        var configPath = await PromptForConfigPathAsync();

        await AnsiConsole.Status()
            .StartAsync("Reading configuration...", async ctx =>
            {
                var config = await _compilerService.ReadConfigurationAsync(configPath);
                DisplayConfiguration(config);
            });
    }

    private async Task CompileRulesAsync(bool copyToRules)
    {
        var configPath = await PromptForConfigPathAsync();

        await AnsiConsole.Status()
            .StartAsync("Compiling rules...", async ctx =>
            {
                ctx.Status("Reading configuration...");
                var result = await _compilerService.RunAsync(
                    configPath,
                    copyToRules: copyToRules);

                DisplayResult(result);
            });
    }

    private async Task<int> RunCompilationAsync(string? configPath, bool copyToRules)
    {
        try
        {
            await AnsiConsole.Status()
                .StartAsync("Compiling rules...", async ctx =>
                {
                    var result = await _compilerService.RunAsync(
                        configPath,
                        copyToRules: copyToRules);

                    DisplayResult(result);
                });

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    private async Task ShowVersionInfoAsync()
    {
        var info = await _compilerService.GetVersionInfoAsync();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Component[/]")
            .AddColumn("[green]Version[/]");

        table.AddRow("Module", info.ModuleVersion);
        table.AddRow(".NET Runtime", info.DotNetVersion);
        table.AddRow("Node.js", info.NodeVersion ?? "[grey]Not found[/]");
        table.AddRow("hostlist-compiler", info.HostlistCompilerVersion ?? "[grey]Not found[/]");
        table.AddRow("Platform", $"{info.Platform.OSName} {info.Platform.Architecture}");

        AnsiConsole.Write(table);
    }

    private async Task<string?> PromptForConfigPathAsync()
    {
        var defaultPath = _configuration["config"];

        if (!string.IsNullOrEmpty(defaultPath))
            return defaultPath;

        return AnsiConsole.Prompt(
            new TextPrompt<string>("Enter config path (or press Enter for default):")
                .AllowEmpty());
    }

    private static void DisplayConfiguration(CompilerConfiguration config)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Property[/]")
            .AddColumn("[green]Value[/]");

        table.AddRow("Name", Markup.Escape(config.Name));
        table.AddRow("Version", config.Version);
        table.AddRow("License", config.License);
        table.AddRow("Homepage", Markup.Escape(config.Homepage));
        table.AddRow("Sources", config.Sources.Count.ToString());
        table.AddRow("Transformations", string.Join(", ", config.Transformations));

        AnsiConsole.Write(table);

        if (config.Sources.Count > 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Sources:[/]");

            var sourcesTable = new Table()
                .Border(TableBorder.Simple)
                .AddColumn("Name")
                .AddColumn("Type")
                .AddColumn("Source");

            foreach (var source in config.Sources)
            {
                sourcesTable.AddRow(
                    Markup.Escape(source.Name),
                    source.Type,
                    Markup.Escape(source.Source.Length > 60
                        ? source.Source[..57] + "..."
                        : source.Source));
            }

            AnsiConsole.Write(sourcesTable);
        }
    }

    private static void DisplayResult(CompilerResult result)
    {
        AnsiConsole.WriteLine();

        if (result.Success)
        {
            AnsiConsole.MarkupLine("[green]Compilation successful![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Compilation failed![/]");
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(result.ErrorMessage)}[/]");
            }
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Property[/]")
            .AddColumn("[green]Value[/]");

        table.AddRow("Config Name", Markup.Escape(result.ConfigName));
        table.AddRow("Config Version", result.ConfigVersion);
        table.AddRow("Rule Count", result.RuleCount.ToString("N0"));
        table.AddRow("Output Path", Markup.Escape(result.OutputPath));
        table.AddRow("Output Hash", result.OutputHash[..32] + "...");
        table.AddRow("Elapsed Time", $"{result.ElapsedMs:N0} ms");

        if (result.CopiedToRules)
        {
            table.AddRow("Copied To", Markup.Escape(result.RulesDestination ?? "N/A"));
        }

        AnsiConsole.Write(table);
    }
}
