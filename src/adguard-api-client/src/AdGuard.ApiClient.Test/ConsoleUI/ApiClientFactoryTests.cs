using System;
using System.Collections.Generic;
using AdGuard.ConsoleUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI;

/// <summary>
/// Unit tests for <see cref="ApiClientFactory"/>.
/// </summary>
public class ApiClientFactoryTests
{
    private readonly Mock<ILogger<ApiClientFactory>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public ApiClientFactoryTests()
    {
        _loggerMock = new Mock<ILogger<ApiClientFactory>>();
        _configurationMock = new Mock<IConfiguration>();
    }

    private ApiClientFactory CreateFactory(IConfiguration? configuration = null)
    {
        return new ApiClientFactory(
            configuration ?? _configurationMock.Object,
            _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var factory = CreateFactory();

        // Assert
        Assert.NotNull(factory);
        Assert.False(factory.IsConfigured);
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ApiClientFactory(null!, _loggerMock.Object));

        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new ApiClientFactory(_configurationMock.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    #endregion

    #region IsConfigured Tests

    [Fact]
    public void IsConfigured_WhenNotConfigured_ReturnsFalse()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.False(factory.IsConfigured);
    }

    [Fact]
    public void IsConfigured_AfterConfigure_ReturnsTrue()
    {
        // Arrange
        var factory = CreateFactory();

        // Act
        factory.Configure("test-api-key-12345");

        // Assert
        Assert.True(factory.IsConfigured);
    }

    #endregion

    #region CurrentApiKey Tests

    [Fact]
    public void CurrentApiKey_WhenNotConfigured_ReturnsNull()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Null(factory.CurrentApiKey);
    }

    [Fact]
    public void CurrentApiKey_AfterConfigure_ReturnsApiKey()
    {
        // Arrange
        var factory = CreateFactory();
        var apiKey = "test-api-key-12345";

        // Act
        factory.Configure(apiKey);

        // Assert
        Assert.Equal(apiKey, factory.CurrentApiKey);
    }

    #endregion

    #region MaskedApiKey Tests

