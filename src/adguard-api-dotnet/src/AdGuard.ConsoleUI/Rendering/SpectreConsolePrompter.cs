namespace AdGuard.ConsoleUI.Rendering;

/// <summary>
/// Spectre.Console implementation of <see cref="IConsolePrompter"/>.
/// </summary>
public class SpectreConsolePrompter : IConsolePrompter
{
    /// <inheritdoc/>
    public string Prompt(string prompt, string? defaultValue = null)
    {
        var textPrompt = new TextPrompt<string>(prompt);
        if (defaultValue != null)
        {
            textPrompt.DefaultValue(defaultValue);
        }
        return AnsiConsole.Prompt(textPrompt);
    }

    /// <inheritdoc/>
    public Task<string> PromptAsync(
        string prompt,
        string? defaultValue = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Prompt(prompt, defaultValue));
    }

    /// <inheritdoc/>
    public string PromptSecret(string prompt)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .Secret());
    }

    /// <inheritdoc/>
    public bool Confirm(string prompt, bool defaultValue = false)
    {
        return AnsiConsole.Confirm(prompt, defaultValue);
    }

    /// <inheritdoc/>
    public T Select<T>(string prompt, IEnumerable<T> choices) where T : notnull
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title(prompt)
                .AddChoices(choices));
    }

    /// <inheritdoc/>
    public IEnumerable<T> MultiSelect<T>(string prompt, IEnumerable<T> choices) where T : notnull
    {
        return AnsiConsole.Prompt(
            new MultiSelectionPrompt<T>()
                .Title(prompt)
                .AddChoices(choices));
    }

    /// <inheritdoc/>
    public T Select<T>(string prompt, IEnumerable<T> choices, Func<T, string> displaySelector) where T : notnull
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title(prompt)
                .UseConverter(displaySelector)
                .AddChoices(choices));
    }
}