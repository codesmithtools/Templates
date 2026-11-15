// The child grids' base: a child list (BusinessListBase) edited in place. Rows are added (AddNewAsync) and removed (to
// the list's DeletedList, until the root's saved), and each row's broken rules show beside it. The generated grids
// (Generated/*Grid.razor) inherit it, and GridCard.razor draws it. Csla.cst wrote this file once: it's yours.
using System.Linq.Expressions;
using Csla.Blazor;
using Csla.Rules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PetShop.Web.Components.Ui;

/// <summary>A grid as GridCard draws it.</summary>
public interface IChildGrid<TItem>
{
    IEditor Editor { get; }
    IReadOnlyList<TItem> Rows { get; }
    string Title { get; }
    string Singular { get; }
    string Icon { get; }
    IPropertyInfo? ListInfo { get; }
    IEnumerable<GridColumn<TItem>> Columns { get; }
    IEnumerable<Stat> Totals { get; }
    bool HasDetail { get; }
    bool IsExpanded(TItem row);
    void Toggle(TItem row);
    Task AddAsync();
    void Remove(TItem row);
    IReadOnlyList<(RuleSeverity Severity, string Message)> MessagesOf(TItem row);
}

/// <summary>
/// A child list's grid. A generated grid says its title and its columns, and numbers a new row; your partial class of it
/// can add Columns (computed ones, after the generated), Totals (its footer's figures) and a new row's defaults
/// (OnRowAdded).
/// </summary>
public abstract class ChildGrid<TList, TItem> : ComponentBase, IChildGrid<TItem>
    where TList : Csla.BusinessListBase<TList, TItem>
    where TItem : Csla.BusinessBase<TItem>
{
    private readonly HashSet<TItem> _expanded = new(ReferenceEqualityComparer.Instance);

    [CascadingParameter] public IEditor Editor { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected Csla.IDataPortalFactory Portals { get; set; } = default!;

    /// <summary>The parent's list.</summary>
    [Parameter, EditorRequired] public TList Items { get; set; } = default!;

    /// <summary>The parent's property that holds the list: rules of the parent's on it (an order needs a line).</summary>
    [Parameter] public IPropertyInfo? ListInfo { get; set; }

    public IReadOnlyList<TItem> Rows => Items;
    public abstract string Title { get; }
    public abstract string Singular { get; }
    public virtual string Icon => "list";
    public virtual bool HasDetail => false;

    /// <summary>Columns of your own, after the generated ones (a subtotal).</summary>
    public virtual IEnumerable<GridColumn<TItem>> Columns => [];

    /// <summary>The footer's figures (a total).</summary>
    public virtual IEnumerable<Stat> Totals => [];

    /// <summary>A new row: AddNewAsync (the child's [CreateChild]); a generated grid numbers a key the app sets.</summary>
    protected virtual async Task<TItem> AddRowAsync() => await Items.AddNewAsync();

    /// <summary>A row's property: CSLA's PropertyInfo for that row.</summary>
    protected IPropertyInfo Info<P>(Expression<Func<P>> property, TItem row) => Editor.Info(property, row);

    /// <summary>The next number for a key the app sets (LineNum): one more than the highest.</summary>
    protected int Next(Func<TItem, int> key) => Items.Count == 0 ? 1 : Items.Max(key) + 1;

    protected long Next(Func<TItem, long> key) => Items.Count == 0 ? 1 : Items.Max(key) + 1;

    protected short Next(Func<TItem, short> key) => (short)(Items.Count == 0 ? 1 : Items.Max(key) + 1);

    public async Task AddAsync()
    {
        var row = await AddRowAsync();
        if (ListInfo is not null)
            Editor.Touch(ListInfo); // the parent's rules on the list show now (an order needs a line)
        if (HasDetail)
            _expanded.Add(row);
        Editor.Changed();
        await Task.Yield();
        await JS.InvokeVoidAsync("ui.focusLastRow", GetType().Name);
    }

    public void Remove(TItem row)
    {
        _expanded.Remove(row);
        Items.Remove(row); // a saved row goes to DeletedList: it's deleted when the root's saved
        if (ListInfo is not null)
            Editor.Touch(ListInfo);
        Editor.Changed();
    }

    public bool IsExpanded(TItem row) => _expanded.Contains(row);

    public void Toggle(TItem row)
    {
        if (!_expanded.Remove(row))
            _expanded.Add(row);
    }

    public IReadOnlyList<(RuleSeverity Severity, string Message)> MessagesOf(TItem row) =>
        row.BrokenRulesCollection
            .Where(rule => Editor.Shows(row, rule.Property))
            .OrderBy(rule => rule.Severity)
            .Select(rule => (rule.Severity, rule.Description))
            .ToList();
}
