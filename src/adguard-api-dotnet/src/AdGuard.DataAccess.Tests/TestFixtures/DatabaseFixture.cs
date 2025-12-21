namespace AdGuard.DataAccess.Tests.TestFixtures;

/// <summary>
/// Provides an in-memory database fixture for tests.
/// </summary>
public class DatabaseFixture : IDisposable
{
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFixture"/> class.
    /// </summary>
    public DatabaseFixture()
    {
        _databaseName = $"AdGuardTestDb_{Guid.NewGuid()}";
    }

    /// <summary>
    /// Creates a new <see cref="AdGuardDbContext"/> with an in-memory database.
    /// </summary>
    /// <returns>A new database context.</returns>
    public AdGuardDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AdGuardDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        return new AdGuardDbContext(options);
    }

    /// <summary>
    /// Creates a mock logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A mock logger.</returns>
    public static ILogger<T> CreateMockLogger<T>()
    {
        return Mock.Of<ILogger<T>>();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Cleanup in-memory database
            using var context = CreateContext();
            context.Database.EnsureDeleted();
        }

        _disposed = true;
    }
}
