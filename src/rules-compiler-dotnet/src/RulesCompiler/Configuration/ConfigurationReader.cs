using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RulesCompiler.Abstractions;
using RulesCompiler.Models;
using Tomlyn;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RulesCompiler.Configuration;

/// <summary>
/// Reads compiler configuration from JSON, YAML, or TOML files.
/// </summary>
public class ConfigurationReader : IConfigurationReader
{
    private readonly ILogger<ConfigurationReader> _logger;
    private readonly IDeserializer _yamlDeserializer;
    private readonly ISerializer _yamlSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationReader"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ConfigurationReader(ILogger<ConfigurationReader> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    /// <inheritdoc/>
    public async Task<CompilerConfiguration> ReadConfigurationAsync(
        string configPath,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configPath))
            throw new ArgumentException("Config path cannot be null or empty.", nameof(configPath));

        if (!File.Exists(configPath))
            throw new FileNotFoundException($"Configuration file not found: {configPath}", configPath);

        var detectedFormat = format ?? DetectFormat(configPath);
        _logger.LogDebug("Reading configuration from {Path} with format {Format}", configPath, detectedFormat);

        var content = await File.ReadAllTextAsync(configPath, cancellationToken);

        return detectedFormat switch
        {
            ConfigurationFormat.Json => ParseJson(content),
            ConfigurationFormat.Yaml => ParseYaml(content),
            ConfigurationFormat.Toml => ParseToml(content),
            _ => throw new ArgumentException($"Unsupported format: {detectedFormat}")
        };
    }

    /// <inheritdoc/>
    public ConfigurationFormat DetectFormat(string filePath)
    {
        var extension = Path.GetExtension(filePath)?.ToLowerInvariant();

        return extension switch
        {
            ".json" => ConfigurationFormat.Json,
            ".yaml" or ".yml" => ConfigurationFormat.Yaml,
            ".toml" => ConfigurationFormat.Toml,
            _ => throw new ArgumentException($"Unknown configuration file extension: {extension}")
        };
    }

    /// <inheritdoc/>
    public string ToJson(CompilerConfiguration configuration)
    {
        return JsonConvert.SerializeObject(configuration, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        });
    }

    private CompilerConfiguration ParseJson(string content)
    {
        var config = JsonConvert.DeserializeObject<CompilerConfiguration>(content);
        return config ?? throw new InvalidOperationException("Failed to parse JSON configuration");
    }

    private CompilerConfiguration ParseYaml(string content)
    {
        var config = _yamlDeserializer.Deserialize<CompilerConfiguration>(content);
        return config ?? throw new InvalidOperationException("Failed to parse YAML configuration");
    }

    private CompilerConfiguration ParseToml(string content)
    {
        // Use Tomlyn's direct model deserialization instead of double serialization
        var config = Toml.ToModel<CompilerConfiguration>(content);
        return config ?? throw new InvalidOperationException("Failed to parse TOML configuration");
    }
}
