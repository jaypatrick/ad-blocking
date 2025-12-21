namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Base class for extended menu services.
/// </summary>
public abstract class MenuServiceBase : IMenuServiceEx
{
    /// <inheritdoc/>
    public abstract string Id { get; }

    /// <inheritdoc/>
    public abstract string Title { get; }

    /// <inheritdoc/>
    public virtual int Order => 0;

    /// <inheritdoc/>
    public virtual string Description => string.Empty;

    /// <inheritdoc/>
    public virtual string? Icon => null;

    /// <inheritdoc/>
    public virtual bool IsEnabled => true;

    /// <inheritdoc/>
    public virtual bool RequiresAuthentication => true;

    /// <inheritdoc/>
    public abstract Task ShowAsync();
}