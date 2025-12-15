# Performance Benchmarking Guide

## Quick Start

### Run All Benchmarks
```bash
cd src
dotnet run --project AdGuard.ApiClient.Benchmarks\AdGuard.ApiClient.Benchmarks.csproj -c Release
```

### List Available Benchmarks
```bash
cd src  
dotnet run --project AdGuard.ApiClient.Benchmarks\AdGuard.ApiClient.Benchmarks.csproj -c Release -- --list tree
```

## Benchmark Categories

### 1. Retry Policy Benchmarks (`RetryPolicyBenchmarks`)

Tests the performance of retry policy creation and execution patterns.

**Scenarios:**
- `CreatePolicyPerCall` - Creates a new policy for each operation (current pattern)
- `ReuseCachedPolicy` - Reuses a single policy instance (optimized pattern)
- `ExecuteWithRetryHelper` - Uses the helper method (with internal caching)

**Run:**
```bash
dotnet run --project AdGuard.ApiClient.Benchmarks\AdGuard.ApiClient.Benchmarks.csproj -c Release -- --filter "*RetryPolicy*"
```

**Expected Results:**
- Cached policy should be 3-5x faster than creating per call
- Helper method should match cached performance after first call

---

### 2. Client Utils Benchmarks (`ClientUtilsBenchmarks`)

Tests parameter serialization performance for different data types.

**Scenarios:**
- `ParameterToString_DateTime` - DateTime formatting
- `ParameterToString_List` - Collection serialization
- `ParameterToString_String` - String passthrough
- `ParameterToString_Int` - Integer to string

**Run:**
```bash
dotnet run --project AdGuard.ApiClient.Benchmarks\AdGuard.ApiClient.Benchmarks.csproj -c Release -- --filter "*ClientUtils*"
```

**Expected Results:**
- List serialization should show improvement with pre-allocated capacity
- DateTime formatting may be a hot path if called frequently

---

### 3. DateTime Extensions Benchmarks (`DateTimeExtensionsBenchmarks`)

Tests Unix milliseconds conversion and time calculation performance.

**Scenarios:**
- `DaysAgo` - Calculate timestamp N days ago
- `HoursAgo` - Calculate timestamp N hours ago
- `NowToUnixMillis` - Current time to Unix milliseconds
- `StartOfToday` - Start of current day
- `EndOfToday` - End of current day
- `DateTimeToUnixMillis` - DateTime to Unix ms conversion
- `UnixMillisToDateTime` - Unix ms to DateTime conversion

**Run:**
```bash
dotnet run --project AdGuard.ApiClient.Benchmarks\AdGuard.ApiClient.Benchmarks.csproj -c Release -- --filter "*DateTime*"
```

**Expected Results:**
- All operations should be sub-microsecond
- Conversion operations are typically very fast

---

## Benchmark Execution Options

### Memory Diagnostics
Already enabled by default with `[MemoryDiagnoser]` attribute.

Results include:
- Allocated memory per operation
- Gen 0/1/2 garbage collection counts

### Export Results

#### Markdown
```bash
dotnet run -c Release -- --exporters markdown
```

#### HTML
```bash
dotnet run -c Release -- --exporters html
```

#### JSON
```bash
dotnet run -c Release -- --exporters json
```

### Filter Specific Benchmarks
```bash
# Run only retry policy benchmarks
dotnet run -c Release -- --filter "*Retry*"

# Run only specific method
dotnet run -c Release -- --filter "*CreatePolicyPerCall"

# Run multiple categories
dotnet run -c Release -- --filter "*Retry* *DateTime*"
```

### Quick Mode (Faster, Less Accurate)
```bash
dotnet run -c Release -- --job short
```

### Statistical Analysis
```bash
# Run with statistical outlier detection
dotnet run -c Release -- --statisticalTest 3ms

# Run with more iterations for accuracy
dotnet run -c Release -- --iterationCount 100
```

---

## Interpreting Results

### Key Metrics

**Mean Time:**
- Average time per operation
- Lower is better
- Use for comparing different implementations

**Allocated Memory:**
- Bytes allocated per operation
- Lower is better
- Zero allocations is ideal for hot paths

**Gen 0/1/2:**
- Number of garbage collections triggered
- Lower is better
- Indicates memory pressure

### Example Output
```
| Method            | Mean       | Error    | StdDev   | Gen0   | Allocated |
|------------------ |-----------:|---------:|---------:|-------:|----------:|
| CreatePolicyPerCall | 1,234.5 ns | 12.3 ns | 11.5 ns | 0.0153 |     256 B |
| ReuseCachedPolicy  |   245.2 ns |  2.1 ns |  1.9 ns |      - |         - |
```

**Analysis:**
- Cached policy is ~5x faster (1234.5 / 245.2)
- Cached policy has zero allocations (ideal!)
- Creating per call allocates 256 bytes each time

---

## Best Practices

### 1. Always Use Release Build
```bash
# ? Correct
dotnet run -c Release

# ? Wrong - Debug builds have different performance characteristics
dotnet run
```

### 2. Close Background Applications
- Close browsers, IDEs, and other heavy applications
- Benchmarks need consistent system resources
- Run on a quiet machine for accurate results

### 3. Multiple Runs for Confidence
```bash
# Run benchmarks 3 times and compare results
dotnet run -c Release > results1.txt
dotnet run -c Release > results2.txt
dotnet run -c Release > results3.txt
```

### 4. Establish Baselines
```bash
# Before optimization
git checkout main
dotnet run -c Release > baseline.txt

# After optimization
git checkout feature/optimizations
dotnet run -c Release > optimized.txt

# Compare
diff baseline.txt optimized.txt
```

---

## Integration with CI/CD

### GitHub Actions Example
```yaml
name: Benchmarks

on:
  pull_request:
    branches: [ main ]

jobs:
  benchmark:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      
      - name: Run Benchmarks
        run: |
          cd src
          dotnet run --project AdGuard.ApiClient.Benchmarks/AdGuard.ApiClient.Benchmarks.csproj -c Release -- --exporters json
      
      - name: Upload Results
        uses: actions/upload-artifact@v3
        with:
          name: benchmark-results
          path: src/BenchmarkDotNet.Artifacts/results/*.json
```

---

## Troubleshooting

### Issue: "No benchmarks found"
**Solution:** Ensure the project builds successfully:
```bash
dotnet build -c Release
```

### Issue: Slow benchmark execution
**Solution:** Use `--job short` for faster iterations during development:
```bash
dotnet run -c Release -- --job short
```

### Issue: High variance in results
**Solution:** Close background applications and run multiple times:
```bash
# Increase warmup and iteration counts
dotnet run -c Release -- --warmupCount 5 --iterationCount 20
```

### Issue: Out of memory
**Solution:** Run benchmarks individually:
```bash
dotnet run -c Release -- --filter "*Retry*"
dotnet run -c Release -- --filter "*DateTime*"
dotnet run -c Release -- --filter "*ClientUtils*"
```

---

## Further Reading

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html)
- [.NET Performance Tips](https://learn.microsoft.com/en-us/dotnet/framework/performance/performance-tips)
- [Benchmark Design Guidelines](https://benchmarkdotnet.org/articles/guides/good-practices.html)

---

**Last Updated:** 2024  
**BenchmarkDotNet Version:** 0.14.0  
**Target Framework:** .NET 8.0
