// What the generated UI (src/generated/ui) gives the kit: a table's definition (its fields, relationships and calls)
// and the routes to its pages. The kit's pages render any table from it; customize.tsx changes what they render.
import type { ComponentType } from "react";
import type { z } from "zod";

/** How a field is shown and edited. */
export type FieldKind =
  | "text" | "multiline" | "email" | "phone" | "url" | "uuid" | "json" | "binary"
  | "integer" | "decimal" | "money" | "boolean" | "date" | "datetime" | "time";

/** A row, as the API returns it. */
export type Row = Record<string, unknown>;

/** A column of the table: its JSON name, label and kind, and what limits it. */
export interface Field<R = Row> {
  name: keyof R & string;
  label: string;
  kind: FieldKind;
  /** The database's type: varchar(80). */
  type: string;
  nullable?: boolean;
  /** A new row must have it: it can't be null, and the database has no default for it. */
  required?: boolean;
  /** Part of the primary key: set when the row's made, never changed. */
  key?: boolean;
  /** The database fills it in (an identity, a computed column): never edited. */
  generated?: boolean;
  maxLength?: number;
  min?: number;
  max?: number;
  precision?: number;
  scale?: number;
  /** A new row's value ("now" for the time). */
  default?: string | number | boolean;
  sortable?: boolean;
  /** The list's filter by it (its query parameter), where there's one. */
  filter?: string;
  /** Shown in a monospaced font (keys, codes). */
  mono?: boolean;
  /** Shown as a badge (a status). */
  badge?: boolean;
  /** The column's description. */
  help?: string;
}

/** A foreign key: the table it's between, and its columns paired, [the referring table's, the referred to's]. */
export interface Relation {
  table: string;
  label: string;
  columns: [string, string][];
}

/** What the API's list asks for (the generated client's ListOptions). */
export interface ListOptions {
  page?: number;
  pageSize?: number;
  sort?: string;
  desc?: boolean;
  search?: string;
  filter?: Record<string, readonly unknown[] | undefined>;
}

/** A page of rows (the generated client's Paged). */
export interface Paged<R> {
  items: R[];
  total: number;
  page: number;
  pageSize: number;
}

/** A table's calls: the generated client's, by the row's key. Only list where the table has no key. */
export interface TableApi<R, C, U> {
  list(options: ListOptions): Promise<Paged<R>>;
  get?(key: R): Promise<R | undefined>;
  create?(request: C): Promise<R>;
  update?(key: R, request: U): Promise<void>;
  delete?(key: R): Promise<void>;
}

/** A table, as the UI shows it. */
export interface TableDefinition<R, C, U> {
  /** Its route and name in the UI (line-items). */
  name: string;
  /** The database's table. */
  table: string;
  title: string;
  singular: string;
  description?: string;
  /** Its icon (icons.tsx). */
  icon: string;
  key: (keyof R & string)[];
  /** The fields a row is shown by (none: by its key). */
  display: (keyof R & string)[];
  /** The list's columns. */
  columns: (keyof R & string)[];
  /** Whether the list searches. */
  search: boolean;
  /** What the newest rows are sorted by (the dashboard's). */
  recent?: keyof R & string;
  fields: Field<R>[];
  /** Its foreign keys: the rows it refers to. */
  references: Relation[];
  /** Other tables' foreign keys to it: the rows that refer to it. */
  children: Relation[];
  create?: z.ZodType<C>;
  update?: z.ZodType<U>;
  api: TableApi<R, C, U>;
}

/** Any table (what the pages take). */
export type Table = TableDefinition<Row, Row, Row>;

/** Checks a table's definition against its row and request types, and makes it any table. */
export function defineTable<R, C, U>(definition: TableDefinition<R, C, U>): Table {
  return definition as unknown as Table;
}

/** A page's props: its route's parameters (a row's key). */
export interface PageProps {
  params: Record<string, string>;
}

export type PageKind = "list" | "detail" | "create" | "edit";

/** A page of a table, at a path (/products/:productId). */
export interface Route {
  path: string;
  table: Table;
  page: PageKind;
  component: ComponentType<PageProps>;
}

/** The database the UI was generated from. */
export interface Database {
  name: string;
  provider: string;
}

/** A form's value of a field: text as the input has it, a bool, or null. */
export type FormValue = string | boolean | null;

/** What a field's input gets (customize.tsx's inputs). */
export interface InputProps {
  id: string;
  table: Table;
  field: Field;
  value: FormValue;
  onChange: (value: FormValue) => void;
  invalid: boolean;
  disabled?: boolean;
  /** The other fields' values (a reference's other columns). */
  values: Record<string, FormValue>;
  /** Sets other fields too (a reference of several columns). */
  onChangeMany?: (values: Record<string, FormValue>) => void;
}

/** What a field's value gets, in a list or on a row's page (customize.tsx's cells). */
export interface CellProps {
  table: Table;
  field: Field;
  row: Row;
  value: unknown;
}

/** Your changes to the generated UI (customize.tsx): keyed by "table.field" or "table.page", with the table's name
 * as the UI has it (products, line-items). */
export interface Customizations {
  /** The app's name in the sidebar (the database's by default). */
  appName?: string;
  /** The currency money is shown in (ISO 4217: USD, EUR). */
  currency?: string;
  /** The locale numbers and dates are shown in (the browser's by default). */
  locale?: string;
  /** Changes to a table's definition: its title, icon, list columns, a field's label. */
  tables?: Record<string, (table: Table) => Table>;
  /** A field's input on the forms ("products.descn"). */
  inputs?: Record<string, ComponentType<InputProps>>;
  /** A field's value in lists and on a row's page ("items.status"). */
  cells?: Record<string, ComponentType<CellProps>>;
  /** A whole page ("products.detail"; list, detail, create, edit). */
  pages?: Record<string, ComponentType<PageProps>>;
}
