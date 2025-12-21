namespace AdGuard.Repositories.Specifications;

/// <summary>
/// Specification that matches all devices (no filter).
/// </summary>
public sealed class AllDevicesSpecification : Specification<Device>
{
    /// <inheritdoc />
    public override Expression<Func<Device, bool>> ToExpression() => _ => true;
}