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
/// Unit tests for <see cref="FilterListRepository"/>.
/// </summary>
public class FilterListRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;
    private readonly Mock<ILogger<FilterListRepository>> _loggerMock;

    public FilterListRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _loggerMock = new Mock<ILogger<FilterListRepository>>();
    }

    private FilterListRepository CreateRepository()
    {
        return new FilterListRepository(_apiClientFactoryMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FilterListRepository(null!, _loggerMock.Object));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FilterListRepository(_apiClientFactoryMock.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsFilterLists()
    {
        // Arrange
        var expectedFilters = new List<FilterList>
        {
            new FilterList(filterId: "filter1", name: "AdGuard Base"),
            new FilterList(filterId: "filter2", name: "Social Media")
        };

        var mockApi = new Mock<FilterListsApi>();
        mockApi.Setup(a => a.ListFilterListsAsync(default))
            .ReturnsAsync(expectedFilters);

        _apiClientFactoryMock.Setup(f => f.CreateFilterListsApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("filter1", result[0].FilterId);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        var mockApi = new Mock<FilterListsApi>();
        mockApi.Setup(a => a.ListFilterListsAsync(default))
            .ReturnsAsync(new List<FilterList>());

        _apiClientFactoryMock.Setup(f => f.CreateFilterListsApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}
