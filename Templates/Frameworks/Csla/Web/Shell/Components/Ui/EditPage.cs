// The editors' base: a business object in CSLA's ViewModel<T>, loaded, saved, reverted with n-level undo and deleted;
// its properties' metastate (Info: CSLA's PropertyInfo); which of its broken rules show yet; and leaving with unsaved
// changes. The generated editors (Generated/*EditPage.razor) inherit it, and Editor.razor draws it. Csla.cst wrote this
// file once: it's yours.
using System.Linq.Expressions;
using Csla.Blazor;
using Csla.Rules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace PetShop.Web.Components.Ui;

/// <summary>What fields and grids ask of the editor they're in (Editor cascades it).</summary>
public interface IEditor
{
    /// <summary>Whether the user may edit the object at all (CSLA's EditObject or CreateObject permission).</summary>
    bool ReadOnly { get; }

    /// <summary>A child's property: CSLA's PropertyInfo, one per row (ViewModel.GetPropertyInfo with the row's id).</summary>
    IPropertyInfo Info<P>(Expression<Func<P>> property, object row);

    /// <summary>Whether a property's messages show yet: once it's been touched, or after a save that wasn't.</summary>
    bool Shows(IPropertyInfo info);

    /// <summary>Whether a row's rule on a property (or on the row, with none) shows yet.</summary>
    bool Shows(object row, string? property);

    void Touch(IPropertyInfo info);

    /// <summary>The id of a property's input, for its label and for the issues' links.</summary>
    string IdOf(IPropertyInfo info);

    /// <summary>Something changed that the page shows (a row added or removed).</summary>
    void Changed();
}

/// <summary>A broken rule, for the issues list: where it is, and the field to go to.</summary>
public sealed record Issue(RuleSeverity Severity, string Message, string? Where, IPropertyInfo? Info);

/// <summary>An editor as Editor.razor draws it.</summary>
public interface IEditorPage : IEditor
{
    RootPage Root { get; }
    Csla.Core.BusinessBase? Object { get; }
    bool Loading { get; }
    Problem? LoadProblem { get; }
    Problem? SaveProblem { get; }
    string Title { get; }
    string TypeName { get; }
    bool IsNew { get; }
    bool HasChanges { get; }
    bool Busy { get; }
    bool ShowAll { get; }
    DateTime? SavedAt { get; }
    bool CanCreate { get; }
    bool CanEdit { get; }
    bool CanDelete { get; }
    string UserName { get; }
    string? DeleteNote { get; }
    IReadOnlyList<Issue> Issues { get; }
    IEnumerable<Stat> Stats { get; }
    Task SaveAsync();
    Task DiscardAsync();
    Task DeleteAsync();
    void Cancel();
    Task ReloadAsync();
    Task GoToAsync(Issue issue);
    Task OnLeavingAsync(LocationChangingContext context);
    void DismissProblem();
}

/// <summary>CSLA's ViewModel, with n-level undo (ManageObjectLifetime): Revert cancels what changed since the object was
/// loaded or saved. Its permissions are checked again when the user changes (ViewModel checks them once).</summary>
public class EditorModel<T>(Csla.ApplicationContext applicationContext) : ViewModel<T>(applicationContext)
{
    public void Revert() => DoCancel();

    public void RefreshPermissions()
    {
        CanCreateObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.CreateObject, typeof(T));
        CanGetObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.GetObject, typeof(T));
        CanEditObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.EditObject, typeof(T));
        CanDeleteObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.DeleteObject, typeof(T));
    }
}

/// <summary>
/// An editable root's page. A generated page says how to load, delete and name its object; your partial class of it can
/// add Stats (the summary's figures) and override OnLoadedAsync and OnSavedAsync.
/// </summary>
public abstract class EditPage<T> : ComponentBase, IEditorPage, IDisposable where T : Csla.BusinessBase<T>
{
    private readonly HashSet<IPropertyInfo> _touched = [];
    private readonly Dictionary<IPropertyInfo, string> _ids = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<(object, string), IPropertyInfo> _byTarget = [];
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, string> _rowIds = [];
    private int _nextRowId;
    private string? _loadedPath;
    private Task<AuthenticationState>? _authentication;
    private bool _edited;
    private bool _leaving;
    private bool _deleting;
    private ILogger? _logger;

