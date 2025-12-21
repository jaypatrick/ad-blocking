namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="StatisticsRepository"/>.
/// </summary>
public class StatisticsRepositoryTests : RepositoryTestBase<StatisticsRepository>
{
    /// <inheritdoc />
    protected override StatisticsRepository CreateRepository()
    {
        return new StatisticsRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override StatisticsRepository CreateRepositoryWithNullFactory()
    {
        return new StatisticsRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override StatisticsRepository CreateRepositoryWithNullLogger()
    {
        return new StatisticsRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetTimeQueriesStatsAsync Tests

    #endregion
}
