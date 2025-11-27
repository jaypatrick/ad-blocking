using Microsoft.Extensions.Logging;

namespace Code.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Writes detailed HTTP request and response information to the console and logs.
    /// </summary>
    /// <param name="response">The HTTP response message to process.</param>
    /// <param name="cts">The cancellation token source for async operations.</param>
    /// <param name="watch">The stopwatch measuring the request duration.</param>
    /// <param name="logger">Optional logger for structured logging output.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method outputs:
    /// <list type="bullet">
    /// <item><description>HTTP method and URI</description></item>
    /// <item><description>HTTP version</description></item>
    /// <item><description>Response status code</description></item>
    /// <item><description>Response content</description></item>
    /// <item><description>Elapsed time statistics</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var response = await httpClient.GetAsync(url);
    /// await response.WriteRequestToConsoleAsync(cts, stopwatch, logger);
    /// </code>
    /// </example>
    public static async Task WriteRequestToConsoleAsync(
        this HttpResponseMessage response,
        CancellationTokenSource cts,
        Stopwatch watch,
        ILogger? logger = null)
    {
        if (response is null)
        {
            logger?.LogWarning("Attempted to write null response to console");
            return;
        }

        var request = response.RequestMessage;

        // Log request details
        logger?.LogDebug("Processing response for {Method} {Uri}",
            request?.Method, request?.RequestUri);

        Console.WriteLine($"Method: {request?.Method}");
        Console.WriteLine($"Uri: {request?.RequestUri}");
        Console.WriteLine($"Content: {request?.Content}");
        Console.WriteLine($"HTTP Version: {request?.Version}");
        Console.WriteLine($"Status: {response.StatusCode}, via HTTP version: {response.Version}");

        try
        {
            string responseContent = await response.Content.ReadAsStringAsync(cts.Token);

            // Truncate long responses for display
            var displayContent = responseContent.Length > 1000
                ? responseContent[..1000] + "... (truncated)"
                : responseContent;

            Console.WriteLine($"Response message: {displayContent}");

            logger?.LogInformation("Response received: {StatusCode}, Content length: {Length} bytes",
                response.StatusCode, responseContent.Length);
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning("Reading response content was cancelled");
            Console.WriteLine("Response message: (cancelled)");
        }

        Console.WriteLine($"Stopwatch elapsed: {watch.Elapsed} (ms: {watch.ElapsedMilliseconds}, ticks: {watch.ElapsedTicks})");
        Console.WriteLine();
        Console.WriteLine();

        logger?.LogDebug("Response processing completed in {ElapsedMs}ms", watch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Checks if the response indicates a rate limit was exceeded.
    /// </summary>
    /// <param name="response">The HTTP response to check.</param>
    /// <returns>True if the response status is 429 Too Many Requests; otherwise, false.</returns>
    public static bool IsRateLimited(this HttpResponseMessage response)
    {
        return response?.StatusCode == System.Net.HttpStatusCode.TooManyRequests;
    }

    /// <summary>
    /// Gets the Retry-After header value from the response, if present.
    /// </summary>
    /// <param name="response">The HTTP response to check.</param>
    /// <returns>The retry-after duration, or null if not present.</returns>
    public static TimeSpan? GetRetryAfter(this HttpResponseMessage response)
    {
        if (response?.Headers.RetryAfter?.Delta != null)
        {
            return response.Headers.RetryAfter.Delta;
        }

        if (response?.Headers.RetryAfter?.Date != null)
        {
            return response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
        }

        return null;
    }
}
