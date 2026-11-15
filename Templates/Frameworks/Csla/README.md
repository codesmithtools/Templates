# CSLA .NET business objects

CSLA .NET business objects for each table, a DAL for them, its EF Core implementation and a Blazor web app over them.

```sh
cs generate Csla.csp
dotnet run --project App
dotnet build PetShop.Web
```

`App` shows them at work against a copy of the PetShop sample database (`PetShop.db3`, SQLite):

- lookups and lists;
- a new order and its lines, with the rules from the schema and from code of its own;
- the order saved in one transaction, then fetched, changed and saved again;
- a key that can't change once saved, and a delete that only an Admin may do.

`PetShop.Web` is a web app over them: run it with `dotnet run --project PetShop.Web`, and open <http://localhost:5180>.
It has a page for each root's list and one to edit it, with its children in grids (see [The web app](#the-web-app)).

CSLA 10 (9 too: set `CslaVersion`), .NET 10, nullable.

## What each table becomes

The keys say. A table whose key starts with another's (`LineItem`'s key is `OrderId, LineNum`), or whose rows go when
the other's are deleted (`ON DELETE CASCADE`), is that table's child. Every other table is a root.

| | Roots | Children |
| --- | --- | --- |
| Editable (`BusinessBase`) | `Order`: create, fetch by key, save (insert, update), delete | `LineItem`, in its order's `LineItems` (`BusinessListBase`) |
| Read-only (`ReadOnlyListBase`) | `OrderList` of `OrderInfo`: every row, or a foreign key's rows (`ProductList` by `categoryId`) | |
| Name/value (`NameValueListBase`) | `CategoryNameValueList`, where another table refers to it: its key and its name column | |

`PetShop.Business/Generated/Model.md` lists what each table became, and why.

- Properties are CSLA's `RegisterProperty`, `GetProperty` and `SetProperty`, nullable where the column is. A text
  column is a `string` either way (`""` for data binding, saved as `NULL` where the column takes it).
- The schema gives the rules: `[Required]`, `[StringLength(80)]`, and `[Range]` for a `decimal(10,2)`. CSLA checks them,
  with `[Display]` names in their messages ("The Ship city field is required.").
- The database's own columns (an identity, a computed column, a rowversion) are read-only, and saving loads them. A key
  the application sets can't change once it's saved (`CanWriteProperty`).
- A root saves with its children, and theirs, in one transaction. Deleting a root deletes its children's rows first.
- The data portal gets everything by dependency injection: `[Inject]` the DAL's interface, `IChildDataPortal<T>` for
  children. `AddCsla()` and `AddPetShopDal(options => options.UseSqlite(...))` (or `UseSqlServer`, `UseNpgsql`, ...)
  register it all.
- A rowversion is checked: a row someone else changed since it was read throws the DAL's `ConcurrencyException`.

There are four projects. Each one's `Generated/` folder holds what `Csla.csp` writes, and the rest is yours:

| Project | What's generated |
| --- | --- |
| `PetShop.Business` | The business objects (`Csla` package). They know the DAL's interfaces, not EF Core. |
| `PetShop.Dal` | A DTO (`record`) and an interface per table, transactions, and the DAL's exceptions. |
| `PetShop.Dal.EfCore` | EF Core: the entities, configurations and `DbContext` (as [ModernEntities](../../Database/ModernEntities/README.md) writes them), each table's DAL, `AddPetShopDal`. |
| `PetShop.Web` | Blazor pages (`Csla.Blazor`): each root's list and editor, each child's grid, and `Site.g.cs` (the roots, for the sidebar). |

## Make it yours: partial classes

Everything generated is a `partial` class with partial methods to fill in. Your file sits beside `Generated/`, and
generating again leaves it alone. `PetShop.Business/Order.cs` does all of these:

```csharp
public partial class Order
{
    public decimal LinesTotal => LineItems.Sum(line => line.Subtotal);       // members of your own

    partial void OnCreated() => OrderDate = DateTime.Today;                    // a new object's defaults
    partial void OnSaving() => TotalPrice = LinesTotal;                        // before an insert or update
    partial void OnAddBusinessRules()
    {
        BusinessRules.AddRule(new HasLines(LineItemsProperty));                // a rule of your own
        BusinessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, AuthorizationNumberProperty, "Admin"));
    }
    static partial void OnAddObjectAuthorizationRules(IAddObjectAuthorizationRulesContext context) =>
        Csla.Rules.BusinessRules.AddRule(typeof(Order), new IsInRole(AuthorizationActions.DeleteObject, "Admin"));
}
```

`LineItem.cs` adds a warning (more than 20 of an item): it shows, and the order still saves.

There are others:

- `OnFetched(dto)` and `OnSaved()` on editable objects.
- `OnFetched(dto)` on read-only objects, to load properties of your own.
- Lists take methods of your own: `LineItems.AddLineAsync` numbers a new line.
- A root's rules on a child list run when the list changes. CSLA alone doesn't run them then.
- The DAL's classes are partial too, with `OnInserting(dto, entity)` and `OnUpdating(dto, entity)`: set a column the
  objects don't have there.

`Csla.csp` settings:

