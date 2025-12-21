namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="DnsServerRepository"/>.
/// </summary>
public class DnsServerRepositoryTests : RepositoryTestBase<DnsServerRepository>
{
    /// <inheritdoc />
    protected override DnsServerRepository CreateRepository()
    {
        return new DnsServerRepository(ApiClientFactoryMock.Object, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override DnsServerRepository CreateRepositoryWithNullFactory()
    {
        return new DnsServerRepository(null!, LoggerMock.Object);
    }

    /// <inheritdoc />
    protected override DnsServerRepository CreateRepositoryWithNullLogger()
    {
        return new DnsServerRepository(ApiClientFactoryMock.Object, null!);
    }

    #region GetAllAsync Tests

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.GetByIdAsync(null!));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithNullServer_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.CreateAsync(null!));
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ThrowsNotSupportedException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        // DNS server deletion is not supported by the AdGuard DNS API
        await Assert.ThrowsAsync<NotSupportedException>(
            () => repository.DeleteAsync("any-id"));
    }

    #endregion
}
