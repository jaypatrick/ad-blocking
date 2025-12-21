namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="QueryLogRepository"/>.
/// </summary>
public class QueryLogRepositoryTests : RepositoryTestBase<QueryLogRepository>
{
    /// <inheritdoc />
    protected override QueryLogRepository CreateRepository()
    {
        return new QueryLogRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override QueryLogRepository CreateRepositoryWithNullFactory()
    {
        return new QueryLogRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override QueryLogRepository CreateRepositoryWithNullLogger()
    {
        return new QueryLogRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetQueryLogAsync Tests

    #endregion
}
