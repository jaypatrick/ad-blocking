namespace AdGuard.ConsoleUI.Exceptions;

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