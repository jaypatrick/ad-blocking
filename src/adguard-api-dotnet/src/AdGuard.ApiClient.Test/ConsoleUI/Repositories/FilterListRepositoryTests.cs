namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="FilterListRepository"/>.
/// </summary>
public class FilterListRepositoryTests : RepositoryTestBase<FilterListRepository>
{
    /// <inheritdoc />
    protected override FilterListRepository CreateRepository()
    {
        return new FilterListRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override FilterListRepository CreateRepositoryWithNullFactory()
    {
        return new FilterListRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override FilterListRepository CreateRepositoryWithNullLogger()
    {
        return new FilterListRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetAllAsync Tests

    #endregion
}
