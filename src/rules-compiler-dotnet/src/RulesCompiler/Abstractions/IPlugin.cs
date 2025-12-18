namespace RulesCompiler.Abstractions;

/// <summary>
/// Base interface for RulesCompiler plugins.
/// Plugins extend the compiler with custom functionality.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Gets the unique identifier for this plugin.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display name for this plugin.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the plugin version.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets a brief description of what this plugin does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Initializes the plugin. Called once during application startup.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value indicating whether this plugin is enabled.
    /// </summary>
    bool IsEnabled { get; }
}

/// <summary>
/// Interface for plugins that process rules during compilation.
/// </summary>
public interface IRuleTransformationPlugin : IPlugin
{
    /// <summary>
    /// Gets the execution order for this transformation. Lower values execute first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Transforms the rules during compilation.
    /// </summary>
    /// <param name="rules">The current set of rules.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The transformed rules.</returns>
    Task<IEnumerable<string>> TransformAsync(
        IEnumerable<string> rules,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for plugins that validate rules.
/// </summary>
public interface IRuleValidationPlugin : IPlugin
{
    /// <summary>
    /// Validates the compiled rules.
    /// </summary>
    /// <param name="rules">The rules to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with any issues found.</returns>
    Task<PluginValidationResult> ValidateAsync(
        IEnumerable<string> rules,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for plugins that provide custom configuration formats.
/// </summary>
public interface IConfigurationFormatPlugin : IPlugin
{
    /// <summary>
    /// Gets the file extensions this plugin handles (e.g., ".xml", ".ini").
    /// </summary>
    IReadOnlyCollection<string> SupportedExtensions { get; }

    /// <summary>
    /// Parses configuration from the specified content.
    /// </summary>
    /// <param name="content">The configuration file content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The parsed configuration.</returns>
    Task<Models.CompilerConfiguration> ParseAsync(
        string content,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for plugins that provide custom output destinations.
/// </summary>
public interface IOutputDestinationPlugin : IPlugin
{
    /// <summary>
    /// Gets the scheme this destination handles (e.g., "s3", "azure", "ftp").
    /// </summary>
    string Scheme { get; }

    /// <summary>
    /// Writes compiled output to the destination.
    /// </summary>
    /// <param name="content">The compiled output content.</param>
    /// <param name="destination">The destination URI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> WriteAsync(
        string content,
        Uri destination,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result from a plugin validation operation.
/// </summary>
public class PluginValidationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether validation passed.
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IList<PluginValidationError> Errors { get; } = [];

    /// <summary>
    /// Gets the validation warnings.
    /// </summary>
    public IList<PluginValidationError> Warnings { get; } = [];

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static PluginValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with the specified errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public static PluginValidationResult Failure(params PluginValidationError[] errors)
    {
        var result = new PluginValidationResult { IsValid = false };
        foreach (var error in errors)
        {
            result.Errors.Add(error);
        }
        return result;
    }
}

/// <summary>
/// Represents a validation error or warning from a plugin.
/// </summary>
public class PluginValidationError
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number where the error occurred, if applicable.
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the rule that caused the error, if applicable.
    /// </summary>
    public string? Rule { get; set; }
}
