// A form's fields: a label above the input (with a mark where it's required), the input its kind takes, what it
// takes or what's wrong with it, and how many characters are left. customize.tsx's inputs replace an input.
import { useLayoutEffect, useRef, useState, type ReactNode } from "react";
import { Combobox } from "./Combobox.tsx";
import { customize, tableNamed } from "./config.ts";
import { describeField, formatInteger } from "./format.ts";
import { Icon } from "./icons.tsx";
import type { Field, FormValue, InputProps, Table } from "./types.ts";
import { cx } from "./ui.tsx";

const leadIcons: Partial<Record<Field["kind"], string>> = { email: "mail", phone: "phone", url: "globe", uuid: "hash" };

function TextArea({ id, value, onChange, invalid, disabled, mono, describedBy }: { id: string; value: string; onChange: (value: string) => void; invalid: boolean; disabled?: boolean; mono?: boolean; describedBy?: string }) {
  const area = useRef<HTMLTextAreaElement>(null);
  // as tall as its text (up to a point)
  useLayoutEffect(() => {
    const element = area.current;
    if (!element) return;
    element.style.height = "auto";
    element.style.height = Math.min(element.scrollHeight + 2, 320) + "px";
  }, [value]);
  return (
    <textarea ref={area} id={id} className={cx("input", "input--area", invalid && "is-invalid", mono && "is-mono")} rows={3} value={value}
      onChange={(event) => onChange(event.target.value)} disabled={disabled} aria-invalid={invalid || undefined} aria-describedby={describedBy} spellCheck={!mono} />
  );
}

/** Some of the options, each on or off: a set's values ("a,b") or a list's of an enum (["a", "b"]), in the options'
 * order. */
function Choices({ id, options, chosen, onChange, disabled, describedBy }: { id: string; options: string[]; chosen: string[]; onChange: (chosen: string[]) => void; disabled?: boolean; describedBy?: string }) {
  return (
    <div className="choices" role="group" aria-labelledby={id + "-label"} aria-describedby={describedBy}>
      {options.map((option, i) => {
        const on = chosen.includes(option);
        return (
          <button key={option} id={i === 0 ? id : undefined} type="button" className={cx("choice", on && "is-on")} aria-pressed={on} disabled={disabled}
            onClick={() => onChange(on ? chosen.filter((item) => item !== option) : options.filter((item) => item === option || chosen.includes(item)))}>
            {on && <Icon name="check" size={13} />}
            {option === "" ? <em>empty</em> : option}
          </button>
        );
      })}
    </div>
  );
}

/** A list's items as chips, and a box that adds one (Enter, or a comma); Backspace in the empty box takes the last. */
function ListInput({ id, field, items, onChange, invalid, disabled, describedBy }: { id: string; field: Field; items: string[]; onChange: (items: string[]) => void; invalid: boolean; disabled?: boolean; describedBy?: string }) {
  const [text, setText] = useState("");
  const add = () => {
    if (text.trim() === "") return;
    onChange([...items, field.items === "text" ? text : text.trim()]);
    setText("");
  };
  return (
    <div className={cx("input", "input--chips", invalid && "is-invalid", disabled && "is-disabled")}>
      {items.map((item, i) => (
        <span key={i} className="chip">
          <span className={cx("chip__text", item === "" && "is-none")}>{item === "" ? (field.items === "text" ? "empty" : "none") : item}</span>
          <button type="button" className="chip__remove" aria-label={`Remove ${item || "this item"}`} disabled={disabled} onClick={() => onChange(items.filter((_, j) => j !== i))}>
            <Icon name="x" size={12} />
          </button>
        </span>
      ))}
      <input id={id} type="text" value={text} placeholder={items.length ? "Add another" : "Add an item"} disabled={disabled} autoComplete="off"
        inputMode={field.items === "integer" ? "numeric" : field.items === "decimal" ? "decimal" : undefined}
        aria-invalid={invalid || undefined} aria-describedby={describedBy}
        onChange={(event) => {
          // a comma adds what's before it (a list of numbers or GUIDs; text may have commas, so only Enter adds it)
          const typed = event.target.value;
          if (field.items !== "text" && typed.includes(",")) {
            const parts = typed.split(",");
            onChange([...items, ...parts.slice(0, -1).map((part) => part.trim()).filter((part) => part !== "")]);
            setText(parts[parts.length - 1]);
          } else setText(typed);
        }}
        onKeyDown={(event) => {
          if (event.key === "Enter") {
            event.preventDefault();
            add();
          } else if (event.key === "Backspace" && text === "" && items.length) onChange(items.slice(0, -1));
        }}
        onBlur={add} />
    </div>
  );
}

