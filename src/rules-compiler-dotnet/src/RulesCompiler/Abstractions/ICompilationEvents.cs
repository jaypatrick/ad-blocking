using RulesCompiler.Models;

namespace RulesCompiler.Abstractions;

/// <summary>
/// Event arguments for compilation events.
/// </summary>
public class CompilationEventArgs : EventArgs
{
    /// <summary>
    /// Gets the compiler options used for this compilation.
    /// </summary>
    public CompilerOptions Options { get; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationEventArgs"/> class.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    public CompilationEventArgs(CompilerOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }
}

/// <summary>
/// Event arguments for when compilation starts.
/// </summary>
public class CompilationStartedEventArgs : CompilationEventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether to cancel the compilation.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation.
    /// </summary>
    public string? CancelReason { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationStartedEventArgs"/> class.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    public CompilationStartedEventArgs(CompilerOptions options) : base(options) { }
}

/// <summary>
/// Event arguments for when configuration is loaded.
/// </summary>
public class ConfigurationLoadedEventArgs : CompilationEventArgs
{
    /// <summary>
    /// Gets the loaded configuration.
    /// </summary>
    public CompilerConfiguration Configuration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationLoadedEventArgs"/> class.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    /// <param name="configuration">The loaded configuration.</param>
    public ConfigurationLoadedEventArgs(CompilerOptions options, CompilerConfiguration configuration)
        : base(options)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
}

/// <summary>
/// Event arguments for when compilation completes.
/// </summary>
public class CompilationCompletedEventArgs : CompilationEventArgs
{
    /// <summary>
    /// Gets the compilation result.
    /// </summary>
    public CompilerResult Result { get; }

    /// <summary>
    /// Gets the compilation duration.
    /// </summary>
    public TimeSpan Duration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    /// <param name="result">The compilation result.</param>
    /// <param name="duration">The compilation duration.</param>
    public CompilationCompletedEventArgs(
        CompilerOptions options,
        CompilerResult result,
        TimeSpan duration)
        : base(options)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Duration = duration;
    }
}

/// <summary>
/// Event arguments for when an error occurs during compilation.
/// </summary>
public class CompilationErrorEventArgs : CompilationEventArgs
{
    /// <summary>
    /// Gets the exception that occurred.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the error was handled.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationErrorEventArgs"/> class.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    /// <param name="exception">The exception that occurred.</param>
    public CompilationErrorEventArgs(CompilerOptions options, Exception exception)
        : base(options)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}

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
