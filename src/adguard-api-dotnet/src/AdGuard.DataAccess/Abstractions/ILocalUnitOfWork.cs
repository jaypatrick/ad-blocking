namespace AdGuard.DataAccess.Abstractions;

/// <summary>
/// Unit of Work interface for local database operations.
/// </summary>
public interface ILocalUnitOfWork : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets the query log repository.
    /// </summary>
    IQueryLogLocalRepository QueryLogs { get; }

    /// <summary>
    /// Gets the statistics repository.
    /// </summary>
    IStatisticsLocalRepository Statistics { get; }

    /// <summary>
    /// Gets the audit log repository.
    /// </summary>
    IAuditLogLocalRepository AuditLogs { get; }

    /// <summary>
    /// Gets the compilation history repository.
    /// </summary>
    ICompilationHistoryLocalRepository CompilationHistory { get; }

    /// <summary>
    /// Gets the user settings repository.
    /// </summary>
    IUserSettingsLocalRepository UserSettings { get; }

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the database is created.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the database was created, false if it already existed.</returns>
    Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies any pending migrations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
