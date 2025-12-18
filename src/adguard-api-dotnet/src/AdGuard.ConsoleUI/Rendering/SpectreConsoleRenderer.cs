using AdGuard.ConsoleUI.Abstractions;
using Spectre.Console;

namespace AdGuard.ConsoleUI.Rendering;

/// <summary>
/// Spectre.Console implementation of <see cref="IConsoleRenderer"/>.
/// </summary>
public class SpectreConsoleRenderer : IConsoleRenderer
{
    /// <inheritdoc/>
    public void WriteLine(string text)
    {
        AnsiConsole.WriteLine(text);
    }

    /// <inheritdoc/>
    public void Write(string text)
    {
        AnsiConsole.Write(text);
    }

    /// <inheritdoc/>
    public void WriteLine()
    {
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc/>
    public void WriteStyled(string text, TextStyle style)
    {
        var spectreStyle = ConvertStyle(style);
        AnsiConsole.Write(new Text(text, spectreStyle));
        AnsiConsole.WriteLine();
    }

    /// <inheritdoc/>
    public void WriteMarkup(string markup)
    {
        AnsiConsole.Markup(markup);
    }

    /// <inheritdoc/>
    public void WriteMarkupLine(string markup)
    {
        AnsiConsole.MarkupLine(markup);
    }

    /// <inheritdoc/>
    public void RenderTable(Abstractions.ConsoleTable table)
    {
        var spectreTable = new Table();

        if (!string.IsNullOrEmpty(table.Title))
        {
            spectreTable.Title(table.Title);
        }

        if (!table.ShowBorders)
        {
            spectreTable.Border(TableBorder.None);
        }

        if (table.Expand)
        {
            spectreTable.Expand();
        }

        foreach (var column in table.Columns)
        {
            var tableColumn = new TableColumn(column.Header);
            tableColumn.Alignment = ConvertAlignment(column.Alignment);
            spectreTable.AddColumn(tableColumn);
        }

        foreach (var row in table.Rows)
        {
            spectreTable.AddRow(row.Values.Select(v => new Markup(v)).ToArray());
        }

        AnsiConsole.Write(spectreTable);
    }

    /// <inheritdoc/>
    public void RenderPanel(string content, string? title = null)
    {
        var panel = new Panel(content);
        if (!string.IsNullOrEmpty(title))
        {
            panel.Header(title);
        }
        AnsiConsole.Write(panel);
    }

    /// <inheritdoc/>
    public void RenderRule(string? title = null)
    {
        if (string.IsNullOrEmpty(title))
        {
            AnsiConsole.Write(new Rule());
        }
        else
        {
            AnsiConsole.Write(new Rule(title));
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        AnsiConsole.Clear();
    }

    /// <inheritdoc/>
    public async Task<T> StatusAsync<T>(string status, Func<Task<T>> operation)
    {
        return await AnsiConsole.Status()
            .StartAsync(status, async ctx => await operation());
    }

    /// <inheritdoc/>
    public async Task StatusAsync(string status, Func<Task> operation)
    {
        await AnsiConsole.Status()
            .StartAsync(status, async ctx => await operation());
    }

    /// <inheritdoc/>
    public async Task<T> ProgressAsync<T>(string description, Func<IProgress<double>, Task<T>> operation)
    {
        T result = default!;
        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(description);
                var progress = new Progress<double>(p => task.Value = p * 100);
                result = await operation(progress);
                task.Value = 100;
            });
        return result;
    }

    private static Style ConvertStyle(TextStyle style)
    {
        var decoration = Decoration.None;
        if (style.Bold) decoration |= Decoration.Bold;
        if (style.Italic) decoration |= Decoration.Italic;
        if (style.Underline) decoration |= Decoration.Underline;

        var foreground = style.ForegroundColor.HasValue
            ? ConvertColor(style.ForegroundColor.Value)
            : Color.Default;

        var background = style.BackgroundColor.HasValue
            ? ConvertColor(style.BackgroundColor.Value)
            : Color.Default;

        return new Style(foreground, background, decoration);
    }

    private static Color ConvertColor(Abstractions.ConsoleColor color)
    {
        return color switch
        {
            Abstractions.ConsoleColor.Black => Color.Black,
            Abstractions.ConsoleColor.DarkBlue => Color.Navy,
            Abstractions.ConsoleColor.DarkGreen => Color.Green,
            Abstractions.ConsoleColor.DarkCyan => Color.Teal,
            Abstractions.ConsoleColor.DarkRed => Color.Maroon,
            Abstractions.ConsoleColor.DarkMagenta => Color.Purple,
            Abstractions.ConsoleColor.DarkYellow => Color.Olive,
            Abstractions.ConsoleColor.Gray => Color.Silver,
            Abstractions.ConsoleColor.DarkGray => Color.Grey,
            Abstractions.ConsoleColor.Blue => Color.Blue,
            Abstractions.ConsoleColor.Green => Color.Lime,
            Abstractions.ConsoleColor.Cyan => Color.Aqua,
            Abstractions.ConsoleColor.Red => Color.Red,
            Abstractions.ConsoleColor.Magenta => Color.Fuchsia,
            Abstractions.ConsoleColor.Yellow => Color.Yellow,
            Abstractions.ConsoleColor.White => Color.White,
            _ => Color.Default
        };
    }

    private static Justify ConvertAlignment(Abstractions.TextAlignment alignment)
    {
        return alignment switch
        {
            Abstractions.TextAlignment.Left => Justify.Left,
            Abstractions.TextAlignment.Center => Justify.Center,
            Abstractions.TextAlignment.Right => Justify.Right,
            _ => Justify.Left
        };
    }
}

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
