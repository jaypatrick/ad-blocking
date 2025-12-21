namespace AdGuard.Repositories.Common;

/// <summary>
/// Represents a successful result with a value.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Success<T> : Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Success{T}"/> class.
    /// </summary>
    /// <param name="value">The success value.</param>
    public Success(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the success value.
    /// </summary>
    public T Value { get; }

    /// <inheritdoc />
    public override bool IsSuccess => true;

    /// <inheritdoc />
    public override void Match(Action<T> onSuccess, Action<Error> onFailure) => onSuccess(Value);

    /// <inheritdoc />
    public override TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure) => onSuccess(Value);

    /// <inheritdoc />
    public override Result<TNew> Map<TNew>(Func<T, TNew> mapper) => Result<TNew>.Success(mapper(Value));

    /// <inheritdoc />
    public override Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder) => binder(Value);

    /// <inheritdoc />
    public override T GetValueOrDefault(T defaultValue) => Value;

    /// <inheritdoc />
    public override T GetValueOrThrow() => Value;

    /// <summary>
    /// Deconstructs the success result.
    /// </summary>
    /// <param name="value">The success value.</param>
    public void Deconstruct(out T value) => value = Value;
}