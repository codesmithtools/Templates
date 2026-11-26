# Full stack

One database, four layers: EF Core entities, an ASP.NET Core minimal API, a TypeScript client and a web UI to browse and
edit every table. They're generated from the PetShop sample database (`PetShop.db3`, SQLite) in this folder, and stay in
step when the schema changes: generate again.

Databases: it generates from any database `cs` reads (SQL Server, SQLite, PostgreSQL, MySQL, Oracle, and SQL
scripts). `App/Program.cs` opens `PetShop.db3` with `UseSqlite`: for another database, see [Your own
database](#your-own-database).

```sh
cs generate FullStack.csp
dotnet build App
cd client
npm install --no-audit --no-fund
npx tsc --noEmit
npm run build
```

Run it with `dotnet run --project App` and open <http://localhost:5000>. The API serves the UI (`npm run build` put it
in `App/wwwroot`), and its OpenAPI document is at <http://localhost:5000/openapi/v1.json>.

To work on the UI, run `npm run dev` in `client/` while the API runs. Vite serves the UI at <http://localhost:5173>,
updates it as you change it, and passes `/api` on to the API. The client's example runs with `npm start` in `client/`.

## The web UI

A React app over the generated client, built with Vite. It has:

- An overview: how many rows each table has, the tables by size, and the newest rows.
- A list for each table, paged by the API. Sort it by a column, search it, and see what each row refers to by name.
- A page for each row: its fields, the rows it refers to, and the rows of other tables that refer to it (a product's
  items), with links to all of them and a button to add one.
- A form to make or change a row. Each column gets the input its type takes: text with a character count, a number, an
  amount, a date, a switch, a select of an enum's values (its values to pick from, for a MySQL SET or an array of an
  enum), a list's items (a PostgreSQL array), a bit string's 0s and 1s, or a searchable list of the rows a foreign key
  can refer to. A new row starts with each column's default where it's a value (`DEFAULT 'low'`, `DEFAULT true`), and
  the time for one that's `now()`; a column the database has a default for may be left empty, and gets it. The form
  checks what you type before it sends it, and shows what the API finds wrong beside the field it names: a value that
  isn't valid, or a key another row already has. A delete the database refuses says why in its toast, and so does a
  search that fails.
- Delete (it asks first), toasts, light and dark themes, and a command palette (Ctrl+K or ⌘K) that finds rows in every
  table.
- Keys: `/` searches, `N` makes a row, `J` and `K` move through a list, `Enter` opens a row, `E` edits it, `Ctrl+S`
  saves, `Esc` cancels, and `?` lists them all.

What's generated, and what's yours:

- Generated, in `client/src/generated/ui/`: a file for each table in `tables/` (its fields, with their labels, kinds of
  input, limits and keys; what it refers to and what refers to it; its calls; its pages), and `index.ts` (the tables in
  the navigation's order, and the routes to their pages). They're written again each time you generate. A table's file
  goes when its table does.
- Yours, in `client/src/app/`: the shell (`App.tsx`), the theme (`theme.css`, `app.css`), and the kit (`kit/`) with the
  pages, the inputs and the other components. Generating again doesn't touch them, so restyle and change them as you
  like.

### Customizing the generated UI

`client/src/app/customize.tsx` changes what the generated files show, so your changes outlive the next generate. Its
keys are a table's name in the UI (its route: `products`, `line-items`) and a field's JSON name or a page:

- `tables` changes a table's definition: its title, icon, list columns, or a field's label. The sample takes the
  image out of the products' list.
- `cells` replaces a field's value in lists and on a row's page. The sample shows an item's list price with its margin.
- `inputs` replaces a field's input on the forms.
- `pages` replaces a whole page: `products.detail` (or `list`, `create`, `edit`).

For anything else, change the kit.

A label is its column's name in words: `ShipAddr1` is "Ship address 1", and an acronym is in capitals (`IsVip` is
"VIP": a bool's label leaves out its Is). A foreign key's label is the table it refers to where its column is named for
it (`ProductId`: "Product"), and its column's otherwise (`ParentId`: "Parent", `ReferredBy`: "Referred by"). A row is
shown by its name (a Name, a Title, a first and last name, an Email), else by a unique text column (`Key`, `Code`),
else by its key. A name all in one case, as PostgreSQL folds them (`listprice`, `productid`), has no capitals to split
at. It's split at the words the schema knows (its tables' names, its other names' words) and a few common ones (`id`,
`name`, `date`, `price`...): "List price", "Product ID". Csla's labels are the same (`NameWords.cs`). `tables` in
`customize.tsx` changes one that comes out wrong.

