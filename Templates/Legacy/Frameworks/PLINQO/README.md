# PLINQO for LINQ to SQL

CodeSmith Generator 8.x's PLINQO templates, kept as they were (see [Legacy samples](../../README.md)). From a SQL Server
database they generate a LINQ to SQL data layer for .NET Framework 3.5 to 4.5: a `.dbml`, two files per entity, managers
and queries. They run in CodeSmith Generator 9.0 on Windows, Linux and macOS.

| Template | What it writes |
| --- | --- |
| `CSharp/Dbml.cst` (`VisualBasic/`) | The `.dbml` from the database. Your changes to it (names, associations) are kept. |
| `CSharp/Entities.cst` | From the `.dbml`: the data context, and for each entity `Product.Generated.cs` (written again every time) and `Product.cs` (yours; only its `Metadata` class is generated). |
| `CSharp/Managers.cst`, `CSharp/Queries.cst` | Managers and query extensions. |
| `QuickStart.cst` | 8.x's Visual Studio solution, with a project (`Common/QuickStartData.csp`) that runs the templates above. |

Run a project with `cs generate PetShop.Data.csp`, and check that the committed code is up to date with
`cs check PetShop.Data.csp`. To generate in CI (the connection string, Windows authentication, schema snapshots,
building the output on Linux), see
[framework templates in CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/moving-from-8x.md#framework-templates-plinqo-csla-nhibernate-in-ci).

## The Metadata class

`Product.cs` is yours: everything in it stays as you wrote it, except the `Metadata` class in its `#region Metadata`,
which Entities.cst writes again on every run. Its members are the entity's columns and associations, so the attributes
you put on them (validation, display, auditing) apply to the entity's properties:

```csharp
#nullable enable
namespace PetShop.Data;

public partial class Product
{
    // Place custom code here.
    public string Title => Name ?? ProductId;

    #region Metadata
    [CodeSmith.Data.Audit.Audit]
    internal class Metadata
    {
        // WARNING: Only attributes inside of this class will be preserved.

        [Required(ErrorMessage = $"{nameof(ProductId)} is required.")]
        [StringLength(10, MinimumLength = 2)]
        public string ProductId { get; init; } = "";

#if DEBUG
        [UIHint("DebugName")]
#endif
        [Display(Name = "Product name")] // shown in the grid
        public string? Name { get; set; } = "";

        public decimal? ListPrice { get; set; }

        [ScaffoldColumn(false)]
        public EntitySet<Item> ItemList { get; set; } = [];

    }

    #endregion
}
```

When it's generated again:
- A member you have is kept as you wrote it: its attributes (with any `#if`, `#elif`, `#else` and `#endif` lines around
  them, and comments between them) and its declaration (`?`, `init`, `required`, `=>`, an initializer), so a file with
  nullable annotations builds as it did. Its type isn't updated when the column's changes: the members are there for
  their attributes, and their types don't matter.
- A new column or association gets a member as 8.x wrote it: `public string Status { get; set; }`, with `[Required]`,
  `[DataType(...)]` or `[Now(...)]` where the column calls for one. In a file with `#nullable enable`, give it a `?` or
  an initializer, or the compiler warns (CS8618); it's kept from then on.
- A member for a column or association that's gone is removed, and so is anything else in the class: put other code
  outside it.
- The class is written at the indentation of the one it replaces, so a file with a file-scoped namespace, or one
  `dotnet format` has formatted, stays as it is: `cs check` and `dotnet format` agree on it.
- An `#if` around a whole member, not only its attributes, isn't kept: put the attributes in the `#if` instead.
- Everything outside the class can be any C# (or VB) the compiler takes: file-scoped namespaces, records, `=>` members,
  raw and interpolated strings.

8.x kept only the attributes, each on its own line, wrote every member as `public T Name { get; set; }`, and kept an
attribute inside `#if` without its `#if` lines (making a debug-only attribute unconditional).

In VB (`Product.vb`), the attributes are kept as written; each member is written as 8.x wrote it
(`Public Property Name() As Object`). VB doesn't allow `#If` between attributes and their member.

The generated files' comment points to `http://www.plinqo.com/metadata.ashx`, which is gone: this page replaces it.
