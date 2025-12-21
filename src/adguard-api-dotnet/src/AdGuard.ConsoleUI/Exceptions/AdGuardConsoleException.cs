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