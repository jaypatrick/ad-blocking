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