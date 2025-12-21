namespace AdGuard.Repositories.Common;

/// <summary>
/// Represents a failed result with an error.
/// </summary>
/// <typeparam name="T">The type of the expected success value.</typeparam>
public sealed class Failure<T> : Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Failure{T}"/> class.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    public Failure(Error error)
    {
        Error = error;
    }

    /// <summary>
    /// Gets the error that caused the failure.
    /// </summary>
    public Error Error { get; }

    /// <inheritdoc />
    public override bool IsSuccess => false;

    /// <inheritdoc />
    public override void Match(Action<T> onSuccess, Action<Error> onFailure) => onFailure(Error);

    /// <inheritdoc />
    public override TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure) => onFailure(Error);

    /// <inheritdoc />
    public override Result<TNew> Map<TNew>(Func<T, TNew> mapper) => Result<TNew>.Failure(Error);

    /// <inheritdoc />
    public override Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder) => Result<TNew>.Failure(Error);

    /// <inheritdoc />
    public override T GetValueOrDefault(T defaultValue) => defaultValue;

    /// <inheritdoc />
    public override T GetValueOrThrow() => throw new ResultException(Error);

    /// <summary>
    /// Deconstructs the failure result.
    /// </summary>
    /// <param name="error">The error.</param>
    public void Deconstruct(out Error error) => error = Error;
}