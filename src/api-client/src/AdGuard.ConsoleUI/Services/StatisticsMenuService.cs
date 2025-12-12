namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for viewing statistics.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class StatisticsMenuService : BaseMenuService
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly StatisticsDisplayStrategy _displayStrategy;

    public StatisticsMenuService(
        IStatisticsRepository statisticsRepository,
        StatisticsDisplayStrategy displayStrategy)
    {
        _statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public override string Title => "Statistics";

    /// <inheritdoc />
    protected override Dictionary<string, Func<Task>> GetMenuActions() => new()
    {
        { "Last 24 Hours", () => ShowStatisticsAsync(DateTimeExtensions.HoursAgo(24), DateTimeExtensions.Now()) },
        { "Last 7 Days", () => ShowStatisticsAsync(DateTimeExtensions.DaysAgo(7), DateTimeExtensions.Now()) },
        { "Last 30 Days", () => ShowStatisticsAsync(DateTimeExtensions.DaysAgo(30), DateTimeExtensions.Now()) },
        { "Custom Time Range", ShowCustomRangeStatisticsAsync }
    };

    private async Task ShowStatisticsAsync(long fromMillis, long toMillis)
    {
        var stats = await ConsoleHelpers.WithStatusAsync(
            "Fetching statistics...",
            () => _statisticsRepository.GetTimeQueriesStatsAsync(fromMillis, toMillis));

        _displayStrategy.Display(stats.Stats, fromMillis, toMillis);
    }

    private async Task ShowCustomRangeStatisticsAsync()
    {
        var daysAgo = AnsiConsole.Ask<int>("Days ago to start from:", 7);
        var fromMillis = DateTimeExtensions.DaysAgo(daysAgo);
        var toMillis = DateTimeExtensions.Now();

        await ShowStatisticsAsync(fromMillis, toMillis);
    }
}
