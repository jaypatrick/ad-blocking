namespace AdGuard.ApiClient.Client;

/// <summary>
/// Formatter for nullable 'date' openapi formats as defined by full-date - RFC3339
/// </summary>
public class OpenAPIDateConverterNullable : JsonConverter<DateTime?>
{
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Reads and converts the JSON to a nullable DateTime.
    /// </summary>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
        {
            return null;
        }

        if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        return DateTime.Parse(dateString, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Writes the nullable DateTime to JSON.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}