| Setting | What it does |
| --- | --- |
| `Exclude` | Tables (`AuditLog`) and columns (`*.RowGuid`, `Orders.Notes`) to leave out; `*` and `?` match. |
| `Children` | Children the keys don't make: `Profiles: Account, Cart` here, as `Account` has its own identity key. |
| `Namespace`, `ContextName` | `PetShop`: the namespaces, the projects' names and `AddPetShopDal`; the `DbContext`'s name. |
| `CslaVersion`, `EfCoreVersion`, `TargetFramework` | Used when a project file is written (one that isn't there). |

## The web app

`PetShop.Web` is a Blazor Web App, interactive on the server, on the business objects and the EF Core DAL. It uses its
own copy of `PetShop.db3`, made at each start, and `DemoData.cs` adds a dozen orders to it.

- Each root has a list (`/orders`): search, sort and pages, and a filter for each fetch by a foreign key
  (`/products?categoryId=FISH` is `ProductList.FetchAsync("FISH")`). A row opens its editor.
- Its editor (`/orders/new`, `/orders/3`) is CSLA's `ViewModel<T>`: each field is bound to a `PropertyInfo` (its
  friendly name, `ErrorText`, `WarningText`, `InformationText`, `CanRead`, `CanWrite`), so a broken rule shows at its
  field as you type, and in the list beside the form. A foreign key is a combobox of its table's name/value list.
- Children are grids, edited in place, each row with its own rules and state (new, changed). A row with more columns
  than fit opens to show the rest, and its own children.
- Save waits for the rules (Ctrl+S); Discard is CSLA's n-level undo; Delete asks first. A concurrency or database error
  says what happened. Leaving with unsaved changes asks too.
- Who you are is the top bar's switcher (Admin or Clerk): it sets CSLA's `ApplicationContext.User`, and the pages ask
  CSLA again. As Clerk, an order has no Delete, and its authorization number is read-only.
- Ctrl+K goes anywhere, `/` searches, `N` is new, Esc cancels. Light and dark, and a phone's width.

`Program.cs` sets it up: `AddCsla(options => options.AddAspNetCore().AddServerSideBlazor())`, and `AddPetShopDal`.

### Make it yours

What's in `PetShop.Web/Generated/` is written again whenever `Csla.csp` runs. The rest is yours:

- `Components/` (the layout, the pages' components), `wwwroot/app.css` (the theme), `Program.cs` and the project file
  are written once, where they aren't there: restyle them, and they stay as you left them. Delete one, and it's written
  again.
- A generated page is a partial class. `Pages/OrderEditPage.cs` adds the order's total beside the form (`Stats`),
  `Pages/LineItemsGrid.cs` a subtotal column and a total (`Columns`, `Totals`), and `Pages/OrderListPage.cs` a column
  and two columns fewer (`MoreColumns`, `HiddenColumns`). A grid's `OnRowAdded` sets a new row's defaults.
- To replace a page, copy it from `Generated/` to `Pages/` (`Pages/OrderEditPage.razor`) and change it: generating again
  leaves yours, and stops writing its own.

## When the schema changes

Generate again. A new column is a property, with its rules, and a field. A new table gets its objects and its pages, and
a dropped one loses its files: the `.g.cs` and `.razor` files that weren't written this time are removed from
`Generated/`. Code of yours that used what's
gone stops compiling, so you know what to fix. `Model.md` changes with the model: review it with the diff.

In CI, `cs check Csla.csp` exits with 3 when what's committed isn't what the schema gives now (`--diff` shows it); see
[CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/ci.md). While you
work, `cs watch Csla.csp` generates whenever a template changes.

## Your own database

```
cs sample new csla MyShop
cd MyShop
```

In `Csla.csp`, set the data source (or pick one in the editor's property grid), `Namespace` (`MyShop`) and
`ContextName`, and delete `PetShop.Business/Order.cs`, `LineItem.cs` and `LineItems.cs`, and `PetShop.Web`: they belong
to PetShop. Then run `cs generate Csla.csp`. It writes `MyShop.Business`, `MyShop.Dal`, `MyShop.Dal.EfCore` and
`MyShop.Web`, each with its project file, and `App` is a start for yours (`AddMyShopDal` and the provider's package).
`MyShop.Web` copies a SQLite database as `PetShop.Web` does; another database's connection string is
`ConnectionStrings:MyShop` (its package is in the project file).

The generated code builds from SQLite, SQL Server, PostgreSQL and MySQL; the tests build it from each one's PetShop.

| File | What it is |
| --- | --- |
| `Csla.cst` | The master: every template for every table, stale files removed, project files and the web app's shell written once, and `Model.md`. |
| `Editable.cst`, `ChildList.cst` | An editable root or child, and a child's list. |
| `Info.cst`, `List.cst`, `NameValueList.cst` | A root's read-only object and list, and its name/value list. |
| `Dal.cst`, `DalCommon.cst` | A table's DTO and DAL interface; the transactions and exceptions every table's DAL shares. |
| `EfDal.cst`, `EfCommon.cst` | The EF Core DAL; its transactions, save and `AddPetShopDal`. |
| `Entity.cst`, `Configuration.cst`, `Context.cst`, `Names.cs` | The entities, as ModernEntities has them. |
| `Model.cs` | The object model the templates share: roots, children, lists, names, types and rules. |
| `Project.cst` | A project's file, when there isn't one. |
| `Web/ListPage.cst`, `Web/EditPage.cst`, `Web/ChildGrid.cst` | A root's list and editor, a child's grid. |
| `Web/Site.cst`, `Web/Imports.cst` | The roots for the sidebar, home page and palette; the pages' namespace. |
| `Web/Web.cs` | What the web templates share: routes, titles, icons, which input a column gets, sections, list columns. |
| `Web/Program.cst`, `Web/Shell/` | The web app's `Program.cs` and its shell (layout, components, theme): written once. |
| `App/`, `PetShop.Business/*.cs` | The demo, and Order's, LineItem's and LineItems' own code (yours). |
| `PetShop.Web/DemoData.cs`, `PetShop.Web/Pages/*.cs` | The web app's demo orders, and its pages' own code (yours). |

8.x's CSLA templates (CSLA 3.8 to 4.5) are in [Legacy](../../Legacy/README.md).
