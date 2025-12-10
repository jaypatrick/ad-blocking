using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using RulesCompiler.Abstractions;

namespace RulesCompiler.Services;

/// <summary>
/// Writes compiled output to destination directories.
/// </summary>
public class OutputWriter : IOutputWriter
{
    private readonly ILogger<OutputWriter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputWriter"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public OutputWriter(ILogger<OutputWriter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<bool> CopyOutputAsync(
        string sourcePath,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("Source path cannot be null or empty.", nameof(sourcePath));

        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Destination path cannot be null or empty.", nameof(destinationPath));

        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}", sourcePath);

        try
        {
            // Ensure destination directory exists
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
                _logger.LogDebug("Created directory: {Directory}", destDir);
            }

            // Copy file with UTF-8 encoding to ensure cross-platform compatibility
            var content = await File.ReadAllTextAsync(sourcePath, Encoding.UTF8, cancellationToken);
            await File.WriteAllTextAsync(destinationPath, content, Encoding.UTF8, cancellationToken);

            _logger.LogInformation("Copied output to {Destination}", destinationPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to copy output to {Destination}", destinationPath);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<string> ComputeHashAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        await using var stream = File.OpenRead(filePath);
        var hashBytes = await SHA384.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <inheritdoc/>
    public async Task<int> CountRulesAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8, cancellationToken);

        // Count non-empty, non-comment lines
        return lines.Count(line =>
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            var trimmed = line.Trim();

            // Skip comments (lines starting with ! or #)
            if (trimmed.StartsWith('!') || trimmed.StartsWith('#'))
                return false;

            return true;
        });
    }
}
