namespace AdGuard.ConsoleUI.Exceptions;

/// <summary>
/// Base exception for all AdGuard Console UI exceptions.
/// </summary>
public class AdGuardConsoleException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdGuardConsoleException"/> class.
    /// </summary>
    public AdGuardConsoleException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdGuardConsoleException"/> class with a message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public AdGuardConsoleException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdGuardConsoleException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AdGuardConsoleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a repository operation fails.
/// </summary>
public class RepositoryException : AdGuardConsoleException
{
    /// <summary>
    /// Gets the name of the repository that failed.
    /// </summary>
    public string RepositoryName { get; }

    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class.
    /// </summary>
    /// <param name="repositoryName">The name of the repository.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The exception message.</param>
    public RepositoryException(string repositoryName, string operation, string message)
        : base(message)
    {
        RepositoryName = repositoryName;
        Operation = operation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with an inner exception.
    /// </summary>
    /// <param name="repositoryName">The name of the repository.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public RepositoryException(string repositoryName, string operation, string message, Exception innerException)
        : base(message, innerException)
    {
        RepositoryName = repositoryName;
        Operation = operation;
    }
}

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
}

/// <summary>
/// Exception thrown when the API client is not configured.
/// </summary>
public class ApiNotConfiguredException : AdGuardConsoleException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiNotConfiguredException"/> class.
    /// </summary>
    public ApiNotConfiguredException()
        : base("API client is not configured. Please configure your API key first.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiNotConfiguredException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ApiNotConfiguredException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : AdGuardConsoleException
{
    /// <summary>
    /// Gets the name of the parameter that failed validation.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="message">The validation message.</param>
    public ValidationException(string parameterName, string message)
        : base(message)
    {
        ParameterName = parameterName;
    }
}

/// <summary>
/// Exception thrown when a menu operation fails.
/// </summary>
public class MenuOperationException : AdGuardConsoleException
{
    /// <summary>
    /// Gets the menu name where the operation failed.
    /// </summary>
    public string MenuName { get; }

    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuOperationException"/> class.
    /// </summary>
    /// <param name="menuName">The menu name.</param>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MenuOperationException(string menuName, string operation, string message, Exception innerException)
        : base(message, innerException)
    {
        MenuName = menuName;
        Operation = operation;
    }
}
