namespace AdGuard.Repositories.Specifications;

/// <summary>
/// A specification that always returns false (matches no entities).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class FalseSpecification<T> : Specification<T>
{
    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression() => _ => false;
}