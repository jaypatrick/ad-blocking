using RulesCompiler.Models;

namespace RulesCompiler.Abstractions;

/// <summary>
/// Represents context passed through the compilation pipeline.
/// </summary>
public class CompilationContext
{
    /// <summary>
    /// Gets or sets the compiler options.
    /// </summary>
    public CompilerOptions Options { get; set; } = new();

    /// <summary>
    /// Gets or sets the loaded configuration.
    /// </summary>
    public CompilerConfiguration? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the compilation result.
    /// </summary>
    public CompilerResult? Result { get; set; }

    /// <summary>
    /// Gets the items collection for passing data between middleware.
    /// </summary>
    public IDictionary<string, object> Items { get; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets a value indicating whether compilation should be cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the reason for cancellation.
    /// </summary>
    public string? CancellationReason { get; set; }

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; set; }
}

/// <summary>
/// Delegate for the next middleware in the pipeline.
/// </summary>
/// <param name="context">The compilation context.</param>
public delegate Task CompilationDelegate(CompilationContext context);

/// <summary>
/// Interface for middleware components in the compilation pipeline.
/// </summary>
public interface ICompilationMiddleware
{
    /// <summary>
    /// Gets the execution order for this middleware. Lower values execute first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Processes the compilation context and optionally calls the next middleware.
    /// </summary>
    /// <param name="context">The compilation context.</param>
    /// <param name="next">The next middleware in the pipeline.</param>
    Task InvokeAsync(CompilationContext context, CompilationDelegate next);
}

/// <summary>
/// Interface for building and executing the compilation pipeline.
/// </summary>
public interface ICompilationPipelineBuilder
{
    /// <summary>
    /// Adds a middleware component to the pipeline.
    /// </summary>
    /// <typeparam name="T">The middleware type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ICompilationPipelineBuilder Use<T>() where T : ICompilationMiddleware;

    /// <summary>
    /// Adds a middleware component to the pipeline.
    /// </summary>
    /// <param name="middleware">The middleware instance.</param>
    /// <returns>The builder for chaining.</returns>
    ICompilationPipelineBuilder Use(ICompilationMiddleware middleware);

    /// <summary>
    /// Adds a middleware delegate to the pipeline.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The builder for chaining.</returns>
    ICompilationPipelineBuilder Use(Func<CompilationContext, CompilationDelegate, Task> middleware);

    /// <summary>
    /// Builds the pipeline and returns the entry delegate.
    /// </summary>
    CompilationDelegate Build();
}

/// <summary>
/// Interface for executing the compilation pipeline.
/// </summary>
public interface ICompilationPipeline
{
    /// <summary>
    /// Executes the compilation pipeline.
    /// </summary>
    /// <param name="options">The compiler options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The compilation result.</returns>
    Task<CompilerResult> ExecuteAsync(
        CompilerOptions options,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for implementing compilation middleware.
/// </summary>
public abstract class CompilationMiddlewareBase : ICompilationMiddleware
{
    /// <inheritdoc/>
    public virtual int Order => 0;

    /// <inheritdoc/>
    public abstract Task InvokeAsync(CompilationContext context, CompilationDelegate next);

    /// <summary>
    /// Cancels the compilation with the specified reason.
    /// </summary>
    /// <param name="context">The compilation context.</param>
    /// <param name="reason">The cancellation reason.</param>
    protected static void Cancel(CompilationContext context, string reason)
    {
        context.IsCancelled = true;
        context.CancellationReason = reason;
    }
}
