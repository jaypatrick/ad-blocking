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