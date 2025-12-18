namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Generic read-only repository interface for entities.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all entities.</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
