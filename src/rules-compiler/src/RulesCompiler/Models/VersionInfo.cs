namespace RulesCompiler.Models;

/// <summary>
/// Contains version information for all compiler components.
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Gets or sets the module version.
    /// </summary>
    public string ModuleVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the PowerShell version (if running in PowerShell context).
    /// </summary>
    public string? PowerShellVersion { get; set; }

    /// <summary>
    /// Gets or sets the .NET runtime version.
    /// </summary>
    public string DotNetVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Node.js version.
    /// </summary>
    public string? NodeVersion { get; set; }

    /// <summary>
    /// Gets or sets the hostlist-compiler version.
    /// </summary>
    public string? HostlistCompilerVersion { get; set; }

    /// <summary>
    /// Gets or sets the path to the hostlist-compiler executable.
    /// </summary>
    public string? HostlistCompilerPath { get; set; }

    /// <summary>
    /// Gets or sets the platform information.
    /// </summary>
    public PlatformInfo Platform { get; set; } = new();
}

/// <summary>
/// Contains platform-specific information.
/// </summary>
public class PlatformInfo
{
    /// <summary>
    /// Gets or sets the operating system name.
    /// </summary>
    public string OSName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operating system version.
    /// </summary>
    public string OSVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the processor architecture.
    /// </summary>
    public string Architecture { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the platform is Windows.
    /// </summary>
    public bool IsWindows { get; set; }

    /// <summary>
    /// Gets or sets whether the platform is Linux.
    /// </summary>
    public bool IsLinux { get; set; }

    /// <summary>
    /// Gets or sets whether the platform is macOS.
    /// </summary>
    public bool IsMacOS { get; set; }
}
