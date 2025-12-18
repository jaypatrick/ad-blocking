using Microsoft.Extensions.Logging;
using RulesCompiler.Abstractions;
using RulesCompiler.Configuration;
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
    public Task<CompilerResult> RunAsync(
        string? configPath = null,
        string? outputPath = null,
        bool copyToRules = false,
        string? rulesDirectory = null,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default)
    {
        var options = new CompilerOptions
        {
            ConfigPath = configPath,
            OutputPath = outputPath,
            CopyToRules = copyToRules,
            RulesDirectory = rulesDirectory,
            Format = format
        };
        return RunAsync(options, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CompilerResult> RunAsync(
        CompilerOptions options,
        CancellationToken cancellationToken = default)
    {
        // Resolve config path
        var actualConfigPath = ResolveConfigPath(options.ConfigPath);
        _logger.LogInformation("Starting compilation with config: {ConfigPath}", actualConfigPath);

        // Validate configuration if requested
        if (options.ValidateConfig)
        {
            var validation = await ValidateConfigurationAsync(actualConfigPath, options.Format, cancellationToken);

            if (!validation.IsValid)
            {
                _logger.LogError("Configuration validation failed:");
                foreach (var error in validation.Errors)
                {
                    _logger.LogError("  [{Field}] {Message}", error.Field, error.Message);
                }

                return new CompilerResult
                {
                    Success = false,
                    ErrorMessage = $"Configuration validation failed: {string.Join("; ", validation.Errors.Select(e => $"{e.Field}: {e.Message}"))}"
                };
            }

            if (validation.Warnings.Count > 0)
            {
                foreach (var warning in validation.Warnings)
                {
                    _logger.LogWarning("Configuration warning: [{Field}] {Message}", warning.Field, warning.Message);
                }

                if (options.FailOnWarnings)
                {
                    return new CompilerResult
                    {
                        Success = false,
                        ErrorMessage = $"Configuration has warnings (FailOnWarnings is enabled): {string.Join("; ", validation.Warnings.Select(w => $"{w.Field}: {w.Message}"))}"
                    };
                }
            }
        }

        // Update options with resolved path
        var compilerOptions = new CompilerOptions
        {
            ConfigPath = actualConfigPath,
            OutputPath = options.OutputPath,
            Format = options.Format,
            Verbose = options.Verbose
        };

        // Run compilation
        var result = await _filterCompiler.CompileAsync(compilerOptions, cancellationToken);

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
        if (options.CopyToRules)
        {
            var rulesPath = ResolveRulesPath(options.RulesDirectory, actualConfigPath);
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

    /// <inheritdoc/>
    public async Task<ConfigurationValidator.ValidationResult> ValidateConfigurationAsync(
        string? configPath = null,
        ConfigurationFormat? format = null,
        CancellationToken cancellationToken = default)
    {
        var config = await ReadConfigurationAsync(configPath, format, cancellationToken);
        return ValidateConfiguration(config);
    }

    /// <inheritdoc/>
    public ConfigurationValidator.ValidationResult ValidateConfiguration(CompilerConfiguration configuration)
    {
        return ConfigurationValidator.Validate(configuration);
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
            Path.Combine("src", "rules-compiler-typescript", DefaultConfigFileName),
            Path.Combine("..", "rules-compiler-typescript", DefaultConfigFileName),
            Path.Combine("..", "..", "src", "rules-compiler-typescript", DefaultConfigFileName)
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
