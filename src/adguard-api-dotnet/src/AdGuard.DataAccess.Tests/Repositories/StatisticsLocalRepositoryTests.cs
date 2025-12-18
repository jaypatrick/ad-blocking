using AdGuard.DataAccess.Entities;
using AdGuard.DataAccess.Repositories;
using AdGuard.DataAccess.Tests.TestFixtures;
using FluentAssertions;
using Xunit;

namespace AdGuard.DataAccess.Tests.Repositories;

public class StatisticsLocalRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public StatisticsLocalRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public async Task AddAsync_ShouldAddStatistics()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var stats = new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date,
            DnsServerId = "server-1",
            TotalQueries = 1000,
            BlockedQueries = 200,
            AllowedQueries = 800,
            Granularity = StatisticsGranularity.Daily
        };

        // Act
        var result = await repository.AddAsync(stats);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.TotalQueries.Should().Be(1000);
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnStatsInRange()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var today = DateTime.UtcNow.Date;
        await repository.AddAsync(new StatisticsEntity
        {
            Date = today.AddDays(-5),
            TotalQueries = 100,
            Granularity = StatisticsGranularity.Daily
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = today.AddDays(-1),
            TotalQueries = 200,
            Granularity = StatisticsGranularity.Daily
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = today,
            TotalQueries = 300,
            Granularity = StatisticsGranularity.Daily
        });

        // Act
        var result = await repository.GetByDateRangeAsync(today.AddDays(-2), today);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByDnsServerAsync_ShouldReturnServerStats()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date,
            DnsServerId = "server-1",
            TotalQueries = 100
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date.AddDays(-1),
            DnsServerId = "server-1",
            TotalQueries = 150
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date,
            DnsServerId = "server-2",
            TotalQueries = 200
        });

        // Act
        var result = await repository.GetByDnsServerAsync("server-1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(s => s.DnsServerId == "server-1");
    }

    [Fact]
    public async Task GetTotalsAsync_ShouldReturnAggregatedTotals()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var today = DateTime.UtcNow.Date;
        await repository.AddAsync(new StatisticsEntity
        {
            Date = today,
            TotalQueries = 1000,
            BlockedQueries = 200,
            AllowedQueries = 800,
            CachedQueries = 300,
            AverageResponseTimeMs = 50
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = today.AddDays(-1),
            TotalQueries = 2000,
            BlockedQueries = 400,
            AllowedQueries = 1600,
            CachedQueries = 500,
            AverageResponseTimeMs = 60
        });

        // Act
        var totals = await repository.GetTotalsAsync(today.AddDays(-7), today);

        // Assert
        totals.TotalQueries.Should().Be(3000);
        totals.BlockedQueries.Should().Be(600);
        totals.AllowedQueries.Should().Be(2400);
        totals.CachedQueries.Should().Be(800);
        totals.BlockRate.Should().Be(20); // 600/3000 = 20%
    }

    [Fact]
    public async Task UpsertAsync_ShouldInsertNewRecord()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var stats = new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date,
            DnsServerId = "server-1",
            TotalQueries = 100,
            Granularity = StatisticsGranularity.Daily
        };

        // Act
        var result = await repository.UpsertAsync(stats);

        // Assert
        result.Should().NotBeNull();
        result.TotalQueries.Should().Be(100);

        var all = await repository.GetAllAsync();
        all.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpsertAsync_ShouldUpdateExistingRecord()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var date = DateTime.UtcNow.Date;
        var serverId = "server-1";

        // First insert
        await repository.UpsertAsync(new StatisticsEntity
        {
            Date = date,
            DnsServerId = serverId,
            TotalQueries = 100,
            Granularity = StatisticsGranularity.Daily
        });

        // Act - Update same record
        var result = await repository.UpsertAsync(new StatisticsEntity
        {
            Date = date,
            DnsServerId = serverId,
            TotalQueries = 200, // Updated value
            Granularity = StatisticsGranularity.Daily
        });

        // Assert
        result.TotalQueries.Should().Be(200);

        var all = await repository.GetAllAsync();
        all.Should().HaveCount(1);
        all.First().TotalQueries.Should().Be(200);
    }

    [Fact]
    public async Task GetLatestAsync_ShouldReturnMostRecentStats()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date.AddDays(-2),
            TotalQueries = 100
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date.AddDays(-1),
            TotalQueries = 200
        });
        await repository.AddAsync(new StatisticsEntity
        {
            Date = DateTime.UtcNow.Date,
            TotalQueries = 300
        });

        // Act
        var result = await repository.GetLatestAsync();

        // Assert
        result.Should().NotBeNull();
        result!.TotalQueries.Should().Be(300);
    }

    [Fact]
    public async Task DeleteOlderThanAsync_ShouldDeleteOldStats()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new StatisticsLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>());

        var today = DateTime.UtcNow.Date;
        await repository.AddAsync(new StatisticsEntity { Date = today.AddDays(-100), TotalQueries = 100 });
        await repository.AddAsync(new StatisticsEntity { Date = today.AddDays(-50), TotalQueries = 200 });
        await repository.AddAsync(new StatisticsEntity { Date = today, TotalQueries = 300 });

        // Act
        var deleted = await repository.DeleteOlderThanAsync(today.AddDays(-60));

        // Assert
        deleted.Should().Be(1);
        var remaining = await repository.GetAllAsync();
        remaining.Should().HaveCount(2);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
