# Full stack

One database, four layers: EF Core entities, an ASP.NET Core minimal API, a TypeScript client and a web UI to browse and
edit every table. They're generated from the PetShop sample database (`PetShop.db3`, SQLite) in this folder, and stay in
step when the schema changes: generate again.

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
  amount, a date, a switch, or a searchable list of the rows a foreign key can refer to. The form checks what you type
  before it sends it, and shows what the API finds wrong beside the field it names.
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

A label is its column's name in words: `ShipAddr1` is "Ship address 1". A name all in one case, as PostgreSQL folds
them (`listprice`, `productid`), has no capitals to split at. It's split at the words the schema knows (its tables'
names, its other names' words) and a few common ones (`id`, `name`, `date`, `price`...): "List price", "Product ID".
`tables` in `customize.tsx` changes one that comes out wrong.

## What FullStack.csp generates

`FullStack.csp` has an output for each layer:

- **Entities** (`Generated/Data/`): an EF Core entity and its configuration per table, and the `DbContext`. These are
  the [ModernEntities](../../Database/ModernEntities/README.md) sample's templates.
- **Api** (`Generated/Api/`): for each table, a response record, create and update requests checked with
  `[MaxLength]` (minimal APIs' validation), and its endpoints at `/api/<table>`. The list is paged and sorted
  (`?sort=name&desc=true`). It searches text columns (`?search=bird`) and filters by a key's columns
  (`/api/items?productId=FI-01`), and it answers with the page's rows and how many there are in all. A row is read,
  updated or deleted by its key (`/api/line-items/{orderId}/{lineNum}`). A change the database refuses (a key that's
  taken, a row that others refer to) is a 409 with the database's message. `GET /api` says how many rows each table
  has. `PetShopApi.cs` maps them all (`app.MapPetShopApi()`).
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
| `NameWords.cs` | The words of a name all in one case, for its label (`listprice`: List price). Csla has the same copy. |
| `Web.cs` | What the API, the client and the UI share: routes, keys, requests, sorts, searches and filters, JSON names and TypeScript types. |
| `App/` | The API's project and `Program.cs` (yours): the database, validation, OpenAPI, `MapPetShopApi()`, and the UI. |
| `client/` | The client's and the UI's package (React, Zod, Vite, TypeScript), the UI's `index.html`, and what's yours in `src/`: `app/` and `example.ts`. |

To use your own database, change the data source in `FullStack.csp`, and the namespaces and names there. The Ui
output's `Exclude` leaves tables out of the UI (`aspnet_*`); the API still has them.

If you commit what's generated, `cs check FullStack.csp` in CI exits with code 3 when it's out of date (a changed
template, or a schema that moved on): see [CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/ci.md).
