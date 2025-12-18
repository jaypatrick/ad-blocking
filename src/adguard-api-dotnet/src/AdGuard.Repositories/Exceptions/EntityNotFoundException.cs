using System.Diagnostics.CodeAnalysis;

namespace AdGuard.Repositories.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found.
/// </summary>
[SuppressMessage("Roslynator", "RCS1194:Implement exception constructors.", Justification = "This exception requires entityType and entityId parameters to provide meaningful context.")]
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
        : base(GetRepositoryName(entityType), "GetById", GetMessage(entityType, entityId))
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
        : base(GetRepositoryName(entityType), "GetById", GetMessage(entityType, entityId), innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    private static string GetRepositoryName(string entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        return $"{entityType}Repository";
    }

    private static string GetMessage(string entityType, string entityId)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(entityId);
        return $"{entityType} with ID '{entityId}' was not found.";
    }
}
