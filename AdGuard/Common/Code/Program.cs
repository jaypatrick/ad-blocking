using Code.Extensions;
using Code.Infrastructure;

namespace Code;
public class Program
{
    public static async Task Main(string[] args)
    {
        var windowOptions = new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            QueueLimit = 10,
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(60),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        };

        using HttpClient httpClient = new(new ClientSideRateLimitedHandler(new FixedWindowRateLimiter(windowOptions)))
        //handler: new ClientSideRateLimitedHandler(
        //	limiter: new TokenBucketRateLimiter(tokenOptions)))
        {
            BaseAddress = new("https://linkip.adguard-dns.com/linkip/db94e3e9/8AdnEQlPCjyMaX74vTDZkraUDUYpCFiZ1tcH8dSk9VH"),
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
                response.EnsureSuccessStatusCode().WriteRequestToConsole(cts, Stopwatch.StartNew());
                await using Stream responseStream = await response.Content.ReadAsStreamAsync();
                string responseString = await response.Content.ReadAsStringAsync();
                byte[] responseByteArray = await response.Content.ReadAsByteArrayAsync();
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

