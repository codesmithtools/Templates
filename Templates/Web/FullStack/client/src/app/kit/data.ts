// What the pages read from the API, kept: a query's result is shown at once when a page comes back, and read again
// behind it (stale while it revalidates). A change to a table drops what was read of it.
import { useEffect, useReducer, useRef } from "react";
import type { Referred } from "./cells.tsx";
import { fieldNamed, tableNamed } from "./config.ts";
import type { Field, Relation, Row, Table } from "./types.ts";

interface Entry {
  data?: unknown;
  error?: unknown;
  at: number;
  stale: boolean;
  loading?: Promise<void>;
  load?: () => Promise<unknown>;
  listeners: Set<() => void>;
}

const cache = new Map<string, Entry>();

const entry = (key: string): Entry => {
  let found = cache.get(key);
  if (!found) cache.set(key, (found = { at: 0, stale: true, listeners: new Set() }));
  return found;
};

const emit = (found: Entry) => found.listeners.forEach((listener) => listener());

function fetchEntry(key: string, load: () => Promise<unknown>): Promise<void> {
  const found = entry(key);
  found.load = load;
  if (found.loading) return found.loading;
  found.loading = load()
    .then((data) => {
      found.data = data;
      found.error = undefined;
      found.at = Date.now();
      found.stale = false;
    }, (error: unknown) => {
      found.error = error;
      found.stale = false;
      found.at = Date.now();
    })
    .finally(() => {
      found.loading = undefined;
      emit(found);
    });
  emit(found);
  return found.loading;
}

export interface Query<T> {
  /** What the query read (undefined until it has). */
  data: T | undefined;
  /** What to show: the data, or while another key loads, the last key's (the previous page of a list). */
  shown: T | undefined;
  error: unknown;
  /** Nothing to show yet. */
  loading: boolean;
  /** Reading (again). */
  fetching: boolean;
  reload: () => void;
}

/** Reads what a key names (null: nothing), kept for the next page that asks; read again when it's older than maxAge. */
export function useQuery<T>(key: string | null, load: () => Promise<T>, maxAge = 2000): Query<T> {
  const [, rerender] = useReducer((n: number) => n + 1, 0);
  const loader = useRef(load);
  loader.current = load;
  useEffect(() => {
    if (key === null) return;
    const found = entry(key);
    found.listeners.add(rerender);
    if (!found.loading && (found.stale || Date.now() - found.at > maxAge)) void fetchEntry(key, () => loader.current());
    return () => void found.listeners.delete(rerender);
  }, [key, maxAge]);
  const found = key === null ? undefined : cache.get(key);
  const previous = useRef<T | undefined>(undefined);
  if (found?.data !== undefined) previous.current = found.data as T;
  return {
    data: found?.data as T | undefined,
    shown: (found?.data as T | undefined) ?? previous.current,
    error: found?.error,
    loading: key !== null && !(found && found.at > 0),
    fetching: !!found?.loading,
    reload: () => {
      if (key !== null) void fetchEntry(key, () => loader.current());
    },
  };
}

/** Changes what a key read, at once (an optimistic change); a function gets what's there. */
export function setQueryData<T>(key: string, change: (data: T | undefined) => T | undefined) {
  const found = entry(key);
  found.data = change(found.data as T | undefined);
  emit(found);
}

/** The keys read so far that start with a prefix, and their data. */
export function queriesStartingWith(prefix: string): [string, unknown][] {
  return [...cache.entries()].filter(([key]) => key.startsWith(prefix)).map(([key, found]) => [key, found.data]);
}

/** Drops what was read of a table (and the counts): a page showing it reads it again, now. */
export function invalidateTable(name: string) {
  for (const key of [...referenced.keys()]) if (key.startsWith(name + "|")) referenced.delete(key);
  for (const [key, found] of cache) {
    if (!key.startsWith(name + ":") && key !== "counts") continue;
    found.stale = true;
    if (found.listeners.size > 0 && found.load) void fetchEntry(key, found.load);
  }
}

// ---- the rows a page refers to ----

// rows by table, columns and values ("categories|categoryId|["BIRDS"]"): null where there's none
const referenced = new Map<string, Row | null>();

const referenceKey = (table: string, columns: string[], tuple: unknown[]) => `${table}|${columns.join(",")}|${JSON.stringify(tuple)}`;

/** The rows of a table that values of its columns name (a page's references: the categories of a list of products),
 * read together, by the list's filters; finds one by its values. */
export function useReferenced(table: Table | undefined, columns: string[], tuples: unknown[][]): (tuple: unknown[]) => Row | null | undefined {
  const filters = table ? columns.map((name) => fieldNamed(table, name)?.filter) : [];
  const usable = table !== undefined && filters.every((filter) => filter !== undefined);
  const missing: unknown[][] = [];
  if (usable) {
    const seen = new Set<string>();
    for (const tuple of tuples) {
      if (tuple.some((value) => value === null || value === undefined)) continue;
      const key = referenceKey(table.name, columns, tuple);
      if (referenced.has(key) || seen.has(key)) continue;
      seen.add(key);
      missing.push(tuple);
    }
  }
  const queryKey = usable && missing.length > 0 ? `${table.name}:refer:${columns.join(",")}:${JSON.stringify(missing)}` : null;
  useQuery(queryKey, async () => {
    if (!table) return [];
    const filter: Record<string, unknown[]> = {};
    columns.forEach((_, i) => (filter[filters[i]!] = [...new Set(missing.map((tuple) => tuple[i]))]));
    const page = await table.api.list({ filter, pageSize: 500 });
    for (const row of page.items) referenced.set(referenceKey(table.name, columns, columns.map((name) => row[name])), row);
    for (const tuple of missing) {
      const key = referenceKey(table.name, columns, tuple);
      if (!referenced.has(key)) referenced.set(key, null);
    }
    return page.items;
  }, 60000);
  return (tuple) => (table ? referenced.get(referenceKey(table.name, columns, tuple)) : undefined);
}

/** What a row refers to: for each of the table's references, the row it names (null: none; undefined: not read yet). */
export interface Parent {
  relation: Relation;
  table: Table;
  /** The referring row's values of the reference's columns. */
  values: unknown[];
  row: Row | null | undefined;
}

/** The rows a page's rows refer to, read together for each of the table's references. */
export function useReferences(table: Table, rows: Row[]) {
  // one lookup per reference: the table's references don't change while its page shows
  const lookups = table.references.map((relation) => {
    const parent = tableNamed(relation.table);
    const own = relation.columns.map(([from]) => from);
    const theirs = relation.columns.map(([, to]) => to);
    const find = useReferenced(parent, theirs, rows.map((row) => own.map((name) => row[name])));
    return { relation, parent, own, find };
  });
  return {
    /** The row a field refers to, where it's a reference alone. */
    of(field: Field, row: Row): Referred | undefined {
      const lookup = lookups.find((l) => l.parent && l.own.length === 1 && l.own[0] === field.name);
      if (!lookup?.parent) return undefined;
      const value = row[field.name];
      if (value === null || value === undefined) return undefined;
      return { table: lookup.parent, row: lookup.find([value]) };
    },
    /** Every row a row refers to. */
    parents(row: Row): Parent[] {
      return lookups.filter((l) => l.parent).map((l) => {
        const values = l.own.map((name) => row[name]);
        const none = values.some((value) => value === null || value === undefined);
        return { relation: l.relation, table: l.parent!, values, row: none ? null : l.find(values) };
      });
    },
  };
}