/** A field's input, by its kind: a reference's combobox, a switch, a number, a date, a select, chips, text. */
export function FieldInput(props: InputProps & { describedBy?: string; reference?: { table: string; column: string } }) {
  const { id, table, field, value, onChange, invalid, disabled, reference, describedBy } = props;
  const Custom = customize().inputs?.[`${table.name}.${field.name}`];
  if (Custom) return <Custom {...props} />;

  const target = reference ? tableNamed(reference.table) : undefined;
  if (reference && target) return <Combobox {...props} target={target} column={reference.column} />;

  const text = typeof value === "string" ? value : "";
  switch (field.kind) {
    case "enum":
      return (
        <div className={cx("input", "input--select", invalid && "is-invalid", disabled && "is-disabled")}>
          <select id={id} value={text} onChange={(event) => onChange(event.target.value)} disabled={disabled} aria-invalid={invalid || undefined} aria-describedby={describedBy}>
            {(field.nullable || field.defaulted || !field.options?.includes(text)) && <option value="">{field.nullable ? "Not set" : field.defaulted ? "The database's default" : "Choose…"}</option>}
            {(field.options ?? []).map((option) => <option key={option} value={option}>{option === "" ? "(empty)" : option}</option>)}
          </select>
          <Icon name="chevron-down" size={15} className="input__chevron" />
        </div>
      );
    case "set":
      return <Choices id={id} options={field.options ?? []} chosen={text === "" ? [] : text.split(",")} onChange={(chosen) => onChange(chosen.join(","))} disabled={disabled} describedBy={describedBy} />;
    case "list": {
      const items = Array.isArray(value) ? value : [];
      if (field.options) return <Choices id={id} options={field.options} chosen={items} onChange={onChange} disabled={disabled} describedBy={describedBy} />;
      return <ListInput id={id} field={field} items={items} onChange={onChange} invalid={invalid} disabled={disabled} describedBy={describedBy} />;
    }
    case "boolean":
      if (field.nullable)
        return (
          <div className="segmented" role="radiogroup" aria-labelledby={id + "-label"}>
            {([[true, "Yes"], [false, "No"], [null, "Not set"]] as const).map(([option, label]) => (
              <button key={label} type="button" role="radio" aria-checked={value === option} className={cx("segmented__option", value === option && "is-on")} onClick={() => onChange(option)} disabled={disabled}>
                {label}
              </button>
            ))}
          </div>
        );
      return (
        <button id={id} type="button" role="switch" aria-checked={value === true} className={cx("switch", value === true && "is-on")} onClick={() => onChange(value !== true)} disabled={disabled} aria-describedby={describedBy}>
          <span className="switch__track"><span className="switch__thumb" /></span>
          <span className="switch__label">{value === true ? "Yes" : "No"}</span>
        </button>
      );
    case "multiline":
    case "json":
    case "binary":
      return <TextArea id={id} value={text} onChange={onChange} invalid={invalid} disabled={disabled} mono={field.kind !== "multiline"} describedBy={describedBy} />;
    default: {
      const numeric = field.kind === "integer" || field.kind === "decimal" || field.kind === "money";
      const type = field.kind === "date" ? "date" : field.kind === "datetime" ? "datetime-local" : field.kind === "time" ? "time"
        : field.kind === "email" ? "email" : field.kind === "phone" ? "tel" : field.kind === "url" ? "url" : "text";
      const lead = leadIcons[field.kind];
      return (
        <div className={cx("input", invalid && "is-invalid", disabled && "is-disabled", numeric && "input--numeric")}>
          {field.kind === "money" && <span className="input__prefix">{currencySymbol()}</span>}
          {lead && <Icon name={lead} size={15} className="input__lead" />}
          <input
            id={id}
            type={type}
            value={text}
            onChange={(event) => onChange(event.target.value)}
            disabled={disabled}
            inputMode={field.kind === "integer" || field.kind === "bits" ? "numeric" : numeric ? "decimal" : undefined}
            step={field.kind === "time" ? 1 : undefined}
            autoComplete="off"
            spellCheck={field.kind === "text" && !field.mono}
            maxLength={field.kind === "bits" ? field.maxLength : undefined}
            className={cx(field.mono && "is-mono")}
            aria-invalid={invalid || undefined}
            aria-describedby={describedBy}
          />
          {(field.kind === "date" || field.kind === "datetime" || field.kind === "time") && !disabled && (
            <button type="button" className="input__action" onClick={() => onChange(nowValue(field.kind))}>Now</button>
          )}
        </div>
      );
    }
  }
}

