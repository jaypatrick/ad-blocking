using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Repositories;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="QueryLogRepository"/>.
/// </summary>
public class QueryLogRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;

    public QueryLogRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
    }

    private QueryLogRepository CreateRepository()
    {
        return new QueryLogRepository(_apiClientFactoryMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new QueryLogRepository(null!));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    #endregion

    #region GetQueryLogAsync Tests

    [Fact]
    public async Task GetQueryLogAsync_ReturnsQueryLog()
    {
        // Arrange
        var fromMillis = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeMilliseconds();
        var toMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expectedResponse = new QueryLogResponse();

        var mockApi = new Mock<QueryLogApi>();
        mockApi.Setup(a => a.GetQueryLogAsync(fromMillis, toMillis, null, null, null, null, null, 0, default))
            .ReturnsAsync(expectedResponse);

        _apiClientFactoryMock.Setup(f => f.CreateQueryLogApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetQueryLogAsync(fromMillis, toMillis);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region ClearAsync Tests

    [Fact]
    public async Task ClearAsync_CallsApi()
    {
        // Arrange
        var mockApi = new Mock<QueryLogApi>();
        mockApi.Setup(a => a.ClearQueryLogAsync(0, default))
            .Returns(Task.CompletedTask);

        _apiClientFactoryMock.Setup(f => f.CreateQueryLogApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        await repository.ClearAsync();

        // Assert
        mockApi.Verify(a => a.ClearQueryLogAsync(0, default), Times.Once);
    }

    #endregion
}
