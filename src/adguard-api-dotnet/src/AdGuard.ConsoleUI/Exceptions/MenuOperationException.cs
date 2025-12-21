namespace AdGuard.ConsoleUI.Exceptions;

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