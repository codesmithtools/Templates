// What every field shares: CSLA's PropertyInfo for its property (its value, its friendly name from [Display], its rules'
// messages, CanRead, CanWrite, IsBusy) and the editor it's in. TextField, NumberField, DateField, SwitchField,
// LookupField and ReadOnlyField inherit it. Csla.cst wrote this file once: it's yours.
using Csla.Blazor;
using Microsoft.AspNetCore.Components;

namespace PetShop.Web.Components.Ui;

public abstract class FieldBase : ComponentBase
{
    [CascadingParameter] public IEditor? Editor { get; set; }

    /// <summary>The property: <c>Info(() => Model.ShipCity)</c>.</summary>
    [Parameter, EditorRequired] public IPropertyInfo Info { get; set; } = default!;

    /// <summary>The label; CSLA's friendly name ([Display]) otherwise.</summary>
    [Parameter] public string? Label { get; set; }

    [Parameter] public string? Help { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool Required { get; set; }

    /// <summary>How many of a section's six columns it takes.</summary>
    [Parameter] public int Span { get; set; } = 2;

    /// <summary>In a grid: no label, its messages under the row.</summary>
    [Parameter] public bool Compact { get; set; }

    /// <summary>A key the app sets: it can't change once saved (CanWrite says so then).</summary>
    [Parameter] public bool KeySetOnce { get; set; }

    public string LabelText => Label ?? Info.FriendlyName;
    public string Id => Editor?.IdOf(Info) is { Length: > 0 } id ? id : "f-" + Info.PropertyName;
    public string MessageId => Id + "-message";
    public bool CanRead => Info.CanRead;
    public bool CanWrite => Info.CanWrite && Editor?.ReadOnly != true;
    public bool Shows => Editor?.Shows(Info) ?? true;

    /// <summary>A problem of the field's own (text that isn't a number), before CSLA's rules.</summary>
    public string? InputError { get; protected set; }

    public string ErrorText => InputError ?? (Shows ? Info.ErrorText : "");
    public string WarningText => Shows ? Info.WarningText : "";
    public string InformationText => Shows ? Info.InformationText : "";
    public bool Invalid => ErrorText.Length > 0;

    public string LockReason => !Info.CanWrite
        ? KeySetOnce ? "Part of its key: it can't change once saved." : "You may not change this."
        : "Read-only for you.";

    public string StateClass => Invalid ? "error" : WarningText.Length > 0 ? "warning" : InformationText.Length > 0 ? "info" : "";

    protected void Touch() => Editor?.Touch(Info);

    /// <summary>The property's type, without Nullable.</summary>
    protected Type ValueType => Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;

    protected bool IsNullable => !PropertyType.IsValueType || Nullable.GetUnderlyingType(PropertyType) != null;

    private Type PropertyType => Info.GetPropertyInfo()?.PropertyType ?? typeof(string);
}
