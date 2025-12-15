using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using AdGuard.Repositories.Implementations;
using AdGuard.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="DnsServerRepository"/>.
/// </summary>
public class DnsServerRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;
    private readonly Mock<ILogger<DnsServerRepository>> _loggerMock;

    public DnsServerRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _loggerMock = new Mock<ILogger<DnsServerRepository>>();
    }

    private DnsServerRepository CreateRepository()
    {
        return new DnsServerRepository(_apiClientFactoryMock.Object, _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new DnsServerRepository(null!, _loggerMock.Object));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new DnsServerRepository(_apiClientFactoryMock.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidFactory_CreatesInstance()
    {
        // Act
        var repository = CreateRepository();

        // Assert
        Assert.NotNull(repository);
    }

    #endregion

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
