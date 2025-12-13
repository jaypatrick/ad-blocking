namespace RulesCompiler.Models;

/// <summary>
/// Defines the available transformations that can be applied during filter list compilation.
/// </summary>
/// <remarks>
/// <para>
/// Transformations are always applied in a fixed order regardless of the order specified
/// in the configuration. The execution order is:
/// <list type="number">
///   <item><description><see cref="RemoveComments"/></description></item>
///   <item><description><see cref="Compress"/></description></item>
///   <item><description><see cref="RemoveModifiers"/></description></item>
///   <item><description><see cref="Validate"/></description></item>
///   <item><description><see cref="ValidateAllowIp"/></description></item>
///   <item><description><see cref="Deduplicate"/></description></item>
///   <item><description><see cref="InvertAllow"/></description></item>
///   <item><description><see cref="RemoveEmptyLines"/></description></item>
///   <item><description><see cref="TrimLines"/></description></item>
///   <item><description><see cref="InsertFinalNewLine"/></description></item>
///   <item><description><see cref="ConvertToAscii"/></description></item>
/// </list>
/// </para>
/// <para>
/// See: https://github.com/AdguardTeam/HostlistCompiler#transformations
/// </para>
/// </remarks>
public enum Transformation
{
    /// <summary>
    /// Removes all comment lines from the filter list.
    /// </summary>
    /// <remarks>
    /// Comment lines start with "!" or "#". This is a simple transformation
    /// that removes all lines beginning with these characters.
    /// </remarks>
    RemoveComments,

    /// <summary>
    /// Compresses and converts hosts format lists to adblock syntax.
    /// </summary>
    /// <remarks>
    /// This transformation is useful when importing hosts files and converting
    /// them to the more compact adblock format supported by AdGuard.
    /// </remarks>
    Compress,

    /// <summary>
    /// Removes unsupported modifiers from rules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// AdGuard Home ignores rules with unsupported modifiers by default.
    /// However, rules with these modifiers are often valid for DNS-level blocking.
    /// </para>
    /// <para>
    /// Use this transformation when importing traditional ad blocker filter lists
    /// to make more rules compatible with DNS-level blocking.
    /// </para>
    /// </remarks>
    RemoveModifiers,

    /// <summary>
    /// Validates and removes dangerous or incompatible rules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This transformation is crucial when using traditional ad blocker filter lists
    /// as sources. It removes rules that could be dangerous or incompatible with
    /// DNS-level blocking.
    /// </para>
    /// <para>
    /// Recommended when importing from uBlock Origin or similar browser-based
    /// ad blocker filter lists.
    /// </para>
    /// </remarks>
    Validate,

    /// <summary>
    /// Validates rules but allows IP address rules.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="Validate"/> but permits rules that block specific
    /// IP addresses. Use this when you need to preserve IP-based blocking rules.
    /// </remarks>
    ValidateAllowIp,

    /// <summary>
    /// Removes duplicate rules from the compiled output.
    /// </summary>
    /// <remarks>
    /// When combining multiple filter list sources, duplicate rules are common.
    /// This transformation removes exact duplicates to reduce the final file size.
    /// </remarks>
    Deduplicate,

    /// <summary>
    /// Inverts allowlist rules to blocklist rules.
    /// </summary>
    /// <remarks>
    /// Converts rules prefixed with "@@" (exception/allow rules) to regular
    /// blocking rules. Useful when you want to block domains that are typically
    /// whitelisted.
    /// </remarks>
    InvertAllow,

    /// <summary>
    /// Removes empty lines from the output.
    /// </summary>
    /// <remarks>
    /// Removes blank lines to create a more compact output file.
    /// </remarks>
    RemoveEmptyLines,

    /// <summary>
    /// Trims leading and trailing whitespace from each line.
    /// </summary>
    /// <remarks>
    /// Removes spaces and tabs from the beginning and end of each rule line.
    /// </remarks>
    TrimLines,

