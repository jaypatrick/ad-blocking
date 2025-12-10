using Newtonsoft.Json;

namespace RulesCompiler.Models;

/// <summary>
/// Represents the configuration for the hostlist-compiler.
/// Supports JSON, YAML, and TOML configuration file formats.
/// </summary>
public class CompilerConfiguration
{
    /// <summary>
    /// Gets or sets the name of the filter list.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the filter list.
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the homepage URL.
    /// </summary>
    [JsonProperty("homepage")]
    public string Homepage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the license identifier.
    /// </summary>
    [JsonProperty("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the filter list.
    /// </summary>
    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of source filter lists to compile.
    /// </summary>
    [JsonProperty("sources")]
    public List<FilterSource> Sources { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of transformations to apply during compilation.
    /// </summary>
    [JsonProperty("transformations")]
    public List<string> Transformations { get; set; } = [];

    /// <summary>
    /// Gets or sets the global inclusions patterns.
    /// </summary>
    [JsonProperty("inclusions")]
    public List<string> Inclusions { get; set; } = [];

    /// <summary>
    /// Gets or sets the global exclusions patterns.
    /// </summary>
    [JsonProperty("exclusions")]
    public List<string> Exclusions { get; set; } = [];
}

/// <summary>
/// Represents a source filter list to compile.
/// </summary>
public class FilterSource
{
    /// <summary>
    /// Gets or sets the name of the source.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source URL or file path.
    /// </summary>
    [JsonProperty("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the source (e.g., "adblock", "hosts").
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "adblock";

    /// <summary>
    /// Gets or sets the inclusions patterns for this source.
    /// </summary>
    [JsonProperty("inclusions")]
    public List<string> Inclusions { get; set; } = [];

    /// <summary>
    /// Gets or sets the exclusions patterns for this source.
    /// </summary>
    [JsonProperty("exclusions")]
    public List<string> Exclusions { get; set; } = [];
}
