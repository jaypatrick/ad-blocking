namespace RulesCompiler.Abstractions;

/// <summary>
/// Interface for the compilation event dispatcher.
/// </summary>
public interface ICompilationEventDispatcher
{
    /// <summary>
    /// Raises the compilation starting event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RaiseCompilationStartingAsync(
        CompilationStartedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raises the configuration loaded event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RaiseConfigurationLoadedAsync(
        ConfigurationLoadedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raises the compilation completed event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RaiseCompilationCompletedAsync(
        CompilationCompletedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Raises the compilation error event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RaiseCompilationErrorAsync(
        CompilationErrorEventArgs args,
        CancellationToken cancellationToken = default);
}