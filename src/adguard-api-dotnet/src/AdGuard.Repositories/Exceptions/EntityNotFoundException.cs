namespace AdGuard.Repositories.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
public class EntityNotFoundException : RepositoryException
{
    /// <summary>
    /// Gets the entity type that was not found.
    /// </summary>
    public string EntityType { get; }

    /// <summary>
    /// Gets the entity identifier that was not found.
    /// </summary>
    public string EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The type of entity.</param>
    /// <param name="entityId">The entity identifier.</param>
    public EntityNotFoundException(string entityType, string entityId)
        : base(entityType + "Repository", "GetById", $"{entityType} with ID '{entityId}' was not found.")
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with an inner exception.
    /// </summary>
    /// <param name="entityType">The type of entity.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="innerException">The inner exception.</param>
    public EntityNotFoundException(string entityType, string entityId, Exception innerException)
        : base(entityType + "Repository", "GetById", $"{entityType} with ID '{entityId}' was not found.", innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
