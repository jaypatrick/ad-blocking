namespace AdGuard.ApiClient.Client;

/// <summary>
/// To Serialize/Deserialize JSON using our custom logic, but only when ContentType is JSON.
/// </summary>
internal class CustomJsonCodec
{
    private readonly IReadableConfiguration _configuration;
    private static readonly string _contentType = "application/json";
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public CustomJsonCodec(IReadableConfiguration configuration)
    {
        _configuration = configuration;
    }

    public CustomJsonCodec(JsonSerializerOptions serializerOptions, IReadableConfiguration configuration)
    {
        _serializerOptions = serializerOptions;
        _configuration = configuration;
    }

    /// <summary>
    /// Serialize the object into a JSON string.
    /// </summary>
    /// <param name="obj">Object to be serialized.</param>
    /// <returns>A JSON string.</returns>
    public string Serialize(object obj)
    {
        if (obj != null && obj is AdGuard.ApiClient.Model.AbstractOpenAPISchema)
        {
            // the object to be serialized is an oneOf/anyOf schema
            return ((AdGuard.ApiClient.Model.AbstractOpenAPISchema)obj).ToJson();
        }
        else
        {
            return JsonSerializer.Serialize(obj, _serializerOptions);
        }
    }

    public async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        var result = (T)await Deserialize(response, typeof(T)).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Deserialize the JSON string into a proper object.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="type">Object type.</param>
    /// <returns>Object representation of the JSON string.</returns>
    internal async Task<object> Deserialize(HttpResponseMessage response, Type type)
    {
        IList<string> headers = new List<string>();
        // process response headers, e.g. Access-Control-Allow-Methods
        foreach (var responseHeader in response.Headers)
        {
            headers.Add(responseHeader.Key + "=" + ClientUtils.ParameterToString(responseHeader.Value));
        }

        // process response content headers, e.g. Content-Type
        foreach (var responseHeader in response.Content.Headers)
        {
            headers.Add(responseHeader.Key + "=" + ClientUtils.ParameterToString(responseHeader.Value));
        }

        // RFC 2183 & RFC 2616
        var fileNameRegex = new Regex(@"Content-Disposition=.*filename=['""]?([^'""\s]+)['""]?$", RegexOptions.IgnoreCase);
        if (type == typeof(byte[])) // return byte array
        {
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        else if (type == typeof(FileParameter))
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var match = fileNameRegex.Match(header.ToString());
                    if (match.Success)
                    {
                        string fileName = ClientUtils.SanitizeFilename(match.Groups[1].Value.Replace("\"", "").Replace("'", ""));
                        return new FileParameter(fileName, await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    }
                }
            }
            return new FileParameter(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
        }

        // TODO: ? if (type.IsAssignableFrom(typeof(Stream)))
        if (type == typeof(Stream))
        {
            var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            if (headers != null)
            {
                var filePath = string.IsNullOrEmpty(_configuration.TempFolderPath)
                    ? Path.GetTempPath()
                    : _configuration.TempFolderPath;

                foreach (var header in headers)
                {
                    var match = fileNameRegex.Match(header.ToString());
                    if (match.Success)
                    {
                        string fileName = filePath + ClientUtils.SanitizeFilename(match.Groups[1].Value.Replace("\"", "").Replace("'", ""));
                        File.WriteAllBytes(fileName, bytes);
                        return new FileStream(fileName, FileMode.Open);
                    }
                }
            }
            var stream = new MemoryStream(bytes);
            return stream;
        }

        if (type.Name.StartsWith("System.Nullable`1[[System.DateTime")) // return a datetime object
        {
            return DateTime.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false), null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        if (type == typeof(string) || type.Name.StartsWith("System.Nullable")) // return primitive type
        {
            return Convert.ChangeType(await response.Content.ReadAsStringAsync().ConfigureAwait(false), type);
        }

        // at this point, it must be a model (json)
        try
        {
            return JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync().ConfigureAwait(false), type, _serializerOptions);
        }
        catch (Exception e)
        {
            throw new ApiException(500, e.Message);
        }
    }

    public string RootElement { get; set; }
    public string Namespace { get; set; }
    public string DateFormat { get; set; }

    public string ContentType
    {
        get { return _contentType; }
        set { throw new InvalidOperationException("Not allowed to set content type."); }
    }
}