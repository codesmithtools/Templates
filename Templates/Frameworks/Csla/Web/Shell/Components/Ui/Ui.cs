// The shell's services and small types: the pages the sidebar lists, toasts, confirmations, the roots' counts, formats
// and the words for what went wrong. Csla.cst wrote this file once: it's yours.
using System.Globalization;
using Csla;
using Microsoft.EntityFrameworkCore;

namespace PetShop.Web.Components.Ui;

/// <summary>A root's pages: its list at Href, its editor at Href/new and Href/{key}. Generated/Site.g.cs has them all.</summary>
public sealed record RootPage(string Title, string Singular, string Href, string Icon, string ListType, Func<IDataPortalFactory, Task<int>> Count)
{
    public string NewHref => Href + "/new";
}

/// <summary>A figure an editor's summary or a grid's footer shows: a page's partial class adds them (Stats, Totals).</summary>
public sealed record Stat(string Label, string Value, bool Big = false);

/// <summary>A column of your own in a child grid (Columns), after the generated ones: a subtotal, say.</summary>
public sealed record GridColumn<TItem>(string Title, Func<TItem, string> Value, bool Numeric = true);

/// <summary>A list's column: a generated one (Column.razor), or one of yours (ListPage.MoreColumns).</summary>
public interface IListColumn<TItem>
{
    string Title { get; }
    Func<TItem, object?> Value { get; }
    ValueKind Kind { get; }
    int Scale { get; }
    IReadOnlyDictionary<object, string>? Lookup { get; }
}

/// <summary>A list's column of your own: <c>new("Bill to", row => row.BillToLastName, After: "Order ID")</c> (at the end
/// without After).</summary>
public sealed record ListColumn<TItem>(string Title, Func<TItem, object?> Value, ValueKind Kind = ValueKind.Text, int Scale = 0,
    IReadOnlyDictionary<object, string>? Lookup = null, string? After = null) : IListColumn<TItem>;

/// <summary>What a column says for a row, and what it sorts by.</summary>
public static class ListColumns
{
    public static string Text<TItem>(this IListColumn<TItem> column, TItem row)
    {
        object? value = column.Value(row);
        if (value is null)
            return "";
        if (column.Lookup is not null && column.Lookup.TryGetValue(value, out var name))
            return name;
        return column.Kind switch
        {
            ValueKind.Money => Formats.Money(value),
            ValueKind.Number => Formats.Number(value, column.Scale),
            ValueKind.Date => Formats.Date(value),
            ValueKind.DateTime => Formats.DateTime(value),
            ValueKind.Bool => value is true ? "Yes" : "No",
            _ => Convert.ToString(value, CultureInfo.CurrentCulture) ?? "",
        };
    }

    public static object? SortKey<TItem>(this IListColumn<TItem> column, TItem row)
    {
        object? value = column.Value(row);
        if (value is not null && column.Lookup is not null && column.Lookup.TryGetValue(value, out var name))
            return name;
        return value;
    }

    public static bool Numeric<TItem>(this IListColumn<TItem> column) => column.Kind is ValueKind.Number or ValueKind.Money;
}

/// <summary>A key in a page's address.</summary>
public static class Url
{
    public static string Segment(object? value) => Uri.EscapeDataString(value switch
    {
        null => "",
        DateTime d => d.ToString("s", CultureInfo.InvariantCulture),
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? "",
    });
}

public static class UiServiceCollectionExtensions
{
    /// <summary>The shell's services, one of each per circuit, and the editors' view model.</summary>
    public static IServiceCollection AddUi(this IServiceCollection services)
    {
        services.AddScoped<Toasts>();
        services.AddScoped<Confirmations>();
        services.AddScoped<Counts>();
        services.AddScoped<Palette>();
        services.AddTransient(typeof(EditorModel<>));
        return services;
    }
}

public enum ToastKind { Success, Error, Info, Warning }

public sealed record Toast(int Id, ToastKind Kind, string Title, string? Detail);

/// <summary>What just happened (a save, a delete, a problem): the layout's ToastHost shows them for a few seconds.</summary>
public sealed class Toasts
{
    private int _next;

    public List<Toast> Items { get; } = [];
    public event Action? Changed;

    public void Success(string title, string? detail = null) => Add(ToastKind.Success, title, detail);
    public void Error(string title, string? detail = null) => Add(ToastKind.Error, title, detail);
    public void Info(string title, string? detail = null) => Add(ToastKind.Info, title, detail);
    public void Warning(string title, string? detail = null) => Add(ToastKind.Warning, title, detail);

    public void Dismiss(Toast toast)
    {
        if (Items.Remove(toast))
            Changed?.Invoke();
    }

    private void Add(ToastKind kind, string title, string? detail)
    {
        var toast = new Toast(++_next, kind, title, detail);
        Items.Add(toast);
        if (Items.Count > 4)
            Items.RemoveAt(0);
        Changed?.Invoke();
        _ = ExpireAsync(toast, kind == ToastKind.Error ? 8000 : 4500);
    }

