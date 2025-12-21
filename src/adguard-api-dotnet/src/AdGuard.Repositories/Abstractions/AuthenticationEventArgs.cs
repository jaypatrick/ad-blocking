namespace AdGuard.Repositories.Abstractions;

/// <summary>
/// Event arguments for authentication events.
/// </summary>
public class AuthenticationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the authentication scheme.
    /// </summary>
    public string Scheme { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp of the event.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets whether to cancel the operation.
    /// </summary>
    public bool Cancel { get; set; }
}