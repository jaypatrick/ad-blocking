using AdGuard.ApiClient.Client;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Helpers;

/// <summary>
/// Helper class for common console UI operations.
/// Eliminates code duplication across menu services.
/// </summary>
public static class ConsoleHelpers
{
    /// <summary>
    /// Executes an async operation with a status spinner.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="statusMessage">The message to display during the operation.</param>
    /// <param name="operation">The async operation to execute.</param>
    /// <returns>The result of the operation.</returns>
    public static async Task<T> WithStatusAsync<T>(string statusMessage, Func<Task<T>> operation)
    {
        return await AnsiConsole.Status()
            .StartAsync(statusMessage, async ctx => await operation());
    }

    /// <summary>
    /// Executes an async operation with a status spinner (void return).
    /// </summary>
    /// <param name="statusMessage">The message to display during the operation.</param>
    /// <param name="operation">The async operation to execute.</param>
    public static async Task WithStatusAsync(string statusMessage, Func<Task> operation)
    {
        await AnsiConsole.Status()
            .StartAsync(statusMessage, async ctx => await operation());
    }

    /// <summary>
    /// Displays a selection prompt and returns the selected item.
    /// </summary>
    /// <typeparam name="T">The type of items to select from.</typeparam>
    /// <param name="title">The prompt title.</param>
    /// <param name="items">The items to select from.</param>
    /// <param name="displaySelector">Function to convert item to display string.</param>
    /// <returns>The selected item, or default if no items available.</returns>
    public static T? SelectItem<T>(string title, IReadOnlyList<T> items, Func<T, string> displaySelector)
        where T : class
    {
        if (items.Count == 0)
        {
            return null;
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .AddChoices(items.Select(displaySelector)));

        return items.FirstOrDefault(item => displaySelector(item) == choice);
    }

    /// <summary>
    /// Displays a selection prompt with custom choices.
    /// </summary>
    /// <param name="title">The prompt title.</param>
    /// <param name="choices">The available choices.</param>
    /// <returns>The selected choice.</returns>
    public static string SelectChoice(string title, params string[] choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .HighlightStyle(new Style(Color.Green))
                .AddChoices(choices));
    }

    /// <summary>
    /// Displays a message indicating no items were found.
    /// </summary>
    /// <param name="itemType">The type of items (e.g., "devices", "servers").</param>
    public static void ShowNoItemsMessage(string itemType)
    {
        AnsiConsole.MarkupLine($"[yellow]No {itemType} found.[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a success message.
    /// </summary>
    /// <param name="message">The success message.</param>
    public static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a success message with an ID.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <param name="id">The related ID.</param>
    public static void ShowSuccessWithId(string message, string id)
    {
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
        AnsiConsole.MarkupLine($"[grey]ID: {id}[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays an API error message.
    /// </summary>
    /// <param name="ex">The API exception.</param>
    public static void ShowApiError(ApiException ex)
    {
        AnsiConsole.MarkupLine($"[red]API Error ({ex.ErrorCode}): {Markup.Escape(ex.Message)}[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a generic error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(message)}[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays an informational message.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void ShowInfo(string message)
    {
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(message)}[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Displays a warning message.
    /// </summary>
    /// <param name="message">The warning message.</param>
    public static void ShowWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(message)}[/]");
    }

    /// <summary>
    /// Asks for confirmation before a destructive action.
    /// </summary>
    /// <param name="message">The confirmation message.</param>
    /// <returns>True if confirmed; otherwise, false.</returns>
    public static bool ConfirmAction(string message)
    {
        return AnsiConsole.Confirm(message, false);
    }

    /// <summary>
    /// Shows a cancellation message.
    /// </summary>
    public static void ShowCancelled()
    {
        AnsiConsole.MarkupLine("[grey]Operation cancelled.[/]");
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Creates a progress bar string representation.
    /// </summary>
    /// <param name="percentage">The percentage (0-100).</param>
    /// <param name="width">The width of the bar in characters.</param>
    /// <returns>A formatted progress bar string.</returns>
    public static string CreateProgressBar(double percentage, int width = 10)
    {
        var filled = (int)Math.Round(percentage / 100 * width);
        filled = Math.Clamp(filled, 0, width);
        var empty = width - filled;

        var color = percentage switch
        {
            >= 90 => "red",
            >= 70 => "yellow",
            _ => "green"
        };

        return $"[{color}]{new string('\u2588', filled)}[/][grey]{new string('\u2591', empty)}[/]";
    }

    /// <summary>
    /// Gets the color-coded markup for a percentage value.
    /// </summary>
    /// <param name="percentage">The percentage value.</param>
    /// <returns>A color-coded markup string.</returns>
    public static string GetPercentageMarkup(double percentage)
    {
        return percentage switch
        {
            >= 90 => $"[red]{percentage:F1}%[/]",
            >= 70 => $"[yellow]{percentage:F1}%[/]",
            _ => $"[green]{percentage:F1}%[/]"
        };
    }
}
