// A row's page: its fields, the rows it refers to (by name, as links), and the rows of other tables that refer to it,
// a few of each with a link to all of them and a button to add one. E edits, Delete (after asking) deletes.
import { Value } from "./cells.tsx";
import { appName, fieldNamed, resolve, tableNamed } from "./config.ts";
import { useQuery, useReferences } from "./data.ts";
import { formatCount, isNumeric, keyFromParams, keyOf, keyText, rowLabel, rowPath, rowTitle } from "./format.ts";
import { Icon } from "./icons.tsx";
import { deleteRow, errorText } from "./ListPage.tsx";
import { toast } from "./overlays.tsx";
import { Link, navigate } from "./router.tsx";
import { useShortcut } from "./shortcuts.ts";
import type { Field, PageProps, Paged, Relation, Row, Table } from "./types.ts";
import { Badge, Button, ButtonLink, Card, Crumbs, EmptyState, PageHeader, Skeleton, TopbarActions, cx } from "./ui.tsx";

/** The rows of a child table that refer to a row (Items of a product): the first few, how many in all, and links. */
function Children({ parent, row, relation }: { parent: Table; row: Row; relation: Relation }) {
  const child = tableNamed(relation.table);
  const filter = child ? Object.fromEntries(relation.columns.map(([theirs, ours]) => [fieldNamed(child, theirs)?.filter ?? theirs, [row[ours]]])) : {};
  const options = { filter, pageSize: 5 };
  const query = useQuery<Paged<Row>>(child ? `${child.name}:list:${JSON.stringify(options)}` : null, () => child!.api.list(options));
  const references = useReferences(child ?? parent, child ? query.shown?.items ?? [] : []);
  if (!child) return null;
  // its columns but those that refer back to this row
  const back = new Set(relation.columns.map(([theirs]) => theirs));
  const columns = child.columns.filter((name) => !back.has(name)).slice(0, 4).map((name) => fieldNamed(child, name)).filter((field): field is Field => !!field);
  const prefill = new URLSearchParams(Object.fromEntries(relation.columns.map(([theirs, ours]) => [theirs, String(row[ours] ?? "")]))).toString();
  const all = new URLSearchParams(Object.fromEntries(relation.columns.map(([theirs, ours]) => [fieldNamed(child, theirs)?.filter ?? theirs, String(row[ours] ?? "")]))).toString();
  const total = query.shown?.total;
  return (
    <Card
      title={relation.label}
      icon={child.icon}
      count={total}
      className="children"
      flush
      actions={<>
        {child.api.create && <ButtonLink href={`/${child.name}/new?${prefill}`} size="sm" variant="ghost" icon="plus">Add</ButtonLink>}
        {!!total && <ButtonLink href={`/${child.name}?${all}`} size="sm" variant="ghost" iconEnd="arrow-right">View all</ButtonLink>}
      </>}
    >
      {query.loading && <div className="mini-list">{[0, 1, 2].map((i) => <div key={i} className="mini-row"><Skeleton width={`${30 + i * 12}%`} /><Skeleton width="18%" /></div>)}</div>}
      {query.shown && query.shown.items.length === 0 && (
        <EmptyState compact icon={child.icon} title={`No ${child.title.toLowerCase()}`}>
          None refer to this {parent.singular.toLowerCase()} yet.
        </EmptyState>
      )}
      {query.shown && query.shown.items.length > 0 && (
        <div className="table-scroll table-scroll--x">
          <table className="grid grid--compact">
            <thead>
              <tr>{columns.map((field) => <th key={field.name} scope="col" className={cx(isNumeric(field) && !field.mono && "is-num")}><span className="grid__heading">{field.label}</span></th>)}<th aria-hidden="true" /></tr>
            </thead>
            <tbody>
              {query.shown.items.map((item) => {
                const path = child.key.length ? rowPath(child, item) : undefined;
                return (
                  <tr key={keyText(child, item)} className={cx(path && "is-link")} onClick={(event) => path && !(event.target as HTMLElement).closest("a, button") && void navigate(path)}>
                    {columns.map((field, c) => (
                      <td key={field.name} className={cx(isNumeric(field) && !field.mono && "is-num", c === 0 && "is-first", child.display[0] === field.name && "is-display")}>
                        {c === 0 && path
                          ? <Link href={path} className="grid__row-link"><Value table={child} field={field} row={item} referred={references.of(field, item)} /></Link>
                          : <Value table={child} field={field} row={item} referred={references.of(field, item)} />}
                      </td>
                    ))}
                    <td className="grid__actions"><div className="row-actions">{path && <Icon name="chevron-right" size={15} className="row-actions__open" />}</div></td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
      {total !== undefined && total > (query.shown?.items.length ?? 0) && (
        <Link href={`/${child.name}?${all}`} className="children__more">and {formatCount(total - query.shown!.items.length)} more <Icon name="arrow-right" size={13} /></Link>
      )}
    </Card>
  );
}

export function DetailPage(props: PageProps & { table: Table }) {
  const table = resolve(props.table);
  const key = keyFromParams(table, props.params);
  const label = keyText(table, key);
  const query = useQuery<Row | undefined>(`${table.name}:get:${JSON.stringify(key)}`, () => table.api.get!(key));
  const row = query.shown && keyText(table, query.shown) === label ? query.shown : query.data;
  const references = useReferences(table, row ? [row] : []);
  useShortcut("e", () => void navigate(`${rowPath(table, key)}/edit`), { enabled: !!row && !!table.api.update });
  useShortcut("backspace, delete", () => row && void deleteRow(table, row, { after: () => void navigate(`/${table.name}`, { force: true }) }), { enabled: !!row && !!table.api.delete });

  const crumbs = [{ label: appName(), href: "/" }, { label: table.title, href: `/${table.name}` }];
  if (query.loading)
    return (
      <div className="page">
        <Crumbs items={[...crumbs, { label: label, mono: true }]} />
        <div className="page-header"><Skeleton width={44} height={44} radius={12} /><div style={{ flex: 1 }}><Skeleton width={220} height={22} /><div style={{ height: 10 }} /><Skeleton width={160} /></div></div>
        <div className="detail-layout">
          <div className="card"><div className="card__body"><div className="facts">{Array.from({ length: 6 }, (_, i) => <div key={i} className="fact"><Skeleton width={80} height={10} /><Skeleton width={`${50 + ((i * 29) % 40)}%`} height={14} /></div>)}</div></div></div>
          <div className="card"><div className="card__body"><Skeleton width="70%" /><div style={{ height: 12 }} /><Skeleton width="50%" /></div></div>
        </div>
      </div>
    );
  if (!row)
    return (
      <div className="page">
        <Crumbs items={[...crumbs, { label: label, mono: true }]} />
        <div className="card">
          {query.error
            ? <EmptyState icon="alert-triangle" title={`Couldn't read ${table.singular.toLowerCase()} ${label}`} actions={<Button icon="refresh" onClick={query.reload}>Try again</Button>}>{errorText(query.error)}</EmptyState>
            : <EmptyState icon="search" title={`There's no ${table.singular.toLowerCase()} ${label}`} actions={<ButtonLink href={`/${table.name}`} icon="arrow-left">All {table.title.toLowerCase()}</ButtonLink>}>
                It may have been deleted, or its key changed.
              </EmptyState>}
        </div>
      </div>
    );

  const title = rowTitle(table, row);
  const parents = references.parents(row);
  const fields = table.fields;
  const copyKey = () => {
    void navigator.clipboard?.writeText(keyText(table, row)).then(() => toast.success("Key copied", keyText(table, row)));
  };

  return (
    <div className="page">
      <Crumbs items={[...crumbs, { label: rowLabel(table, row), mono: rowLabel(table, row) === label }]} />
      <TopbarActions>
        {table.api.delete && <Button variant="danger-ghost" icon="trash" onClick={() => void deleteRow(table, row, { after: () => void navigate(`/${table.name}`, { force: true }) })}>Delete</Button>}
        {table.api.update && <ButtonLink href={`${rowPath(table, row)}/edit`} variant="primary" icon="pencil" shortcut="E">Edit</ButtonLink>}
      </TopbarActions>

      <PageHeader
        icon={table.icon}
        title={title}
        meta={<button type="button" className="key-pill" onClick={copyKey} title="Copy the key"><Icon name="key" size={12} /><span className="is-mono">{keyText(table, row)}</span><Icon name="copy" size={12} className="key-pill__copy" /></button>}
        description={<>{table.singular} in <Link href={`/${table.name}`} className="text-link">{table.title}</Link></>}
      />

      <div className="detail-layout">
        <Card title="Details" icon="list">
          <dl className="facts">
            {fields.map((field) => (
              <div key={field.name} className={cx("fact", (field.kind === "multiline" || field.kind === "json") && "fact--wide")}>
                <dt className="fact__label">
                  {field.label}
                  {field.key && <Icon name="key" size={11} className="fact__key" aria-label="Key" />}
                </dt>
                <dd className="fact__value"><Value table={table} field={field} row={row} referred={references.of(field, row)} full /></dd>
              </div>
            ))}
          </dl>
        </Card>

        <div className="detail-side">
          {parents.length > 0 && (
            <Card title="Refers to" icon="link">
              <ul className="relations">
                {parents.map((parent) => (
                  <li key={parent.relation.label + parent.relation.table}>
                    {parent.row
                      ? <Link href={rowPath(parent.table, parent.row)} className="relation">
                          <span className="relation__icon"><Icon name={parent.table.icon} size={15} /></span>
                          <span className="relation__text"><span className="relation__label">{parent.relation.label}</span><span className="relation__value">{rowLabel(parent.table, parent.row)}</span></span>
                          <Icon name="chevron-right" size={15} className="relation__go" />
                        </Link>
                      : <div className="relation is-static">
                          <span className="relation__icon"><Icon name={parent.table.icon} size={15} /></span>
                          <span className="relation__text"><span className="relation__label">{parent.relation.label}</span>
                            <span className="relation__value is-muted">{parent.row === undefined ? <Skeleton width={90} /> : parent.values.every((v) => v === null || v === undefined) ? "None" : `${parent.values.join(" · ")} (not there)`}</span></span>
                        </div>}
                  </li>
                ))}
              </ul>
            </Card>
          )}
          <Card title="Record" icon="database">
            <dl className="record">
              <div><dt>Table</dt><dd className="is-mono">{table.table}</dd></div>
              <div><dt>Key</dt><dd className="is-mono">{table.key.map((name) => `${name} = ${String(keyOf(table, row)[name])}`).join(", ")}</dd></div>
              <div><dt>Columns</dt><dd>{table.fields.length}</dd></div>
              {table.children.length > 0 && <div><dt>Referred to by</dt><dd>{table.children.map((c) => c.label).join(", ")}</dd></div>}
            </dl>
          </Card>
        </div>
      </div>

      {table.children.length > 0 && (
        <section className="children-section">
          <h2 className="section-title">Related <Badge>{table.children.length}</Badge></h2>
          <div className="children-grid">
            {table.children.map((relation) => <Children key={relation.table + relation.label} parent={table} row={row} relation={relation} />)}
          </div>
        </section>
      )}
    </div>
  );
}
