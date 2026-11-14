# Dapper repositories

A C# record and an async Dapper repository for each table. No ORM: the repositories run SQL written for the
database. `Repositories.csp` reads the PetShop sample database (`PetShop.db3`, SQLite) in this folder, and `App/` runs
the generated code against it.

```sh
cs generate Repositories.csp
dotnet run --project App
```

- A record has an `init` property for each column, with its C# type (`column.GetCSharpTypeName()`), nullable where the
  column is and `required` where a new row must set it. Names are .NET's: `Orders` is `Order`, `order_id` is `OrderId`.
- A repository takes a `DbConnection`, and has `GetAllAsync`, `GetAsync` by primary key (composite keys too),
  `InsertAsync`, `UpdateAsync` and `DeleteAsync`. A table without a primary key gets only `GetAllAsync`.
- The SQL is the database's: parameterized, every identifier quoted (`[x]` for SQL Server, `"x"` for PostgreSQL and
  SQLite, `` `x` `` for MySQL), and a column aliased to its property where the names differ (`"order_id" AS "OrderId"`).
- An insert leaves out a key the database numbers (an identity, `serial`, `auto_increment` or SQLite's `AUTOINCREMENT`)
  and returns it: `RETURNING` (PostgreSQL, SQLite), `SCOPE_IDENTITY()` (SQL Server) or `LAST_INSERT_ID()` (MySQL).
- The repositories are `partial` classes: add your own queries in a file of your own.

| File | What it is |
| --- | --- |
| `Repositories.cst` | The master: renders `Record.cst` and `Repository.cst` for each table, into `OutputDirectory`. |
| `Record.cst` | One table's record. |
| `Repository.cst` | One table's repository, and its SQL. |
| `Names.cs` | What the templates share (`<%@ Assembly Src="Names.cs" %>`): C# names, and SQL names quoted for the database. |
| `codesmith.options.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
| `Repositories.csp` | Generates from the PetShop sample database in this folder (`PetShop.db3`, SQLite). |
| `App/` | A console app that compiles `Generated/` and runs it: it reads `PetShop.db3`, and writes to a copy in memory. |
| `App/SqliteTypes.cs` | Dapper type handlers for what SQLite stores loosely: decimals (as `REAL` or `INTEGER`) and dates (as text). |

For IntelliSense in `Names.cs`, run `cs new code-project` here (an editor-only `CodeSmith.Templates.csproj`).

To use your own database, change the data source in `Repositories.csp` (or pick one in the editor's property grid), and
`Namespace`. The generated code needs Dapper and the database's ADO.NET provider (`Microsoft.Data.SqlClient`, `Npgsql`,
`MySqlConnector`, `Microsoft.Data.Sqlite`).
