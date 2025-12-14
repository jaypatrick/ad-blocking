using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AdGuard.ApiClient.Api;
using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Exceptions;
using AdGuard.ConsoleUI.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AdGuard.ApiClient.Test.ConsoleUI.Repositories;

/// <summary>
/// Unit tests for <see cref="UserRulesRepository"/>.
/// </summary>
public class UserRulesRepositoryTests : IDisposable
{
    private readonly Mock<IApiClientFactory> _apiClientFactoryMock;
    private readonly Mock<IDnsServerRepository> _dnsServerRepositoryMock;
    private readonly Mock<ILogger<UserRulesRepository>> _loggerMock;
    private readonly string _testFilePath;

    public UserRulesRepositoryTests()
    {
        _apiClientFactoryMock = new Mock<IApiClientFactory>();
        _dnsServerRepositoryMock = new Mock<IDnsServerRepository>();
        _loggerMock = new Mock<ILogger<UserRulesRepository>>();
        _testFilePath = Path.GetTempFileName();
    }

    public void Dispose()
    {
        // Cleanup temp file
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    private UserRulesRepository CreateRepository()
    {
        return new UserRulesRepository(
            _apiClientFactoryMock.Object,
            _dnsServerRepositoryMock.Object,
            _loggerMock.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new UserRulesRepository(null!, _dnsServerRepositoryMock.Object, _loggerMock.Object));

        Assert.Equal("apiClientFactory", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullDnsServerRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new UserRulesRepository(_apiClientFactoryMock.Object, null!, _loggerMock.Object));

        Assert.Equal("dnsServerRepository", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new UserRulesRepository(_apiClientFactoryMock.Object, _dnsServerRepositoryMock.Object, null!));

        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Act
        var repository = CreateRepository();

        // Assert
        Assert.NotNull(repository);
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetAsync(null!));
    }

