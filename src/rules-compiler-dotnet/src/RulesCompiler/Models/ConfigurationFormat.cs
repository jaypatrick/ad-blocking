namespace RulesCompiler.Models;

/// <summary>
/// Supported configuration file formats.
/// </summary>
public enum ConfigurationFormat
{
    /// <summary>
    /// JSON format (.json extension).
    /// </summary>
    Json,

    /// <summary>
    /// YAML format (.yaml or .yml extension).
    /// </summary>
    Yaml,

    /// <summary>
    /// TOML format (.toml extension).
    /// </summary>
    Toml
}
