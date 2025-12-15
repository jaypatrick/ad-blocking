using System.Linq.Expressions;

namespace AdGuard.Repositories.Specifications;

/// <summary>
/// Defines a specification that encapsulates query logic for filtering entities.
/// </summary>
/// <typeparam name="T">The entity type to query.</typeparam>
/// <remarks>
/// The Specification pattern encapsulates domain logic as a boolean condition,
/// allowing for reusable, composable, and testable query criteria.
/// </remarks>
/// <example>
/// <code>
/// // Define a specification
/// public class ActiveDeviceSpecification : Specification&lt;Device&gt;
/// {
///     public override Expression&lt;Func&lt;Device, bool&gt;&gt; ToExpression() =>
///         device => device.Status == "active";
/// }
///
/// // Use the specification
/// var spec = new ActiveDeviceSpecification();
/// var activeDevices = devices.Where(spec.IsSatisfiedBy).ToList();
/// </code>
/// </example>
public interface ISpecification<T>
{
    /// <summary>
    /// Determines whether the specified entity satisfies this specification.
    /// </summary>
    /// <param name="entity">The entity to test.</param>
    /// <returns>True if the entity satisfies the specification; otherwise, false.</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// Converts the specification to a LINQ expression.
    /// </summary>
    /// <returns>An expression that can be used in LINQ queries.</returns>
    Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Creates a new specification that is the logical AND of this and another specification.
    /// </summary>
    /// <param name="other">The other specification.</param>
    /// <returns>A composite specification.</returns>
    ISpecification<T> And(ISpecification<T> other);

    /// <summary>
    /// Creates a new specification that is the logical OR of this and another specification.
    /// </summary>
    /// <param name="other">The other specification.</param>
    /// <returns>A composite specification.</returns>
    ISpecification<T> Or(ISpecification<T> other);

    /// <summary>
    /// Creates a new specification that is the logical NOT of this specification.
    /// </summary>
    /// <returns>A negated specification.</returns>
    ISpecification<T> Not();
}

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

/// <summary>
/// A specification that always returns true (matches all entities).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class TrueSpecification<T> : Specification<T>
{
    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression() => _ => true;
}

/// <summary>
/// A specification that always returns false (matches no entities).
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public sealed class FalseSpecification<T> : Specification<T>
{
    /// <inheritdoc />
    public override Expression<Func<T, bool>> ToExpression() => _ => false;
}

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

/// <summary>
/// A composite specification representing a logical AND.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}

/// <summary>
/// A composite specification representing a logical OR.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.OrElse(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}

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