    [Inject] protected Csla.ApplicationContext ApplicationContext { get; set; } = default!;
    [Inject] protected Csla.IDataPortalFactory Portals { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected Toasts Toasts { get; set; } = default!;
    [Inject] protected Confirmations Confirmations { get; set; } = default!;
    [Inject] protected Counts Counts { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected ILoggerFactory LoggerFactory { get; set; } = default!;
    [Inject] protected IHostEnvironment Environment { get; set; } = default!;

    /// <summary>CSLA's view model: the object, whether it's busy, its permissions and what went wrong.</summary>
    [Inject] protected EditorModel<T> Vm { get; set; } = default!;

    [CascadingParameter] protected Task<AuthenticationState>? Authentication { get; set; }

    /// <summary>The object (null while it loads).</summary>
    protected T Model => Vm.Model!;

    // ---- what a generated page says ----

    public abstract RootPage Root { get; }

    /// <summary>Fetches the object the route names, or creates a new one; and loads what its fields look up.</summary>
    protected abstract Task<T> LoadAsync();

    /// <summary>Deletes the object by its key.</summary>
    protected abstract Task DeleteAsync(T model);

    /// <summary>The page of a saved object.</summary>
    protected abstract string Href(T model);

    /// <summary>A saved object's name, for its title.</summary>
    protected abstract string Name(T model);

    /// <summary>What else a delete deletes (its children), for the question before it.</summary>
    public virtual string? DeleteNote => null;

    // ---- what yours may add ----

    /// <summary>Figures for the summary beside the form (a total, a count).</summary>
    public virtual IEnumerable<Stat> Stats => [];

    /// <summary>After the object's loaded (or created).</summary>
    protected virtual Task OnLoadedAsync() => Task.CompletedTask;

    /// <summary>After it's saved.</summary>
    protected virtual Task OnSavedAsync() => Task.CompletedTask;

    // ---- loading ----

    protected override void OnInitialized()
    {
        Vm.ManageObjectLifetime = true;
        Vm.ModelPropertyChanged += OnModelChanged;
        Vm.ModelChildChanged += OnChildChanged;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!ReferenceEquals(_authentication, Authentication))
        {
            bool switched = _authentication is not null;
            _authentication = Authentication;
            // a user who's just signed in (the header's switcher) may do more, or less: once CSLA's ApplicationContext
            // has them (it tells Blazor first)
            if (switched)
                await Task.Yield();
            Vm.RefreshPermissions();
        }
        string path = CurrentPath();
        if (path != _loadedPath)
        {
            _loadedPath = path;
            await ReloadAsync();
        }
    }

    public async Task ReloadAsync()
    {
        Loading = true;
        LoadProblem = SaveProblem = null;
        SavedAt = null;
        ResetMessages();
        await Vm.RefreshAsync(async () => await LoadAsync());
        if (Vm.Exception is { } e)
            LoadProblem = Log(Problem.From(e, Root.Singular.ToLowerInvariant(), development: Environment.IsDevelopment()), e, "Loading");
        else
            await OnLoadedAsync();
        _edited = false;
        Loading = false;
    }

    private string CurrentPath()
    {
        string path = Navigation.ToBaseRelativePath(Navigation.Uri);
        int end = path.IndexOfAny(['?', '#']);
        return end < 0 ? path : path[..end];
    }

    private void OnModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _edited = true;
        _ = InvokeAsync(StateHasChanged);
    }

    private void OnChildChanged(object sender, Csla.Core.ChildChangedEventArgs e)
    {
        _edited = true;
        _ = InvokeAsync(StateHasChanged);
    }

    // ---- the object's state ----

    public Csla.Core.BusinessBase? Object => Vm.Model;
    public bool Loading { get; private set; } = true;
    public Problem? LoadProblem { get; private set; }
    public Problem? SaveProblem { get; private set; }
    public DateTime? SavedAt { get; private set; }
    public bool ShowAll { get; private set; }
    public bool IsNew => Vm.Model?.IsNew ?? true;
    public bool Busy => Vm.IsBusy || _deleting;
    public string Title => Vm.Model is null ? Root.Singular : IsNew ? "New " + Root.Singular.ToLowerInvariant() : Name(Model);
    public string TypeName => $"{typeof(T).Name} : BusinessBase<{typeof(T).Name}>";

