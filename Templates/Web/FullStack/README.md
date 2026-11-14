# Full stack

One database, three layers: EF Core entities, an ASP.NET Core minimal API and a TypeScript client. They're generated
from the PetShop sample database (`PetShop.db3`, SQLite) in this folder, and stay in step when the schema changes:
generate again.

```sh
cs generate FullStack.csp
dotnet build App
cd client
npm install --no-audit --no-fund
npx tsc --noEmit
```

Run the API with `dotnet run --project App` (its OpenAPI document is at <http://localhost:5000/openapi/v1.json>), and
the client's example beside it with `npm start` in `client/`.

`FullStack.csp` has an output for each layer:

- **Entities** (`Generated/Data/`): an EF Core entity and its configuration per table, and the `DbContext`. These are
  the [ModernEntities](../../Database/ModernEntities/README.md) sample's templates.
- **Api** (`Generated/Api/`): for each table, a response record, create and update requests checked with
  `[MaxLength]` (minimal APIs' validation), and its endpoints at `/api/<table>`. The list is paged, and a row is read,
  updated or deleted by its key (`/api/line-items/{orderId}/{lineNum}`). `PetShopApi.cs` maps them all
  (`app.MapPetShopApi()`).
- **Client** (`client/src/generated/petshop.ts`): a Zod schema and a type for each table's rows and requests, and
  `createPetShopClient`, with a typed call per endpoint that checks what it sends and what comes back.

| File | What it is |
| --- | --- |
| `Entities.cst`, `Entity.cst`, `Configuration.cst`, `Context.cst`, `Names.cs` | The entities (as ModernEntities has them). |
| `Api.cst`, `Endpoints.cst` | The API: `Endpoints.cst` for each table, and the class that maps them all. |
| `Client.cst` | The TypeScript client. |
| `Web.cs` | What the API and the client share: routes, keys, requests, JSON names and TypeScript types. |
| `App/` | The API's project and `Program.cs` (yours): the database, validation, OpenAPI, `MapPetShopApi()`. |
| `client/` | The client's package (zod, TypeScript) and `src/example.ts` (yours). |

To use your own database, change the data source in `FullStack.csp`, and the namespaces and names there.

If you commit what's generated, `cs check FullStack.csp` in CI exits with code 3 when it's out of date (a changed
template, or a schema that moved on): see [CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/ci.md).
