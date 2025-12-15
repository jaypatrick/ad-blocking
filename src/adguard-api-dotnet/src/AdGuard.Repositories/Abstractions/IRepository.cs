namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Generic repository interface for entities with full CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
public interface IRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the entity is not found.</exception>
    Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="EntityNotFoundException">Thrown when the entity is not found.</exception>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic repository interface for entities with create operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TCreateModel">The creation model type.</typeparam>
public interface IRepository<TEntity, in TKey, in TCreateModel> : IRepository<TEntity, TKey>
    where TEntity : class
    where TCreateModel : class
{
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="createModel">The creation model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created entity.</returns>
    Task<TEntity> CreateAsync(TCreateModel createModel, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic repository interface for entities with create and update operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TCreateModel">The creation model type.</typeparam>
/// <typeparam name="TUpdateModel">The update model type.</typeparam>
public interface IRepository<TEntity, in TKey, in TCreateModel, in TUpdateModel> : IRepository<TEntity, TKey, TCreateModel>
    where TEntity : class
    where TCreateModel : class
    where TUpdateModel : class
{
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="updateModel">The update model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated entity.</returns>
    /// <exception cref="EntityNotFoundException">Thrown when the entity is not found.</exception>
    Task<TEntity> UpdateAsync(TKey id, TUpdateModel updateModel, CancellationToken cancellationToken = default);
}
