namespace AdGuard.DataAccess;

/// <summary>
/// Converts DateTime values to UTC for storage.
/// </summary>
internal class DateTimeUtcConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeUtcConverter"/> class.
    /// </summary>
    public DateTimeUtcConverter()
        : base(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}