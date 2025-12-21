namespace AdGuard.Repositories.Common;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Message">The error message.</param>
/// <param name="InnerException">Optional inner exception.</param>
public record Error(string Code, string Message, Exception? InnerException = null)
{
    /// <summary>
    /// Common error codes used throughout the application.
    /// </summary>
    public static class Codes
    {
        /// <summary>Entity not found error code.</summary>
        public const string NotFound = "NOT_FOUND";

        /// <summary>Validation error code.</summary>
        public const string ValidationError = "VALIDATION_ERROR";

        /// <summary>API error code.</summary>
        public const string ApiError = "API_ERROR";

        /// <summary>Network error code.</summary>
        public const string NetworkError = "NETWORK_ERROR";

        /// <summary>Authentication error code.</summary>
        public const string Unauthorized = "UNAUTHORIZED";

        /// <summary>General operation failure code.</summary>
        public const string OperationFailed = "OPERATION_FAILED";

        /// <summary>Configuration error code.</summary>
        public const string ConfigurationError = "CONFIGURATION_ERROR";
    }

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="id">The entity identifier.</param>
    /// <returns>A not found error.</returns>
    public static Error NotFound(string entityType, object id) =>
        new(Codes.NotFound, $"{entityType} with id '{id}' was not found.");

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="message">The validation message.</param>
    /// <returns>A validation error.</returns>
    public static Error Validation(string message) =>
        new(Codes.ValidationError, message);

    /// <summary>
    /// Creates an API error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">The inner exception.</param>
    /// <returns>An API error.</returns>
    public static Error Api(string message, Exception? exception = null) =>
        new(Codes.ApiError, message, exception);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An unauthorized error.</returns>
    public static Error Unauthorized(string message = "Authentication required.") =>
        new(Codes.Unauthorized, message);
}