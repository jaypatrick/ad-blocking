using AdGuard.Repositories.Abstractions;

namespace AdGuard.Repositories.Contracts;

/// <summary>
/// Repository interface for DNS server operations.
/// </summary>
public interface IDnsServerRepository : IRepository<DNSServer, string, DNSServerCreate, DNSServerSettingsUpdate>
{
}
