namespace AdGuard.Repositories.Specifications;

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