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
}
