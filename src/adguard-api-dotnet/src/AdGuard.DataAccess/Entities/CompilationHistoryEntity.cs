namespace AdGuard.DataAccess.Entities;

/// <summary>
/// Represents a record of a filter rules compilation job.
/// </summary>
public class CompilationHistoryEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for this compilation record.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets when the compilation started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when the compilation completed (null if still running or failed).
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the path to the configuration file used.
    /// </summary>
    public required string ConfigurationPath { get; set; }

    /// <summary>
    /// Gets or sets the format of the configuration file (JSON, YAML, TOML).
    /// </summary>
    public string? ConfigurationFormat { get; set; }

    /// <summary>
    /// Gets or sets the name of the filter list being compiled.
    /// </summary>
    public string? FilterListName { get; set; }

    /// <summary>
    /// Gets or sets the path where the output was written.
    /// </summary>
    public string? OutputPath { get; set; }

    /// <summary>
    /// Gets or sets the number of rules in the compiled output.
    /// </summary>
    public int RuleCount { get; set; }

    /// <summary>
    /// Gets or sets the SHA256 hash of the compiled output.
    /// </summary>
    public string? OutputHash { get; set; }

    /// <summary>
    /// Gets or sets the size of the output file in bytes.
    /// </summary>
    public long? OutputSizeBytes { get; set; }

    /// <summary>
    /// Gets or sets whether the compilation succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if compilation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the stack trace if compilation failed.
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the duration of the compilation in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Gets or sets whether the output was copied to the rules directory.
    /// </summary>
    public bool CopiedToRules { get; set; }

    /// <summary>
    /// Gets or sets the number of sources processed.
    /// </summary>
    public int SourceCount { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized list of sources.
    /// </summary>
    public string? SourcesJson { get; set; }

    /// <summary>
    /// Gets or sets the JSON-serialized list of transformations applied.
    /// </summary>
    public string? TransformationsJson { get; set; }

    /// <summary>
    /// Gets or sets the version of the compiler used.
    /// </summary>
    public string? CompilerVersion { get; set; }

    /// <summary>
    /// Gets or sets the version of the hostlist-compiler used.
    /// </summary>
    public string? HostlistCompilerVersion { get; set; }

    /// <summary>
    /// Gets or sets the machine name where compilation was executed.
    /// </summary>
    public string? MachineName { get; set; }

    /// <summary>
    /// Gets or sets additional context as JSON.
    /// </summary>
    public string? ContextJson { get; set; }

    /// <summary>
    /// Gets the duration as a TimeSpan.
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromMilliseconds(DurationMs);
}
