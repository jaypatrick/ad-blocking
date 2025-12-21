namespace AdGuard.ConsoleUI.Abstractions;

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