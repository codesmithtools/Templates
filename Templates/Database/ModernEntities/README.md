# Modern entities: EF Core, nullable

EF Core entities and a `DbContext` for a database. An entity and its configuration per table, and the `DbContext`,
from the database's schema.

- Entities are `partial` classes with `get; set;` properties: nullable reference types, `required` where a row must
  set a value, a navigation to each row a foreign key refers to, and a collection of the rows that refer to it.
- Names are .NET's: `order_id` is `OrderId` (with `HasColumnName("order_id")`), `Orders` is `Order` in a `DbSet` named
  `Orders`, and a table name that two schemas have gets its schema in front. A table named like a type the generated
  code uses (`Task`, `Guid`, `File`, `Type`) gets `Entity` after it: `TaskEntity`, in a `DbSet` named `Tasks`.
- A foreign key's property has the type of the key it refers to (an SQLite `integer` key is a `long`, and so are the
  columns that refer to it).
- A column with no .NET type (SQL Server's `geography`, `geometry`, `hierarchyid` or `sql_variant`, PostgreSQL's `point`
  or a range) has no property, but a comment where it would be: EF Core can't map an `object`, and the whole model
  would fail (its scaffolding leaves such columns out too). Map one in a partial class of your own (with
  `Microsoft.SqlServer.Types` or NetTopologySuite, say). A PostgreSQL enum is a `string`: it reads, but Npgsql sends a
  string as `text`, so writing one needs the enum mapped (Npgsql's `MapEnum`) in a partial class.
- Each entity's configuration (an `IEntityTypeConfiguration<T>`) maps it to its table (with its schema, unless it's
  the database's default), its key (composite keys too, in the table's column order; `HasNoKey` without one), its relationships
  (`HasPrincipalKey` for one to a unique key that isn't the primary key), a key the application numbers
  (`ValueGeneratedNever`), a rowversion and a `jsonb` column. The context applies them all.
- The partial classes take your own members, `ConfigurePartial` an entity's own mapping and `OnModelCreatingPartial`
  the model's, without editing generated code.

| File | What it is |
| --- | --- |
| `Entities.cst` | The master: renders `Entity.cst` and `Configuration.cst` for each table and `Context.cst` once, into `OutputDirectory`. |
| `Entity.cst` | One table's class. Types come from `column.GetCSharpTypeName()` (`DateOnly`, `int?`, `string?`). |
| `Configuration.cst` | One table's `IEntityTypeConfiguration<T>`: its table, key, columns and relationships. |
| `Context.cst` | The `DbContext`: a primary constructor, a `DbSet` per table, and `OnModelCreating` applying the configurations. |
| `Names.cs` | What the templates share (`<%@ Assembly Src="Names.cs" %>`): names, types and relationships. |
| `codesmith.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
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
