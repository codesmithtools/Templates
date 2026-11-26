// A reference's input: a combobox that searches the table it refers to (as you type, by its list's search) and shows
// each row by what it's shown by (its name), with its key. Arrow keys move, Enter picks, Escape closes.
import { useEffect, useId, useMemo, useRef, useState, type KeyboardEvent } from "react";
import { useQuery, useReferenced } from "./data.ts";
import { fieldNamed } from "./config.ts";
import { keyText, lower, rowLabel } from "./format.ts";
import { Icon } from "./icons.tsx";
import { errorText } from "./ListPage.tsx";
import type { InputProps, Paged, Row, Table } from "./types.ts";
import { Spinner, cx } from "./ui.tsx";

export interface ComboboxProps extends InputProps {
  /** The table referred to, and its column the field's value is. */
  target: Table;
  column: string;
}

// what the input's text matches in a label, marked
function Highlight({ text, query }: { text: string; query: string }) {
  const at = query ? text.toLowerCase().indexOf(query.toLowerCase()) : -1;
  if (at < 0) return <>{text}</>;
  return <>{text.slice(0, at)}<mark>{text.slice(at, at + query.length)}</mark>{text.slice(at + query.length)}</>;
}

export function Combobox({ id, field, target, column, value, onChange, invalid, disabled }: ComboboxProps) {
  const [open, setOpen] = useState(false);
  const [text, setText] = useState("");
  const [query, setQuery] = useState("");
  const [active, setActive] = useState(0);
  const input = useRef<HTMLInputElement>(null);
  const list = useRef<HTMLUListElement>(null);
  const listId = useId();

  // the row the value names, to show it by its name
  const selected = typeof value === "string" ? (value === "" ? undefined : value) : typeof value === "boolean" ? value : undefined;
  const find = useReferenced(target, [column], selected === undefined ? [] : [[cast(target, column, selected)]]);
  const chosen = selected === undefined ? undefined : find([cast(target, column, selected)]);

  // the rows to pick from: searched (a moment after typing stops), sorted by what they're shown by
  useEffect(() => {
    const timer = window.setTimeout(() => setQuery(text.trim()), 160);
    return () => window.clearTimeout(timer);
  }, [text]);
  const sortField = target.display.map((name) => fieldNamed(target, name)).find((f) => f?.sortable);
  const options = { search: target.search ? query || undefined : undefined, pageSize: 20, sort: sortField?.name };
  const result = useQuery<Paged<Row>>(open ? `${target.name}:list:${JSON.stringify(options)}` : null, () => target.api.list(options), 10000);
  // what was read for what's typed now; or until it is, what was read before, narrowed to what's typed (so Enter
  // never picks a row that doesn't match)
  const typed = text.trim().toLowerCase();
  const current = result.data !== undefined && query === text.trim();
  const rows = useMemo(() => {
    const items = result.shown?.items ?? [];
    if (current && target.search) return items;
    return items.filter((row) => !typed || rowLabel(target, row).toLowerCase().includes(typed) || String(row[column]).toLowerCase().includes(typed));
  }, [result.shown, current, target, typed, column]);

  useEffect(() => setActive(0), [typed, current]);
  useEffect(() => {
    list.current?.querySelector<HTMLElement>(`[data-index="${active}"]`)?.scrollIntoView({ block: "nearest" });
  }, [active]);

  const pick = (row: Row) => {
    onChange(String(row[column]));
    setOpen(false);
    setText("");
    input.current?.focus();
  };

  const keydown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "ArrowDown") {
      event.preventDefault();
      if (!open) setOpen(true);
      else setActive((i) => Math.min(i + 1, rows.length - 1));
    } else if (event.key === "ArrowUp") {
      event.preventDefault();
      setActive((i) => Math.max(i - 1, 0));
    } else if (event.key === "Enter" && open) {
      event.preventDefault();
      if (rows[active]) pick(rows[active]);
    } else if (event.key === "Escape" && open) {
      event.preventDefault();
      event.stopPropagation();
      setOpen(false);
      setText("");
    } else if (event.key === "Tab") {
      setOpen(false);
      setText("");
    }
  };

  const label = chosen ? rowLabel(target, chosen) : selected !== undefined ? String(selected) : "";
  const shownText = open ? text : label;
  const more = (result.shown?.total ?? 0) - rows.length;

  return (
    <div className={cx("combobox", open && "is-open")}>
      <div className={cx("input", "input--combobox", invalid && "is-invalid", disabled && "is-disabled")} onMouseDown={(event) => {
        if (disabled || event.target === input.current) return;
        event.preventDefault();
        input.current?.focus();
        setOpen(true);
      }}>
        <Icon name={target.icon} size={15} className="input__lead" />
        <input
          ref={input}
          id={id}
          role="combobox"
          aria-expanded={open}
          aria-controls={listId}
          aria-autocomplete="list"
          aria-activedescendant={open && rows[active] ? `${listId}-${active}` : undefined}
          aria-invalid={invalid || undefined}
          autoComplete="off"
          spellCheck={false}
          disabled={disabled}
          placeholder={open ? `Search ${target.title.toLowerCase()}…` : `Choose ${/^[aeiou]/i.test(field.label) ? "an" : "a"} ${lower(field.label)}`}
          value={shownText}
          onChange={(event) => {
            setText(event.target.value);
            setOpen(true);
          }}
          onFocus={() => setOpen(true)}
          onBlur={() => window.setTimeout(() => {
            setOpen(false);
            setText("");
          }, 120)}
          onKeyDown={keydown}
        />
        {!open && chosen && <span className="input__trail is-mono">{String(selected)}</span>}
        {open && result.fetching && <Spinner size={14} />}
        {field.nullable && selected !== undefined && !disabled && (
          <button type="button" className="input__clear" aria-label={`Clear ${field.label}`} onMouseDown={(event) => event.preventDefault()} onClick={() => onChange("")}>
            <Icon name="x" size={14} />
          </button>
        )}
        <Icon name="chevrons-up-down" size={14} className="input__chevron" />
      </div>
      {open && (
        <ul ref={list} id={listId} role="listbox" className="combobox__list" aria-label={target.title}>
          {result.loading && !result.shown && Array.from({ length: 4 }, (_, i) => (
            <li key={i} className="combobox__option combobox__option--skeleton" aria-hidden="true"><span className="skeleton" style={{ width: 90 + ((i * 37) % 80), height: 10 }} /></li>
          ))}
          {result.error !== undefined && !result.fetching && (
            <li className="combobox__empty" role="alert">Couldn't search {target.title.toLowerCase()}: {errorText(result.error)}</li>
          )}
          {!(result.loading && !result.shown) && rows.length === 0 && result.error === undefined && (
            <li className="combobox__empty">{query ? <>No {target.title.toLowerCase()} match “{query}”</> : <>No {target.title.toLowerCase()} yet</>}</li>
          )}
          {rows.map((row, i) => {
            const isSelected = selected !== undefined && String(row[column]) === String(selected);
            const name = rowLabel(target, row);
            const key = keyText(target, row);
            return (
              <li
                key={key}
                id={`${listId}-${i}`}
                data-index={i}
                role="option"
                aria-selected={isSelected}
                className={cx("combobox__option", i === active && "is-active", isSelected && "is-selected")}
                onMouseDown={(event) => event.preventDefault()}
                onMouseMove={() => setActive(i)}
                onClick={() => pick(row)}
              >
                <span className="combobox__label"><Highlight text={name} query={query} /></span>
                {name !== key && <span className="combobox__key is-mono"><Highlight text={key} query={query} /></span>}
                {isSelected && <Icon name="check" size={14} className="combobox__check" />}
              </li>
            );
          })}
          {more > 0 && <li className="combobox__more">{more} more — type to narrow it down</li>}
        </ul>
      )}
    </div>
  );
}

// a form's value (text) as the column's type has it: a number for a number
function cast(table: Table, column: string, value: string | boolean): unknown {
  const field = fieldNamed(table, column);
  if (typeof value === "string" && field && (field.kind === "integer" || field.kind === "decimal" || field.kind === "money")) {
    const number = Number(value);
    return Number.isNaN(number) ? value : number;
  }
  return value;
}