    [Fact]
    public async Task GetAsync_WithEmptyServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetAsync(""));
    }

    [Fact]
    public async Task GetAsync_WithWhitespaceServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.GetAsync("   "));
    }

    [Fact]
    public async Task GetAsync_WithValidServerIdAndUserRules_ReturnsUserRulesSettings()
    {
        // Arrange
        var repository = CreateRepository();
        var serverId = "test-server-id";

        var settings = JObject.Parse(@"{
            ""user_rules_settings"": {
                ""enabled"": true,
                ""rules"": [""||blocked.com^"", ""@@||allowed.com^""],
                ""rules_count"": 2
            }
        }");

        var server = new DNSServer(
            varDefault: true,
            deviceIds: new List<string>(),
            id: serverId,
            name: "Test Server",
            settings: settings);

        _dnsServerRepositoryMock
            .Setup(x => x.GetByIdAsync(serverId))
            .ReturnsAsync(server);

        // Act
        var result = await repository.GetAsync(serverId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Enabled);
        Assert.Equal(2, result.Rules.Count);
        Assert.Contains("||blocked.com^", result.Rules);
        Assert.Contains("@@||allowed.com^", result.Rules);
    }

    [Fact]
    public async Task GetAsync_WithNullSettings_ReturnsDefaultUserRulesSettings()
    {
        // Arrange
        var repository = CreateRepository();
        var serverId = "test-server-id";

        var server = new DNSServer(
            varDefault: true,
            deviceIds: new List<string>(),
            id: serverId,
            name: "Test Server",
            settings: new object());

        _dnsServerRepositoryMock
            .Setup(x => x.GetByIdAsync(serverId))
            .ReturnsAsync(server);

        // Act
        var result = await repository.GetAsync(serverId);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Enabled);
        Assert.Empty(result.Rules);
        Assert.Equal(0, result.RulesCount);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();
        var update = new UserRulesSettingsUpdate(enabled: true, rules: new List<string>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.UpdateAsync(null!, update));
    }

    [Fact]
    public async Task UpdateAsync_WithNullUpdate_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => repository.UpdateAsync("server-id", null!));
    }

    #endregion

    #region UpdateFromFileAsync Tests

    [Fact]
    public async Task UpdateFromFileAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.UpdateFromFileAsync(null!, _testFilePath));
    }

    [Fact]
    public async Task UpdateFromFileAsync_WithNullFilePath_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.UpdateFromFileAsync("server-id", null!));
    }

    [Fact]
    public async Task UpdateFromFileAsync_WithEmptyFilePath_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.UpdateFromFileAsync("server-id", ""));
    }

    [Fact]
    public async Task UpdateFromFileAsync_WithNonexistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var repository = CreateRepository();
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent-file-12345.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => repository.UpdateFromFileAsync("server-id", nonExistentFile));
    }

    [Fact]
    public async Task UpdateFromFileAsync_ParsesRulesCorrectly()
    {
        // Arrange
        var rules = @"! This is a comment
||blocked.domain.com^
# Another comment

@@||allowed.domain.com^
||another-blocked.com^
";
        await File.WriteAllTextAsync(_testFilePath, rules);

        var repository = CreateRepository();
        var serverId = "test-server-id";

        var dnsApiMock = new Mock<DNSServersApi>();
        _apiClientFactoryMock
            .Setup(x => x.CreateDnsServersApi())
            .Returns(dnsApiMock.Object);

        UserRulesSettingsUpdate? capturedUpdate = null;
        dnsApiMock
            .Setup(x => x.UpdateDNSServerSettingsAsync(
                serverId,
                It.IsAny<DNSServerSettingsUpdate>(),
                default))
            .Callback<string, DNSServerSettingsUpdate, System.Threading.CancellationToken>(
                (id, update, ct) => capturedUpdate = update.UserRulesSettings)
            .Returns(Task.CompletedTask);

        // Act
        var count = await repository.UpdateFromFileAsync(serverId, _testFilePath);

        // Assert
        Assert.Equal(3, count); // 3 actual rules (excluding comments and empty lines)
        Assert.NotNull(capturedUpdate);
        Assert.Equal(3, capturedUpdate.Rules?.Count);
        Assert.Contains("||blocked.domain.com^", capturedUpdate.Rules);
        Assert.Contains("@@||allowed.domain.com^", capturedUpdate.Rules);
        Assert.Contains("||another-blocked.com^", capturedUpdate.Rules);
    }

    [Fact]
    public async Task UpdateFromFileAsync_FiltersComments()
    {
        // Arrange
        var rules = @"! Comment starting with !
# Comment starting with #
||actual.rule^
  ! Indented comment
";
        await File.WriteAllTextAsync(_testFilePath, rules);

        var repository = CreateRepository();
        var serverId = "test-server-id";

        var dnsApiMock = new Mock<DNSServersApi>();
        _apiClientFactoryMock
            .Setup(x => x.CreateDnsServersApi())
            .Returns(dnsApiMock.Object);

        UserRulesSettingsUpdate? capturedUpdate = null;
        dnsApiMock
            .Setup(x => x.UpdateDNSServerSettingsAsync(
                serverId,
                It.IsAny<DNSServerSettingsUpdate>(),
                default))
            .Callback<string, DNSServerSettingsUpdate, System.Threading.CancellationToken>(
                (id, update, ct) => capturedUpdate = update.UserRulesSettings)
            .Returns(Task.CompletedTask);

        // Act
        var count = await repository.UpdateFromFileAsync(serverId, _testFilePath);

        // Assert
        Assert.Equal(1, count);
        Assert.NotNull(capturedUpdate);
        Assert.Single(capturedUpdate.Rules!);
        Assert.Contains("||actual.rule^", capturedUpdate.Rules);
    }

    [Fact]
    public async Task UpdateFromFileAsync_FiltersTooLongRules()
    {
        // Arrange
        var longRule = "||" + new string('x', 1025) + "^"; // Over 1024 chars
        var shortRule = "||short.com^";
        var rules = $"{longRule}\n{shortRule}";
        await File.WriteAllTextAsync(_testFilePath, rules);

        var repository = CreateRepository();
        var serverId = "test-server-id";

        var dnsApiMock = new Mock<DNSServersApi>();
        _apiClientFactoryMock
            .Setup(x => x.CreateDnsServersApi())
            .Returns(dnsApiMock.Object);

        UserRulesSettingsUpdate? capturedUpdate = null;
        dnsApiMock
            .Setup(x => x.UpdateDNSServerSettingsAsync(
                serverId,
                It.IsAny<DNSServerSettingsUpdate>(),
                default))
            .Callback<string, DNSServerSettingsUpdate, System.Threading.CancellationToken>(
                (id, update, ct) => capturedUpdate = update.UserRulesSettings)
            .Returns(Task.CompletedTask);

        // Act
        var count = await repository.UpdateFromFileAsync(serverId, _testFilePath);

        // Assert
        Assert.Equal(1, count); // Only the short rule should be included
        Assert.NotNull(capturedUpdate);
        Assert.Single(capturedUpdate.Rules!);
        Assert.Contains("||short.com^", capturedUpdate.Rules);
    }

    [Fact]
    public async Task UpdateFromFileAsync_RemovesDuplicates()
    {
        // Arrange
        var rules = @"||duplicate.com^
||duplicate.com^
||unique.com^
||duplicate.com^
";
        await File.WriteAllTextAsync(_testFilePath, rules);

        var repository = CreateRepository();
        var serverId = "test-server-id";

        var dnsApiMock = new Mock<DNSServersApi>();
        _apiClientFactoryMock
            .Setup(x => x.CreateDnsServersApi())
            .Returns(dnsApiMock.Object);

        UserRulesSettingsUpdate? capturedUpdate = null;
        dnsApiMock
            .Setup(x => x.UpdateDNSServerSettingsAsync(
                serverId,
                It.IsAny<DNSServerSettingsUpdate>(),
                default))
            .Callback<string, DNSServerSettingsUpdate, System.Threading.CancellationToken>(
                (id, update, ct) => capturedUpdate = update.UserRulesSettings)
            .Returns(Task.CompletedTask);

        // Act
        var count = await repository.UpdateFromFileAsync(serverId, _testFilePath);

        // Assert
        Assert.Equal(2, count); // Duplicates removed
        Assert.NotNull(capturedUpdate);
        Assert.Equal(2, capturedUpdate.Rules!.Count);
    }

    #endregion

    #region SetEnabledAsync Tests

    [Fact]
    public async Task SetEnabledAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.SetEnabledAsync(null!, true));
    }

    [Fact]
    public async Task SetEnabledAsync_WithEmptyServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.SetEnabledAsync("", true));
    }

    #endregion

    #region AddRuleAsync Tests

    [Fact]
    public async Task AddRuleAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.AddRuleAsync(null!, "||rule.com^"));
    }

    [Fact]
    public async Task AddRuleAsync_WithNullRule_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.AddRuleAsync("server-id", null!));
    }

    [Fact]
    public async Task AddRuleAsync_WithEmptyRule_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.AddRuleAsync("server-id", ""));
    }

    [Fact]
    public async Task AddRuleAsync_WithWhitespaceRule_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.AddRuleAsync("server-id", "   "));
    }

    #endregion

    #region ClearRulesAsync Tests

    [Fact]
    public async Task ClearRulesAsync_WithNullServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.ClearRulesAsync(null!));
    }

    [Fact]
    public async Task ClearRulesAsync_WithEmptyServerId_ThrowsArgumentException()
    {
        // Arrange
        var repository = CreateRepository();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => repository.ClearRulesAsync(""));
    }

    #endregion
}
