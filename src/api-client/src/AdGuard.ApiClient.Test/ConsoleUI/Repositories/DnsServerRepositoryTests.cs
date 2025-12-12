using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using AdGuard.ConsoleUI.Repositories;
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

    [Fact]
    public async Task GetAllAsync_CallsApiAndReturnsServers()
    {
        // Arrange
        var expectedServers = new List<DNSServer>
        {
            new DNSServer(id: "srv1", name: "Server 1", varDefault: true),
            new DNSServer(id: "srv2", name: "Server 2", varDefault: false)
        };

        var mockApi = new Mock<DNSServersApi>();
        mockApi.Setup(a => a.ListDNSServersAsync(default))
            .ReturnsAsync(expectedServers);

        _apiClientFactoryMock.Setup(f => f.CreateDnsServersApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("srv1", result[0].Id);
        Assert.True(result[0].Default);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsServer()
    {
        // Arrange
        var serverId = "srv1";
        var servers = new List<DNSServer>
        {
            new DNSServer(id: serverId, name: "Server 1", varDefault: true),
            new DNSServer(id: "srv2", name: "Server 2", varDefault: false)
        };

        var mockApi = new Mock<DNSServersApi>();
        mockApi.Setup(a => a.ListDNSServersAsync(default))
            .ReturnsAsync(servers);

        _apiClientFactoryMock.Setup(f => f.CreateDnsServersApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetByIdAsync(serverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(serverId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowsEntityNotFoundException()
    {
        // Arrange
        var servers = new List<DNSServer>
        {
            new DNSServer(id: "srv1", name: "Server 1", varDefault: true)
        };

        var mockApi = new Mock<DNSServersApi>();
        mockApi.Setup(a => a.ListDNSServersAsync(default))
            .ReturnsAsync(servers);

        _apiClientFactoryMock.Setup(f => f.CreateDnsServersApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => repository.GetByIdAsync("non-existent-id"));
    }

    [Fact]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetByIdAsync(null!));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidServer_ReturnsCreatedServer()
    {
        // Arrange
        var serverCreate = new DNSServerCreate(name: "New Server");
        var createdServer = new DNSServer(id: "new-srv", name: "New Server", varDefault: false);

        var mockApi = new Mock<DNSServersApi>();
        mockApi.Setup(a => a.CreateDNSServerAsync(serverCreate, default))
            .ReturnsAsync(createdServer);

        _apiClientFactoryMock.Setup(f => f.CreateDnsServersApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.CreateAsync(serverCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-srv", result.Id);
    }

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
