// A row's text: what it's shown by, its key and its page's path, and a field's value as text (numbers, money and
// dates in the locale, customize.tsx's or the browser's).
import { customize, fieldNamed } from "./config.ts";
import type { Field, Row, Table } from "./types.ts";

const numbers = new Map<string, Intl.NumberFormat>();
const formatter = (key: string, make: () => Intl.NumberFormat) => {
  let format = numbers.get(key);
  if (!format) numbers.set(key, (format = make()));
  return format;
};

const locale = () => customize().locale;

export function formatInteger(value: number, grouped = true): string {
  return grouped ? formatter("integer", () => new Intl.NumberFormat(locale(), { maximumFractionDigits: 0 })).format(value) : String(value);
}

export function formatDecimal(value: number, scale?: number): string {
  return formatter(`decimal:${scale ?? ""}`, () =>
    new Intl.NumberFormat(locale(), { minimumFractionDigits: 0, maximumFractionDigits: scale ?? 6 })).format(value);
}

export function formatMoney(value: number): string {
  const currency = customize().currency ?? "USD";
  return formatter(`money:${currency}`, () => new Intl.NumberFormat(locale(), { style: "currency", currency })).format(value);
}

/** A compact count: 1.2K. */
export function formatCount(value: number): string {
  return value < 10000 ? formatInteger(value) : formatter("compact", () => new Intl.NumberFormat(locale(), { notation: "compact" })).format(value);
}

/** A date or time from the API (ISO 8601), as a Date; undefined where it isn't one. */
export function parseDate(value: unknown): Date | undefined {
  if (typeof value !== "string" || !value) return undefined;
  const date = new Date(/^\d{4}-\d{2}-\d{2}$/.test(value) ? value + "T00:00:00" : value);
  return Number.isNaN(date.getTime()) ? undefined : date;
}

export function formatDate(date: Date, withTime: boolean): string {
  return date.toLocaleString(locale(), withTime
    ? { year: "numeric", month: "short", day: "numeric", hour: "numeric", minute: "2-digit" }
    : { year: "numeric", month: "short", day: "numeric" });
}

const relative = () => new Intl.RelativeTimeFormat(locale(), { numeric: "auto" });

/** How long ago (or from now): 3 days ago, in 2 hours. */
export function formatRelative(date: Date, now = new Date()): string {
  const seconds = (date.getTime() - now.getTime()) / 1000;
  const units: [Intl.RelativeTimeFormatUnit, number][] = [["year", 31536000], ["month", 2592000], ["week", 604800], ["day", 86400], ["hour", 3600], ["minute", 60]];
  for (const [unit, size] of units)
    if (Math.abs(seconds) >= size) return relative().format(Math.round(seconds / size), unit);
  return relative().format(Math.round(seconds), "second");
}

/** A field's value as text: "" for nothing. */
export function formatValue(field: Field, value: unknown): string {
  if (value === null || value === undefined || value === "") return "";
  switch (field.kind) {
    case "integer":
      return typeof value === "number" ? formatInteger(value, !field.mono) : String(value);
    case "decimal":
      return typeof value === "number" ? formatDecimal(value, field.scale) : String(value);
    case "money":
      return typeof value === "number" ? formatMoney(value) : String(value);
    case "boolean":
      return value ? "Yes" : "No";
    case "date":
    case "datetime": {
      const date = parseDate(value);
      return date ? formatDate(date, field.kind === "datetime") : String(value);
    }
    case "binary":
      return typeof value === "string" ? `${formatInteger(Math.floor((value.length * 3) / 4))} bytes` : "";
    default:
      return String(value);
  }
}

/** A label in a sentence: in lower case, but for what's an acronym (Category ID: category ID). */
export const lower = (label: string) => label.split(" ").map((word) => (/^[A-Z0-9]{2,}$/.test(word) ? word : word.toLowerCase())).join(" ");

// ---- rows ----

/** The fields of a row's key, and their values (what get, update and delete take). */
export function keyOf(table: Table, row: Row): Row {
  return Object.fromEntries(table.key.map((name) => [name, row[name]]));
}

/** The key as text: BD-01, or 1001 · 2. */
export function keyText(table: Table, row: Row): string {
  return table.key.map((name) => String(row[name] ?? "")).join(" · ");
}

/** What a row is shown by: its display fields (Pelican; Adam Barr), or its key (#1001 for a number). */
export function rowLabel(table: Table, row: Row): string {
  const shown = table.display.map((name) => row[name]).filter((value) => value !== null && value !== undefined && value !== "").join(" ");
  if (shown) return shown;
  if (table.key.length === 1 && typeof row[table.key[0]] === "number") return "#" + String(row[table.key[0]]);
  return keyText(table, row) || table.singular;
}

/** A row's title on its page: what it's shown by, or the table's singular and its key (Order #1001). */
export function rowTitle(table: Table, row: Row): string {
  return table.display.length && table.display.some((name) => row[name] !== null && row[name] !== undefined && row[name] !== "")
    ? rowLabel(table, row)
    : `${table.singular} ${rowLabel(table, row)}`;
}

/** A row's page: /products/BD-01, /line-items/1001/2. */
export function rowPath(table: Table, row: Row): string {
  return "/" + table.name + table.key.map((name) => "/" + encodeURIComponent(String(row[name] ?? ""))).join("");
}

/** A row's key from its page's parameters, as the key's types have it (numbers as numbers). */
export function keyFromParams(table: Table, params: Record<string, string>): Row {
  return Object.fromEntries(table.key.map((name) => {
    const field = fieldNamed(table, name);
    const text = params[name] ?? "";
    const number = Number(text);
    return [name, field && (field.kind === "integer" || field.kind === "decimal" || field.kind === "money") && text !== "" && !Number.isNaN(number) ? number : text];
  }));
}

/** Whether a field is a number (right-aligned, in tabular figures). */
export const isNumeric = (field: Field) => field.kind === "integer" || field.kind === "decimal" || field.kind === "money";

/** A field's description for a form: what it takes. */
export function describeField(field: Field): string {
  switch (field.kind) {
    case "integer": return "A whole number";
    case "decimal": return field.scale !== undefined ? `A number, up to ${field.scale} decimal place${field.scale === 1 ? "" : "s"}` : "A number";
    case "money": return field.scale !== undefined ? `An amount, up to ${field.scale} decimal place${field.scale === 1 ? "" : "s"}` : "An amount";
    case "email": return "An email address";
    case "phone": return "A phone number";
    case "url": return "A web address";
    case "uuid": return "A GUID";
    case "date": return "A date";
    case "datetime": return "A date and time";
    case "time": return "A time of day";
    case "json": return "JSON";
    case "binary": return "Base64";
    default: return "";
  }
}
