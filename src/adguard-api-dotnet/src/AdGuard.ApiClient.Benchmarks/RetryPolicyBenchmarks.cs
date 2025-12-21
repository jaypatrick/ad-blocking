namespace AdGuard.ApiClient.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
public class RetryPolicyBenchmarks
{
    private const int Iterations = 100;

    [GlobalSetup]
    public void Setup()
    {
        RetryPolicyHelper.SetLogger(NullLogger.Instance);
    }

    [Benchmark(Description = "Create policy per call (current)")]
    public async Task CreatePolicyPerCall()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var policy = RetryPolicyHelper.CreateDefaultRetryPolicy<int>(maxRetries: 3);
            await policy.ExecuteAsync(() => Task.FromResult(42));
        }
    }

    [Benchmark(Description = "Reuse cached policy")]
    public async Task ReuseCachedPolicy()
    {
        var policy = RetryPolicyHelper.CreateDefaultRetryPolicy<int>(maxRetries: 3);
        for (int i = 0; i < Iterations; i++)
        {
            await policy.ExecuteAsync(() => Task.FromResult(42));
        }
    }

    [Benchmark(Description = "Execute with retry helper")]
    public async Task ExecuteWithRetryHelper()
    {
        for (int i = 0; i < Iterations; i++)
        {
            await RetryPolicyHelper.ExecuteWithRetryAsync(() => Task.FromResult(42), maxRetries: 3);
        }
    }
}