    private async Task ExpireAsync(Toast toast, int milliseconds)
    {
        await Task.Delay(milliseconds);
        Dismiss(toast);
    }
}

public sealed record Confirmation(string Title, string Message, string Confirm, bool Danger, string Icon);

/// <summary>A question before something that can't be undone: <c>await Confirmations.AskAsync(...)</c> (ConfirmHost asks it).</summary>
public sealed class Confirmations
{
    private TaskCompletionSource<bool>? _answer;

    public Confirmation? Current { get; private set; }
    public event Action? Changed;

    public Task<bool> AskAsync(string title, string message, string confirm = "Confirm", bool danger = false, string icon = "alert-triangle")
    {
        _answer?.TrySetResult(false);
        _answer = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Current = new Confirmation(title, message, confirm, danger, icon);
        Changed?.Invoke();
        return _answer.Task;
    }

    public void Answer(bool yes)
    {
        var answer = _answer;
        _answer = null;
        Current = null;
        Changed?.Invoke();
        answer?.TrySetResult(yes);
    }
}

/// <summary>The command palette (Ctrl+K): open it from anywhere.</summary>
public sealed class Palette
{
    public bool IsOpen { get; private set; }
    public bool ShowsShortcuts { get; private set; }
    public event Action? Changed;

    public void Open(bool shortcuts = false)
    {
        IsOpen = true;
        ShowsShortcuts = shortcuts;
        Changed?.Invoke();
    }

    public void Close()
    {
        IsOpen = false;
        Changed?.Invoke();
    }
}

/// <summary>How many rows each root has, for the sidebar and the home page: each root's count (OrderListCount, a
/// command the database answers with a COUNT), once a circuit, and again after a save or a delete.</summary>
public sealed class Counts(IDataPortalFactory portals)
{
    private readonly Dictionary<RootPage, int> _counts = [];
    private Task? _loading;

    public event Action? Changed;

    public int? this[RootPage root] => _counts.TryGetValue(root, out int count) ? count : null;

    public Task LoadAsync() => _loading ??= Task.WhenAll(Site.Roots.Select(RefreshAsync));

    public async Task RefreshAsync(RootPage root)
    {
        try
        {
            _counts[root] = await root.Count(portals);
        }
        catch (Exception)
        {
            _counts.Remove(root); // the page says what's wrong when it's opened
        }
        Changed?.Invoke();
    }
}

/// <summary>How values read: in the app's culture (Program.cs: UseRequestLocalization).</summary>
public static class Formats
{
    public static string Money(object? value) => value is null ? "" : Convert.ToDecimal(value, CultureInfo.InvariantCulture).ToString("C2", CultureInfo.CurrentCulture);

    public static string Number(object? value, int scale = 0) => value switch
    {
        null => "",
        decimal d => d.ToString("N" + scale, CultureInfo.CurrentCulture),
        double d => d.ToString("N" + scale, CultureInfo.CurrentCulture),
        float f => f.ToString("N" + scale, CultureInfo.CurrentCulture),
        IFormattable f => f.ToString("N0", CultureInfo.CurrentCulture),
        _ => value.ToString() ?? "",
    };

    public static string Date(object? value) => value switch
    {
        DateTime d => d.ToString("MMM d, yyyy", CultureInfo.CurrentCulture),
        DateTimeOffset d => d.ToString("MMM d, yyyy", CultureInfo.CurrentCulture),
        DateOnly d => d.ToString("MMM d, yyyy", CultureInfo.CurrentCulture),
        _ => value?.ToString() ?? "",
    };

    public static string DateTime(object? value) => value switch
    {
        System.DateTime d => d.ToString("MMM d, yyyy · h:mm tt", CultureInfo.CurrentCulture),
        DateTimeOffset d => d.ToString("MMM d, yyyy · h:mm tt", CultureInfo.CurrentCulture),
        _ => value?.ToString() ?? "",
    };

    /// <summary>A type's name in words: LineItem, "Line item".</summary>
    public static string Words(string name)
    {
        var words = System.Text.RegularExpressions.Regex.Split(name, "(?<=[a-z])(?=[A-Z0-9])|(?<=[A-Z])(?=[A-Z][a-z])").Where(w => w.Length > 0).ToList();
        for (int i = 1; i < words.Count; i++)
            if (words[i].Any(char.IsLower))
                words[i] = words[i].ToLowerInvariant();
        return string.Join(" ", words);
    }

    public static string Plural(int count, string singular) => count == 1 ? "1 " + singular : count.ToString("N0", CultureInfo.CurrentCulture) + " " + PluralOf(singular);

    public static string PluralOf(string singular) =>
        singular.EndsWith('y') && !singular.EndsWith("ey") ? singular[..^1] + "ies" :
        singular.EndsWith('s') || singular.EndsWith('x') || singular.EndsWith("ch") || singular.EndsWith("sh") ? singular + "es" : singular + "s";
}

