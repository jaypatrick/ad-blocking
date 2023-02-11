<Query Kind="Program">
  <NuGetReference>System.Net.Http</NuGetReference>
  <NuGetReference>System.Threading</NuGetReference>
  <NuGetReference>System.Threading.RateLimiting</NuGetReference>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Http</Namespace>
  <Namespace>Microsoft.Extensions.Http.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.Logging.Abstractions</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>Microsoft.Extensions.Primitives</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Threading.RateLimiting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Timers</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
  <IncludeWinSDK>true</IncludeWinSDK>
</Query>

async Task Main()
{
	var windowOptions = new FixedWindowRateLimiterOptions
	{
		AutoReplenishment = true,
		QueueLimit = 10,
		PermitLimit = 10,
		Window = TimeSpan.FromSeconds(10),
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

	public static class HttpResponseMessageExtensions
	{
		public static void WriteRequestToConsole(this HttpResponseMessage response, CancellationTokenSource cts, Stopwatch watch)
		{
			if (response is null)
			{
				return;
			}
			while (!cts.Token.IsCancellationRequested)
			{
				var request = response.RequestMessage;
				Console.Write($"{request?.Method} ");
				Console.Write($"{request?.RequestUri} ");
				Console.Write($"{request?.Content} ");
				Console.WriteLine($"HTTP/{request?.Version}");
				Console.WriteLine($"status: {response.StatusCode}, via HTTP version: {response.Version}");
				Console.WriteLine($" Stopwatch elapsed {watch.Elapsed} milliseconds {watch.ElapsedMilliseconds} and ticks {watch.ElapsedTicks}");
				Console.WriteLine(); Console.WriteLine();
				Console.WriteLine($"{watch.Reset}");
			}
		}
	}

internal sealed class ClientSideRateLimitedHandler
	: DelegatingHandler, IAsyncDisposable
{
	private readonly RateLimiter _rateLimiter;

	public ClientSideRateLimitedHandler(RateLimiter limiter)
		: base(new HttpClientHandler()) => _rateLimiter = limiter;

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		using RateLimitLease lease = await _rateLimiter.AcquireAsync(
			permitCount: 1, cancellationToken);

		if (lease.IsAcquired)
		{
			return await base.SendAsync(request, cancellationToken);
		}

		var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
		if (lease.TryGetMetadata(
				MetadataName.RetryAfter, out TimeSpan retryAfter))
		{
			response.Headers.Add("Retry-After",((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
		}

		return response;
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		await _rateLimiter.DisposeAsync().ConfigureAwait(false);

		Dispose(disposing: false);
		GC.SuppressFinalize(this);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		if (disposing)
		{
			_rateLimiter.Dispose();
		}
	}
}