    /// <summary>Unsaved changes: an object's that's been saved, IsDirty; a new one's, anything typed (it's always dirty).</summary>
    public bool HasChanges => Vm.Model is { } model && (model.IsNew ? _edited : model.IsDirty);

    public bool CanCreate => Vm.CanCreateObject;
    public bool CanEdit => IsNew ? Vm.CanCreateObject : Vm.CanEditObject;
    public bool CanDelete => Vm.CanDeleteObject;
    public bool ReadOnly => !CanEdit;
    public string UserName => ApplicationContext.User?.Identity?.Name is { Length: > 0 } name ? name : "Anonymous";

    /// <summary>The object's broken rules, and its children's: CSLA's tree of them.</summary>
    public IReadOnlyList<Issue> Issues
    {
        get
        {
            if (Vm.Model is null)
                return [];
            var issues = new List<Issue>();
            foreach (var node in BusinessRules.GetAllBrokenRules(Vm.Model))
            {
                string? where = ReferenceEquals(node.Object, Vm.Model) ? null : RowName(node.Object);
                foreach (var rule in node.BrokenRules.OrderBy(r => r.Severity))
                {
                    var info = rule.Property is { } property && node.Object is { } target && _byTarget.TryGetValue((target, property), out var found) ? found : null;
                    issues.Add(new Issue(rule.Severity, rule.Description, where, info));
                }
            }
            return issues.OrderBy(i => i.Severity).ToList();
        }
    }

    // a child's name in the issues list: its type's words and its place in its list (Line item 2)
    private static string RowName(object? row)
    {
        if (row is null)
            return "";
        string name = Formats.Words(row.GetType().Name);
        if (row is Csla.Core.BusinessBase { Parent: System.Collections.IList list } && list.IndexOf(row) is int index and >= 0)
            return name + " " + (index + 1);
        return name;
    }

    // ---- fields ----

    /// <summary>A property of the object: CSLA's PropertyInfo (its value, friendly name, rules' messages, CanRead,
    /// CanWrite, IsBusy).</summary>
    protected IPropertyInfo Info<P>(Expression<Func<P>> property) => Remember(Vm.GetPropertyInfo(property), property);

    public IPropertyInfo Info<P>(Expression<Func<P>> property, object row) =>
        Remember(Vm.GetPropertyInfo(property, _rowIds.GetValue(row, _ => "r" + ++_nextRowId)), property);

    private IPropertyInfo Remember<P>(IPropertyInfo info, Expression<Func<P>> property)
    {
        if (!_ids.ContainsKey(info))
        {
            var field = FieldIdentifier.Create(property);
            _byTarget[(field.Model, field.FieldName)] = info;
            _ids[info] = "f-" + field.FieldName + "-" + _ids.Count;
        }
        return info;
    }

    public bool Shows(IPropertyInfo info) => ShowAll || _touched.Contains(info);

    public bool Shows(object row, string? property)
    {
        if (ShowAll)
            return true;
        if (property is not null)
            return _byTarget.TryGetValue((row, property), out var info) && _touched.Contains(info);
        return _byTarget.Any(t => ReferenceEquals(t.Key.Item1, row) && _touched.Contains(t.Value));
    }

    public void Touch(IPropertyInfo info)
    {
        if (_touched.Add(info))
            StateHasChanged();
    }

    public string IdOf(IPropertyInfo info) => _ids.TryGetValue(info, out var id) ? id : "";

    public void Changed()
    {
        _edited = true;
        StateHasChanged();
    }

    private void ResetMessages()
    {
        _touched.Clear();
        ShowAll = false;
    }

    public async Task GoToAsync(Issue issue)
    {
        if (issue.Info is { } info)
        {
            Touch(info);
            await JS.InvokeVoidAsync("ui.focus", "#" + IdOf(info));
        }
    }

    // ---- saving, reverting, deleting, leaving ----

