using RulesCompiler.Models;

namespace RulesCompiler.Abstractions;

/// <summary>
/// Interface for compiling filter rules using hostlist-compiler.
/// </summary>
public interface IFilterCompiler
{
    /// <summary>
    /// Compiles filter rules using the provided configuration.
    /// </summary>
    /// <param name="configPath">Path to the configuration file.</param>
    /// <param name="outputPath">Optional output file path. If null, uses default.</param>
    /// <param name="format">Optional format override for the configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The compilation result.</returns>
    Task<CompilerResult> CompileAsync(
        string configPath,
        string? outputPath = null,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets version information for all compiler components.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Version information.</returns>
    Task<VersionInfo> GetVersionInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if hostlist-compiler is available.
    /// </summary>
    /// <returns>True if available, false otherwise.</returns>
    Task<bool> IsCompilerAvailableAsync();
}
