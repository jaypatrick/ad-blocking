using RulesCompiler.Models;

namespace RulesCompiler.Configuration;

/// <summary>
/// Validates compiler configuration before compilation.
/// </summary>
public static class ConfigurationValidator
{
    /// <summary>
    /// Represents a validation error in the configuration.
    /// </summary>
    /// <param name="Field">The field or path where the error occurred.</param>
    /// <param name="Message">A description of the validation error.</param>
    public record ValidationError(string Field, string Message);

    /// <summary>
    /// Represents the result of configuration validation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Gets whether the configuration is valid.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Gets the list of validation errors.
        /// </summary>
        public List<ValidationError> Errors { get; } = [];

        /// <summary>
        /// Gets the list of validation warnings (non-fatal issues).
        /// </summary>
        public List<ValidationError> Warnings { get; } = [];

        /// <summary>
        /// Adds an error to the validation result.
        /// </summary>
        public void AddError(string field, string message)
        {
            Errors.Add(new ValidationError(field, message));
        }

        /// <summary>
        /// Adds a warning to the validation result.
        /// </summary>
        public void AddWarning(string field, string message)
        {
            Warnings.Add(new ValidationError(field, message));
        }
    }

    /// <summary>
    /// Validates a compiler configuration.
    /// </summary>
    /// <param name="config">The configuration to validate.</param>
    /// <returns>A validation result containing any errors or warnings.</returns>
    public static ValidationResult Validate(CompilerConfiguration config)
    {
        var result = new ValidationResult();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            result.AddError("name", "Name is required");
        }

        if (config.Sources.Count == 0)
        {
            result.AddError("sources", "At least one source is required");
        }

        // Validate global transformations
        ValidateTransformations(config.Transformations, "transformations", result);

        // Validate each source
        for (int i = 0; i < config.Sources.Count; i++)
        {
            ValidateSource(config.Sources[i], $"sources[{i}]", result);
        }

        // Validate inclusion/exclusion patterns
        ValidatePatterns(config.Inclusions, "inclusions", result);
        ValidatePatterns(config.Exclusions, "exclusions", result);
        ValidateSourceFiles(config.InclusionsSources, "inclusions_sources", result);
        ValidateSourceFiles(config.ExclusionsSources, "exclusions_sources", result);

        return result;
    }

    private static void ValidateSource(FilterSource source, string path, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(source.Source))
        {
            result.AddError($"{path}.source", "Source URL or path is required");
        }

        // Validate source type
        if (!string.IsNullOrEmpty(source.Type) && !SourceTypeHelper.IsValid(source.Type))
        {
            result.AddError($"{path}.type",
                $"Invalid source type '{source.Type}'. Valid types are: {string.Join(", ", SourceTypeHelper.AllSourceTypes)}");
        }

        // Validate source-specific transformations
        ValidateTransformations(source.Transformations, $"{path}.transformations", result);

        // Validate patterns
        ValidatePatterns(source.Inclusions, $"{path}.inclusions", result);
        ValidatePatterns(source.Exclusions, $"{path}.exclusions", result);
        ValidateSourceFiles(source.InclusionsSources, $"{path}.inclusions_sources", result);
        ValidateSourceFiles(source.ExclusionsSources, $"{path}.exclusions_sources", result);
    }

    private static void ValidateTransformations(List<string> transformations, string path, ValidationResult result)
    {
        var invalidTransformations = TransformationHelper.GetInvalidTransformations(transformations);
        foreach (var invalid in invalidTransformations)
        {
            result.AddError(path,
                $"Invalid transformation '{invalid}'. Valid transformations are: {string.Join(", ", TransformationHelper.AllTransformations)}");
        }
    }

    private static void ValidatePatterns(List<string> patterns, string path, ValidationResult result)
    {
        for (int i = 0; i < patterns.Count; i++)
        {
            var pattern = patterns[i];

            // Check for regex patterns
            if (pattern.StartsWith("/") && pattern.EndsWith("/") && pattern.Length > 2)
            {
                // Try to validate regex syntax
                try
                {
                    var regexPattern = pattern[1..^1]; // Remove leading and trailing slashes
                    _ = new System.Text.RegularExpressions.Regex(regexPattern);
                }
                catch (System.Text.RegularExpressions.RegexParseException ex)
                {
                    result.AddWarning($"{path}[{i}]", $"Invalid regex pattern: {ex.Message}");
                }
            }
        }
    }

    private static void ValidateSourceFiles(List<string> sources, string path, ValidationResult result)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            var source = sources[i];

            // Skip URL sources - they'll be validated at runtime
            if (Uri.TryCreate(source, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "http" || uri.Scheme == "https"))
            {
                continue;
            }

            // For local files, check if they exist
            if (!string.IsNullOrEmpty(source) && !File.Exists(source))
            {
                result.AddWarning($"{path}[{i}]", $"Source file not found: {source}");
            }
        }
    }
}