    /// <summary>
    /// Ensures the output file ends with a newline character.
    /// </summary>
    /// <remarks>
    /// Many tools expect text files to end with a newline. This transformation
    /// ensures POSIX compliance by adding a final newline if one is not present.
    /// </remarks>
    InsertFinalNewLine,

    /// <summary>
    /// Converts non-ASCII characters to ASCII equivalents using punycode.
    /// </summary>
    /// <remarks>
    /// International domain names (IDN) with non-ASCII characters are converted
    /// to their ASCII-compatible encoding (ACE) format (punycode).
    /// </remarks>
    ConvertToAscii
}

/// <summary>
/// Provides helper methods for working with transformations.
/// </summary>
public static class TransformationHelper
{
    /// <summary>
    /// Gets all available transformation names as strings.
    /// </summary>
    public static readonly IReadOnlyList<string> AllTransformations =
    [
        nameof(Transformation.RemoveComments),
        nameof(Transformation.Compress),
        nameof(Transformation.RemoveModifiers),
        nameof(Transformation.Validate),
        nameof(Transformation.ValidateAllowIp),
        nameof(Transformation.Deduplicate),
        nameof(Transformation.InvertAllow),
        nameof(Transformation.RemoveEmptyLines),
        nameof(Transformation.TrimLines),
        nameof(Transformation.InsertFinalNewLine),
        nameof(Transformation.ConvertToAscii)
    ];

    /// <summary>
    /// Gets recommended transformations for a typical filter list compilation.
    /// </summary>
    /// <remarks>
    /// These transformations provide a good balance of validation, deduplication,
    /// and output formatting for most use cases.
    /// </remarks>
    public static readonly IReadOnlyList<string> RecommendedTransformations =
    [
        nameof(Transformation.Validate),
        nameof(Transformation.Deduplicate),
        nameof(Transformation.RemoveEmptyLines),
        nameof(Transformation.TrimLines),
        nameof(Transformation.InsertFinalNewLine)
    ];

    /// <summary>
    /// Gets minimal transformations for basic compilation.
    /// </summary>
    /// <remarks>
    /// Suitable when you want to preserve most of the original content
    /// while ensuring proper output formatting.
    /// </remarks>
    public static readonly IReadOnlyList<string> MinimalTransformations =
    [
        nameof(Transformation.Deduplicate),
        nameof(Transformation.InsertFinalNewLine)
    ];

    /// <summary>
    /// Gets transformations optimized for hosts file sources.
    /// </summary>
    /// <remarks>
    /// Use these transformations when compiling from /etc/hosts format sources.
    /// </remarks>
    public static readonly IReadOnlyList<string> HostsFileTransformations =
    [
        nameof(Transformation.Compress),
        nameof(Transformation.RemoveComments),
        nameof(Transformation.Deduplicate),
        nameof(Transformation.RemoveEmptyLines),
        nameof(Transformation.TrimLines),
        nameof(Transformation.InsertFinalNewLine)
    ];

    /// <summary>
    /// Validates that a transformation name is recognized.
    /// </summary>
    /// <param name="transformation">The transformation name to validate.</param>
    /// <returns>True if the transformation is valid; otherwise, false.</returns>
    public static bool IsValid(string transformation)
    {
        return AllTransformations.Contains(transformation, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates a list of transformation names.
    /// </summary>
    /// <param name="transformations">The transformation names to validate.</param>
    /// <returns>A list of invalid transformation names, or an empty list if all are valid.</returns>
    public static IReadOnlyList<string> GetInvalidTransformations(IEnumerable<string> transformations)
    {
        return transformations
            .Where(t => !IsValid(t))
            .ToList();
    }

    /// <summary>
    /// Parses a transformation name string to the enum value.
    /// </summary>
    /// <param name="transformation">The transformation name.</param>
    /// <param name="result">The parsed transformation enum value.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string transformation, out Transformation result)
    {
        return Enum.TryParse(transformation, ignoreCase: true, out result);
    }
}
