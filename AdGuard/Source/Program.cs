using Code.Extensions;
using Code.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Code;
public class Program
{
    public static async Task Main(string[] args)
    {
        // Build configuration first to access settings
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var webhookUrl = configuration["AdGuardWebHookUrl"] ?? Environment.GetEnvironmentVariable("ADGUARD_WEBHOOK_URL");
        if (string.IsNullOrEmpty(webhookUrl))
        {
            throw new InvalidOperationException("AdGuard webhook URL not configured. Set ADGUARD_WEBHOOK_URL environment variable or configure in appsettings.json");
        }

        var windowOptions = new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            QueueLimit = 10,
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(60),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        };

        using HttpClient httpClient = new(new ClientSideRateLimitedHandler(new FixedWindowRateLimiter(windowOptions)))
        {
            BaseAddress = new(webhookUrl),
            Timeout = TimeSpan.FromSeconds(10),
            DefaultRequestVersion = System.Net.HttpVersion.Version11,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
        };

        using var cts = new CancellationTokenSource();

        await GetAsync(httpClient, cts);
    }

    public static async Task GetAsync(HttpClient httpClient, CancellationTokenSource cts)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(httpClient.BaseAddress, cts.Token);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                await response.WriteRequestToConsoleAsync(cts, Stopwatch.StartNew());
            }
        }
        catch (TaskCanceledException ex) when (cts.IsCancellationRequested)
        {
            // When the token has been canceled, it is not a timeout.
            Console.WriteLine($"Canceled: {ex.Message}");
            cts.Cancel();
        }
    }
}

