// A table's list: a page of rows from the API, sorted by a column, searched, filtered by what they refer to (from a
// row's page: "every item of this product"), with what each refers to by name. The search, sort, page and filters are
// in the address, so the back button and a link keep them. / searches, N makes a row, J and K (or the arrows) move,
// Enter opens.
import { useEffect, useMemo, useRef, useState } from "react";
import { Value } from "./cells.tsx";
import { appName, fieldNamed, resolve } from "./config.ts";
import { invalidateTable, setQueryData, useQuery, useReferences } from "./data.ts";
import { formatCount, formatInteger, isNumeric, keyOf, keyText, rowLabel, rowPath } from "./format.ts";
import { Icon } from "./icons.tsx";
import { confirm, toast } from "./overlays.tsx";
import { navigate, useLocation, withSearch } from "./router.tsx";
import { useShortcut } from "./shortcuts.ts";
import type { Field, ListOptions, PageProps, Paged, Row, Table } from "./types.ts";
import { Badge, Button, ButtonLink, Crumbs, EmptyState, Kbd, PageHeader, Skeleton, TopbarActions, cx } from "./ui.tsx";

const pageSize = 50;

/** The message of an error from the API (or anything else). */
export const errorText = (error: unknown) =>
  error instanceof Error ? ("problem" in error && (error as { problem?: { detail?: string; title?: string } }).problem?.detail) || error.message : String(error);

/** Deletes a row, after asking: true where it's gone. */
export async function deleteRow(table: Table, row: Row, options: { before?: () => void; after?: () => void } = {}): Promise<boolean> {
  const label = rowLabel(table, row);
  const yes = await confirm({
    title: `Delete ${table.singular.toLowerCase()} ${label}?`,
    body: <>It's gone for good. A row that refers to it keeps the database from deleting it.</>,
    confirm: `Delete ${table.singular.toLowerCase()}`,
    tone: "danger",
  });
  if (!yes || !table.api.delete) return false;
  options.before?.();
  try {
    await table.api.delete(keyOf(table, row));
    toast.success(`${table.singular} deleted`, `${label} is gone.`);
    invalidateTable(table.name);
    options.after?.();
    return true;
  } catch (error) {
    toast.error(`Couldn't delete ${label}`, errorText(error));
    invalidateTable(table.name);
    return false;
  }
}

function SortIcon({ direction }: { direction: "asc" | "desc" | undefined }) {
  return <Icon name={direction === "asc" ? "arrow-up" : direction === "desc" ? "arrow-down" : "chevrons-up-down"} size={13} className={cx("sort-icon", direction && "is-sorted")} />;
}

/** Page numbers to show: the first, the last, and those around the current one, with gaps (0). */
function pageNumbers(current: number, count: number): number[] {
  const shown = new Set([1, count, current - 1, current, current + 1].filter((n) => n >= 1 && n <= count));
  const sorted = [...shown].sort((a, b) => a - b);
  const result: number[] = [];
  sorted.forEach((n, i) => {
    if (i > 0 && n - sorted[i - 1] > 1) result.push(0);
    result.push(n);
  });
  return result;
}

