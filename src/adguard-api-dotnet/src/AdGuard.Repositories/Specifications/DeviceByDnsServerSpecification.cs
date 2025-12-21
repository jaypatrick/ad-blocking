namespace AdGuard.Repositories.Specifications;

/// <summary>
/// Specification for filtering devices by DNS server.
/// </summary>
public sealed class DeviceByDnsServerSpecification : Specification<Device>
{
    private readonly string _dnsServerId;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceByDnsServerSpecification"/> class.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID to match.</param>
    public DeviceByDnsServerSpecification(string dnsServerId)
    {
        _dnsServerId = dnsServerId ?? throw new ArgumentNullException(nameof(dnsServerId));
    }

    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() =>
        device => device.DnsServerId == _dnsServerId;
}