namespace RulesCompiler.Abstractions;

/// <summary>
/// Interface for handling compilation lifecycle events.
/// </summary>
public interface ICompilationEventHandler
{
    /// <summary>
    /// Called when compilation is about to start.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task OnCompilationStartingAsync(
        CompilationStartedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when configuration has been loaded.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task OnConfigurationLoadedAsync(
        ConfigurationLoadedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when compilation has completed successfully.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task OnCompilationCompletedAsync(
        CompilationCompletedEventArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when an error occurs during compilation.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task OnCompilationErrorAsync(
        CompilationErrorEventArgs args,
        CancellationToken cancellationToken = default);
}