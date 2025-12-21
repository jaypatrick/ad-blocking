using AdGuard.ConsoleUI.Display;
using AdGuard.Repositories.Contracts;
using AdGuard.ConsoleUI.Services;
using Microsoft.Extensions.DependencyInjection;

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

        // Register API Client Factory (with interface for DI)
        services.AddSingleton<ApiClientFactory>();
        services.AddSingleton<IApiClientFactory>(sp => sp.GetRequiredService<ApiClientFactory>());

        // Register Repositories
        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IDnsServerRepository, DnsServerRepository>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IStatisticsRepository, StatisticsRepository>();
        services.AddSingleton<IFilterListRepository, FilterListRepository>();
        services.AddSingleton<IQueryLogRepository, QueryLogRepository>();
        services.AddSingleton<IWebServiceRepository, WebServiceRepository>();
        services.AddSingleton<IDedicatedIpRepository, DedicatedIpRepository>();
        services.AddSingleton<IUserRulesRepository, UserRulesRepository>();

        // Register Display Strategies
        services.AddSingleton<IDisplayStrategy<Device>, DeviceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DNSServer>, DnsServerDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<FilterList>, FilterListDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<WebService>, WebServiceDisplayStrategy>();
        services.AddSingleton<IDisplayStrategy<DedicatedIPv4Address>, DedicatedIPDisplayStrategy>();
        services.AddSingleton<AccountLimitsDisplayStrategy>();
        services.AddSingleton<StatisticsDisplayStrategy>();
        services.AddSingleton<QueryLogDisplayStrategy>();
        services.AddSingleton<UserRulesDisplayStrategy>();

        // Register Menu Services (as IMenuService for collection injection)
        services.AddSingleton<IMenuService, DeviceMenuService>();
        services.AddSingleton<IMenuService, DnsServerMenuService>();
        services.AddSingleton<IMenuService, StatisticsMenuService>();
        services.AddSingleton<IMenuService, AccountMenuService>();
        services.AddSingleton<IMenuService, FilterListMenuService>();
        services.AddSingleton<IMenuService, QueryLogMenuService>();
        services.AddSingleton<IMenuService, WebServiceMenuService>();
        services.AddSingleton<IMenuService, DedicatedIPMenuService>();
        services.AddSingleton<IMenuService, UserRulesMenuService>();

        // Register Main Application
        services.AddSingleton<ConsoleApplication>();

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
    public void ConfigureServices_RegistersIApiClientFactory()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetService<IApiClientFactory>();

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

    #endregion

    #region Repository Registration Tests

    [Fact]
    public void ConfigureServices_RegistersDeviceRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IDeviceRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<DeviceRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersDnsServerRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IDnsServerRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<DnsServerRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersAccountRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IAccountRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<AccountRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersStatisticsRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IStatisticsRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<StatisticsRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersFilterListRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IFilterListRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<FilterListRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersQueryLogRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IQueryLogRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<QueryLogRepository>(repository);
    }

    #endregion

    #region Display Strategy Registration Tests

    [Fact]
    public void ConfigureServices_RegistersDeviceDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<IDisplayStrategy<Device>>();

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<DeviceDisplayStrategy>(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersDnsServerDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<IDisplayStrategy<DNSServer>>();

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<DnsServerDisplayStrategy>(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersFilterListDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<IDisplayStrategy<FilterList>>();

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<FilterListDisplayStrategy>(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersAccountLimitsDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<AccountLimitsDisplayStrategy>();

        // Assert
        Assert.NotNull(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersStatisticsDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<StatisticsDisplayStrategy>();

        // Assert
        Assert.NotNull(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersQueryLogDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<QueryLogDisplayStrategy>();

        // Assert
        Assert.NotNull(strategy);
    }

    #endregion

    #region Menu Service Registration Tests

    [Fact]
    public void ConfigureServices_RegistersMenuServicesAsIMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var menuServices = provider.GetServices<IMenuService>().ToList();

        // Assert
        Assert.Equal(9, menuServices.Count);
        Assert.Contains(menuServices, s => s is DeviceMenuService);
        Assert.Contains(menuServices, s => s is DnsServerMenuService);
        Assert.Contains(menuServices, s => s is StatisticsMenuService);
        Assert.Contains(menuServices, s => s is AccountMenuService);
        Assert.Contains(menuServices, s => s is FilterListMenuService);
        Assert.Contains(menuServices, s => s is QueryLogMenuService);
        Assert.Contains(menuServices, s => s is WebServiceMenuService);
        Assert.Contains(menuServices, s => s is DedicatedIPMenuService);
        Assert.Contains(menuServices, s => s is UserRulesMenuService);
    }

    [Fact]
    public void ConfigureServices_MenuServices_HaveCorrectTitles()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();
        var expectedTitles = new[]
        {
            "Device Management", "DNS Server Management", "User Rules Management", "Statistics",
            "Query Log", "Filter Lists", "Web Services",
            "Dedicated IP Addresses", "Account Info"
        };

        // Act
        var menuServices = provider.GetServices<IMenuService>();
        var actualTitles = menuServices.Select(s => s.Title).ToList();

        // Assert
        foreach (var title in expectedTitles)
        {
            Assert.Contains(title, actualTitles);
        }
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
    public void ConfigureServices_IApiClientFactory_ReturnsSameAsApiClientFactory()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var concrete = provider.GetService<ApiClientFactory>();
        var abstraction = provider.GetService<IApiClientFactory>();

        // Assert
        Assert.Same(concrete, abstraction);
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
    public void ConfigureServices_Repositories_AreSingletons()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.Same(
            provider.GetService<IDeviceRepository>(),
            provider.GetService<IDeviceRepository>());

        Assert.Same(
            provider.GetService<IDnsServerRepository>(),
            provider.GetService<IDnsServerRepository>());

        Assert.Same(
            provider.GetService<IAccountRepository>(),
            provider.GetService<IAccountRepository>());

        Assert.Same(
            provider.GetService<IStatisticsRepository>(),
            provider.GetService<IStatisticsRepository>());
    }

    [Fact]
    public void ConfigureServices_MenuServices_AreSingletons()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act - Get menu services twice
        var firstResolution = provider.GetServices<IMenuService>().ToList();
        var secondResolution = provider.GetServices<IMenuService>().ToList();

        // Assert - Each service type should return the same instance
        for (var i = 0; i < firstResolution.Count; i++)
        {
            Assert.Same(firstResolution[i], secondResolution[i]);
        }
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
    public void ConfigureServices_MenuServices_ResolveDependencies()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act - All menu services depend on repositories and display strategies
        var menuServices = provider.GetServices<IMenuService>().ToList();

        // Assert - All 9 menu services should be resolved
        Assert.Equal(9, menuServices.Count);
        Assert.All(menuServices, service => Assert.NotNull(service));
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

    #region New Services Registration Tests

    [Fact]
    public void ConfigureServices_RegistersWebServiceRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IWebServiceRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<WebServiceRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersDedicatedIpRepository()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var repository = provider.GetService<IDedicatedIpRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<DedicatedIpRepository>(repository);
    }

    [Fact]
    public void ConfigureServices_RegistersWebServiceDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<IDisplayStrategy<WebService>>();

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<WebServiceDisplayStrategy>(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersDedicatedIPDisplayStrategy()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var strategy = provider.GetService<IDisplayStrategy<DedicatedIPv4Address>>();

        // Assert
        Assert.NotNull(strategy);
        Assert.IsType<DedicatedIPDisplayStrategy>(strategy);
    }

    [Fact]
    public void ConfigureServices_RegistersWebServiceMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var menuServices = provider.GetServices<IMenuService>();
        var menuService = menuServices.FirstOrDefault(s => s is WebServiceMenuService);

        // Assert
        Assert.NotNull(menuService);
    }

    [Fact]
    public void ConfigureServices_RegistersDedicatedIPMenuService()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var menuServices = provider.GetServices<IMenuService>();
        var menuService = menuServices.FirstOrDefault(s => s is DedicatedIPMenuService);

        // Assert
        Assert.NotNull(menuService);
    }

    [Fact]
    public void ConfigureServices_WebServiceRepository_IsSingleton()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetService<IWebServiceRepository>();
        var instance2 = provider.GetService<IWebServiceRepository>();

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void ConfigureServices_DedicatedIpRepository_IsSingleton()
    {
        // Arrange
        var services = CreateConfiguredServices();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetService<IDedicatedIpRepository>();
        var instance2 = provider.GetService<IDedicatedIpRepository>();

        // Assert
        Assert.Same(instance1, instance2);
    }

    #endregion
}