export function ListPage(props: PageProps & { table: Table }) {
  const table = resolve(props.table);
  const location = useLocation();
  const searchBox = useRef<HTMLInputElement>(null);
  const body = useRef<HTMLTableSectionElement>(null);

  const search = location.search.get("q") ?? "";
  const sort = location.search.get("sort") ?? undefined;
  const desc = location.search.get("desc") === "1";
  const page = Math.max(1, Number(location.search.get("page")) || 1);
  const filtering = table.fields.filter((field) => field.filter && location.search.has(field.filter));
  const filter = Object.fromEntries(filtering.map((field) => [field.filter!, location.search.getAll(field.filter!)]));
  const options: ListOptions = { page, pageSize, sort, desc: desc || undefined, search: search || undefined, filter: filtering.length ? filter : undefined };
  const queryKey = `${table.name}:list:${JSON.stringify(options)}`;
  const query = useQuery<Paged<Row>>(queryKey, () => table.api.list(options));
  const rows = query.shown?.items ?? [];
  const initial = query.loading && !query.shown;
  const total = query.shown?.total ?? 0;
  const columns = table.columns.map((name) => fieldNamed(table, name)).filter((field): field is Field => !!field);
  const references = useReferences(table, rows);

  // the filters' rows, by name: "Product: Pelican"
  const filterRow = useMemo(() => Object.fromEntries(filtering.map((field) => [field.name, castFilter(field, location.search.get(field.filter!) ?? "")])), [location.href]);
  const filterReferences = useReferences(table, filtering.length ? [filterRow] : []);

  // the search box: the address follows it a moment after typing stops
  const [text, setText] = useState(search);
  useEffect(() => setText(search), [search]);
  useEffect(() => {
    if (text === search) return;
    const timer = window.setTimeout(() => void navigate(withSearch(location, { q: text.trim(), page: null }), { replace: true }), 220);
    return () => window.clearTimeout(timer);
  }, [text]);

  // the row the keyboard is on
  const [active, setActive] = useState(-1);
  useEffect(() => setActive(-1), [queryKey]);
  const move = (by: number) => setActive((i) => {
    const next = Math.max(0, Math.min(rows.length - 1, (i < 0 ? (by > 0 ? -1 : rows.length) : i) + by));
    body.current?.querySelector<HTMLElement>(`[data-row="${next}"]`)?.scrollIntoView({ block: "nearest" });
    return next;
  });
  const keyed = table.key.length > 0 && !!table.api.get;
  useShortcut("/", () => searchBox.current?.focus(), { enabled: table.search });
  useShortcut("n", () => void navigate(`/${table.name}/new`), { enabled: !!table.api.create });
  useShortcut("j, arrowdown", () => move(1), { enabled: rows.length > 0 });
  useShortcut("k, arrowup", () => move(-1), { enabled: rows.length > 0 });
  useShortcut("enter", () => rows[active] && void navigate(rowPath(table, rows[active])), { enabled: keyed && active >= 0 });
  useShortcut("e", () => rows[active] && void navigate(rowPath(table, rows[active]) + "/edit"), { enabled: !!table.api.update && active >= 0 });

  const sortBy = (field: Field) => {
    const direction = sort === field.name ? (desc ? undefined : "desc") : "asc";
    void navigate(withSearch(location, { sort: direction ? field.name : null, desc: direction === "desc" ? 1 : null, page: null }), { replace: true });
  };

  const remove = (row: Row) => deleteRow(table, row, {
    // gone from the list at once; back where it was if the database refuses
    before: () => setQueryData<Paged<Row>>(queryKey, (data) => data && { ...data, items: data.items.filter((item) => keyText(table, item) !== keyText(table, row)), total: data.total - 1 }),
  });

  const pages = Math.max(1, Math.ceil(total / pageSize));
  const first = total === 0 ? 0 : (page - 1) * pageSize + 1;
  const last = Math.min(total, page * pageSize);
  const goTo = (n: number) => void navigate(withSearch(location, { page: n === 1 ? null : n }));

  return (
    <div className="page">
      <Crumbs items={[{ label: appName(), href: "/" }, { label: table.title }]} />
      {table.api.create && (
        <TopbarActions>
          <ButtonLink href={`/${table.name}/new${filtering.length ? "?" + new URLSearchParams(Object.fromEntries(filtering.map((field) => [field.name, location.search.get(field.filter!) ?? ""]))).toString() : ""}`} variant="primary" icon="plus" shortcut="N">
            New {table.singular.toLowerCase()}
          </ButtonLink>
        </TopbarActions>
      )}

      <PageHeader
        icon={table.icon}
        title={table.title}
        meta={query.shown ? <Badge>{formatCount(total)} {total === 1 ? "row" : "rows"}</Badge> : <Skeleton width={56} height={20} radius={999} />}
        description={table.description ?? <>The <span className="is-mono">{table.table}</span> table{table.key.length ? <> · key <span className="is-mono">{table.key.join(", ")}</span></> : <> · no key, so its rows are read-only</>}</>}
      />

      <div className="toolbar">
        {table.search && (
          <label className="search">
            <Icon name="search" size={15} className="search__icon" />
            <input ref={searchBox} type="search" className="search__input" placeholder={`Search ${table.title.toLowerCase()}…`} value={text} aria-label={`Search ${table.title.toLowerCase()}`}
              onChange={(event) => setText(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === "Escape") {
                  if (text) setText("");
                  else event.currentTarget.blur();
                } else if (event.key === "ArrowDown" && rows.length) {
                  event.preventDefault();
                  event.currentTarget.blur();
                  move(1);
                }
              }} />
            {text ? <button type="button" className="search__clear" aria-label="Clear the search" onClick={() => setText("")}><Icon name="x" size={14} /></button> : <Kbd className="search__kbd">/</Kbd>}
          </label>
        )}
        {filtering.map((field) => {
          const referred = filterReferences.of(field, filterRow);
          const value = location.search.get(field.filter!) ?? "";
          return (
            <span key={field.name} className="filter-chip">
              <Icon name={referred?.table.icon ?? "filter"} size={13} />
              <span className="filter-chip__label">{field.label}</span>
              <span className="filter-chip__value">{referred?.row ? rowLabel(referred.table, referred.row) : value}</span>
              <button type="button" aria-label={`Stop filtering by ${field.label.toLowerCase()}`} onClick={() => void navigate(withSearch(location, { [field.filter!]: null, page: null }), { replace: true })}>
                <Icon name="x" size={12} />
              </button>
            </span>
          );
        })}
        <div className="toolbar__end">
          {query.fetching && query.shown && <span className="toolbar__status"><span className="spinner" style={{ width: 13, height: 13 }} />Updating</span>}
          <Button variant="ghost" size="sm" icon="refresh" aria-label="Read again" title="Read again" onClick={query.reload} />
        </div>
      </div>

      <div className={cx("table-card", query.fetching && query.shown && "is-fetching")}>
        <div className="table-scroll">
          <table className="grid">
            <thead>
              <tr>
                {columns.map((field) => {
                  const direction = sort === field.name ? (desc ? "desc" : "asc") : undefined;
                  return (
                    <th key={field.name} scope="col" className={cx(isNumeric(field) && !field.mono && "is-num")} aria-sort={direction === "asc" ? "ascending" : direction === "desc" ? "descending" : undefined}>
                      {field.sortable
                        ? <button type="button" className="grid__sort" onClick={() => sortBy(field)}>{field.label}<SortIcon direction={direction} /></button>
                        : <span className="grid__heading">{field.label}</span>}
                    </th>
                  );
                })}
                <th scope="col" className="grid__actions-head"><span className="sr-only">Actions</span></th>
              </tr>
            </thead>
            <tbody ref={body}>
              {initial && Array.from({ length: 9 }, (_, i) => (
                <tr key={i} className="grid__skeleton" aria-hidden="true">
                  {columns.map((field, c) => <td key={field.name} className={cx(isNumeric(field) && !field.mono && "is-num")}><Skeleton width={`${45 + ((i * 17 + c * 23) % 45)}%`} /></td>)}
                  <td />
                </tr>
              ))}
              {rows.map((row, i) => {
                const path = keyed ? rowPath(table, row) : undefined;
                return (
                  <tr key={keyText(table, row) || i} data-row={i} className={cx(path && "is-link", i === active && "is-active")}
                    onClick={(event) => {
                      if (!path || (event.target as HTMLElement).closest("a, button")) return;
                      if (event.metaKey || event.ctrlKey) window.open(path, "_blank");
                      else void navigate(path);
                    }}>
                    {columns.map((field, c) => (
                      <td key={field.name} className={cx(isNumeric(field) && !field.mono && "is-num", c === 0 && "is-first", table.display[0] === field.name && "is-display")}>
                        {c === 0 && path
                          ? <a href={path} className="grid__row-link" onClick={(event) => { event.preventDefault(); void navigate(path); }}><Value table={table} field={field} row={row} referred={references.of(field, row)} /></a>
                          : <Value table={table} field={field} row={row} referred={references.of(field, row)} />}
                      </td>
                    ))}
                    <td className="grid__actions">
                      <div className="row-actions">
                        {table.api.update && <ButtonLink href={rowPath(table, row) + "/edit"} variant="ghost" size="sm" icon="pencil" aria-label={`Edit ${rowLabel(table, row)}`} title="Edit" />}
                        {table.api.delete && <Button variant="ghost" size="sm" icon="trash" className="row-actions__delete" aria-label={`Delete ${rowLabel(table, row)}`} title="Delete" onClick={() => void remove(row)} />}
                        {path && <Icon name="chevron-right" size={15} className="row-actions__open" />}
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>

        {!initial && rows.length === 0 && (
          query.error && !query.shown
            ? <EmptyState icon="alert-triangle" title={`Couldn't read ${table.title.toLowerCase()}`} actions={<Button icon="refresh" onClick={query.reload}>Try again</Button>}>{errorText(query.error)}</EmptyState>
            : search || filtering.length
              ? <EmptyState icon="search" title={search ? `No ${table.title.toLowerCase()} match “${search}”` : `No ${table.title.toLowerCase()} here`}
                  actions={<Button onClick={() => { setText(""); void navigate(location.path, { replace: true }); }}>Clear {search ? "the search" : "the filter"}</Button>}>
                  Try other words, or look at every {table.singular.toLowerCase()}.
                </EmptyState>
              : <EmptyState icon={table.icon} title={`No ${table.title.toLowerCase()} yet`}
                  actions={table.api.create && <ButtonLink href={`/${table.name}/new`} variant="primary" icon="plus">New {table.singular.toLowerCase()}</ButtonLink>}>
                  The {table.table} table is empty.{table.api.create ? " Make its first row." : ""}
                </EmptyState>
        )}

        {total > 0 && (
          <footer className="table-footer">
            <span className="table-footer__range">
              <strong>{formatInteger(first)}–{formatInteger(last)}</strong> of <strong>{formatInteger(total)}</strong>
            </span>
            {pages > 1 && (
              <nav className="pager" aria-label="Pages">
                <Button variant="ghost" size="sm" icon="chevron-left" aria-label="Previous page" disabled={page <= 1} onClick={() => goTo(page - 1)} />
                {pageNumbers(page, pages).map((n, i) => n === 0
                  ? <span key={`gap${i}`} className="pager__gap">…</span>
                  : <button key={n} type="button" className={cx("pager__page", n === page && "is-current")} aria-current={n === page ? "page" : undefined} onClick={() => goTo(n)}>{n}</button>)}
                <Button variant="ghost" size="sm" icon="chevron-right" aria-label="Next page" disabled={page >= pages} onClick={() => goTo(page + 1)} />
              </nav>
            )}
            <span className="table-footer__hint"><Kbd>J</Kbd><Kbd>K</Kbd> to move · <Kbd>↵</Kbd> to open</span>
          </footer>
        )}
      </div>
    </div>
  );
}

// a filter's value from the address, as the field's type has it
function castFilter(field: Field, value: string): unknown {
  if (isNumeric(field)) {
    const number = Number(value);
    return Number.isNaN(number) ? value : number;
  }
  return value;
}
