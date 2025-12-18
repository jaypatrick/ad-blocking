using AdGuard.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Unit of Work implementation for local database operations.
/// </summary>
public class LocalUnitOfWork : ILocalUnitOfWork
{
    private readonly AdGuardDbContext _context;
    private readonly ILogger<LocalUnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private IQueryLogLocalRepository? _queryLogs;
    private IStatisticsLocalRepository? _statistics;
    private IAuditLogLocalRepository? _auditLogs;
    private ICompilationHistoryLocalRepository? _compilationHistory;
    private IUserSettingsLocalRepository? _userSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalUnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="queryLogLogger">Logger for query log repository.</param>
    /// <param name="statisticsLogger">Logger for statistics repository.</param>
    /// <param name="auditLogLogger">Logger for audit log repository.</param>
    /// <param name="compilationHistoryLogger">Logger for compilation history repository.</param>
    /// <param name="userSettingsLogger">Logger for user settings repository.</param>
    public LocalUnitOfWork(
        AdGuardDbContext context,
        ILogger<LocalUnitOfWork> logger,
        ILogger<QueryLogLocalRepository> queryLogLogger,
        ILogger<StatisticsLocalRepository> statisticsLogger,
        ILogger<AuditLogLocalRepository> auditLogLogger,
        ILogger<CompilationHistoryLocalRepository> compilationHistoryLogger,
        ILogger<UserSettingsLocalRepository> userSettingsLogger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize repositories lazily with proper loggers
        _queryLogs = new QueryLogLocalRepository(context, queryLogLogger);
        _statistics = new StatisticsLocalRepository(context, statisticsLogger);
        _auditLogs = new AuditLogLocalRepository(context, auditLogLogger);
        _compilationHistory = new CompilationHistoryLocalRepository(context, compilationHistoryLogger);
        _userSettings = new UserSettingsLocalRepository(context, userSettingsLogger);
    }

    /// <inheritdoc />
    public IQueryLogLocalRepository QueryLogs => _queryLogs!;

    /// <inheritdoc />
    public IStatisticsLocalRepository Statistics => _statistics!;

    /// <inheritdoc />
    public IAuditLogLocalRepository AuditLogs => _auditLogs!;

    /// <inheritdoc />
    public ICompilationHistoryLocalRepository CompilationHistory => _compilationHistory!;

    /// <inheritdoc />
    public IUserSettingsLocalRepository UserSettings => _userSettings!;

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogDebug("Database transaction started");
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed");
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back");
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default)
    {
        var created = await _context.Database.EnsureCreatedAsync(cancellationToken);
        if (created)
        {
            _logger.LogInformation("Database created");
        }
        return created;
    }

    /// <inheritdoc />
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Database migrations applied");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the managed resources.
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
            _transaction?.Dispose();
            _context.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Asynchronously disposes managed resources.
    /// </summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        await _context.DisposeAsync();
    }
}
