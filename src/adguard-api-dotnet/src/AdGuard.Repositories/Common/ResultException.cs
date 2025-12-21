namespace AdGuard.Repositories.Common;

/// <summary>
/// Exception thrown when attempting to get a value from a failed result.
/// </summary>
public sealed class ResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    /// <param name="error">The error that caused the exception.</param>
    public ResultException(Error error)
        : base($"[{error.Code}] {error.Message}", error.InnerException)
    {
        Error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    public ResultException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ResultException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public ResultException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets the error that caused the exception.
    /// </summary>
    public Error? Error { get; }
}