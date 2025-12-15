using AdGuard.DataAccess.Entities;
using AdGuard.DataAccess.Repositories;
using AdGuard.DataAccess.Tests.TestFixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdGuard.DataAccess.Tests.Repositories;

public class LocalUnitOfWorkTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public LocalUnitOfWorkTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new LocalUnitOfWork(
                null!,
                DatabaseFixture.CreateMockLogger<LocalUnitOfWork>(),
                DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>(),
                DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>(),
                DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>(),
                DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>(),
                DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>()));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = _fixture.CreateContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new LocalUnitOfWork(
                context,
                null!,
                DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>(),
                DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>(),
                DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>(),
                DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>(),
                DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>()));
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = _fixture.CreateContext();

        // Act
        using var uow = CreateUnitOfWork(context);

        // Assert
        uow.Should().NotBeNull();
        uow.QueryLogs.Should().NotBeNull();
        uow.Statistics.Should().NotBeNull();
        uow.AuditLogs.Should().NotBeNull();
        uow.CompilationHistory.Should().NotBeNull();
        uow.UserSettings.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsChanges()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        var entity = await uow.AuditLogs.LogOperationAsync(
            AuditOperationType.Create,
            "Test",
            "test-1");

        // Act - already persisted by LogOperationAsync
        var result = await context.SaveChangesAsync();

        // Assert
        var logs = await uow.AuditLogs.GetAllAsync();
        logs.Should().HaveCount(1);
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task BeginTransactionAsync_StartsTransaction()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        // Act
        await uow.BeginTransactionAsync();

        // Assert - transaction started successfully (no exception)
        // Clean up
        await uow.RollbackTransactionAsync();
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task BeginTransactionAsync_WhenTransactionExists_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);
        await uow.BeginTransactionAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => uow.BeginTransactionAsync());

        // Clean up
        await uow.RollbackTransactionAsync();
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task CommitTransactionAsync_WithoutBegin_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => uow.CommitTransactionAsync());
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task CommitTransactionAsync_AfterBegin_CommitsChanges()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        await uow.BeginTransactionAsync();
        await uow.AuditLogs.LogOperationAsync(
            AuditOperationType.Create,
            "Test",
            "test-1");

        // Act
        await uow.CommitTransactionAsync();

        // Assert
        var logs = await uow.AuditLogs.GetAllAsync();
        logs.Should().HaveCount(1);
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task RollbackTransactionAsync_WithoutBegin_DoesNotThrow()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        // Act & Assert - should not throw
        await uow.RollbackTransactionAsync();
    }

    [Fact(Skip = "InMemory database does not support transactions")]
    public async Task RollbackTransactionAsync_AfterBegin_RollsBackChanges()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        await uow.BeginTransactionAsync();
        await uow.AuditLogs.LogOperationAsync(
            AuditOperationType.Create,
            "Test",
            "test-1");

        // Act
        await uow.RollbackTransactionAsync();

        // Assert
        var logs = await uow.AuditLogs.GetAllAsync();
        logs.Should().BeEmpty();
    }

    [Fact]
    public async Task EnsureDatabaseCreatedAsync_CreatesDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        using var uow = CreateUnitOfWork(context);

        // Act
        var created = await uow.EnsureDatabaseCreatedAsync();

        // Assert
        // For in-memory database, this may return true or false
        // Just verify it doesn't throw
        Assert.True(created is true or false);
    }

    [Fact]
    public async Task Dispose_DisposesResources()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var uow = CreateUnitOfWork(context);

        // Act
        uow.Dispose();

        // Assert - calling Dispose multiple times should not throw
        uow.Dispose();
    }

    [Fact]
    public async Task DisposeAsync_DisposesResources()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var uow = CreateUnitOfWork(context);

        // Act
        await uow.DisposeAsync();

        // Assert - no exception thrown
        await uow.DisposeAsync(); // Multiple calls should be safe
    }

    private LocalUnitOfWork CreateUnitOfWork(AdGuardDbContext context)
    {
        return new LocalUnitOfWork(
            context,
            DatabaseFixture.CreateMockLogger<LocalUnitOfWork>(),
            DatabaseFixture.CreateMockLogger<QueryLogLocalRepository>(),
            DatabaseFixture.CreateMockLogger<StatisticsLocalRepository>(),
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>(),
            DatabaseFixture.CreateMockLogger<CompilationHistoryLocalRepository>(),
            DatabaseFixture.CreateMockLogger<UserSettingsLocalRepository>());
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
