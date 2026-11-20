# Tests and test data

Test data builders (Bogus) for each table, and xUnit integration tests for each table's repository. They're generated
with the [Dapper repositories](../DapperRepositories/README.md) they test, from the PetShop sample database
(`PetShop.db3`, SQLite) in this folder, and each test runs against a copy of it.

Databases: those of the [Dapper repositories](../DapperRepositories/README.md): SQL Server, SQLite, PostgreSQL and
MySQL (and SQL scripts in their dialects). Not Oracle: generating from one stops and says so.

```sh
cs generate TestData.csp
cs check TestData.csp
dotnet test --project Tests
```

## The builders

A builder makes rows that the database takes: `Data.Orders`, `Data.LineItems` and one for each table.

```csharp
var order = await Data.Orders.CreateAsync(cancellationToken: ct);           // saved; its OrderId is the database's
var line = await Data.LineItems.CreateAsync(order, cancellationToken: ct);  // in that order
var item = await Data.Items.CreateAsync(change: i => i with { Status = "P" }, cancellationToken: ct);
var values = Data.Products.Build();                                         // not saved
```

- **Each value fits its column:** its type, its length (`varchar(5)`), its precision and scale (`decimal(10,2)`) and
  whether it can be null. A column's name picks the kind of value where it says what it holds: `Email`, `FirstName`,
  `City`, `Zip`, `Phone`, `Image`, `Locale`. The rules are [Bogus](https://github.com/bchavez/Bogus) rules.
- **Keys and unique columns are new each time:** a code for `CategoryId varchar(10)`, a random part in front of a
  username. The database numbers an identity (`AUTOINCREMENT`, `IDENTITY`, `serial`) itself.
- **Foreign keys refer to rows that are there.** `CreateAsync` and `BuildAsync` make the rows a row refers to first
  (an item's product, and the product's category), or take the ones you give. With `Data.ReuseParents = true` they
  take rows the table has instead. A key that leads back to its own table (an employee's manager) takes a row that's
  there, or none. `Build()` makes values only, so its foreign keys refer to nothing.
- **The same seed makes the same rows:** `new TestData(connection, seed: 7)`. Dates are before `TestData.Today`.

| Method | What it does |
| --- | --- |
| `Build()` | A row's values, not saved. |
| `BuildAsync(parents, change)` | A row's values, not saved, with the rows it refers to (those are saved). |
| `CreateAsync(parents, change)` | A row saved in the database, as the database has it (its new key). |
| `ExistingAsync()` | One of the rows the table has, or null. |

## The tests

Each table gets a test class, `OrderRepositoryTests`, with a row of its own in each test:

- `InsertAsync` adds a row that `GetAsync` reads by its key (composite keys too: `LineItem`'s `OrderId, LineNum`),
  with every column as it was written.
- `UpdateAsync` changes every column but the key and the foreign keys.
- `DeleteAsync` removes the row. Then `GetAsync` finds nothing, and deleting or updating it again returns `false`.
- `GetAllAsync` lists one more row, and the new one is there.

A table without a primary key has only `GetAllAsync`, and gets a test that it reads. Each test gets a copy of
`PetShop.db3` in memory (`Tests/DatabaseTest.cs`), so they run side by side, and none sees another's rows.

## What's generated, and what's yours

| Folder | What it is |
| --- | --- |
| `Generated/Data/` | The records and repositories (the Dapper sample's templates). |
| `Generated/TestData.cs` | `TestData`, with a builder for each table, and what their rules share (`Fake`). |
| `Generated/Builders/` | A builder for each table. |
| `Generated/Tests/` | A test class for each table. |
| `Tests/` | Yours: the test project, `DatabaseTest` (what the tests derive from), `BuilderTests.cs`, and your changes to the builders in `Builders/`. |

Generating again writes `Generated/` and leaves `Tests/` alone. A table that's gone takes its builder and its tests
with it. Every generated class is `partial`, so a file of your own can add to it. A builder has two hooks for that:

- `Configure` changes the rules. `Tests/Builders/AccountBuilder.cs` gives emails at example.com, which no one
  receives:

  ```csharp
  public sealed partial class AccountBuilder
  {
      partial void Configure(Faker<Account> faker) =>
          faker.RuleFor(a => a.Email, f => f.Internet.Email(provider: "example.com"));
  }
  ```

- `OnBuilt` changes each row once its rules have run, so it sees the whole row. `Tests/Builders/OrderBuilder.cs`
  bills an order to its ship-to address.

Add tests to a generated class in a file of your own (`public sealed partial class OrderRepositoryTests`), or write
classes of your own that derive from `DatabaseTest`.

| File | What it is |
| --- | --- |
| `TestData.cst` | The master: renders `Builder.cst` and `RepositoryTests.cst` for each table, and is `TestData` itself. |
| `Builder.cst` | A table's builder. |
| `RepositoryTests.cst` | A table's tests. |
| `Fakes.cs` | What the test templates share (`<%@ Assembly Src="Fakes.cs" %>`): each column's rule, the foreign keys, and the names. |
| `Repositories.cst`, `Record.cst`, `Repository.cst`, `Names.cs` | The [Dapper repositories](../DapperRepositories/README.md), as that sample has them. |
| `codesmith.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
| `TestData.csp` | Generates both from the PetShop sample database in this folder (`PetShop.db3`, SQLite). |
| `global.json` | `dotnet test` runs xUnit v3 on Microsoft.Testing.Platform. |
| `Tests/SqliteTypes.cs` | Dapper type handlers for what SQLite stores loosely (the Dapper sample's). |

To test your own database, change the data source in `TestData.csp`, the namespaces, and `DatabaseTest`, which says
how a test gets its database: a copy of a SQLite file, or a database in a container, with a transaction per test that
rolls back. The SQL is the database's, as the Dapper sample's is (SQL Server, PostgreSQL, MySQL, SQLite).
