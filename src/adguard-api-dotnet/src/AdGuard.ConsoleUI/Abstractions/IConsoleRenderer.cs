namespace AdGuard.ConsoleUI.Abstractions;

/// <summary>
/// Interface for console rendering operations.
/// Enables decoupling from specific console rendering libraries.
/// </summary>
public interface IConsoleRenderer
{
    /// <summary>
    /// Writes a line of text to the console.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void WriteLine(string text);

    /// <summary>
    /// Writes text to the console without a newline.
    /// </summary>
    /// <param name="text">The text to write.</param>
    void Write(string text);

    /// <summary>
    /// Writes a blank line to the console.
    /// </summary>
    void WriteLine();

    /// <summary>
    /// Writes a styled text line to the console.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="style">The style to apply.</param>
    void WriteStyled(string text, TextStyle style);

    /// <summary>
    /// Writes a markup string (with embedded formatting).
    /// </summary>
    /// <param name="markup">The markup string.</param>
    void WriteMarkup(string markup);

    /// <summary>
    /// Writes a markup line (with embedded formatting).
    /// </summary>
    /// <param name="markup">The markup string.</param>
    void WriteMarkupLine(string markup);

    /// <summary>
    /// Renders a table to the console.
    /// </summary>
    /// <param name="table">The table to render.</param>
    void RenderTable(ConsoleTable table);

    /// <summary>
    /// Renders a panel (boxed content) to the console.
    /// </summary>
    /// <param name="content">The panel content.</param>
    /// <param name="title">Optional panel title.</param>
    void RenderPanel(string content, string? title = null);

    /// <summary>
    /// Renders a rule (horizontal line) to the console.
    /// </summary>
    /// <param name="title">Optional title for the rule.</param>
    void RenderRule(string? title = null);

    /// <summary>
    /// Clears the console.
    /// </summary>
    void Clear();

    /// <summary>
    /// Displays a status spinner while an operation is running.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="status">The status message.</param>
    /// <param name="operation">The operation to execute.</param>
    /// <returns>The operation result.</returns>
    Task<T> StatusAsync<T>(string status, Func<Task<T>> operation);

    /// <summary>
    /// Displays a status spinner while an operation is running.
    /// </summary>
    /// <param name="status">The status message.</param>
    /// <param name="operation">The operation to execute.</param>
    Task StatusAsync(string status, Func<Task> operation);

    /// <summary>
    /// Displays a progress bar for an operation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="description">The progress description.</param>
    /// <param name="operation">The operation with progress reporting.</param>
    /// <returns>The operation result.</returns>
    Task<T> ProgressAsync<T>(string description, Func<IProgress<double>, Task<T>> operation);
}

/// <summary>
/// Interface for console prompts.
/// </summary>
public interface IConsolePrompter
{
    /// <summary>
    /// Prompts the user for text input.
    /// </summary>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="defaultValue">Optional default value.</param>
    /// <returns>The user's input.</returns>
    string Prompt(string prompt, string? defaultValue = null);

    /// <summary>
    /// Prompts the user for text input asynchronously.
    /// </summary>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="defaultValue">Optional default value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's input.</returns>
    Task<string> PromptAsync(
        string prompt,
        string? defaultValue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Prompts the user for secret input (e.g., password).
    /// </summary>
    /// <param name="prompt">The prompt message.</param>
    /// <returns>The user's secret input.</returns>
    string PromptSecret(string prompt);

    /// <summary>
    /// Prompts the user for confirmation (yes/no).
    /// </summary>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="defaultValue">Optional default value.</param>
    /// <returns>True if confirmed; otherwise, false.</returns>
    bool Confirm(string prompt, bool defaultValue = false);

    /// <summary>
    /// Prompts the user to select from a list of choices.
    /// </summary>
    /// <typeparam name="T">The choice type.</typeparam>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choice.</returns>
    T Select<T>(string prompt, IEnumerable<T> choices) where T : notnull;

    /// <summary>
    /// Prompts the user to select multiple items from a list.
    /// </summary>
    /// <typeparam name="T">The choice type.</typeparam>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choices.</returns>
    IEnumerable<T> MultiSelect<T>(string prompt, IEnumerable<T> choices) where T : notnull;

    /// <summary>
    /// Prompts the user to select from a list with custom display.
    /// </summary>
    /// <typeparam name="T">The choice type.</typeparam>
    /// <param name="prompt">The prompt message.</param>
    /// <param name="choices">The available choices.</param>
    /// <param name="displaySelector">Function to convert choices to display strings.</param>
    /// <returns>The selected choice.</returns>
    T Select<T>(string prompt, IEnumerable<T> choices, Func<T, string> displaySelector) where T : notnull;
}

/// <summary>
/// Represents a console table for rendering.
/// </summary>
public class ConsoleTable
{
    /// <summary>
    /// Gets or sets the table title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the table columns.
    /// </summary>
    public IList<ConsoleTableColumn> Columns { get; } = [];

    /// <summary>
    /// Gets the table rows.
    /// </summary>
    public IList<ConsoleTableRow> Rows { get; } = [];

    /// <summary>
    /// Gets or sets whether to show borders.
    /// </summary>
    public bool ShowBorders { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to expand the table to full width.
    /// </summary>
    public bool Expand { get; set; }

    /// <summary>
    /// Adds a column to the table.
    /// </summary>
    /// <param name="header">The column header.</param>
    /// <param name="alignment">The column alignment.</param>
    /// <returns>The table for chaining.</returns>
    public ConsoleTable AddColumn(string header, TextAlignment alignment = TextAlignment.Left)
    {
        Columns.Add(new ConsoleTableColumn { Header = header, Alignment = alignment });
        return this;
    }

    /// <summary>
    /// Adds a row to the table.
    /// </summary>
    /// <param name="values">The row values.</param>
    /// <returns>The table for chaining.</returns>
    public ConsoleTable AddRow(params object?[] values)
    {
        Rows.Add(new ConsoleTableRow { Values = values.Select(v => v?.ToString() ?? string.Empty).ToList() });
        return this;
    }
}

/// <summary>
/// Represents a table column.
/// </summary>
public class ConsoleTableColumn
{
    /// <summary>
    /// Gets or sets the column header.
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the column alignment.
    /// </summary>
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
}

/// <summary>
/// Represents a table row.
/// </summary>
public class ConsoleTableRow
{
    /// <summary>
    /// Gets or sets the row values.
    /// </summary>
    public IList<string> Values { get; set; } = [];
}

/// <summary>
/// Text alignment options.
/// </summary>
public enum TextAlignment
{
    /// <summary>
    /// Left-aligned text.
    /// </summary>
    Left,

    /// <summary>
    /// Center-aligned text.
    /// </summary>
    Center,

    /// <summary>
    /// Right-aligned text.
    /// </summary>
    Right
}

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

/// <summary>
/// Console color enumeration.
/// </summary>
public enum ConsoleColor
{
    Black,
    DarkBlue,
    DarkGreen,
    DarkCyan,
    DarkRed,
    DarkMagenta,
    DarkYellow,
    Gray,
    DarkGray,
    Blue,
    Green,
    Cyan,
    Red,
    Magenta,
    Yellow,
    White
}
