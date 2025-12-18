using System.Linq.Expressions;
using AdGuard.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdGuard.DataAccess.Repositories;

/// <summary>
/// Base implementation of <see cref="ILocalRepository{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class LocalRepositoryBase<TEntity> : ILocalRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the database context.
    /// </summary>
    protected AdGuardDbContext Context { get; }

    /// <summary>
    /// Gets the DbSet for this entity type.
    /// </summary>
    protected DbSet<TEntity> DbSet { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    protected LocalRepositoryBase(AdGuardDbContext context, ILogger logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DbSet = context.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        if (entity == null)
        {
            return false;
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public virtual async Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }
}
