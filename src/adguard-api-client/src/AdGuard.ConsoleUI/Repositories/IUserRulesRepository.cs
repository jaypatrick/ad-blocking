namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository interface for user rules operations on DNS servers.
/// Abstracts data access from the UI layer.
/// </summary>
public interface IUserRulesRepository
{
    /// <summary>
    /// Gets the user rules settings for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <returns>The user rules settings.</returns>
    Task<UserRulesSettings> GetAsync(string dnsServerId);

    /// <summary>
    /// Updates the user rules settings for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="update">The settings update.</param>
    Task UpdateAsync(string dnsServerId, UserRulesSettingsUpdate update);

    /// <summary>
    /// Updates the user rules from a file for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="filePath">The path to the rules file.</param>
    /// <returns>The number of rules loaded from the file.</returns>
    Task<int> UpdateFromFileAsync(string dnsServerId, string filePath);

    /// <summary>
    /// Enables or disables user rules for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="enabled">Whether to enable rules.</param>
    Task SetEnabledAsync(string dnsServerId, bool enabled);

    /// <summary>
    /// Adds a single rule to a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    /// <param name="rule">The rule to add.</param>
    Task AddRuleAsync(string dnsServerId, string rule);

    /// <summary>
    /// Clears all user rules from a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    Task ClearRulesAsync(string dnsServerId);
}
