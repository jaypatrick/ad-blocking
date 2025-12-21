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

/*
/// <summary>
/// Specification for filtering devices created after a specific date.
/// Note: The Device model in API v1.11 does not include a CreatedAt property.
/// This specification is commented out until the API provides timestamp information.
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
*/