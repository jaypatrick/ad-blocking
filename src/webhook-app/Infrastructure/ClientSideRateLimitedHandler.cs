using Microsoft.Extensions.Logging;

namespace Code.Infrastructure;

/// <summary>
/// HTTP message handler that implements client-side rate limiting.
/// </summary>
/// <remarks>
/// This handler uses a <see cref="RateLimiter"/> to control the rate of outgoing HTTP requests.
/// When the rate limit is exceeded, it returns a 429 Too Many Requests response instead of
/// sending the request to the server.
/// </remarks>
internal sealed class ClientSideRateLimitedHandler
    : DelegatingHandler, IAsyncDisposable
{
    private readonly RateLimiter _rateLimiter;
    private readonly ILogger<ClientSideRateLimitedHandler>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientSideRateLimitedHandler"/> class.
    /// </summary>
    /// <param name="limiter">The rate limiter to use for controlling request rates.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limiter"/> is null.</exception>
    public ClientSideRateLimitedHandler(RateLimiter limiter, ILogger<ClientSideRateLimitedHandler>? logger = null)
        : base(new HttpClientHandler())
    {
        _rateLimiter = limiter ?? throw new ArgumentNullException(nameof(limiter));
        _logger = logger;
        _logger?.LogDebug("ClientSideRateLimitedHandler initialized");
    }

    /// <summary>
    /// Sends an HTTP request with rate limiting applied.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message, or a 429 response if rate limited.</returns>
    /// <remarks>
    /// If a rate limit lease cannot be acquired, this method returns a synthetic 429 response
    /// with a Retry-After header indicating when the request can be retried.
    /// </remarks>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Attempting to acquire rate limit lease for request to {Uri}",
            request.RequestUri);

        using RateLimitLease lease = await _rateLimiter.AcquireAsync(
            permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
        {
            _logger?.LogDebug("Rate limit lease acquired, sending request");
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                _logger?.LogDebug("Request completed with status code {StatusCode}", response.StatusCode);
                return response;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Request failed with exception");
                throw;
            }
        }

        _logger?.LogWarning("Rate limit exceeded, returning 429 Too Many Requests");

        var rateLimitResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            var retryAfterSeconds = (int)retryAfter.TotalSeconds;
            rateLimitResponse.Headers.Add("Retry-After", retryAfterSeconds.ToString(NumberFormatInfo.InvariantInfo));
            _logger?.LogInformation("Rate limit retry after {Seconds} seconds", retryAfterSeconds);
        }

        return rateLimitResponse;
    }

    /// <summary>
    /// Asynchronously disposes of the handler and its rate limiter.
    /// </summary>
    /// <returns>A value task representing the asynchronous dispose operation.</returns>
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        _logger?.LogDebug("Disposing ClientSideRateLimitedHandler asynchronously");

        await _rateLimiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the handler and its rate limiter.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _logger?.LogDebug("Disposing ClientSideRateLimitedHandler");
            _rateLimiter.Dispose();
        }
    }
}
