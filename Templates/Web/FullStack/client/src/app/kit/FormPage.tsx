// A row's form, new or to edit: its fields as their kinds take them, checked as you go (once you've left a field, or
// tried to save) and before anything's sent; what the API says is wrong goes back to the fields it names. Ctrl+S
// saves, Escape cancels; leaving with changes that aren't saved asks first.
import { useEffect, useMemo, useState, type FormEvent } from "react";
import { Value } from "./cells.tsx";
import { appName, resolve, tableNamed } from "./config.ts";
import { invalidateTable, useQuery } from "./data.ts";
import { FormField } from "./fields.tsx";
import { buildRequest, checkField, defaultFormValue, fieldInMessage, formFields, problemErrors, toFormValue } from "./form.ts";
import { keyFromParams, keyOf, keyText, lower, rowLabel, rowPath, rowTitle } from "./format.ts";
import { Icon } from "./icons.tsx";
import { deleteRow, errorText } from "./ListPage.tsx";
import { confirm, toast } from "./overlays.tsx";
import { navigate, setGuard, useLocation } from "./router.tsx";
import { useShortcut } from "./shortcuts.ts";
import type { Field, FormValue, PageProps, Row, Table } from "./types.ts";
import { Badge, Button, ButtonLink, Card, Crumbs, EmptyState, Kbd, PageHeader, Skeleton, modKey } from "./ui.tsx";

interface Problem {
  title: string;
  detail?: string;
}

