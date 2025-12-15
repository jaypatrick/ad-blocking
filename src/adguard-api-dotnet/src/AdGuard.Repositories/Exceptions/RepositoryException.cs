namespace AdGuard.Repositories.Exceptions;

/// <summary>
/// Base exception for all repository operations.
/// </summary>
public class RepositoryException : Exception
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
