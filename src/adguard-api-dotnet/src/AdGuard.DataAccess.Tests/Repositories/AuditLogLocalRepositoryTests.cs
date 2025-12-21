namespace AdGuard.DataAccess.Tests.Repositories;

public class AuditLogLocalRepositoryTests : IDisposable
{
    private readonly DatabaseFixture _fixture;

    public AuditLogLocalRepositoryTests()
    {
        _fixture = new DatabaseFixture();
    }

    [Fact]
    public async Task LogOperationAsync_ShouldCreateAuditLog()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        // Act
        var result = await repository.LogOperationAsync(
            AuditOperationType.Create,
            "Device",
            "device-123",
            success: true,
            durationMs: 150,
            source: "ConsoleUI");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.OperationType.Should().Be(AuditOperationType.Create);
        result.EntityType.Should().Be("Device");
        result.EntityId.Should().Be("device-123");
        result.Success.Should().BeTrue();
        result.DurationMs.Should().Be(150);
        result.Source.Should().Be("ConsoleUI");
    }

    [Fact]
    public async Task GetByEntityAsync_ShouldReturnEntityAuditLogs()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        await repository.LogOperationAsync(AuditOperationType.Create, "Device", "device-1");
        await repository.LogOperationAsync(AuditOperationType.Update, "Device", "device-1");
        await repository.LogOperationAsync(AuditOperationType.Create, "Device", "device-2");
        await repository.LogOperationAsync(AuditOperationType.Create, "DnsServer", "server-1");

        // Act
        var result = await repository.GetByEntityAsync("Device", "device-1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.EntityType == "Device" && a.EntityId == "device-1");
    }

    [Fact]
    public async Task GetByOperationTypeAsync_ShouldReturnCorrectOperations()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        await repository.LogOperationAsync(AuditOperationType.Create, "Device", "device-1");
        await repository.LogOperationAsync(AuditOperationType.Update, "Device", "device-1");
        await repository.LogOperationAsync(AuditOperationType.Delete, "Device", "device-2");
        await repository.LogOperationAsync(AuditOperationType.Create, "DnsServer", "server-1");

        // Act
        var result = await repository.GetByOperationTypeAsync(AuditOperationType.Create);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.OperationType == AuditOperationType.Create);
    }

    [Fact]
    public async Task GetFailedAsync_ShouldReturnFailedOperations()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        await repository.LogOperationAsync(AuditOperationType.Create, "Device", "device-1", success: true);
        await repository.LogOperationAsync(AuditOperationType.Update, "Device", "device-1", success: false, errorMessage: "Network error");
        await repository.LogOperationAsync(AuditOperationType.Delete, "Device", "device-2", success: false, errorMessage: "Not found");

        // Act
        var result = await repository.GetFailedAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => !a.Success);
    }

    [Fact]
    public async Task GetByTimeRangeAsync_ShouldReturnLogsInRange()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        var now = DateTime.UtcNow;

        // Add logs at different times
        await repository.AddAsync(new AuditLogEntity
        {
            EntityType = "Device",
            OperationType = AuditOperationType.Create,
            Timestamp = now.AddDays(-5)
        });
        await repository.AddAsync(new AuditLogEntity
        {
            EntityType = "Device",
            OperationType = AuditOperationType.Update,
            Timestamp = now.AddDays(-1)
        });
        await repository.AddAsync(new AuditLogEntity
        {
            EntityType = "Device",
            OperationType = AuditOperationType.Delete,
            Timestamp = now
        });

        // Act
        var result = await repository.GetByTimeRangeAsync(now.AddDays(-2), now);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentAsync_ShouldReturnMostRecentLogs()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new AuditLogLocalRepository(
            context,
            DatabaseFixture.CreateMockLogger<AuditLogLocalRepository>());

        for (int i = 0; i < 10; i++)
        {
            await repository.AddAsync(new AuditLogEntity
            {
                EntityType = "Device",
                OperationType = AuditOperationType.Read,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        // Act
        var result = await repository.GetRecentAsync(5);

        // Assert
        result.Should().HaveCount(5);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