const pad = (n: number) => String(n).padStart(2, "0");
function nowValue(kind: Field["kind"]): string {
  const now = new Date();
  const day = `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
  const time = `${pad(now.getHours())}:${pad(now.getMinutes())}`;
  return kind === "date" ? day : kind === "time" ? `${time}:${pad(now.getSeconds())}` : `${day}T${time}`;
}

let symbol: string | undefined;
function currencySymbol(): string {
  if (symbol === undefined) {
    const parts = new Intl.NumberFormat(customize().locale, { style: "currency", currency: customize().currency ?? "USD" }).formatToParts(0);
    symbol = parts.find((part) => part.type === "currency")?.value ?? "$";
  }
  return symbol;
}

export interface FormFieldProps {
  table: Table;
  field: Field;
  value: FormValue;
  values: Record<string, FormValue>;
  onChange: (value: FormValue) => void;
  error?: string;
  disabled?: boolean;
  /** Shown instead of the input (a key that can't change). */
  readOnly?: ReactNode;
  reference?: { table: string; column: string; label: string };
  /** A new row's form: a field the database has a default for may be left empty. */
  creating?: boolean;
}

/** A field of a form: its label, input, help or error, and counter. */
export function FormField({ table, field, value, values, onChange, error, disabled, readOnly, reference, creating }: FormFieldProps) {
  const id = `field-${table.name}-${field.name}`;
  const text = typeof value === "string" ? value : "";
  const counted = field.maxLength !== undefined && !readOnly && !reference;
  const ratio = counted ? text.length / field.maxLength! : 0;
  const described = field.help ?? (reference ? `Refers to ${tableNamed(reference.table)?.singular.toLowerCase() ?? reference.table}` : describeField(field));
  const help = creating && field.defaulted && field.kind !== "boolean" ? (described ? described + " · " : "") + "empty: the database's default" : described;
  const wide = field.kind === "multiline" || field.kind === "json" || field.kind === "binary";
  return (
    <div className={cx("field", wide && "field--wide", error && "has-error")}>
      <div className="field__top">
        <label className="field__label" htmlFor={id} id={id + "-label"}>
          {field.label}
          {field.required && !readOnly && <span className="field__required" aria-label="required">*</span>}
        </label>
        {field.key && <span className="field__tag"><Icon name="key" size={11} />Key</span>}
      </div>
      {readOnly ?? (
        <FieldInput id={id} table={table} field={field} value={value} values={values} onChange={onChange} invalid={!!error} disabled={disabled}
          describedBy={`${id}-note`} reference={reference} />
      )}
      <div className="field__bottom" id={`${id}-note`}>
        {error
          ? <span className="field__error" role="alert"><Icon name="alert-circle" size={13} />{error}</span>
          : <span className="field__help">{readOnly ? "Part of the key: it can't change" : help}</span>}
        {counted && (
          <span className={cx("field__counter", ratio > 1 ? "is-over" : ratio >= 0.9 && "is-near")} aria-live={ratio >= 0.9 ? "polite" : "off"}>
            {formatInteger(text.length)}/{formatInteger(field.maxLength!)}
          </span>
        )}
      </div>
    </div>
  );
}
