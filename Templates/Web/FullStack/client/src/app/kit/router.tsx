// A small router over the History API: the location as a hook, navigation (which a page with unsaved changes can
// hold up), links, and matching a route's path (/products/:productId) to the location.
import { useMemo, useSyncExternalStore, type AnchorHTMLAttributes, type MouseEvent } from "react";

const listeners = new Set<() => void>();
const notify = () => listeners.forEach((listener) => listener());
window.addEventListener("popstate", notify);

const subscribe = (listener: () => void) => {
  listeners.add(listener);
  return () => void listeners.delete(listener);
};
const current = () => location.pathname + location.search;

export interface Location {
  path: string;
  search: URLSearchParams;
  /** The path and the search: /products?q=bird. */
  href: string;
}

/** Where the app is; a change renders again. */
export function useLocation(): Location {
  const href = useSyncExternalStore(subscribe, current);
  return useMemo(() => {
    const url = new URL(href, location.origin);
    return { path: url.pathname, search: url.searchParams, href };
  }, [href]);
}

// What asks before the app goes elsewhere: a form with changes that aren't saved.
let guard: (() => Promise<boolean>) | undefined;

/** Holds up navigation while a page has something to lose: it asks (true goes on). Returns what lets go. */
export function setGuard(ask: () => Promise<boolean>): () => void {
  guard = ask;
  return () => {
    if (guard === ask) guard = undefined;
  };
}

export interface NavigateOptions {
  /** Instead of the current entry in the history (a change of search or page). */
  replace?: boolean;
  /** Without asking the guard (after a save). */
  force?: boolean;
}

/** Goes to a path in the app: false where the guard held it up. */
export async function navigate(to: string, options: NavigateOptions = {}): Promise<boolean> {
  if (!options.force && guard && !(await guard())) return false;
  if (to !== current()) {
    history[options.replace ? "replaceState" : "pushState"](null, "", to);
    notify();
    if (!options.replace) window.scrollTo({ top: 0 });
  }
  return true;
}

/** The location with its search changed: values of "" or null leave a parameter out. */
export function withSearch(location: Location, changes: Record<string, string | number | null | undefined>): string {
  const search = new URLSearchParams(location.search);
  for (const [name, value] of Object.entries(changes)) {
    if (value === null || value === undefined || value === "") search.delete(name);
    else search.set(name, String(value));
  }
  const text = search.toString();
  return location.path + (text ? "?" + text : "");
}

/** A route's parameters where its path matches (/products/:productId and /products/BD-01: { productId: "BD-01" }). */
export function match(pattern: string, path: string): Record<string, string> | undefined {
  const parts = pattern.split("/").filter(Boolean);
  const segments = path.split("/").filter(Boolean);
  if (parts.length !== segments.length) return undefined;
  const params: Record<string, string> = {};
  for (let i = 0; i < parts.length; i++) {
    if (parts[i].startsWith(":")) params[parts[i].slice(1)] = decode(segments[i]);
    else if (parts[i] !== segments[i]) return undefined;
  }
  return params;
}

const decode = (segment: string) => {
  try {
    return decodeURIComponent(segment);
  } catch {
    return segment;
  }
};

export type LinkProps = AnchorHTMLAttributes<HTMLAnchorElement> & { href: string; replace?: boolean };

/** A link in the app: navigates without loading the page (a modified click opens a tab, as a link does). */
export function Link({ href, replace, onClick, ...rest }: LinkProps) {
  const click = (event: MouseEvent<HTMLAnchorElement>) => {
    onClick?.(event);
    if (event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey || rest.target) return;
    event.preventDefault();
    void navigate(href, { replace });
  };
  return <a href={href} onClick={click} {...rest} />;
}
