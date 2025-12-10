using AdGuard.ApiClient.Model;
using AdGuard.ConsoleUI.Abstractions;

namespace AdGuard.ConsoleUI.Repositories;

/// <summary>
/// Repository implementation for DNS server operations.
/// </summary>
public class DnsServerRepository : IDnsServerRepository
{
    private readonly IApiClientFactory _apiClientFactory;

    public DnsServerRepository(IApiClientFactory apiClientFactory)
    {
        _apiClientFactory = apiClientFactory ?? throw new ArgumentNullException(nameof(apiClientFactory));
    }

    /// <inheritdoc />
    public async Task<List<DNSServer>> GetAllAsync()
    {
        using var api = _apiClientFactory.CreateDnsServersApi();
        return await api.ListDNSServersAsync();
    }

    /// <inheritdoc />
    public async Task<DNSServer> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("DNS Server ID cannot be null or empty.", nameof(id));

        using var api = _apiClientFactory.CreateDnsServersApi();
        var servers = await api.ListDNSServersAsync();
        return servers.FirstOrDefault(s => s.Id == id)
            ?? throw new KeyNotFoundException($"DNS Server with ID '{id}' not found.");
    }

    /// <inheritdoc />
    public async Task<DNSServer> CreateAsync(DNSServerCreate serverCreate)
    {
        ArgumentNullException.ThrowIfNull(serverCreate);

        using var api = _apiClientFactory.CreateDnsServersApi();
        return await api.CreateDNSServerAsync(serverCreate);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("DNS Server ID cannot be null or empty.", nameof(id));

        using var api = _apiClientFactory.CreateDnsServersApi();
        await api.RemoveDNSServerAsync(id);
    }
}
