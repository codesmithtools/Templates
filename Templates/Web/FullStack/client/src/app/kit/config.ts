// The app's tables and your customizations, as the kit finds them: configure() is called once, before the app renders
// (main.tsx), with the generated tables and customize.tsx.
import type { Customizations, Database, Field, Table } from "./types.ts";

let all: Table[] = [];
let byName = new Map<string, Table>();
let database: Database = { name: "", provider: "" };
let customizations: Customizations = {};
let counter: () => Promise<RowCount[]> = async () => [];

/** How many rows a table has (the API's GET /api). */
export interface RowCount {
  table: string;
  rows: number;
}

export function configure(options: { tables: Table[]; database: Database; customize?: Customizations; rowCounts?: () => Promise<RowCount[]> }) {
  customizations = options.customize ?? {};
  counter = options.rowCounts ?? counter;
  all = options.tables.map((table) => customizations.tables?.[table.name]?.(table) ?? table);
  byName = new Map(all.map((table) => [table.name, table]));
  database = options.database;
}

/** Every table, customized. */
export const tables = () => all;

/** A table by its name (products), customized; undefined where the UI leaves it out. */
export const tableNamed = (name: string): Table | undefined => byName.get(name);

/** A table as customize.tsx has it (a page gets the generated one). */
export const resolve = (table: Table): Table => byName.get(table.name) ?? table;

export const theDatabase = () => database;

/** Reads how many rows each table has. */
export const rowCounts = () => counter();

export const customize = () => customizations;

/** A field of a table by its name. */
export const fieldNamed = (table: Table, name: string): Field | undefined => table.fields.find((field) => field.name === name);

/** The app's name: customize.tsx's, or the database's. */
export const appName = () => customizations.appName ?? database.name;
