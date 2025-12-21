namespace RulesCompiler.Abstractions;

/// <summary>
/// Base class for implementing compilation event handlers.
/// Override only the methods you need.
/// </summary>
public abstract class CompilationEventHandlerBase : ICompilationEventHandler
{
    /// <inheritdoc/>
    public virtual Task OnCompilationStartingAsync(
        CompilationStartedEventArgs args,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnConfigurationLoadedAsync(
        ConfigurationLoadedEventArgs args,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnCompilationCompletedAsync(
        CompilationCompletedEventArgs args,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnCompilationErrorAsync(
        CompilationErrorEventArgs args,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}