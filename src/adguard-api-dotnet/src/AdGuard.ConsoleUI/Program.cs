namespace AdGuard.ConsoleUI;

/// <summary>
/// Entry point for the AdGuard DNS Console UI application.
/// </summary>
/// <remarks>
/// This application provides a console-based interface for managing AdGuard DNS
/// accounts, devices, DNS servers, and viewing statistics and query logs.
/// </remarks>
public class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code: 0 for success, 1 for failure.</returns>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var configuration = BuildConfiguration();
            var services = ConfigureServices(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Configure logging for the API client helpers
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            RetryPolicyHelper.SetLogger(loggerFactory.CreateLogger("RetryPolicy"));

            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("AdGuard DNS Console UI starting");

            var app = serviceProvider.GetRequiredService<ConsoleApplication>();
            await app.RunAsync();

            logger.LogInformation("AdGuard DNS Console UI shutting down gracefully");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    /// <summary>
    /// Builds the application configuration from various sources.
    /// </summary>
    /// <returns>The built configuration.</returns>
    /// <remarks>
    /// Configuration sources (in order of precedence):
    /// <list type="number">
    /// <item><description>appsettings.json (optional)</description></item>
    /// <item><description>Environment variables with ADGUARD_ prefix</description></item>
    /// </list>
    /// </remarks>
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("ADGUARD_")
            .Build();
    }

    /// <summary>
    /// Configures the dependency injection container with all required services.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service collection.</returns>
    /// <remarks>
    /// Uses <see cref="ServiceCollectionExtensions.AddAdGuardConsoleUI"/> to register all services
    /// including repositories, display strategies, menu services, and the main application.
    /// </remarks>
    private static IServiceCollection ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        // Configuration
        services.AddSingleton(configuration);

        // Logging - file-based logging to avoid interfering with Spectre.Console
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Register all Console UI services (repositories, display strategies, menu services)
        services.AddAdGuardConsoleUI();

        return services;
    }
}
