namespace AdGuard.Repositories.Specifications;

/// <summary>
/// A composite specification representing a logical NOT.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _inner;

    public NotSpecification(ISpecification<T> inner)
    {
        _inner = inner;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var innerExpression = _inner.ToExpression();
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.Not(Expression.Invoke(innerExpression, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}