    [Fact]
    public void MaskedApiKey_WhenNotConfigured_ReturnsNull()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Null(factory.MaskedApiKey);
    }

    [Fact]
    public void MaskedApiKey_AfterConfigure_ReturnsMaskedKey()
    {
        // Arrange
        var factory = CreateFactory();
        var apiKey = "test-api-key-12345";

        // Act
        factory.Configure(apiKey);

        // Assert
        Assert.Equal("test...", factory.MaskedApiKey);
    }

    [Fact]
    public void MaskedApiKey_WithShortApiKey_ReturnsMaskedKey()
    {
        // Arrange
        var factory = CreateFactory();
        var apiKey = "ab";

        // Act
        factory.Configure(apiKey);

        // Assert
        Assert.Equal("ab...", factory.MaskedApiKey);
    }

    #endregion

    #region Configure Tests

    [Fact]
    public void Configure_WithValidApiKey_ConfiguresSuccessfully()
    {
        // Arrange
        var factory = CreateFactory();
        var apiKey = "valid-api-key-12345";

        // Act
        factory.Configure(apiKey);

        // Assert
        Assert.True(factory.IsConfigured);
        Assert.Equal(apiKey, factory.CurrentApiKey);
    }

    [Fact]
    public void Configure_WithNullApiKey_ThrowsArgumentException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => factory.Configure(null!));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Configure_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => factory.Configure(string.Empty));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Configure_WithWhitespaceApiKey_ThrowsArgumentException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => factory.Configure("   "));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Configure_CalledMultipleTimes_UpdatesApiKey()
    {
        // Arrange
        var factory = CreateFactory();
        var firstKey = "first-api-key";
        var secondKey = "second-api-key";

        // Act
        factory.Configure(firstKey);
        factory.Configure(secondKey);

        // Assert
        Assert.Equal(secondKey, factory.CurrentApiKey);
    }

    #endregion

    #region ConfigureFromSettings Tests

    [Fact]
    public void ConfigureFromSettings_WithApiKeyInConfig_ConfiguresSuccessfully()
    {
        // Arrange
        var apiKey = "config-api-key-12345";
        _configurationMock.Setup(c => c["AdGuard:ApiKey"]).Returns(apiKey);
        var factory = CreateFactory();

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.True(factory.IsConfigured);
        Assert.Equal(apiKey, factory.CurrentApiKey);
    }

    [Fact]
    public void ConfigureFromSettings_WithNoApiKeyInConfig_DoesNotConfigure()
    {
        // Arrange
        _configurationMock.Setup(c => c["AdGuard:ApiKey"]).Returns((string?)null);
        var factory = CreateFactory();

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.False(factory.IsConfigured);
    }

    [Fact]
    public void ConfigureFromSettings_WithEmptyApiKeyInConfig_DoesNotConfigure()
    {
        // Arrange
        _configurationMock.Setup(c => c["AdGuard:ApiKey"]).Returns(string.Empty);
        var factory = CreateFactory();

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.False(factory.IsConfigured);
    }

    [Fact]
    public void ConfigureFromSettings_WithWhitespaceApiKeyInConfig_DoesNotConfigure()
    {
        // Arrange
        _configurationMock.Setup(c => c["AdGuard:ApiKey"]).Returns("   ");
        var factory = CreateFactory();

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.False(factory.IsConfigured);
    }

    #endregion

    #region API Client Creation Tests

    [Fact]
    public void CreateAccountApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateAccountApi());
    }

    [Fact]
    public void CreateAccountApi_WhenConfigured_ReturnsAccountApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateAccountApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateDevicesApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateDevicesApi());
    }

    [Fact]
    public void CreateDevicesApi_WhenConfigured_ReturnsDevicesApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateDevicesApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateDnsServersApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateDnsServersApi());
    }

    [Fact]
    public void CreateDnsServersApi_WhenConfigured_ReturnsDnsServersApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateDnsServersApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateStatisticsApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateStatisticsApi());
    }

    [Fact]
    public void CreateStatisticsApi_WhenConfigured_ReturnsStatisticsApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateStatisticsApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateFilterListsApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateFilterListsApi());
    }

    [Fact]
    public void CreateFilterListsApi_WhenConfigured_ReturnsFilterListsApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateFilterListsApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateQueryLogApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateQueryLogApi());
    }

    [Fact]
    public void CreateQueryLogApi_WhenConfigured_ReturnsQueryLogApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateQueryLogApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateWebServicesApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateWebServicesApi());
    }

    [Fact]
    public void CreateWebServicesApi_WhenConfigured_ReturnsWebServicesApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateWebServicesApi();

        // Assert
        Assert.NotNull(api);
    }

    [Fact]
    public void CreateDedicatedIpAddressesApi_WhenNotConfigured_ThrowsInvalidOperationException()
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.CreateDedicatedIpAddressesApi());
    }

    [Fact]
    public void CreateDedicatedIpAddressesApi_WhenConfigured_ReturnsDedicatedIpAddressesApi()
    {
        // Arrange
        var factory = CreateFactory();
        factory.Configure("test-api-key");

        // Act
        var api = factory.CreateDedicatedIpAddressesApi();

        // Assert
        Assert.NotNull(api);
    }

    #endregion

    #region Integration Tests with Real Configuration

    [Fact]
    public void ConfigureFromSettings_WithInMemoryConfiguration_ConfiguresSuccessfully()
    {
        // Arrange
        var apiKey = "integration-test-api-key";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdGuard:ApiKey"] = apiKey
            })
            .Build();

        var factory = CreateFactory(configuration);

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.True(factory.IsConfigured);
        Assert.Equal(apiKey, factory.CurrentApiKey);
    }

    [Fact]
    public void ConfigureFromSettings_WithNestedConfiguration_ConfiguresSuccessfully()
    {
        // Arrange
        var apiKey = "nested-config-api-key";
        var configValues = new Dictionary<string, string?>
        {
            ["AdGuard:ApiKey"] = apiKey,
            ["AdGuard:BaseUrl"] = "https://api.adguard-dns.io"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var factory = CreateFactory(configuration);

        // Act
        factory.ConfigureFromSettings();

        // Assert
        Assert.True(factory.IsConfigured);
    }

    #endregion
}
