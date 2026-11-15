// Your changes to the generated UI. The generated files (src/generated/ui) are written again each time you generate,
// so change what they show here: keyed by a table's name in the UI (its route: products, line-items), and a field's
// JSON name (products.descn) or a page (products.detail; list, detail, create, edit).
import type { CellProps, Customizations, Table } from "./kit/index.ts";
import { Value, formatMoney } from "./kit/index.ts";

// An item's list price, and its margin over what it costs.
function ListPrice({ table, field, row }: CellProps) {
  const price = row.listPrice;
  const cost = row.unitCost;
  if (typeof price !== "number" || typeof cost !== "number" || price <= 0) return <Value table={table} field={{ ...field, kind: "money" }} row={row} />;
  const margin = Math.round(((price - cost) / price) * 100);
  return (
    <span className="price" title={`Costs ${formatMoney(cost)}`}>
      {formatMoney(price)}
      <span className={margin >= 0 ? "price__margin" : "price__margin is-negative"}>{margin}%</span>
    </span>
  );
}

export const customize: Customizations = {
  // appName: "Pet shop",
  currency: "USD",

  // a table's definition: its title, icon, the list's columns, a field's label
  tables: {
    // the image is a path: not worth a column in the list
    products: (table: Table): Table => ({ ...table, columns: table.columns.filter((name) => name !== "image") }),
  },

  // a field's value in lists and on a row's page (it gets CellProps)
  cells: {
    "items.listPrice": ListPrice,
  },

  // a field's input on the forms: "products.image": ImageInput (it gets InputProps)
  inputs: {},

  // a whole page: "orders.detail": OrderPage (it gets the route's params)
  pages: {},
};
