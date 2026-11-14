# Modern entities: EF Core, nullable

Templates for today's .NET: an EF Core entity per table and a `DbContext`, shaped as EF Core's scaffolding writes them,
from a database's schema.

- Entities are `partial` classes with `get; set;` properties: nullable reference types, `required` where a row must
  set a value, a navigation to each row a foreign key refers to, and a collection of the rows that refer to it.
- Names are .NET's: `order_id` is `OrderId` (with `HasColumnName("order_id")`), `Orders` is `Order` in a `DbSet` named
  `Orders`, and a table name that two schemas have gets its schema in front.
- A foreign key's property has the type of the key it refers to (an SQLite `integer` key is a `long`, and so are the
  columns that refer to it).
- The context maps each entity to its table (with its schema, unless it's the database's default), its key (composite
  keys too; `HasNoKey` without one), its relationships (`HasPrincipalKey` for one to a unique key that isn't the
  primary key), a key the application numbers (`ValueGeneratedNever`), a rowversion and a `jsonb` column.
- `OnModelCreatingPartial` and the partial classes take your own configuration and members without editing generated
  code.

| File | What it is |
| --- | --- |
| `Entities.cst` | The master: renders `Entity.cst` for each table and `Context.cst` once, into `OutputDirectory`. |
| `Entity.cst` | One table's class. Types come from `column.GetCSharpTypeName()` (`DateOnly`, `int?`, `string?`). |
| `Context.cst` | The `DbContext`: a primary constructor, a `DbSet` per table, and `OnModelCreating`. |
| `Names.cs` | What the templates share (`<%@ Assembly Src="Names.cs" %>`): names, types and relationships. |
| `codesmith.options.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
| `Entities.csp` | Generates from the PetShop sample database in this folder (`PetShop.db3`, SQLite). |

For IntelliSense in `Names.cs`, run `cs new code-project` here (an editor-only `CodeSmith.Templates.csproj`).

Run it from this folder:

```sh
cs generate Entities.csp
```

The files go to `Generated/`. To use your own database, change the data source in `Entities.csp` (or pick one in the
editor's property grid), and `Namespace` and `ContextName`.

The generated project needs the EF Core package for the database (`Microsoft.EntityFrameworkCore.Sqlite`,
`Npgsql.EntityFrameworkCore.PostgreSQL`, ...). The tests generate the example, compile it against EF Core, build its
model and read every table of PetShop with it.
