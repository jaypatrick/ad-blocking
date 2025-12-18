using Microsoft.Extensions.Logging;
using RulesCompiler.Abstractions;

namespace RulesCompiler.Services;

/// <summary>
/// Default implementation of the compilation event dispatcher.
/// </summary>
public class CompilationEventDispatcher : ICompilationEventDispatcher
{
    private readonly ILogger<CompilationEventDispatcher> _logger;
    private readonly IEnumerable<ICompilationEventHandler> _handlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilationEventDispatcher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="handlers">The registered event handlers.</param>
    public CompilationEventDispatcher(
        ILogger<CompilationEventDispatcher> logger,
        IEnumerable<ICompilationEventHandler> handlers)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    /// <inheritdoc/>
    public async Task RaiseCompilationStartingAsync(
        CompilationStartedEventArgs args,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Raising CompilationStarting event to {Count} handlers", _handlers.Count());

        foreach (var handler in _handlers)
        {
            try
            {
                await handler.OnCompilationStartingAsync(args, cancellationToken);
                if (args.Cancel)
                {
                    _logger.LogInformation(
                        "Compilation cancelled by handler {Handler}: {Reason}",
                        handler.GetType().Name,
                        args.CancelReason);
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in event handler {Handler} during CompilationStarting",
                    handler.GetType().Name);
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public async Task RaiseConfigurationLoadedAsync(
        ConfigurationLoadedEventArgs args,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Raising ConfigurationLoaded event to {Count} handlers", _handlers.Count());

        foreach (var handler in _handlers)
        {
            try
            {
                await handler.OnConfigurationLoadedAsync(args, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in event handler {Handler} during ConfigurationLoaded",
                    handler.GetType().Name);
                throw;
            }
        }
    }

    /// <inheritdoc/>
    public async Task RaiseCompilationCompletedAsync(
        CompilationCompletedEventArgs args,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Raising CompilationCompleted event to {Count} handlers", _handlers.Count());

        foreach (var handler in _handlers)
        {
            try
            {
                await handler.OnCompilationCompletedAsync(args, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in event handler {Handler} during CompilationCompleted",
                    handler.GetType().Name);
                // Don't rethrow for completion events - compilation already succeeded
            }
        }
    }

    /// <inheritdoc/>
    public async Task RaiseCompilationErrorAsync(
        CompilationErrorEventArgs args,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Raising CompilationError event to {Count} handlers", _handlers.Count());

        foreach (var handler in _handlers)
        {
            try
            {
                await handler.OnCompilationErrorAsync(args, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in event handler {Handler} during CompilationError",
                    handler.GetType().Name);
                // Don't rethrow for error events
            }
        }
    }
}
