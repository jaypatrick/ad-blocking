namespace AdGuard.Repositories.Specifications;

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
        device => string.Equals(device.DeviceType.ToString(), _deviceType, StringComparison.OrdinalIgnoreCase);
}