function Form({ table, row }: { table: Table; row?: Row }) {
  const editing = row !== undefined;
  const location = useLocation();
  const fields = useMemo(() => formFields(table, editing), [table, editing]);
  const keys = editing ? table.fields.filter((field) => field.key) : [];

  // the values to start from: the row's, or a new row's defaults and what the address fills in (?productId=BD-01)
  const [initial] = useState<Record<string, FormValue>>(() => Object.fromEntries(fields.map((field) => {
    if (row) return [field.name, toFormValue(field, row[field.name])];
    const given = location.search.get(field.name);
    return [field.name, given !== null ? toFormValue(field, field.kind === "boolean" ? given === "true" : given) : defaultFormValue(field)];
  })));
  const [values, setValues] = useState(initial);
  const [touched, setTouched] = useState<Record<string, boolean>>({});
  const [tried, setTried] = useState(false);
  const [server, setServer] = useState<Record<string, string>>({});
  const [problem, setProblem] = useState<Problem>();
  const [saving, setSaving] = useState(false);

  const dirty = fields.some((field) => values[field.name] !== initial[field.name]);
  const checks = useMemo(() => Object.fromEntries(fields.map((field) => [field.name, checkField(field, values[field.name] ?? null)])), [fields, values]);
  const errorOf = (field: Field) => server[field.name] ?? (tried || touched[field.name] ? checks[field.name] : undefined);
  const wrong = fields.filter((field) => errorOf(field)).length;
  const required = fields.filter((field) => field.required);
  const filled = required.filter((field) => {
    const value = values[field.name];
    return value !== null && value !== "" && value !== undefined;
  }).length;

  // leaving with changes that aren't saved: asks (in the app), or the browser does (closing, reloading)
  useEffect(() => {
    if (!dirty || saving) return;
    const release = setGuard(() => confirm({ title: "Discard your changes?", body: "You've changed this form and haven't saved it.", confirm: "Discard changes", cancel: "Keep editing", tone: "danger" }));
    const unload = (event: BeforeUnloadEvent) => event.preventDefault();
    window.addEventListener("beforeunload", unload);
    return () => {
      release();
      window.removeEventListener("beforeunload", unload);
    };
  }, [dirty, saving]);

  const change = (field: Field, value: FormValue) => {
    setValues((current) => ({ ...current, [field.name]: value }));
    if (server[field.name]) setServer(({ [field.name]: _, ...rest }) => rest);
  };

  const back = row ? rowPath(table, row) : `/${table.name}`;
  const cancel = () => void navigate(back);

  const save = async () => {
    if (saving) return;
    setTried(true);
    setProblem(undefined);
    const built = buildRequest(table, editing, values);
    if (!built.request) {
      const first = fields.find((field) => built.errors[field.name]);
      if (first) document.getElementById(`field-${table.name}-${first.name}`)?.focus();
      return;
    }
    setSaving(true);
    try {
      let saved: Row;
      if (row) {
        await table.api.update!(keyOf(table, row), built.request);
        saved = { ...row, ...built.request };
      } else {
        saved = await table.api.create!(built.request);
      }
      invalidateTable(table.name);
      toast.success(editing ? `${table.singular} saved` : `${table.singular} created`, rowLabel(table, saved));
      await navigate(rowPath(table, saved), { force: true, replace: !editing });
    } catch (error) {
      setSaving(false);
      const details = (error as { problem?: { title?: string; detail?: string; errors?: Record<string, string[]> }; status?: number }).problem;
      const status = (error as { status?: number }).status;
      if (details?.errors) {
        const mapped = problemErrors(table, details.errors);
        setServer(mapped.fields);
        setProblem({ title: details.title ?? "The API found problems.", detail: mapped.other.join(" ") || undefined });
      } else if (status === 409 && details) {
        const named = fieldInMessage(table, details.detail ?? "");
        if (named && /unique|duplicate|primary key|already/i.test(details.detail ?? "")) setServer({ [named.name]: `Another ${table.singular.toLowerCase()} has this ${lower(named.label)}.` });
        setProblem({ title: details.title ?? "The database refused the change.", detail: details.detail });
      } else {
        setProblem({ title: editing ? "Couldn't save the changes." : `Couldn't create the ${table.singular.toLowerCase()}.`, detail: errorText(error) });
      }
    }
  };

  useShortcut("mod+s", () => void save(), { inInputs: true });
  useShortcut("escape", cancel, { inInputs: true });

  const submit = (event: FormEvent) => {
    event.preventDefault();
    void save();
  };

  const referenceOf = (field: Field) => {
    const relation = table.references.find((r) => r.columns.length === 1 && r.columns[0][0] === field.name);
    return relation && tableNamed(relation.table) ? { table: relation.table, column: relation.columns[0][1], label: relation.label } : undefined;
  };
  const keyFields = editing ? [] : fields.filter((field) => field.key);
  const otherFields = fields.filter((field) => !keyFields.includes(field));
  const title = row ? rowTitle(table, row) : "";
  const render = (field: Field) => (
    <FormField key={field.name} table={table} field={field} value={values[field.name] ?? null} values={values}
      onChange={(value) => change(field, value)} error={errorOf(field)} disabled={saving} reference={referenceOf(field)} />
  );

  return (
    <div className="page page--form">
      <Crumbs items={[{ label: appName(), href: "/" }, { label: table.title, href: `/${table.name}` }, ...(row ? [{ label: rowLabel(table, row), href: rowPath(table, row) }, { label: "Edit" }] : [{ label: "New" }])]} />
      <PageHeader
        icon={table.icon}
        title={row ? <>Edit <span className="page-header__subject">{title}</span></> : `New ${table.singular.toLowerCase()}`}
        meta={dirty ? <Badge tone="warning" dot>Unsaved changes</Badge> : undefined}
        description={row ? <>{table.singular} <span className="is-mono">{keyText(table, row)}</span> in {table.title}</> : <>A row of the <span className="is-mono">{table.table}</span> table</>}
      />

      {problem && (
        <div className="banner banner--danger" role="alert">
          <Icon name="alert-circle" size={18} />
          <div>
            <div className="banner__title">{problem.title}</div>
            {problem.detail && <div className="banner__detail">{problem.detail}</div>}
          </div>
          <button type="button" className="banner__close" aria-label="Dismiss" onClick={() => setProblem(undefined)}><Icon name="x" size={14} /></button>
        </div>
      )}

      <div className="form-layout">
        <form className="form" onSubmit={submit} noValidate onBlur={(event) => {
          const name = (event.target as HTMLElement).id?.replace(`field-${table.name}-`, "");
          if (name && fields.some((field) => field.name === name)) setTouched((current) => (current[name] ? current : { ...current, [name]: true }));
        }}>
          {keyFields.length > 0 && (
            <Card title="Key" icon="key" className="form-section">
              <p className="form-section__note">What the row will be found by. It's set once: it can't change later.</p>
              <div className="form-grid">{keyFields.map(render)}</div>
            </Card>
          )}
          {(otherFields.length > 0 || keys.length > 0) && (
            <Card title="Details" icon="list" className="form-section">
              <div className="form-grid">
                {keys.map((field) => (
                  <FormField key={field.name} table={table} field={field} value={null} values={values} onChange={() => {}}
                    readOnly={<div className="input is-readonly"><Icon name="lock" size={14} className="input__lead" /><Value table={table} field={field} row={row!} /></div>} />
                ))}
                {otherFields.map(render)}
              </div>
            </Card>
          )}
          <button type="submit" hidden />
        </form>

        <aside className="form-aside" aria-label="The form at a glance">
          <Card title="At a glance" icon="check-circle">
            <div className="glance__meter">
              <div className="glance__numbers"><strong>{filled}</strong> of {required.length} required {required.length === 1 ? "field" : "fields"} filled</div>
              <div className="meter" aria-hidden="true"><span style={{ width: `${required.length ? (filled / required.length) * 100 : 100}%` }} /></div>
            </div>
            {wrong > 0 ? (
              <ul className="glance__list">
                {fields.filter((field) => errorOf(field)).map((field) => (
                  <li key={field.name}>
                    <button type="button" className="glance__problem" onClick={() => document.getElementById(`field-${table.name}-${field.name}`)?.focus()}>
                      <Icon name="alert-circle" size={14} />
                      <span><strong>{field.label}</strong> {errorOf(field)}</span>
                    </button>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="glance__ok">
                <Icon name={dirty ? "check-circle" : "info"} size={14} />
                {dirty ? "Ready to save." : editing ? "Change a field to save it." : "Fill in the fields, then create it."}
              </p>
            )}
          </Card>
          <Card title="Keys" icon="keyboard">
            <dl className="glance__keys">
              <div><dt>Save</dt><dd><Kbd>{modKey}</Kbd><Kbd>S</Kbd></dd></div>
              <div><dt>Cancel</dt><dd><Kbd>Esc</Kbd></dd></div>
              <div><dt>Next field</dt><dd><Kbd>Tab</Kbd></dd></div>
            </dl>
          </Card>
        </aside>
      </div>

      <div className="action-bar">
        <div className="action-bar__status">
          {wrong > 0
            ? <span className="action-bar__wrong"><Icon name="alert-circle" size={15} />{wrong} {wrong === 1 ? "field needs" : "fields need"} attention</span>
            : dirty
              ? <span className="action-bar__dirty"><span className="dot dot--warning" />Unsaved changes</span>
              : <span className="action-bar__clean"><Icon name="check" size={14} />{editing ? "No changes" : "Fill in the fields to create it"}</span>}
        </div>
        <div className="action-bar__buttons">
          {row && table.api.delete && (
            <Button variant="danger-ghost" icon="trash" disabled={saving} onClick={() => void deleteRow(table, row, { before: () => setSaving(true), after: () => void navigate(`/${table.name}`, { force: true }) }).then((gone) => !gone && setSaving(false))}>
              Delete
            </Button>
          )}
          <Button onClick={cancel} disabled={saving} shortcut="Esc">Cancel</Button>
          <Button variant="primary" busy={saving} onClick={() => void save()} shortcut={`${modKey} S`}>
            {editing ? "Save changes" : `Create ${table.singular.toLowerCase()}`}
          </Button>
        </div>
      </div>
    </div>
  );
}

export function FormPage(props: PageProps & { table: Table; edit?: boolean }) {
  const table = resolve(props.table);
  const key = props.edit ? keyFromParams(table, props.params) : undefined;
  const query = useQuery<Row | undefined>(key ? `${table.name}:get:${JSON.stringify(key)}` : null, () => table.api.get!(key!));
  const crumbs = [{ label: appName(), href: "/" }, { label: table.title, href: `/${table.name}` }];
  if (!key) return <Form key="new" table={table} />;
  if (query.loading)
    return (
      <div className="page page--form">
        <Crumbs items={[...crumbs, { label: keyText(table, key), mono: true }, { label: "Edit" }]} />
        <div className="page-header"><Skeleton width={44} height={44} radius={12} /><div style={{ flex: 1 }}><Skeleton width={240} height={22} /><div style={{ height: 10 }} /><Skeleton width={150} /></div></div>
        <div className="card"><div className="card__body"><div className="form-grid">{Array.from({ length: 6 }, (_, i) => <div key={i} className="field"><Skeleton width={90} height={10} /><Skeleton height={36} radius={8} /></div>)}</div></div></div>
      </div>
    );
  if (!query.data)
    return (
      <div className="page">
        <Crumbs items={[...crumbs, { label: keyText(table, key), mono: true }]} />
        <div className="card">
          <EmptyState icon="search" title={`There's no ${table.singular.toLowerCase()} ${keyText(table, key)}`} actions={<ButtonLink href={`/${table.name}`} icon="arrow-left">All {table.title.toLowerCase()}</ButtonLink>}>
            {query.error ? errorText(query.error) : "It may have been deleted."}
          </EmptyState>
        </div>
      </div>
    );
  return <Form key={JSON.stringify(key)} table={table} row={query.data} />;
}

