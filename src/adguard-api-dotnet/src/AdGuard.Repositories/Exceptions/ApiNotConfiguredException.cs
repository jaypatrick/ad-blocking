namespace AdGuard.Repositories.Exceptions;

/// <summary>
/// Exception thrown when the API client is not configured.
/// </summary>
public class ApiNotConfiguredException : RepositoryException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiNotConfiguredException"/> class.
    /// </summary>
    public ApiNotConfiguredException()
        : base("ApiClientFactory", "Configure", "API client is not configured. Please configure your API key first.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiNotConfiguredException"/> class with a custom message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ApiNotConfiguredException(string message)
        : base("ApiClientFactory", "Configure", message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiNotConfiguredException"/> class with a custom message and inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiNotConfiguredException(string message, Exception innerException)
        : base("ApiClientFactory", "Configure", message, innerException)
    {
    }

    /// <summary>
    /// Summary
    /// </summary>
    /// <param name="repositoryName"></param>
    /// <param name="operation"></param>
    /// <param name="message"></param>
    public ApiNotConfiguredException(string repositoryName, string operation, string message) 
        : base(repositoryName, operation, message)
    {
    }

    /// <summary>
    /// Summary
    /// </summary>
    /// <param name="repositoryName"></param>
    /// <param name="operation"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ApiNotConfiguredException(string repositoryName, string operation, string message, Exception innerException) 
        : base(repositoryName, operation, message, innerException)
    {
    }
}
