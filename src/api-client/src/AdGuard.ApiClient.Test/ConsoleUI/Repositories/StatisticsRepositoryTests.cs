using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="StatisticsRepository"/>.
/// </summary>
public class StatisticsRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;
    private readonly Mock<ILogger<StatisticsRepository>> _loggerMock;

    public StatisticsRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _loggerMock = new Mock<ILogger<StatisticsRepository>>();
    }

    private StatisticsRepository CreateRepository()
    {
        return new StatisticsRepository(_apiClientFactoryMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new StatisticsRepository(null!, _loggerMock.Object));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new StatisticsRepository(_apiClientFactoryMock.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    #endregion

    #region GetTimeQueriesStatsAsync Tests

    [Fact]
    public async Task GetTimeQueriesStatsAsync_ReturnsStats()
    {
        // Arrange
        var fromMillis = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeMilliseconds();
        var toMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var expectedStats = new TimeQueriesStatsList();

        var mockApi = new Mock<StatisticsApi>();
        mockApi.Setup(a => a.GetTimeQueriesStatsAsync(fromMillis, toMillis, default))
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
