using Microsoft.Extensions.Logging;
using RulesCompiler.Abstractions;
using RulesCompiler.Models;

namespace RulesCompiler.Services;

/// <summary>
/// Main orchestration service for the rules compiler pipeline.
/// </summary>
public class RulesCompilerService : IRulesCompilerService
{
    private readonly ILogger<RulesCompilerService> _logger;
    private readonly IConfigurationReader _configurationReader;
    private readonly IFilterCompiler _filterCompiler;
    private readonly IOutputWriter _outputWriter;

    private const string DefaultConfigFileName = "compiler-config.json";
    private const string DefaultRulesFileName = "adguard_user_filter.txt";

    /// <summary>
    /// Initializes a new instance of the <see cref="RulesCompilerService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="configurationReader">The configuration reader.</param>
    /// <param name="filterCompiler">The filter compiler.</param>
    /// <param name="outputWriter">The output writer.</param>
    public RulesCompilerService(
        ILogger<RulesCompilerService> logger,
        IConfigurationReader configurationReader,
        IFilterCompiler filterCompiler,
        IOutputWriter outputWriter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
        _filterCompiler = filterCompiler ?? throw new ArgumentNullException(nameof(filterCompiler));
        _outputWriter = outputWriter ?? throw new ArgumentNullException(nameof(outputWriter));
    }

    /// <inheritdoc/>
    public async Task<CompilerResult> RunAsync(
        string? configPath = null,
        string? outputPath = null,
        bool copyToRules = false,
        string? rulesDirectory = null,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default)
    {
        // Resolve config path
        var actualConfigPath = ResolveConfigPath(configPath);
        _logger.LogInformation("Starting compilation with config: {ConfigPath}", actualConfigPath);

        // Run compilation
        var result = await _filterCompiler.CompileAsync(
            actualConfigPath,
            outputPath,
            format,
            cancellationToken);

        if (!result.Success)
        {
            _logger.LogError("Compilation failed: {Error}", result.ErrorMessage);
            return result;
        }

        // Count rules and compute hash
        result.RuleCount = await _outputWriter.CountRulesAsync(result.OutputPath, cancellationToken);
        result.OutputHash = await _outputWriter.ComputeHashAsync(result.OutputPath, cancellationToken);

        _logger.LogInformation("Compiled {RuleCount} rules, hash: {Hash}", result.RuleCount, result.OutputHash[..16] + "...");

        // Copy to rules directory if requested
        if (copyToRules)
        {
            var rulesPath = ResolveRulesPath(rulesDirectory, actualConfigPath);
            result.CopiedToRules = await _outputWriter.CopyOutputAsync(result.OutputPath, rulesPath, cancellationToken);
            result.RulesDestination = rulesPath;

            if (result.CopiedToRules)
            {
                _logger.LogInformation("Copied output to rules directory: {Path}", rulesPath);
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<VersionInfo> GetVersionInfoAsync(CancellationToken cancellationToken = default)
    {
        return await _filterCompiler.GetVersionInfoAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CompilerConfiguration> ReadConfigurationAsync(
        string? configPath = null,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default)
    {
        var actualConfigPath = ResolveConfigPath(configPath);
        return await _configurationReader.ReadConfigurationAsync(actualConfigPath, format, cancellationToken);
    }

    private static string ResolveConfigPath(string? configPath)
    {
        if (!string.IsNullOrWhiteSpace(configPath))
        {
            if (Path.IsPathRooted(configPath))
                return configPath;

            return Path.GetFullPath(configPath);
        }

        // Try to find default config in common locations
        var searchPaths = new[]
        {
            DefaultConfigFileName,
            Path.Combine("src", "filter-compiler", DefaultConfigFileName),
            Path.Combine("..", "filter-compiler", DefaultConfigFileName),
            Path.Combine("..", "..", "src", "filter-compiler", DefaultConfigFileName)
        };

        foreach (var path in searchPaths)
        {
            var fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
                return fullPath;
        }

        throw new FileNotFoundException(
            $"Configuration file not found. Searched: {string.Join(", ", searchPaths)}. " +
            "Please specify the config path explicitly.");
    }

    private static string ResolveRulesPath(string? rulesDirectory, string configPath)
    {
        if (!string.IsNullOrWhiteSpace(rulesDirectory))
        {
            return Path.Combine(rulesDirectory, DefaultRulesFileName);
        }

        // Default: relative to config location
        var configDir = Path.GetDirectoryName(configPath) ?? ".";
        var defaultRulesDir = Path.Combine(configDir, "..", "..", "rules");

        return Path.GetFullPath(Path.Combine(defaultRulesDir, DefaultRulesFileName));
    }
}
