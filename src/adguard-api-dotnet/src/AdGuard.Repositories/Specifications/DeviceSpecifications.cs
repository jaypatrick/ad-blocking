using System.Linq.Expressions;
using AdGuard.ApiClient.Model;

namespace AdGuard.Repositories.Specifications;

/// <summary>
/// Specification for filtering devices by name.
/// </summary>
public sealed class DeviceByNameSpecification : Specification<Device>
{
    private readonly string _name;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceByNameSpecification"/> class.
    /// </summary>
    /// <param name="name">The name to match.</param>
    /// <param name="comparison">The comparison type.</param>
    public DeviceByNameSpecification(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _comparison = comparison;
    }

    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() =>
        device => device.Name.Contains(_name, _comparison);
}

/// <summary>
/// Specification for filtering devices by exact name match.
/// </summary>
public sealed class DeviceByExactNameSpecification : Specification<Device>
{
    private readonly string _name;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceByExactNameSpecification"/> class.
    /// </summary>
    /// <param name="name">The exact name to match.</param>
    /// <param name="comparison">The comparison type.</param>
    public DeviceByExactNameSpecification(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _comparison = comparison;
    }

    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() =>
        device => device.Name.Equals(_name, _comparison);
}

/// <summary>
/// Specification for filtering devices by type.
/// </summary>
public sealed class DeviceByTypeSpecification : Specification<Device>
{
    private readonly string _deviceType;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceByTypeSpecification"/> class.
    /// </summary>
    /// <param name="deviceType">The device type to match (e.g., WINDOWS, ANDROID, IOS).</param>
    public DeviceByTypeSpecification(string deviceType)
    {
        _deviceType = deviceType ?? throw new ArgumentNullException(nameof(deviceType));
    }

    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() =>
        device => device.DeviceType.Equals(_deviceType, StringComparison.OrdinalIgnoreCase);
}

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

/// <summary>
/// Specification for filtering devices created after a specific date.
/// </summary>
public sealed class DeviceCreatedAfterSpecification : Specification<Device>
{
    private readonly DateTimeOffset _date;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceCreatedAfterSpecification"/> class.
    /// </summary>
    /// <param name="date">The cutoff date.</param>
    public DeviceCreatedAfterSpecification(DateTimeOffset date)
    {
        _date = date;
    }

    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() =>
        device => device.CreatedAt >= _date;
}

/// <summary>
/// Specification that matches all devices (no filter).
/// </summary>
public sealed class AllDevicesSpecification : Specification<Device>
{
    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() => _ => true;
}

/// <summary>
/// Provides static factory methods for creating device specifications.
/// </summary>
public static class DeviceSpecs
{
    /// <summary>
    /// Creates a specification for all devices.
    /// </summary>
    public static ISpecification<Device> All() => new AllDevicesSpecification();

    /// <summary>
    /// Creates a specification for devices with a name containing the specified text.
    /// </summary>
    /// <param name="name">The text to search for.</param>
    /// <param name="comparison">The string comparison type.</param>
    public static ISpecification<Device> ByName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
        new DeviceByNameSpecification(name, comparison);

    /// <summary>
    /// Creates a specification for devices with the exact name.
    /// </summary>
    /// <param name="name">The exact name.</param>
    /// <param name="comparison">The string comparison type.</param>
    public static ISpecification<Device> ByExactName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) =>
        new DeviceByExactNameSpecification(name, comparison);

    /// <summary>
    /// Creates a specification for devices of a specific type.
    /// </summary>
    /// <param name="deviceType">The device type.</param>
    public static ISpecification<Device> ByType(string deviceType) =>
        new DeviceByTypeSpecification(deviceType);

    /// <summary>
    /// Creates a specification for Windows devices.
    /// </summary>
    public static ISpecification<Device> Windows() => ByType("WINDOWS");

    /// <summary>
    /// Creates a specification for Android devices.
    /// </summary>
    public static ISpecification<Device> Android() => ByType("ANDROID");

    /// <summary>
    /// Creates a specification for iOS devices.
    /// </summary>
    public static ISpecification<Device> iOS() => ByType("IOS");

    /// <summary>
    /// Creates a specification for Mac devices.
    /// </summary>
    public static ISpecification<Device> Mac() => ByType("MAC");

    /// <summary>
    /// Creates a specification for Linux devices.
    /// </summary>
    public static ISpecification<Device> Linux() => ByType("LINUX");

    /// <summary>
    /// Creates a specification for devices connected to a specific DNS server.
    /// </summary>
    /// <param name="dnsServerId">The DNS server ID.</param>
    public static ISpecification<Device> ByDnsServer(string dnsServerId) =>
        new DeviceByDnsServerSpecification(dnsServerId);

    /// <summary>
    /// Creates a specification for devices created after a specific date.
    /// </summary>
    /// <param name="date">The cutoff date.</param>
    public static ISpecification<Device> CreatedAfter(DateTimeOffset date) =>
        new DeviceCreatedAfterSpecification(date);
}
