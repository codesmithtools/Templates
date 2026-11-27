// Using the generated client: the API's answers are checked against their schemas, and so are the requests. Node runs
// it as it is (npm start), with the API running (dotnet run --project App). It names no table, so it runs on any
// database's: how many rows each table has, and the first table's first rows. A table's calls are typed by its schema:
// petShop.products.list({ sort: "name" }), .get(key), .create(request), .update(key, request), .delete(key).
import { createPetShopClient } from "./generated/petshop.ts";

const petShop = createPetShopClient({ baseUrl: process.env.PETSHOP_API ?? "http://localhost:5000" });

for (const { table, rows } of await petShop.rowCounts()) {
  console.log(`${table}: ${rows} ${rows === 1 ? "row" : "rows"}`);
}

// every table has a list call (its rows are checked against the table's schema as they come back)
interface Calls {
  list(options: { pageSize?: number }): Promise<{ items: unknown[]; total: number }>;
}
const first = Object.entries(petShop).find(([, calls]) => typeof calls === "object");
if (first) {
  const [name, calls] = first;
  const page = await (calls as unknown as Calls).list({ pageSize: 3 });
  console.log(`The first ${page.items.length} of ${page.total} ${name}:`, page.items);
}