    public async Task SaveAsync()
    {
        if (Vm.Model is not { } model || Busy)
            return;
        if (!model.IsNew && !model.IsDirty)
        {
            Toasts.Info("Nothing to save", "No changes since it was saved.");
            return;
        }
        if (!model.IsValid)
        {
            ShowAll = true;
            int errors = Issues.Count(i => i.Severity == RuleSeverity.Error);
            Toasts.Warning(errors == 1 ? "Fix 1 issue to save" : $"Fix {errors} issues to save", "What's wrong is marked on the form.");
            StateHasChanged();
            await JS.InvokeVoidAsync("ui.focusFirstError");
            return;
        }
        if (!CanEdit)
        {
            Toasts.Error("You can't save this", $"{UserName} may not {(model.IsNew ? "create" : "edit")} a {Root.Singular.ToLowerInvariant()}.");
            return;
        }

        bool wasNew = model.IsNew;
        SaveProblem = null;
        await Vm.SaveAsync(); // a copy's saved and merged back (CSLA's GraphMerger): the page keeps its object
        if (Vm.Exception is { } e)
        {
            SaveProblem = Log(Problem.From(e, Root.Singular.ToLowerInvariant(), "saved", Environment.IsDevelopment()), e, "Saving");
            Toasts.Error(SaveProblem.Title, SaveProblem.Message);
            return;
        }

        ResetMessages();
        _edited = false;
        SavedAt = DateTime.Now;
        Toasts.Success(wasNew ? Root.Singular + " created" : "Changes saved", Name(Model));
        _ = Counts.RefreshAsync(Root);
        await OnSavedAsync();
        if (wasNew)
        {
            _loadedPath = Href(Model); // the same object, at its own address now
            Navigation.NavigateTo(_loadedPath, replace: true);
        }
    }

    public async Task DiscardAsync()
    {
        if (!HasChanges)
            return;
        if (!await Confirmations.AskAsync("Discard your changes?", IsNew ? "The form goes back to how it started." : $"{Name(Model)} goes back to how it was saved, its rows too.", "Discard", danger: true, icon: "undo"))
            return;
        Vm.Revert(); // n-level undo: CancelEdit, then BeginEdit again
        ResetMessages();
        SaveProblem = null;
        _edited = false;
        Toasts.Info("Changes discarded");
    }

    public async Task DeleteAsync()
    {
        if (Vm.Model is not { } model || model.IsNew || Busy)
            return;
        string name = Name(model);
        if (!await Confirmations.AskAsync($"Delete {name}?", $"It can't be undone.{(DeleteNote is null ? "" : " " + DeleteNote)}", "Delete", danger: true, icon: "trash"))
            return;
        _deleting = true;
        try
        {
            await DeleteAsync(model);
        }
        catch (Exception e)
        {
            SaveProblem = Log(Problem.From(e, Root.Singular.ToLowerInvariant(), "deleted", Environment.IsDevelopment()), e, "Deleting");
            Toasts.Error("It wasn't deleted", SaveProblem.Message);
            return;
        }
        finally
        {
            _deleting = false;
        }
        _leaving = true;
        Toasts.Success(Root.Singular + " deleted", name);
        _ = Counts.RefreshAsync(Root);
        Navigation.NavigateTo(Root.Href);
    }

    public void Cancel() => Navigation.NavigateTo(Root.Href);

    public async Task OnLeavingAsync(LocationChangingContext context)
    {
        if (_leaving || !HasChanges || Busy)
            return;
        string target = Navigation.ToBaseRelativePath(context.TargetLocation);
        if (target == _loadedPath)
            return;
        if (!await Confirmations.AskAsync("Leave without saving?", $"Your changes to {(IsNew ? "this new " + Root.Singular.ToLowerInvariant() : Name(Model))} aren't saved.", "Leave", danger: true))
            context.PreventNavigation();
    }

    public void DismissProblem() => SaveProblem = null;

    // What went wrong, in the log with its exception: an error where it's a fault (the database's, or another), a warning
    // where it's the user's to put right (a row that's gone, a key that's taken, a rule)
    private Problem Log(Problem problem, Exception exception, string doing)
    {
        _logger ??= LoggerFactory.CreateLogger(GetType());
        _logger.Log(problem.Unexpected ? LogLevel.Error : LogLevel.Warning, exception, "{Doing} {Path} failed: {Title}. {Message}",
            doing, CurrentPath(), problem.Title, problem.Message);
        return problem;
    }

    public void Dispose()
    {
        Vm.ModelPropertyChanged -= OnModelChanged;
        Vm.ModelChildChanged -= OnChildChanged;
        GC.SuppressFinalize(this);
    }
}
