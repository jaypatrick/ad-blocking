using AdGuard.DataAccess.Entities;
using AdGuard.DataAccess.Repositories;
using AdGuard.DataAccess.Tests.TestFixtures;
using FluentAssertions;
using Xunit;

namespace AdGuard.DataAccess.Tests.Repositories;

public class CompilationHistoryLocalRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public CompilationHistoryLocalRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public async Task AddAsync_ShouldAddCompilationHistory()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        var history = new CompilationHistoryEntity
        {
            ConfigurationPath = "/path/to/config.yaml",
            FilterListName = "My Filter List",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow.AddSeconds(5),
            RuleCount = 1000,
            Success = true,
            DurationMs = 5000
        };

        // Act
        var result = await repository.AddAsync(history);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.ConfigurationPath.Should().Be("/path/to/config.yaml");
        result.RuleCount.Should().Be(1000);
    }

    [Fact]
    public async Task GetSuccessfulAsync_ShouldReturnOnlySuccessful()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config1.yaml",
            StartedAt = DateTime.UtcNow,
            Success = true,
            RuleCount = 100
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config2.yaml",
            StartedAt = DateTime.UtcNow,
            Success = false,
            ErrorMessage = "Compilation failed"
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config3.yaml",
            StartedAt = DateTime.UtcNow,
            Success = true,
            RuleCount = 200
        });

        // Act
        var result = await repository.GetSuccessfulAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Success);
    }

    [Fact]
    public async Task GetFailedAsync_ShouldReturnOnlyFailed()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config1.yaml",
            StartedAt = DateTime.UtcNow,
            Success = true
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config2.yaml",
            StartedAt = DateTime.UtcNow,
            Success = false,
            ErrorMessage = "Failed"
        });

        // Act
        var result = await repository.GetFailedAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetLatestByConfigPathAsync_ShouldReturnLatestForPath()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        var configPath = "/path/to/config.yaml";
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = configPath,
            StartedAt = DateTime.UtcNow.AddHours(-2),
            RuleCount = 100
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = configPath,
            StartedAt = DateTime.UtcNow.AddHours(-1),
            RuleCount = 150
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = configPath,
            StartedAt = DateTime.UtcNow,
            RuleCount = 200
        });

        // Act
        var result = await repository.GetLatestByConfigPathAsync(configPath);

        // Assert
        result.Should().NotBeNull();
        result!.RuleCount.Should().Be(200);
    }

    [Fact]
    public async Task GetByOutputHashAsync_ShouldFindByHash()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        var hash = "abc123hash";
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config.yaml",
            StartedAt = DateTime.UtcNow,
            OutputHash = hash,
            Success = true
        });

        // Act
        var result = await repository.GetByOutputHashAsync(hash);

        // Assert
        result.Should().NotBeNull();
        result!.OutputHash.Should().Be(hash);
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnCorrectStatistics()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        // Add successful compilations
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config1.yaml",
            StartedAt = DateTime.UtcNow,
            Success = true,
            RuleCount = 100,
            DurationMs = 1000
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config2.yaml",
            StartedAt = DateTime.UtcNow,
            Success = true,
            RuleCount = 200,
            DurationMs = 2000
        });
        // Add failed compilation
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config3.yaml",
            StartedAt = DateTime.UtcNow,
            Success = false,
            DurationMs = 500
        });

        // Act
        var stats = await repository.GetStatisticsAsync();

        // Assert
        stats.TotalCompilations.Should().Be(3);
        stats.SuccessfulCompilations.Should().Be(2);
        stats.FailedCompilations.Should().Be(1);
        stats.TotalRulesCompiled.Should().Be(300);
        stats.AverageRulesPerCompilation.Should().Be(150);
        stats.SuccessRate.Should().BeApproximately(66.67, 0.01);
    }

    [Fact]
    public async Task GetByFilterListAsync_ShouldReturnFilterListHistory()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CompilationHistoryLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>());

        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config1.yaml",
            FilterListName = "AdGuard DNS Filter",
            StartedAt = DateTime.UtcNow
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config2.yaml",
            FilterListName = "AdGuard DNS Filter",
            StartedAt = DateTime.UtcNow
        });
        await repository.AddAsync(new CompilationHistoryEntity
        {
            ConfigurationPath = "config3.yaml",
            FilterListName = "Other Filter",
            StartedAt = DateTime.UtcNow
        });

        // Act
        var result = await repository.GetByFilterListAsync("AdGuard DNS Filter");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.FilterListName == "AdGuard DNS Filter");
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
