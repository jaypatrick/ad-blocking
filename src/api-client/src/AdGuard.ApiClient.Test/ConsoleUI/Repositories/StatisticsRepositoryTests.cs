using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Repositories;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="StatisticsRepository"/>.
/// </summary>
public class StatisticsRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;

    public StatisticsRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
    }

    private StatisticsRepository CreateRepository()
    {
        return new StatisticsRepository(_apiClientFactoryMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new StatisticsRepository(null!));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    #endregion

    #region GetTimeQueriesStatsAsync Tests

    [Fact]
    public async Task GetTimeQueriesStatsAsync_ReturnsStats()
    {
        // Arrange
        var fromMillis = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();
        var toMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expectedStats = new StatsResponse();

        var mockApi = new Mock<StatisticsApi>();
        mockApi.Setup(a => a.GetTimeQueriesStatsAsync(fromMillis, toMillis, null, null, 0, default))
            .ReturnsAsync(expectedStats);

        _apiClientFactoryMock.Setup(f => f.CreateStatisticsApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetTimeQueriesStatsAsync(fromMillis, toMillis);

        // Assert
        Assert.NotNull(result);
        _apiClientFactoryMock.Verify(f => f.CreateStatisticsApi(), Times.Once);
    }

    #endregion
}
