namespace AdGuard.Repositories.Specifications;

/// <summary>
/// A specification created from an expression.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class ExpressionSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionSpecification{T}"/> class.
    /// </summary>
    /// <param name="expression">The expression to wrap.</param>
    public ExpressionSpecification(Expression<Func<T, bool>> expression)
    {
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression() => _expression;
}