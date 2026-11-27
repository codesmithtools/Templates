// A field's value, as a list or a row's page shows it: numbers and money in the locale, dates with how long ago,
// bools as a check, a status as a badge, what it refers to as a link to that row. customize.tsx's cells replace it.
import { customize } from "./config.ts";
import { formatRelative, formatValue, parseDate, rowLabel, rowPath } from "./format.ts";
import { Icon } from "./icons.tsx";
import { Link } from "./router.tsx";
import type { Field, Row, Table } from "./types.ts";
import { Badge, Skeleton, cx, type Tone } from "./ui.tsx";

const tones: [RegExp, Tone][] = [
  [/^(active|approved|open|paid|shipped|delivered|complete|completed|done|ok|enabled|published|live|success|succeeded|available|in stock)$/i, "success"],
  [/^(pending|new|draft|waiting|on hold|hold|processing|review|in review|backordered|low|partial|scheduled)$/i, "warning"],
  [/^(cancelled|canceled|closed|failed|error|rejected|inactive|disabled|deleted|refunded|out of stock|blocked|expired)$/i, "danger"],
];

/** A status's tone, by what it says (a code of its own is neutral). */
export const toneOf = (value: string): Tone => tones.find(([pattern]) => pattern.test(value.trim()))?.[1] ?? "neutral";

/** What a reference's cell shows: the row it refers to (null: none; undefined: not read yet). */
export interface Referred {
  table: Table;
  row: Row | null | undefined;
}

export interface ValueProps {
  table: Table;
  field: Field;
  row: Row;
  /** The row the field refers to, where it's a foreign key. */
  referred?: Referred;
  /** On a row's page: whole text, and email and web addresses as links. */
  full?: boolean;
}

export function Value({ table, field, row, referred, full }: ValueProps) {
  const value = row[field.name];
  const Custom = customize().cells?.[`${table.name}.${field.name}`];
  if (Custom) return <Custom table={table} field={field} row={row} value={value} />;
  if (value === null || value === undefined || value === "" || (Array.isArray(value) && value.length === 0)) return <span className="value-none" aria-label="None">—</span>;

  if (referred) {
    if (referred.row === undefined) return <span className="ref-chip ref-chip--loading"><Skeleton width={72} height={10} /></span>;
    if (referred.row === null)
      return <span className={cx("value-text", field.mono && "is-mono")} title={`No ${referred.table.singular.toLowerCase()} has it`}>{String(value)}</span>;
    return (
      <Link href={rowPath(referred.table, referred.row)} className="ref-chip" onClick={(event) => event.stopPropagation()} title={`${referred.table.singular} ${String(value)}`}>
        <Icon name={referred.table.icon} size={13} />
        <span className="ref-chip__label">{rowLabel(referred.table, referred.row)}</span>
      </Link>
    );
  }

  const text = formatValue(field, value);
  switch (field.kind) {
    case "list":
    case "set": {
      const items = Array.isArray(value) ? value : String(value).split(",");
      return (
        <span className="value-chips" title={text}>
          {items.map((item, i) => <span key={i} className="chip"><span className={cx("chip__text", (item === null || item === "") && "is-none")}>{item === null ? "null" : item === "" ? "empty" : String(item)}</span></span>)}
        </span>
      );
    }
    case "boolean":
      return value
        ? <span className="value-bool value-bool--yes"><Icon name="check" size={14} strokeWidth={2.25} />Yes</span>
        : <span className="value-bool">No</span>;
    case "date":
    case "datetime": {
      const date = parseDate(value);
      return (
        <time className="value-date" dateTime={String(value)} title={date ? formatRelative(date) : undefined}>
          {text}
          {full && date && <span className="value-date__relative">{formatRelative(date)}</span>}
        </time>
      );
    }
    case "email":
      return full ? <a className="value-link" href={`mailto:${String(value)}`}>{text}</a> : <span className="value-text">{text}</span>;
    case "url":
      return full ? <a className="value-link" href={String(value)} target="_blank" rel="noreferrer">{text}<Icon name="arrow-up-right" size={12} /></a> : <span className="value-text">{text}</span>;
    case "phone":
      return full ? <a className="value-link" href={`tel:${String(value)}`}>{text}</a> : <span className="value-text">{text}</span>;
    case "multiline":
    case "json":
      return <span className={cx("value-text", full ? "value-text--full" : "value-text--clip", field.kind === "json" && "is-mono")} title={full ? undefined : text}>{text}</span>;
    default:
      if (field.badge) return <Badge tone={toneOf(text)} dot>{text}</Badge>;
      return <span className={cx("value-text", field.mono && "is-mono")}>{text}</span>;
  }
}