/// <summary>
/// What went wrong, in words for whoever was using the page. Its detail is the exception's own words, in Development
/// (a database's can name tables, columns and values); Unexpected says it's a fault to look into (EditPage logs it as
/// an error), not something the user can put right (a row that's gone, a key that's taken).
/// </summary>
public sealed record Problem(string Title, string Message, string? Detail = null, bool CanReload = false, bool Unexpected = false)
{
    /// <summary>What went wrong loading (done null), or saving or deleting (done "saved", "deleted") a what. The database's
    /// refusals by why, as FullStack's API has them: a unique key another row has, a reference to a row that isn't there,
    /// rows that refer to the one deleted, or something else.</summary>
    public static Problem From(Exception exception, string what, string? done = null, bool development = false)
    {
        var all = Unwrap(exception).ToList();
        string? detail = development ? all.Last().Message : null;
        if (all.OfType<ConcurrencyException>().FirstOrDefault() is { } concurrency)
            return new Problem("Someone else changed it", $"This {what} changed (or was deleted) since you opened it. Reload it to see what's there now; your changes aren't saved.", concurrency.Message, CanReload: true);
        if (all.OfType<DataNotFoundException>().FirstOrDefault() is { } missing)
            return done is null
                ? new Problem($"There's no such {what}", $"It may have been deleted. ({missing.Message})")
                : new Problem("Someone deleted it", $"This {what} was deleted since you opened it, so your changes can't be saved.", missing.Message);
        if (all.OfType<Csla.Security.SecurityException>().FirstOrDefault() is { } security)
            return new Problem("You can't do that", security.Message);
        if (all.OfType<Csla.Rules.ValidationException>().FirstOrDefault() is { } invalid)
            return new Problem("It isn't valid yet", invalid.Message);
        if (all.Any(e => e is DbUpdateException or System.Data.Common.DbException))
        {
            string not = $"The {what} wasn't {done ?? "loaded"}";
            return Refusal(all, done) switch
            {
                SaveRefusal.Unique => new Problem("The database said no", $"{not}: another {what} has the same key, or a value only one may have.", detail),
                SaveRefusal.Reference => new Problem("The database said no", $"{not}: it refers to a row that isn't there.", detail),
                SaveRefusal.Referenced => new Problem("The database said no", $"{not}: other rows refer to it. Delete or change them first.", detail),
                _ => new Problem("The database said no", done is null ? $"{not}: the database couldn't give it." : $"{not}: the database won't take one of its values.", detail, Unexpected: true),
            };
        }
        return new Problem("Something went wrong", $"The {what} wasn't {done ?? "loaded"}.", detail, Unexpected: true);
    }

    /// <summary>Why the database refused a save, by each provider's code: PostgreSQL's SQLSTATE, SQL Server's and MySQL's
    /// error numbers, SQLite's extended result code. A reference that fails while a row is deleted is to that row.</summary>
    public static SaveRefusal Refusal(IEnumerable<Exception> exceptions, string? done)
    {
        foreach (var error in exceptions.OfType<System.Data.Common.DbException>())
        {
            string? state = error.SqlState;
            int number = error.GetType().GetProperty("Number")?.GetValue(error) is int n ? n : 0;
            int sqlite = error.GetType().GetProperty("SqliteExtendedErrorCode")?.GetValue(error) is int s ? s : 0;
            if (state == "23505" || number is 2627 or 2601 or 1062 || sqlite is 2067 or 1555)
                return SaveRefusal.Unique;
            // SQL Server's 547 is a CHECK constraint's too
            if (state == "23503" || number is 1451 or 1452 || number == 547 && !error.Message.Contains("CHECK") || sqlite == 787)
                return done == "deleted" || number == 1451 ? SaveRefusal.Referenced : SaveRefusal.Reference;
        }
        return SaveRefusal.Other;
    }

    private static IEnumerable<Exception> Unwrap(Exception exception)
    {
        for (var e = exception; e != null; e = e.InnerException)
        {
            yield return e;
            if (e is DataPortalException { BusinessException: { } business } && !ReferenceEquals(business, e.InnerException))
                foreach (var inner in Unwrap(business))
                    yield return inner;
        }
    }
}

/// <summary>Why the database refused a save: a unique key another row has, a reference to a row that isn't there, rows
/// that refer to the one deleted, or something else (a check, a value it won't take).</summary>
public enum SaveRefusal { Other, Unique, Reference, Referenced }

/// <summary>How a value reads in a list, or in a field the database sets.</summary>
public enum ValueKind { Text, LongText, Key, Number, Money, Date, DateTime, Bool }

/// <summary>A name/value list as a lookup: its keys and names, for LookupField, LookupPicker and a list's Column.</summary>
public static class Lookup
{
    public static IReadOnlyDictionary<object, string> Of<TKey>(NameValueListBase<TKey, string> list) where TKey : notnull
    {
        var lookup = new Dictionary<object, string>();
        foreach (var pair in list)
            lookup[pair.Key] = pair.Value;
        return lookup;
    }
}
