using AdGuard.DataAccess.Entities;
using AdGuard.DataAccess.Repositories;
using AdGuard.DataAccess.Tests.TestFixtures;
using FluentAssertions;
using Xunit;

namespace AdGuard.DataAccess.Tests.Repositories;

public class QueryLogLocalRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public QueryLogLocalRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public async Task AddAsync_ShouldAddQueryLog()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        var queryLog = new QueryLogEntity
        {
            Domain = "example.com",
            Timestamp = DateTime.UtcNow,
            IsBlocked = false,
            ResponseCode = "NOERROR"
        };

        // Act
        var result = await repository.AddAsync(queryLog);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Domain.Should().Be("example.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllQueryLogs()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        await repository.AddAsync(new QueryLogEntity { Domain = "domain1.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "domain2.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "domain3.com", Timestamp = DateTime.UtcNow });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBlockedAsync_ShouldReturnOnlyBlockedQueries()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        await repository.AddAsync(new QueryLogEntity { Domain = "allowed.com", Timestamp = DateTime.UtcNow, IsBlocked = false });
        await repository.AddAsync(new QueryLogEntity { Domain = "blocked1.com", Timestamp = DateTime.UtcNow, IsBlocked = true });
        await repository.AddAsync(new QueryLogEntity { Domain = "blocked2.com", Timestamp = DateTime.UtcNow, IsBlocked = true });

        // Act
        var result = await repository.GetBlockedAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(q => q.IsBlocked);
    }

    [Fact]
    public async Task GetByDomainAsync_WithExactMatch_ShouldReturnExactMatches()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        await repository.AddAsync(new QueryLogEntity { Domain = "example.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "sub.example.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "other.com", Timestamp = DateTime.UtcNow });

        // Act
        var result = await repository.GetByDomainAsync("example.com", exactMatch: true);

        // Assert
        result.Should().HaveCount(1);
        result.First().Domain.Should().Be("example.com");
    }

    [Fact]
    public async Task GetByDomainAsync_WithPartialMatch_ShouldReturnPartialMatches()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        await repository.AddAsync(new QueryLogEntity { Domain = "example.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "sub.example.com", Timestamp = DateTime.UtcNow });
        await repository.AddAsync(new QueryLogEntity { Domain = "other.com", Timestamp = DateTime.UtcNow });

        // Act
        var result = await repository.GetByDomainAsync("example", exactMatch: false);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(q => q.Domain.Contains("example"));
    }

    [Fact]
    public async Task GetByTimeRangeAsync_ShouldReturnQueriesInRange()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        var now = DateTime.UtcNow;
        await repository.AddAsync(new QueryLogEntity { Domain = "old.com", Timestamp = now.AddDays(-5) });
        await repository.AddAsync(new QueryLogEntity { Domain = "recent.com", Timestamp = now.AddDays(-1) });
        await repository.AddAsync(new QueryLogEntity { Domain = "today.com", Timestamp = now });

        // Act
        var result = await repository.GetByTimeRangeAsync(now.AddDays(-2), now);

        // Assert
        result.Should().HaveCount(2);
        result.Should().NotContain(q => q.Domain == "old.com");
    }

    [Fact]
    public async Task GetRecentAsync_ShouldReturnMostRecentQueries()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        for (int i = 0; i < 10; i++)
        {
            await repository.AddAsync(new QueryLogEntity
            {
                Domain = $"domain{i}.com",
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        // Act
        var result = await repository.GetRecentAsync(5);

        // Assert
        result.Should().HaveCount(5);
        result.First().Domain.Should().Be("domain0.com"); // Most recent
    }

    [Fact]
    public async Task GetTopBlockedDomainsAsync_ShouldReturnTopDomains()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        // Add blocked queries with different frequencies
        for (int i = 0; i < 5; i++)
            await repository.AddAsync(new QueryLogEntity { Domain = "top.com", Timestamp = DateTime.UtcNow, IsBlocked = true });
        for (int i = 0; i < 3; i++)
            await repository.AddAsync(new QueryLogEntity { Domain = "second.com", Timestamp = DateTime.UtcNow, IsBlocked = true });
        await repository.AddAsync(new QueryLogEntity { Domain = "third.com", Timestamp = DateTime.UtcNow, IsBlocked = true });

        // Act
        var result = await repository.GetTopBlockedDomainsAsync(10);

        // Assert
        result.Should().HaveCount(3);
        result["top.com"].Should().Be(5);
        result["second.com"].Should().Be(3);
        result["third.com"].Should().Be(1);
    }

    [Fact]
    public async Task DeleteOlderThanAsync_ShouldDeleteOldRecords()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new QueryLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>());

        var now = DateTime.UtcNow;
        await repository.AddAsync(new QueryLogEntity { Domain = "old.com", Timestamp = now.AddDays(-10) });
        await repository.AddAsync(new QueryLogEntity { Domain = "recent.com", Timestamp = now.AddDays(-1) });

        // Act
        var deleted = await repository.DeleteOlderThanAsync(now.AddDays(-5));

        // Assert
        deleted.Should().Be(1);
        var remaining = await repository.GetAllAsync();
        remaining.Should().HaveCount(1);
        remaining.First().Domain.Should().Be("recent.com");
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
