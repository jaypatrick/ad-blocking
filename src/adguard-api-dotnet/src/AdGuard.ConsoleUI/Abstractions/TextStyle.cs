namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Text style options.
/// </summary>
public class TextStyle
{
    /// <summary>
    /// Gets or sets the foreground color.
    /// </summary>
    public ConsoleColor? ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public ConsoleColor? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets whether the text is bold.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Gets or sets whether the text is italic.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Gets or sets whether the text is underlined.
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Creates a default style.
    /// </summary>
    public static TextStyle Default => new();

    /// <summary>
    /// Creates an error style (red text).
    /// </summary>
    public static TextStyle Error => new() { ForegroundColor = ConsoleColor.Red, Bold = true };

    /// <summary>
    /// Creates a warning style (yellow text).
    /// </summary>
    public static TextStyle Warning => new() { ForegroundColor = ConsoleColor.Yellow };

    /// <summary>
    /// Creates a success style (green text).
    /// </summary>
    public static TextStyle Success => new() { ForegroundColor = ConsoleColor.Green };

    /// <summary>
    /// Creates an info style (blue text).
    /// </summary>
    public static TextStyle Info => new() { ForegroundColor = ConsoleColor.Cyan };
}