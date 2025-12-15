namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for user rules operations on DNS servers.
/// </summary>
public interface IUserRulesRepository
{
    /// <summary>
    /// Gets the user rules for a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user rules settings.</returns>
    Task<UserRulesSettings> GetAsync(string dnsServerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user rules for a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="userRules">The user rules to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated DNS server.</returns>
    Task<DNSServer> UpdateAsync(string dnsServerId, UserRulesSettingsUpdate userRules, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user rules from a file for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="filePath">The path to the rules file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of rules loaded from the file.</returns>
    Task<int> UpdateFromFileAsync(string dnsServerId, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables user rules for a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="enabled">Whether to enable rules.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetEnabledAsync(string dnsServerId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a single rule to a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="rule">The rule to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRuleAsync(string dnsServerId, string rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends rules to the existing user rules for a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="rulesToAdd">The rules to append.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated DNS server.</returns>
    Task<DNSServer> AppendRulesAsync(string dnsServerId, IEnumerable<string> rulesToAdd, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all user rules from a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ClearRulesAsync(string dnsServerId, CancellationToken cancellationToken = default);
}
