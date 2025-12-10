using AdGuard.ApiClient.Client;
using AdGuard.ConsoleUI.Abstractions;
using AdGuard.ConsoleUI.Helpers;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Services;

/// <summary>
/// Abstract base class for menu services implementing the Template Method pattern.
/// Provides common menu loop logic and error handling.
/// </summary>
public abstract class BaseMenuService : IMenuService
{
    /// <summary>
    /// Gets the display title for this menu.
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Gets the menu choices and their corresponding actions.
    /// </summary>
    /// <returns>A dictionary mapping choice text to async action.</returns>
    protected abstract Dictionary<string, Func<Task>> GetMenuActions();

    /// <summary>
    /// Shows the menu and handles user interaction using the template method pattern.
    /// </summary>
    public virtual async Task ShowAsync()
    {
        var running = true;

        while (running)
        {
            var actions = GetMenuActions();
            var choices = actions.Keys.Concat(new[] { "Back" }).ToArray();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[green]{Title}[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(Color.Green))
                    .AddChoices(choices));

            AnsiConsole.WriteLine();

            if (choice == "Back")
            {
                running = false;
                continue;
            }

            try
            {
                if (actions.TryGetValue(choice, out var action))
                {
                    await action();
                }
            }
            catch (ApiException ex)
            {
                HandleApiError(ex);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }
    }

    /// <summary>
    /// Handles API exceptions. Can be overridden for custom handling.
    /// </summary>
    /// <param name="ex">The API exception.</param>
    protected virtual void HandleApiError(ApiException ex)
    {
        ConsoleHelpers.ShowApiError(ex);
    }

    /// <summary>
    /// Handles general exceptions. Can be overridden for custom handling.
    /// </summary>
    /// <param name="ex">The exception.</param>
    protected virtual void HandleError(Exception ex)
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        AnsiConsole.WriteLine();
    }
}

/// <summary>
/// A menu action representing a named action that can be executed.
/// </summary>
public record MenuAction(string Name, Func<Task> Action);
