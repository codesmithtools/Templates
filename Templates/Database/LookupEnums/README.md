# Lookup enums

Enums from a lookup table's rows, so code says `Category.Fish`, not `"FISH"`. It reads the `Category` table of the
PetShop sample database (`PetShop.db3`, SQLite, in this folder) and writes a C# enum and a TypeScript const.

Databases: any `cs` reads (SQL Server, SQLite, PostgreSQL, MySQL, Oracle, and SQL scripts).

```sh
cs generate LookupEnums.csp
dotnet run --project App
```

- `Generated/Category.cs`: a member for each row, named from `Name`, with a `[Description("Birds")]`. `Category`'s key
  is text (`BIRDS`), so `CategoryExtensions` goes from a member to its code and back: `Category.Fish.ToCode()` is
  `"FISH"`, and `CategoryExtensions.TryParseCode("FISH", out var category)` gives `Category.Fish`. For an integer key,
  the members get the key's values instead (`New = 1`).
- `Generated/category.ts`: `export const Category = { Birds: "BIRDS", ... } as const`, a `Category` type of its codes,
  and `categoryNames`, the display names by code. A const object rather than a TypeScript `enum`: plain values.
- Names are made identifiers: `Back yard` is `BackYard`, `3D models` is `_3DModels`, a name twice gets a number
  (`Fish2`), and a row without one is named from its code. Rows are in value order.

The files are a copy of the table's rows: generate them again when the rows change. A table with no rows makes an
enum with no members, with a `#warning` in the C# (and a comment in the TypeScript) that says so.

| Property | What it is |
| --- | --- |
| `SourceTable` | The lookup table. |
| `ValueColumn` | The column the values come from. Default: the primary key. |
| `NameColumn` | The column the names and display text come from. Default: the first text column that isn't the key. |
| `EnumName` | Default: the table's name, singular, in Pascal case (`Categories`: `Category`). |
| `TargetNamespace` | The C# enum's namespace. |

| File | What it is |
| --- | --- |
| `CSharpEnum.cst` | The C# enum, and for a text key its `ToCode` and `TryParseCode`. |
| `TypeScriptEnum.cst` | The TypeScript const, its type, and the display names. |
| `Lookup.cs` | What the templates share (`<%@ Assembly Src="Lookup.cs" %>`): the columns, the rows, the names. |
| `LookupEnums.csp` | Generates `Generated/Category.cs` and `Generated/category.ts` from `PetShop.db3`. |
| `App/` | A console app that compiles the generated C#: it prints each category and parses a code back. |
| `codesmith.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |

For another table, change `<table>` and `output` in both of `LookupEnums.csp`'s property sets, or, in VS Code, pick
one for `SourceTable` in Template Properties.
