namespace AdGuard.ApiClient.Benchmarks;

[MemoryDiagnoser]
[MarkdownExporter]
public class ClientUtilsBenchmarks
{
    private readonly DateTime _testDate = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc);
    private readonly List<string> _testList = new() { "item1", "item2", "item3", "item4", "item5" };
    
    [Benchmark(Description = "ParameterToString - DateTime")]
    public string ParameterToString_DateTime()
    {
        return ClientUtils.ParameterToString(_testDate);
    }

    [Benchmark(Description = "ParameterToString - List")]
    public string ParameterToString_List()
    {
        return ClientUtils.ParameterToString(_testList);
    }

    [Benchmark(Description = "ParameterToString - String")]
    public string ParameterToString_String()
    {
        return ClientUtils.ParameterToString("test-value-12345");
    }

    [Benchmark(Description = "ParameterToString - Int")]
    public string ParameterToString_Int()
    {
        return ClientUtils.ParameterToString(12345);
    }
}
