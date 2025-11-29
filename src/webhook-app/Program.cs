using Code.Extensions;
using Code.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Code;

/// <summary>
/// Entry point for the AdGuard Webhook application.
/// </summary>
/// <remarks>
/// This application triggers AdGuard DNS updates via webhook calls.
/// It includes rate limiting to prevent overwhelming the API.
/// </remarks>
public class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when webhook URL is not configured.</exception>
    public static async Task Main(string[] args)
    {
        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("AdGuard Webhook application starting");

        try
        {
            // Build configuration
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var webhookUrl = GetWebhookUrl(configuration, logger);

            // Configure rate limiter
            var windowOptions = new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                QueueLimit = 10,
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(60),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            };

            logger.LogDebug("Rate limiter configured: PermitLimit={PermitLimit}, Window={Window}s, QueueLimit={QueueLimit}",
                windowOptions.PermitLimit, windowOptions.Window.TotalSeconds, windowOptions.QueueLimit);

            using HttpClient httpClient = new(new ClientSideRateLimitedHandler(
                new FixedWindowRateLimiter(windowOptions),
                serviceProvider.GetRequiredService<ILogger<ClientSideRateLimitedHandler>>()))
            {
                BaseAddress = new Uri(webhookUrl),
                Timeout = TimeSpan.FromSeconds(10),
                DefaultRequestVersion = System.Net.HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

            using var cts = new CancellationTokenSource();

            await ExecuteWebhookAsync(httpClient, cts, logger);

            logger.LogInformation("AdGuard Webhook application completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "AdGuard Webhook application failed with unhandled exception");
            throw;
        }
    }

    /// <summary>
    /// Configures the dependency injection services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
    }

    /// <summary>
    /// Gets the webhook URL from configuration or environment variables.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger instance.</param>
    /// <returns>The webhook URL.</returns>
    /// <exception cref="InvalidOperationException">Thrown when webhook URL is not configured.</exception>
    private static string GetWebhookUrl(IConfiguration configuration, ILogger logger)
    {
        var webhookUrl = configuration["AdGuardWebHookUrl"]
            ?? Environment.GetEnvironmentVariable("ADGUARD_WEBHOOK_URL");

        if (string.IsNullOrEmpty(webhookUrl))
        {
            logger.LogError("Webhook URL not configured");
            throw new InvalidOperationException(
                "AdGuard webhook URL not configured. Set ADGUARD_WEBHOOK_URL environment variable or configure AdGuardWebHookUrl in appsettings.json");
        }

        logger.LogInformation("Webhook URL configured: {WebhookUrl}", MaskUrl(webhookUrl));
        return webhookUrl;
    }

    /// <summary>
    /// Masks the webhook URL for logging (hides sensitive parts).
    /// </summary>
    /// <param name="url">The URL to mask.</param>
    /// <returns>The masked URL.</returns>
    private static string MaskUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return $"{uri.Scheme}://{uri.Host}:{uri.Port}/***";
        }
        catch
        {
            return "***";
        }
    }

    /// <summary>
    /// Executes the webhook request asynchronously.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use.</param>
    /// <param name="cts">The cancellation token source.</param>
    /// <param name="logger">The logger instance.</param>
    public static async Task ExecuteWebhookAsync(HttpClient httpClient, CancellationTokenSource cts, ILogger logger)
    {
        logger.LogInformation("Executing webhook request to {BaseAddress}", httpClient.BaseAddress);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(httpClient.BaseAddress, cts.Token);

            stopwatch.Stop();
            logger.LogInformation("Webhook response received: {StatusCode} in {ElapsedMs}ms",
                response.StatusCode, stopwatch.ElapsedMilliseconds);

            if (response.IsSuccessStatusCode)
            {
                await response.WriteRequestToConsoleAsync(cts, stopwatch, logger);
            }
            else
            {
                logger.LogWarning("Webhook request failed with status code: {StatusCode}", response.StatusCode);
                var content = await response.Content.ReadAsStringAsync(cts.Token);
                logger.LogDebug("Response content: {Content}", content);
            }
        }
        catch (TaskCanceledException ex) when (cts.IsCancellationRequested)
        {
            logger.LogWarning("Webhook request was cancelled: {Message}", ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError("Webhook request timed out: {Message}", ex.Message);
            throw;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
            throw;
        }
    }
}
