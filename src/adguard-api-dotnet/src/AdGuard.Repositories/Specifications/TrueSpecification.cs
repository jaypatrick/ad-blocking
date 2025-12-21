namespace AdGuard.Repositories.Specifications;

/// <summary>
/// A specification that always returns true (matches all entities).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class TrueSpecification<T> : Specification<T>
{
    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression() => _ => true;
}