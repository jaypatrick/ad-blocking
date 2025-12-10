using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Repositories;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="AccountRepository"/>.
/// </summary>
public class AccountRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;

    public AccountRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
    }

    private AccountRepository CreateRepository()
    {
        return new AccountRepository(_apiClientFactoryMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new AccountRepository(null!));

        Assert.Equal("apiClientFactory", exception.ParamName);
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

    #region GetLimitsAsync Tests

    [Fact]
    public async Task GetLimitsAsync_ReturnsAccountLimits()
    {
        // Arrange
        var expectedLimits = new AccountLimits(
            devices: new Limit(used: 5, varLimit: 10),
            dnsServers: new Limit(used: 2, varLimit: 5));

        var mockApi = new Mock<AccountApi>();
        mockApi.Setup(a => a.GetAccountLimitsAsync(0, default))
            .ReturnsAsync(expectedLimits);

        _apiClientFactoryMock.Setup(f => f.CreateAccountApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetLimitsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Devices);
        Assert.Equal(5, result.Devices.Used);
        Assert.Equal(10, result.Devices.VarLimit);
    }

    [Fact]
    public async Task GetLimitsAsync_CallsApiOnce()
    {
        // Arrange
        var mockApi = new Mock<AccountApi>();
        mockApi.Setup(a => a.GetAccountLimitsAsync(0, default))
            .ReturnsAsync(new AccountLimits());

        _apiClientFactoryMock.Setup(f => f.CreateAccountApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        await repository.GetLimitsAsync();

        // Assert
        _apiClientFactoryMock.Verify(f => f.CreateAccountApi(), Times.Once);
        mockApi.Verify(a => a.GetAccountLimitsAsync(0, default), Times.Once);
    }

    #endregion
}
