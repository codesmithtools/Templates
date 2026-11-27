// A form's values: from a row (or a new row's defaults), to the request the API takes, and checked as the field's
// kind and limits say before it's sent; then the request's schema (the client's Zod schema) checks the rest. What
// the API says is wrong (a ProblemDetails) goes back to the fields it names.
import type { z } from "zod";
import { formatDecimal, formatInteger, formatMoney } from "./format.ts";
import type { Field, FieldKind, FormValue, Row, Table } from "./types.ts";

const pad = (n: number) => String(n).padStart(2, "0");

/** A field's entry in a map of the form's (its errors, what's been touched): its own, not one every object has (a
 * field named constructor or valueOf). */
export const fieldEntry = <T,>(map: Record<string, T>, name: string): T | undefined => (Object.hasOwn(map, name) ? map[name] : undefined);

/** A date as a date, a datetime-local or a time input has it, in the browser's time. */
function local(date: Date, kind: FieldKind): string {
  const day = `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
  const time = `${pad(date.getHours())}:${pad(date.getMinutes())}`;
  return kind === "date" ? day : kind === "time" ? time + ":00" : `${day}T${time}`;
}

/** Now, as a date, a datetime-local or a time input has it. */
const now = (kind: FieldKind): string => local(new Date(), kind);

/** Whether two form values are the same (a list's items too). */
export const sameValue = (a: FormValue | undefined, b: FormValue | undefined): boolean =>
  Array.isArray(a) || Array.isArray(b) ? JSON.stringify(a) === JSON.stringify(b) : a === b;

/** Whether a form value is nothing: null, "" (a list's: no items). */
export const emptyValue = (value: FormValue | undefined): boolean =>
  value === null || value === undefined || value === "" || (Array.isArray(value) && value.length === 0);

/** A field's value in its input: the API's value as text (a date as its input has it, a zoned one in the browser's
 * time), a bool, a list's items as text, or null. */
export function toFormValue(field: Field, value: unknown): FormValue {
  if (field.kind === "boolean") return value === null || value === undefined ? (field.nullable ? null : false) : Boolean(value);
  if (field.kind === "list") return Array.isArray(value) ? value.map((item) => (item === null || item === undefined ? "" : String(item))) : value === null || value === undefined || value === "" ? [] : [String(value)];
  if (value === null || value === undefined) return "";
  if (typeof value === "number" && (field.kind === "money" || field.kind === "decimal") && field.scale !== undefined) return value.toFixed(field.scale);
  const text = String(value);
  switch (field.kind) {
    case "date": return text.slice(0, 10);
    case "datetime": {
      // with a time zone (Z, +02:00): in the browser's time
      if (field.zoned && /(Z|[+-]\d{2}:?\d{2})$/i.test(text)) {
        const date = new Date(text);
        if (!Number.isNaN(date.getTime())) return local(date, "datetime");
      }
      return text.replace(" ", "T").slice(0, 16);
    }
    case "time": return text.slice(0, 8);
    case "json": {
      try {
        return JSON.stringify(JSON.parse(text), null, 2);
      } catch {
        return text;
      }
    }
    default: return text;
  }
}

/** A new row's value of a field: its default (now, for "now"), or nothing. */
export function defaultFormValue(field: Field): FormValue {
  if (field.default === "now") return now(field.kind);
  if (field.default !== undefined) return toFormValue(field, field.default);
  return toFormValue(field, null);
}

const numeric = (kind: FieldKind | undefined) => kind === "integer" || kind === "decimal" || kind === "money";

/** A field's input as the API takes it: a number, a bool, text, a list, or null for nothing; undefined to leave it
 * out (a new row's column the database has a default for: it gives it). */
export function fromFormValue(field: Field, value: FormValue, creating = false): unknown {
  if (field.kind === "boolean") return value;
  if (emptyValue(value) && creating && field.defaulted) return undefined;
  if (field.kind === "list") {
    const items = Array.isArray(value) ? value : [];
    if (items.length === 0 && field.nullable) return null;
    return items.map((item) => (item === "" && field.items !== "text" ? null : numeric(field.items) ? Number(item.trim().replace(/[,\s]/g, "")) : item));
  }
  if (emptyValue(value)) return field.nullable ? null : numeric(field.kind) || field.kind === "enum" ? undefined : "";
  const text = String(value);
  switch (field.kind) {
    case "integer":
    case "decimal":
    case "money": return Number(text.trim().replace(/[,\s]/g, ""));
    case "datetime": {
      const full = text.length === 16 ? text + ":00" : text;
      // with a time zone: the instant, in UTC
      if (field.zoned) {
        const date = new Date(full);
        if (!Number.isNaN(date.getTime())) return date.toISOString();
      }
      return full;
    }
    case "time": return text.length === 5 ? text + ":00" : text;
    case "json": {
      try {
        return JSON.stringify(JSON.parse(text));
      } catch {
        return text;
      }
    }
    default: return text;
  }
}

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const uuidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

/** What's wrong with a list's item (undefined: nothing). */
function checkItem(field: Field, item: string): string | undefined {
  if (field.options) return field.options.includes(item) ? undefined : "isn't one of its values.";
  const clean = item.trim().replace(/[,\s]/g, "");
  switch (field.items) {
    case "integer": return /^[-+]?\d+$/.test(clean) ? undefined : "isn't a whole number.";
    case "decimal": return /^[-+]?(\d+\.?\d*|\.\d+)$/.test(clean) ? undefined : "isn't a number.";
    case "uuid": return uuidPattern.test(item.trim()) ? undefined : "isn't a GUID.";
    default: return undefined;
  }
}

/** What's wrong with a field's value (undefined: nothing). A new row may leave out what the database has a default
 * for. */
export function checkField(field: Field, value: FormValue, creating = false): string | undefined {
  if (field.kind === "boolean") return !field.nullable && value === null ? "Choose yes or no." : undefined;
  if (field.kind === "list") {
    const items = Array.isArray(value) ? value : [];
    const wrong = items.findIndex((item) => checkItem(field, item) !== undefined);
    return wrong < 0 ? undefined : `Item ${wrong + 1}, “${items[wrong]}”, ${checkItem(field, items[wrong])}`;
  }
  const text = typeof value === "string" ? value : "";
  if (text.trim() === "") {
    if (creating && field.defaulted) return undefined;
    return field.required || (!field.nullable && field.kind !== "text" && field.kind !== "multiline" && field.kind !== "set") ? "Required." : undefined;
  }
  if (field.kind === "bits") {
    if (!/^[01]+$/.test(text)) return "Enter 0s and 1s, like 1010.";
    if (field.minLength !== undefined && field.minLength === field.maxLength && text.length !== field.minLength) return `Enter ${formatInteger(field.minLength)} bits.`;
    if (field.maxLength !== undefined && text.length > field.maxLength) return `At most ${formatInteger(field.maxLength)} bits.`;
    return undefined;
  }
  if (field.maxLength !== undefined && text.length > field.maxLength)
    return `${formatInteger(text.length - field.maxLength)} character${text.length - field.maxLength === 1 ? "" : "s"} too long (at most ${formatInteger(field.maxLength)}).`;
  switch (field.kind) {
    case "enum": return field.options && !field.options.includes(text) ? "Choose one of its values." : undefined;
    case "set": return field.options && text.split(",").some((item) => !field.options!.includes(item)) ? "Choose from its values." : undefined;
    case "integer": {
      const clean = text.trim().replace(/[,\s]/g, "");
      if (!/^[-+]?\d+$/.test(clean)) return "Enter a whole number.";
      const number = Number(clean);
      if ((field.min !== undefined && number < field.min) || (field.max !== undefined && number > field.max))
        return `Enter a number from ${formatInteger(field.min ?? Number.MIN_SAFE_INTEGER)} to ${formatInteger(field.max ?? Number.MAX_SAFE_INTEGER)}.`;
      return undefined;
    }
    case "decimal":
    case "money": {
      const clean = text.trim().replace(/[,\s]/g, "");
      if (!/^[-+]?(\d+\.?\d*|\.\d+)$/.test(clean)) return field.kind === "money" ? "Enter an amount, like 12.50." : "Enter a number.";
      const decimals = clean.includes(".") ? clean.split(".")[1].length : 0;
      if (field.scale !== undefined && decimals > field.scale)
        return field.scale === 0 ? "No decimal places." : `At most ${field.scale} decimal place${field.scale === 1 ? "" : "s"}.`;
      if (field.precision !== undefined && field.scale !== undefined) {
        const limit = 10 ** (field.precision - field.scale);
        if (Math.abs(Number(clean)) >= limit)
          return `Must be less than ${field.kind === "money" ? formatMoney(limit) : formatDecimal(limit)}.`;
      }
      return undefined;
    }
    case "email": return emailPattern.test(text.trim()) ? undefined : "Enter an email address, like name@example.com.";
    case "url": {
      try {
        new URL(text.trim());
        return undefined;
      } catch {
        return "Enter a full web address, like https://example.com.";
      }
    }
    case "uuid": return uuidPattern.test(text.trim()) ? undefined : "Enter a GUID, like 3f2504e0-4f89-11d3-9a0c-0305e82c3301.";
    case "date":
    case "datetime": return Number.isNaN(new Date(text).getTime()) ? "Enter a date." : undefined;
    case "json": {
      try {
        JSON.parse(text);
        return undefined;
      } catch {
        return "That isn't JSON.";
      }
    }
    default: return undefined;
  }
}

/** The fields a form edits: a new row's (not what the database fills in), or a row's changes (not its key either). */
export function formFields(table: Table, editing: boolean): Field[] {
  return table.fields.filter((field) => !field.generated && !(editing && field.key));
}

/** The form's request, and what's wrong with it: each field's check, then the request's schema. */
export function buildRequest(table: Table, editing: boolean, values: Record<string, FormValue>): { request?: Row; errors: Record<string, string> } {
  const errors: Record<string, string> = {};
  const request: Row = {};
  for (const field of formFields(table, editing)) {
    const problem = checkField(field, values[field.name] ?? null, !editing);
    if (problem) errors[field.name] = problem;
    const value = fromFormValue(field, values[field.name] ?? null, !editing);
    if (value !== undefined) request[field.name] = value;
  }
  const schema: z.ZodType | undefined = editing ? table.update : table.create;
  if (schema) {
    const parsed = schema.safeParse(request);
    if (!parsed.success)
      for (const issue of parsed.error.issues) {
        const name = String(issue.path[0] ?? "");
        if (name && fieldEntry(errors, name) === undefined) errors[name] = issue.message;
      }
  }
  return Object.keys(errors).length ? { errors } : { request, errors };
}

/** A ProblemDetails' errors, by field: its keys are the API's properties (ProductId), the fields' JSON names in
 * another case (productId). What names no field is the form's. */
export function problemErrors(table: Table, errors: Record<string, string[]> | undefined): { fields: Record<string, string>; other: string[] } {
  const fields: Record<string, string> = {};
  const other: string[] = [];
  for (const [key, messages] of Object.entries(errors ?? {})) {
    const name = key.split(".").pop()!.replace(/^\$\.?/, "").replace(/\[\d+\]$/, "").toLowerCase();
    const field = table.fields.find((f) => f.name.toLowerCase() === name);
    const text = messages.join(" ");
    if (field) fields[field.name] = text;
    else other.push(text);
  }
  return { fields, other };
}
