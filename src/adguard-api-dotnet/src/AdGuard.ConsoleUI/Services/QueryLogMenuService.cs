namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Menu service for viewing and managing query logs.
/// Uses Repository pattern for data access and Strategy pattern for display.
/// </summary>
public class QueryLogMenuService : BaseMenuService
{
    private readonly IQueryLogRepository _queryLogRepository;
    private readonly QueryLogDisplayStrategy _displayStrategy;

    public QueryLogMenuService(
        IQueryLogRepository queryLogRepository,
        QueryLogDisplayStrategy displayStrategy)
    {
        _queryLogRepository = queryLogRepository ?? throw new ArgumentNullException(nameof(queryLogRepository));
        _displayStrategy = displayStrategy ?? throw new ArgumentNullException(nameof(displayStrategy));
    }

    /// <inheritdoc />
    public override string Title => "Query Log";

    /// <inheritdoc />
    protected override Dictionary<string, Func<Task>> GetMenuActions() => new()
    {
        { "View Recent Queries (Last Hour)", () => ShowQueryLogAsync(DateTimeExtensions.HoursAgo(1), DateTimeExtensions.Now()) },
        { "View Today's Queries", () => ShowQueryLogAsync(DateTimeExtensions.StartOfToday(), DateTimeExtensions.Now()) },
        { "View Custom Time Range", ShowCustomRangeQueryLogAsync },
        { "Clear Query Log", ClearQueryLogAsync }
    };

    private async Task ShowQueryLogAsync(long fromMillis, long toMillis)
    {
        var queryLog = await ConsoleHelpers.WithStatusAsync(
            "Fetching query log...",
            () => _queryLogRepository.GetQueryLogAsync(fromMillis, toMillis));

        _displayStrategy.Display(queryLog.Items, fromMillis, toMillis);
    }

    private async Task ShowCustomRangeQueryLogAsync()
    {
        var hoursAgo = AnsiConsole.Ask<int>("Hours ago to start from:", 24);
        var fromMillis = DateTimeExtensions.HoursAgo(hoursAgo);
        var toMillis = DateTimeExtensions.Now();

        await ShowQueryLogAsync(fromMillis, toMillis);
    }

    private async Task ClearQueryLogAsync()
    {
        if (!ConsoleHelpers.ConfirmAction("Are you sure you want to [red]clear all query logs[/]?"))
        {
            ConsoleHelpers.ShowCancelled();
            return;
        }

        await ConsoleHelpers.WithStatusAsync(
            "Clearing query log...",
            () => _queryLogRepository.ClearAsync());

        ConsoleHelpers.ShowSuccess("Query log cleared successfully!");
    }
}
