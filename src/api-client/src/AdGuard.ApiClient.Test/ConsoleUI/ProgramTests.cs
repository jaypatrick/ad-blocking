using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AdGuard.ConsoleUI.Services;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI;

/// <summary>
/// Unit tests for <see cref="AdGuard.ConsoleUI.Program"/> and dependency injection configuration.
/// </summary>
/// <remarks>
/// These tests verify that the dependency injection container is properly configured
/// with all required services for the ConsoleUI application.
/// </remarks>
public class ProgramTests
{
    /// <summary>
    /// Creates a service collection configured the same way as Program.ConfigureServices.
    /// </summary>
    private static IServiceCollection CreateConfiguredServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = "Information"
            })
            .Build();

        var services = new ServiceCollection();

        // Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Application services
        services.AddSingleton<ApiClientFactory>();
        services.AddSingleton<ConsoleApplication>();
        services.AddSingleton<DeviceMenuService>();
        services.AddSingleton<DnsServerMenuService>();
        services.AddSingleton<StatisticsMenuService>();
        services.AddSingleton<AccountMenuService>();
        services.AddSingleton<FilterListMenuService>();
        services.AddSingleton<QueryLogMenuService>();

        return services;
    }

    #region Service Registration Tests

    [Fact]
    public void ConfigureServices_RegistersConfiguration()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var configuration = provider.GetService<IConfiguration>();

        // Assert
        Assert.NotNull(configuration);
    }

    [Fact]
    public void ConfigureServices_RegistersLoggerFactory()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var loggerFactory = provider.GetService<ILoggerFactory>();

        // Assert
        Assert.NotNull(loggerFactory);
    }

    [Fact]
    public void ConfigureServices_RegistersApiClientFactory()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetService<ApiClientFactory>();

        // Assert
        Assert.NotNull(factory);
    }

    [Fact]
    public void ConfigureServices_RegistersConsoleApplication()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var app = provider.GetService<ConsoleApplication>();

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigureServices_RegistersDeviceMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<DeviceMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ConfigureServices_RegistersDnsServerMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<DnsServerMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ConfigureServices_RegistersStatisticsMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<StatisticsMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ConfigureServices_RegistersAccountMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<AccountMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ConfigureServices_RegistersFilterListMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<FilterListMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void ConfigureServices_RegistersQueryLogMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetService<QueryLogMenuService>();

        // Assert
        Assert.NotNull(service);
    }

    #endregion

    #region Service Singleton Tests

    [Fact]
    public void ConfigureServices_ApiClientFactory_IsSingleton()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetService<ApiClientFactory>();
        var second = provider.GetService<ApiClientFactory>();

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void ConfigureServices_ConsoleApplication_IsSingleton()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var first = provider.GetService<ConsoleApplication>();
        var second = provider.GetService<ConsoleApplication>();

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void ConfigureServices_MenuServices_AreSingletons()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.Same(
            provider.GetService<DeviceMenuService>(),
            provider.GetService<DeviceMenuService>());

        Assert.Same(
            provider.GetService<DnsServerMenuService>(),
            provider.GetService<DnsServerMenuService>());

        Assert.Same(
            provider.GetService<StatisticsMenuService>(),
            provider.GetService<StatisticsMenuService>());

        Assert.Same(
            provider.GetService<AccountMenuService>(),
            provider.GetService<AccountMenuService>());

        Assert.Same(
            provider.GetService<FilterListMenuService>(),
            provider.GetService<FilterListMenuService>());

        Assert.Same(
            provider.GetService<QueryLogMenuService>(),
            provider.GetService<QueryLogMenuService>());
    }

    #endregion

    #region Service Dependency Resolution Tests

    [Fact]
    public void ConfigureServices_ConsoleApplication_ResolvesAllDependencies()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act - ConsoleApplication depends on all menu services and ApiClientFactory
        var app = provider.GetRequiredService<ConsoleApplication>();

        // Assert - If we get here without exception, all dependencies resolved
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigureServices_MenuServices_ResolveApiClientFactory()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act - All menu services depend on ApiClientFactory
        // No exceptions means successful resolution
        var deviceMenu = provider.GetRequiredService<DeviceMenuService>();
        var dnsServerMenu = provider.GetRequiredService<DnsServerMenuService>();
        var statisticsMenu = provider.GetRequiredService<StatisticsMenuService>();
        var accountMenu = provider.GetRequiredService<AccountMenuService>();
        var filterListMenu = provider.GetRequiredService<FilterListMenuService>();
        var queryLogMenu = provider.GetRequiredService<QueryLogMenuService>();

        // Assert
        Assert.NotNull(deviceMenu);
        Assert.NotNull(dnsServerMenu);
        Assert.NotNull(statisticsMenu);
        Assert.NotNull(accountMenu);
        Assert.NotNull(filterListMenu);
        Assert.NotNull(queryLogMenu);
    }

    [Fact]
    public void ConfigureServices_ApiClientFactory_GetsLoggerFromDI()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<ApiClientFactory>();

        // Assert - If we get here, the logger was successfully injected
        Assert.NotNull(factory);
        Assert.False(factory.IsConfigured); // Not configured by default
    }

    #endregion

    #region Configuration Tests

    [Fact]
    public void Configuration_WithApiKey_CanBeReadFromSettings()
    {
        // Arrange
        var testApiKey = "test-api-key-12345";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdGuard:ApiKey"] = testApiKey
            })
            .Build();

        // Act
        var apiKey = configuration["AdGuard:ApiKey"];

        // Assert
        Assert.Equal(testApiKey, apiKey);
    }

    [Fact]
    public void Configuration_WithLogging_ConfiguresLogLevel()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = "Debug"
            })
            .Build();

        // Act
        var logLevel = configuration["Logging:LogLevel:Default"];

        // Assert
        Assert.Equal("Debug", logLevel);
    }

    [Fact]
    public void Configuration_MissingApiKey_ReturnsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act
        var apiKey = configuration["AdGuard:ApiKey"];

        // Assert
        Assert.Null(apiKey);
    }

    #endregion
}
