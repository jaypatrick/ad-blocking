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
    Task<UserRulesSettings> GetByDnsServerIdAsync(string dnsServerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user rules for a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="userRules">The user rules to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated DNS server.</returns>
    Task<DNSServer> UpdateAsync(string dnsServerId, UserRulesSettingsUpdate userRules, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends rules to the existing user rules for a DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server identifier.</param>
    /// <param name="rulesToAdd">The rules to append.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated DNS server.</returns>
    Task<DNSServer> AppendRulesAsync(string dnsServerId, IEnumerable<string> rulesToAdd, CancellationToken cancellationToken = default);
}
