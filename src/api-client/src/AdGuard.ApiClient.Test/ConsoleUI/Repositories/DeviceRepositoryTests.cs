using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Repositories;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="DeviceRepository"/>.
/// </summary>
public class DeviceRepositoryTests
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;
    private readonly Mock<DevicesApi> _devicesApiMock;

    public DeviceRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _devicesApiMock = new Mock<DevicesApi>();
    }

    private DeviceRepository CreateRepository()
    {
        return new DeviceRepository(_apiClientFactoryMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new DeviceRepository(null!));

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

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_CallsApiAndReturnsDevices()
    {
        // Arrange
        var expectedDevices = new List<Device>
        {
            new Device(id: "dev1", name: "Device 1", deviceType: Device.DeviceTypeEnum.WINDOWS, dnsServerId: "srv1"),
            new Device(id: "dev2", name: "Device 2", deviceType: Device.DeviceTypeEnum.ANDROID, dnsServerId: "srv1")
        };

        var mockApi = new Mock<DevicesApi>();
        mockApi.Setup(a => a.ListDevicesAsync(0, default))
            .ReturnsAsync(expectedDevices);

        _apiClientFactoryMock.Setup(f => f.CreateDevicesApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("dev1", result[0].Id);
        Assert.Equal("dev2", result[1].Id);

        _apiClientFactoryMock.Verify(f => f.CreateDevicesApi(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenApiReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        var mockApi = new Mock<DevicesApi>();
        mockApi.Setup(a => a.ListDevicesAsync(0, default))
            .ReturnsAsync(new List<Device>());

        _apiClientFactoryMock.Setup(f => f.CreateDevicesApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsDevice()
    {
        // Arrange
        var deviceId = "test-device-id";
        var expectedDevice = new Device(id: deviceId, name: "Test Device", deviceType: Device.DeviceTypeEnum.WINDOWS, dnsServerId: "srv1");

        var mockApi = new Mock<DevicesApi>();
        mockApi.Setup(a => a.GetDeviceAsync(deviceId, 0, default))
            .ReturnsAsync(expectedDevice);

        _apiClientFactoryMock.Setup(f => f.CreateDevicesApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.GetByIdAsync(deviceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(deviceId, result.Id);
        Assert.Equal("Test Device", result.Name);
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

    [Fact]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetByIdAsync(string.Empty));
    }

    [Fact]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetByIdAsync("   "));
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidDevice_ReturnsCreatedDevice()
    {
        // Arrange
        var deviceCreate = new DeviceCreate(name: "New Device", deviceType: "WINDOWS", dnsServerId: "srv1");
        var createdDevice = new Device(id: "new-id", name: "New Device", deviceType: Device.DeviceTypeEnum.WINDOWS, dnsServerId: "srv1");

        var mockApi = new Mock<DevicesApi>();
        mockApi.Setup(a => a.CreateDeviceAsync(deviceCreate, 0, default))
            .ReturnsAsync(createdDevice);

        _apiClientFactoryMock.Setup(f => f.CreateDevicesApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        var result = await repository.CreateAsync(deviceCreate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-id", result.Id);
        Assert.Equal("New Device", result.Name);
    }

    [Fact]
    public async Task CreateAsync_WithNullDevice_ThrowsArgumentNullException()
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
    public async Task DeleteAsync_WithValidId_CallsApi()
    {
        // Arrange
        var deviceId = "device-to-delete";
        var mockApi = new Mock<DevicesApi>();
        mockApi.Setup(a => a.RemoveDeviceAsync(deviceId, 0, default))
            .Returns(Task.CompletedTask);

        _apiClientFactoryMock.Setup(f => f.CreateDevicesApi())
            .Returns(mockApi.Object);

        var repository = CreateRepository();

        // Act
        await repository.DeleteAsync(deviceId);

        // Assert
        mockApi.Verify(a => a.RemoveDeviceAsync(deviceId, 0, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.DeleteAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.DeleteAsync(string.Empty));
    }

    #endregion
}