## What FullStack.csp generates

`FullStack.csp` has an output for each layer:

- **Entities** (`Generated/Data/`): an EF Core entity and its configuration per table, and the `DbContext`. These are
  the [ModernEntities](../../Database/ModernEntities/README.md) sample's templates.
- **Api** (`Generated/Api/`): for each table, a response record, create and update requests checked with
  `[MaxLength]`, an enum's `[AllowedValues]` and a bit string's pattern (minimal APIs' validation: a 400 that names the
  field), and its endpoints at `/api/<table>`. The list is paged and sorted (`?sort=name&desc=true`). It searches text
  columns (`?search=bird`) and filters by a key's columns (`/api/items?productId=FI-01`), and it answers with the page's
  rows and how many there are in all. A row is read, updated or deleted by its key
  (`/api/line-items/{orderId}/{lineNum}`); a key's last column of text takes the rest of the path, so a key with a `/`
  works (`/api/settings/a%20b/c`). A new row may leave out a column the database has a default for (`CreatedAt
  DEFAULT now()`): the database gives it. A PostgreSQL array is a list in JSON, and a bit string (`bit(8)`, MySQL's
  `BIT(8)`) text of 0s and 1s. `GET /api` says how many rows each table has. `PetShopApi.cs` maps them all
  (`app.MapPetShopApi()`).
  - A change the database refuses is a 409 that says why, for people, and names the fields it's about: "Another
    category already has this category ID." (`errors.categoryId`), "There's no such category.", or "Products refer to
    this category. Delete or change them first." `refusal` says which: `unique`, `reference`, `referenced` or `other`.
    SQLite, SQL Server, PostgreSQL and MySQL each say it with a code of their own, which the API reads.
  - The database's own message (`database`) is there only in Development: it can name tables, columns and values.
  - Any column's name works. A column keeps its name in the entity, the records and the JSON; where it's named like
    a member a record has (`From`, `ToEntity`, `ApplyTo`, or the record itself), the member gives way (`FromEntity`,
    `CreateEntity`, `ApplyToEntity`, `TripResponseRecord`). A column named like its table is the one exception: its
    property, and so its JSON name, has `Value` after it (`Request.Request` is `requestValue`).
  - Any table's name works too. Where a table's entity is named like a type FullStack generates, the generated type
    gives way, and a warning says so: a table named `ProductResponse` makes Product's response record
    `ProductResponseRecord`, one named `SaveProblem` makes the API's `SaveProblemRecord`, and one named
    `ProductConfiguration` makes Product's EF Core configuration `ProductEntityConfiguration`. The API's code names
    each entity by its namespace. A key's or a filter's parameter named like a C# or
    TypeScript keyword, or like one the code around it has (`db`, `request`, `row`; a list's `page`, `sort`, `items`,
    the UI's `q`), gets `Value` after it (a filter: `Filter`): `classValue`, `dbValue`, `?pageFilter=2`.
- **Client** (`client/src/generated/petshop.ts`): a Zod schema and a type for each table's rows and requests, and
  `createPetShopClient`, with a typed call per endpoint that checks what it sends and what comes back.
- **Ui** (`client/src/generated/ui/`): the web UI's tables and routes.

| File | What it is |
| --- | --- |
| `Entities.cst`, `Entity.cst`, `Configuration.cst`, `Context.cst`, `Names.cs` | The entities (as ModernEntities has them). |
| `Api.cst`, `Endpoints.cst` | The API: `Endpoints.cst` for each table, and the class that maps them all. |
| `Client.cst` | The TypeScript client. |
| `Ui.cst`, `UiTable.cst`, `UiIndex.cst` | The web UI: `UiTable.cst` for each table, and `UiIndex.cst` for the routes. |
| `Ui.cs` | What the UI knows of a column and a table: labels, kinds of input, what a row is shown by, icons. |
| `NameWords.cs` | Labels: a name's words (`listprice`: List price; `IsVip`: VIP). Csla has the same copy, so its labels are the same. |
| `Web.cs` | What the API, the client and the UI share: routes, keys, requests, sorts, searches and filters, JSON names and TypeScript types. |
| `App/` | The API's project and `Program.cs` (yours): the database, validation, OpenAPI, `MapPetShopApi()`, and the UI. |
| `client/` | The client's and the UI's package (React, Zod, Vite, TypeScript), the UI's `index.html`, and what's yours in `src/`: `app/` and `example.ts`. |

`Exclude` in `FullStack.csp` leaves tables out of every output: no entity, endpoints, client calls or pages
(`sysdiagrams, __EFMigrationsHistory` by default: EF Core's migration history has no API to delete it with). Add
yours, separated by commas; `*` and `?` match any characters (`aspnet_*`).

## Your own database

1. In `FullStack.csp`, change `SourceDatabase`'s connection string and provider to your database's (or, in VS Code,
   pick a data source for it), and add the tables to leave out to `Exclude`.
2. In `App/PetShop.Api.csproj`, change the EF Core provider's package, and remove the `PetShop.db3` that's copied
   beside the app:

   | Database | Package | In `Program.cs` |
   | --- | --- | --- |
   | SQL Server | `Microsoft.EntityFrameworkCore.SqlServer` | `UseSqlServer` |
   | PostgreSQL | `Npgsql.EntityFrameworkCore.PostgreSQL` | `UseNpgsql` |
   | MySQL | `MySql.EntityFrameworkCore` (Oracle's: Pomelo's has no EF Core 10 release) | `UseMySQL` |
   | SQLite | `Microsoft.EntityFrameworkCore.Sqlite` | `UseSqlite` |

3. In `App/Program.cs`, open it with that provider and your connection string, from `App/appsettings.json`'s
   `ConnectionStrings` or `dotnet user-secrets` (not `PetShop.db3`'s path):

   ```csharp
   builder.Services.AddDbContext<PetShopContext>(options => options.UseNpgsql(
       builder.Configuration.GetConnectionString("PetShop") ?? throw new InvalidOperationException("No connection string PetShop.")));
   ```

4. `cs generate FullStack.csp`, read its warnings (what the app can't do with your schema), then build and run as above.
   `client/src/example.ts` names no table, so it runs on any database's.

To rename `PetShop` too, change `Namespace`, `DataNamespace`, `ContextName` and `ApiName` in `FullStack.csp` (and the
Client output's `petshop.ts`, with the Ui output's `Client`, which names it), then what refers to them:

- `App/Program.cs`: `using PetShop.Api` and `using PetShop.Data`, `PetShopContext`, `MapPetShopApi()`, and the connection
  string's name;
- `App/PetShop.Api.csproj`: its `RootNamespace` (and its file's name, if you like);
- `client/src/app/api.ts` and `client/src/example.ts`: `createPetShopClient` and the file they import it from;
- `client/package.json`: its `name`.

## Supported, and not

What the app does with what a schema has. `cs generate` warns of each of these it finds in yours (CSG0007).

| The schema has | The app |
| --- | --- |
| A table without a primary key | Lists its rows; no get, create, update or delete, and the UI shows it read-only. |
| A view | Leaves it out: no API, no pages. |
| A table named like a type FullStack generates (`ProductResponse`, `SaveProblem`, `ProductConfiguration`) | Works: the generated type gives way (`ProductResponseRecord`). |
| An enum (MySQL's `ENUM`, PostgreSQL's), MySQL's `SET` | A select (a SET: its values to pick from); the API checks the value (400). |
| A PostgreSQL array (`text[]`, `int[]`, an enum's) | A list in JSON, and a list's input. An array of two dimensions isn't: one such row makes its table's list fail. |
| A bit string (PostgreSQL's `bit(n)`, `bit varying`; MySQL's `BIT(n)`) | Text of 0s and 1s. `BIT(1)` and PostgreSQL's `bit(1)` are a bool. |
| A column with a default (`DEFAULT now()`, `DEFAULT 'low'`) | A new row may leave it out, and the database gives it; the form starts with it where it's a value. |
| A `timestamptz` (MySQL's `TIMESTAMP`, SQL Server's `datetimeoffset`) | Sent in UTC (or with its offset); the form shows and sends it in the browser's time. |
| A column without a .NET type (PostgreSQL's `point`, SQL Server's `geography`) | Leaves it out of the entity, the API and the UI. |
| A column of a type the API and the UI don't know (PostgreSQL's `inet`, a range) | Takes it as text: check it reads and saves. |
| A key with a `/` in it | Works where it's the key's last column of text (`{**key}`); not in a key's other columns. |
| A `BIGINT UNSIGNED` (or `bigint`) key past 2^53 | Loses precision in JavaScript: the UI can't open that row. |

If you commit what's generated, `cs check FullStack.csp` in CI exits with code 3 when it's out of date (a changed
template, or a schema that moved on): see [CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/ci.md).
