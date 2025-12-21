namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Event arguments for authentication failure events.
/// </summary>
public class AuthenticationFailedEventArgs : AuthenticationEventArgs
{
    /// <summary>
    /// Gets the exception that caused the failure.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the failure reason.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the failure was handled.
    /// </summary>
    public bool Handled { get; set; }
}