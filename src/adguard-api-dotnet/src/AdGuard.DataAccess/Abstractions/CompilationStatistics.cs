namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Represents compilation statistics summary.
/// </summary>
public record CompilationStatistics
{
    /// <summary>
    /// Gets the total number of compilations.
    /// </summary>
    public int TotalCompilations { get; init; }

    /// <summary>
    /// Gets the number of successful compilations.
    /// </summary>
    public int SuccessfulCompilations { get; init; }

    /// <summary>
    /// Gets the number of failed compilations.
    /// </summary>
    public int FailedCompilations { get; init; }

    /// <summary>
    /// Gets the average compilation duration in milliseconds.
    /// </summary>
    public double AverageDurationMs { get; init; }

    /// <summary>
    /// Gets the total rules compiled across all successful compilations.
    /// </summary>
    public long TotalRulesCompiled { get; init; }

    /// <summary>
    /// Gets the average rules per compilation.
    /// </summary>
    public double AverageRulesPerCompilation { get; init; }

    /// <summary>
    /// Gets the success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalCompilations > 0 ? (double)SuccessfulCompilations / TotalCompilations * 100 : 0;
}