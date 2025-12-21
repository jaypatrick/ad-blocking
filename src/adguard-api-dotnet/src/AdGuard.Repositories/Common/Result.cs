namespace AdGuard.Repositories.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <remarks>
/// This is a discriminated union type that provides type-safe error handling
/// without relying on exceptions for control flow.
/// </remarks>
public abstract class Result
{
    /// <summary>
    /// Gets a value indicating whether the result represents success.
    /// </summary>
    public abstract bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result instance.</returns>
    public static Result Success() => new SuccessResult();

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result instance.</returns>
    public static Result Failure(Error error) => new FailureResult(error);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result instance.</returns>
    public static Result Failure(string code, string message) => new FailureResult(new Error(code, message));

    private sealed class SuccessResult : Result
    {
        public override bool IsSuccess => true;
    }

    private sealed class FailureResult : Result
    {
        public FailureResult(Error error)
        {
            Error = error;
        }

        public override bool IsSuccess => false;

        public Error Error { get; }
    }
}

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <remarks>
/// This is a discriminated union type that provides type-safe error handling
/// without relying on exceptions for control flow. Use pattern matching to handle results.
/// </remarks>
/// <example>
/// <code>
/// var result = await repository.GetByIdAsync(id);
///
/// // Using pattern matching (C# 9+)
/// var message = result switch
/// {
///     Success&lt;Device&gt; success => $"Found device: {success.Value.Name}",
///     Failure&lt;Device&gt; failure => $"Error: {failure.Error.Message}",
///     _ => "Unknown result"
/// };
///
/// // Using Match method
/// result.Match(
///     onSuccess: device => Console.WriteLine($"Found: {device.Name}"),
///     onFailure: error => Console.WriteLine($"Error: {error.Message}")
/// );
/// </code>
/// </example>
public abstract class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the result represents success.
    /// </summary>
    public abstract bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result instance.</returns>
    public static Result<T> Success(T value) => new Success<T>(value);

    /// <summary>
    /// Creates a failed result with an error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result instance.</returns>
    public static Result<T> Failure(Error error) => new Failure<T>(error);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failed result instance.</returns>
    public static Result<T> Failure(string code, string message) => new Failure<T>(new Error(code, message));

    /// <summary>
    /// Matches the result to execute the appropriate action.
    /// </summary>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    public abstract void Match(Action<T> onSuccess, Action<Error> onFailure);

    /// <summary>
    /// Matches the result to return a value.
    /// </summary>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="onSuccess">Function to execute on success.</param>
    /// <param name="onFailure">Function to execute on failure.</param>
    /// <returns>The result of the matched function.</returns>
    public abstract TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure);

    /// <summary>
    /// Maps the success value to a new type.
    /// </summary>
    /// <typeparam name="TNew">The new value type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    /// <returns>A new result with the mapped value, or the original failure.</returns>
    public abstract Result<TNew> Map<TNew>(Func<T, TNew> mapper);

    /// <summary>
    /// Binds (flat maps) the success value to a new result.
    /// </summary>
    /// <typeparam name="TNew">The new value type.</typeparam>
    /// <param name="binder">The binding function.</param>
    /// <returns>A new result, or the original failure.</returns>
    public abstract Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder);

    /// <summary>
    /// Gets the value if successful, or the default value if failed.
    /// </summary>
    /// <param name="defaultValue">The default value to return on failure.</param>
    /// <returns>The success value or the default.</returns>
    public abstract T GetValueOrDefault(T defaultValue);

    /// <summary>
    /// Gets the value if successful, or throws the error as an exception.
    /// </summary>
    /// <returns>The success value.</returns>
    /// <exception cref="ResultException">Thrown when the result is a failure.</exception>
    public abstract T GetValueOrThrow();

    /// <summary>
    /// Implicit conversion from a value to a successful result.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicit conversion from an error to a failed result.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    public static implicit operator Result<T>(Error error) => Failure(error);
}