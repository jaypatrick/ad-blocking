namespace AdGuard.Repositories.Specifications;

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

    /*
    /// <summary>
    /// Creates a specification for devices created after a specific date.
    /// Note: Commented out until Device model includes CreatedAt property.
    /// </summary>
    /// <param name="date">The cutoff date.</param>
    public static ISpecification<Device> CreatedAfter(DateTimeOffset date) =>
        new DeviceCreatedAfterSpecification(date);
    */
}