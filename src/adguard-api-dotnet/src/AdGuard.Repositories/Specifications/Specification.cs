namespace AdGuard.Repositories.Specifications;

/// <summary>
/// Base implementation of the Specification pattern.
/// </summary>
/// <typeparam name="T">The entity type to query.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    private Func<T, bool>? _compiledExpression;

    /// <inheritdoc />
    public bool IsSatisfiedBy(T entity)
    {
        _compiledExpression ??= ToExpression().Compile();
        return _compiledExpression(entity);
    }

    /// <inheritdoc />
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <inheritdoc />
    public ISpecification<T> And(ISpecification<T> other) =>
        new AndSpecification<T>(this, other);

    /// <inheritdoc />
    public ISpecification<T> Or(ISpecification<T> other) =>
        new OrSpecification<T>(this, other);

    /// <inheritdoc />
    public ISpecification<T> Not() =>
        new NotSpecification<T>(this);

    /// <summary>
    /// Implicitly converts a specification to a predicate function.
    /// </summary>
    /// <param name="spec">The specification to convert.</param>
    public static implicit operator Func<T, bool>(Specification<T> spec) =>
        spec.IsSatisfiedBy;

    /// <summary>
    /// Implicitly converts a specification to a LINQ expression.
    /// </summary>
    /// <param name="spec">The specification to convert.</param>
    public static implicit operator Expression<Func<T, bool>>(Specification<T> spec) =>
        spec.ToExpression();
}