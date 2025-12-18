using AdGuard.ApiClient.Helpers;

namespace AdGuard.ApiClient.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
public class DateTimeExtensionsBenchmarks
{
    [Benchmark(Description = "DaysAgo calculation")]
    public long DaysAgo()
    {
        return DateTimeExtensions.DaysAgo(7);
    }

    [Benchmark(Description = "HoursAgo calculation")]
    public long HoursAgo()
    {
        return DateTimeExtensions.HoursAgo(24);
    }

    [Benchmark(Description = "Now to Unix milliseconds")]
    public long NowToUnixMillis()
    {
        return DateTimeExtensions.Now();
    }

    [Benchmark(Description = "StartOfToday calculation")]
    public long StartOfToday()
    {
        return DateTimeExtensions.StartOfToday();
    }

    [Benchmark(Description = "EndOfToday calculation")]
    public long EndOfToday()
    {
        return DateTimeExtensions.EndOfToday();
    }

    [Benchmark(Description = "DateTime to Unix milliseconds")]
    public long DateTimeToUnixMillis()
    {
        var date = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
        return date.ToUnixMilliseconds();
    }

    [Benchmark(Description = "Unix milliseconds to DateTime")]
    public DateTime UnixMillisToDateTime()
    {
        return DateTimeExtensions.FromUnixMilliseconds(1705318245000);
    }
}
