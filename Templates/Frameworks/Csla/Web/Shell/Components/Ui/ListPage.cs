// The lists' base: a root's read-only list (ReadOnlyListBase), fetched whole or by a foreign key's value (the URL's
// query), searched, sorted and paged in the circuit. The generated lists (Generated/*ListPage.razor) inherit it, and
// ListView.razor draws it. Csla.cst wrote this file once: it's yours.
using Csla.Rules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace PetShop.Web.Components.Ui;

public abstract class ListPage<TItem> : ComponentBase where TItem : class
{
    private object? _filter = Unset;
    private Task? _lookups;
    private static readonly object Unset = new();

    [Inject] protected Csla.IDataPortalFactory Portals { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected Csla.ApplicationContext ApplicationContext { get; set; } = default!;
    [Inject] protected Counts Counts { get; set; } = default!;

    [CascadingParameter] protected Task<AuthenticationState>? Authentication { get; set; }

    public abstract RootPage Root { get; }

    /// <summary>The editable object's type: whether the user may create one (the New button).</summary>
    protected abstract Type EditableType { get; }

    /// <summary>The rows: the list's FetchAsync, with the filter's criteria where one's picked.</summary>
    protected abstract Task<IEnumerable<TItem>> FetchAsync();

    /// <summary>A row's editor.</summary>
    public abstract string RowHref(TItem row);

    /// <summary>The filters' values (the URL's query), null for none: fetched again when they change.</summary>
    protected virtual object? Filter => null;

    /// <summary>The filters' names in the URL's query: one's picked at a time (a fetch takes one criteria).</summary>
    protected virtual string[] FilterNames => [];

    /// <summary>The name/value lists the columns and filters show names from.</summary>
    protected virtual Task LoadLookupsAsync() => Task.CompletedTask;

    /// <summary>Columns of yours: after the one each names, or at the end.</summary>
    public virtual IEnumerable<ListColumn<TItem>> MoreColumns => [];

    /// <summary>Generated columns not to show, by their titles.</summary>
    public virtual IEnumerable<string> HiddenColumns => [];

    public IReadOnlyList<TItem>? Rows { get; private set; }
    public Problem? Problem { get; private set; }
    public bool Refreshing { get; private set; }
    public bool Filtered => Filter is not null;

    public bool CanCreate => BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.CreateObject, EditableType);

    private Task<AuthenticationState>? _authentication;

    protected override async Task OnParametersSetAsync()
    {
        // a user who's just signed in may create, or not: once CSLA's ApplicationContext has them (it tells Blazor first)
        if (!ReferenceEquals(_authentication, Authentication))
        {
            if (_authentication is not null)
                await Task.Yield();
            _authentication = Authentication;
        }
        object? filter = Filter;
        if (Equals(filter, _filter))
            return;
        _filter = filter;
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        Refreshing = Rows is not null;
        Problem = null;
        try
        {
            var lookups = _lookups ??= LoadLookupsQuietlyAsync();
            var rows = await FetchAsync();
            await lookups;
            Rows = rows.ToList();
            if (!Filtered)
                _ = Counts.RefreshAsync(Root);
        }
        catch (Exception e)
        {
            Problem = Problem.From(e, Root.Title.ToLowerInvariant() + " list");
            Rows ??= [];
        }
        Refreshing = false;
    }

    // a lookup that fails leaves its keys showing, not its names
    private async Task LoadLookupsQuietlyAsync()
    {
        try
        {
            await LoadLookupsAsync();
        }
        catch (Exception)
        {
        }
    }

    /// <summary>Picks a filter's value (the others go): the URL's query says it, so a filtered list is a link.</summary>
    protected void Pick(string property, object? value)
    {
        string name = char.ToLowerInvariant(property[0]) + property[1..];
        var query = FilterNames.ToDictionary(n => n, n => n == name ? value : null);
        Navigation.NavigateTo(Navigation.GetUriWithQueryParameters(query));
    }
}
