namespace AdGuard.ApiClient.Client;

/// <summary>
/// Provides a non-generic contract for the ApiResponse wrapper.
/// </summary>
public interface IApiResponse
{
    /// <summary>
    /// The data type of <see cref="Content"/>
    /// </summary>
    Type ResponseType { get; }

    /// <summary>
    /// The content of this response
    /// </summary>
    Object Content { get; }

    /// <summary>
    /// Gets or sets the status code (HTTP status code)
    /// </summary>
    /// <value>The status code.</value>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets or sets the HTTP headers
    /// </summary>
    /// <value>HTTP headers</value>
    Multimap<string, string> Headers { get; }

    /// <summary>
    /// Gets or sets any error text defined by the calling client.
    /// </summary>
    string ErrorText { get; set; }

    /// <summary>
    /// Gets or sets any cookies passed along on the response.
    /// </summary>
    List<Cookie> Cookies { get; set; }

    /// <summary>
    /// The raw content of this response
    /// </summary>
    string RawContent { get; }
}