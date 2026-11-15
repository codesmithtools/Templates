// Keyboard shortcuts: useShortcut("n", ...) while a page is showing. Single keys don't fire while you type in a field
// or a modal is open; "mod+s" (Ctrl or ⌘) and Escape do, where they're allowed to.
import { useEffect, useRef } from "react";
import { modalOpen } from "./overlays.tsx";

export interface ShortcutOptions {
  /** Fires while focus is in an input too (mod+s, Escape). */
  inInputs?: boolean;
  /** Fires while a modal is open too. */
  inModals?: boolean;
  enabled?: boolean;
}

const typing = (target: EventTarget | null) =>
  target instanceof HTMLElement && (target.isContentEditable || ["INPUT", "TEXTAREA", "SELECT"].includes(target.tagName));

/** Whether an event is a shortcut's keys: "n", "/", "?", "escape", "mod+s", "mod+k", "arrowdown". */
export function matches(event: KeyboardEvent, keys: string): boolean {
  const parts = keys.toLowerCase().split("+");
  const key = parts[parts.length - 1];
  const mod = parts.includes("mod");
  if (mod !== (event.ctrlKey || event.metaKey)) return false;
  if (!mod && event.altKey) return false;
  return event.key.toLowerCase() === key;
}

/** Calls a handler on a shortcut's keys (any of several, separated by commas: "j, arrowdown"). */
export function useShortcut(keys: string, handler: (event: KeyboardEvent) => void, options: ShortcutOptions = {}) {
  const latest = useRef(handler);
  latest.current = handler;
  const { inInputs = false, inModals = false, enabled = true } = options;
  useEffect(() => {
    if (!enabled) return;
    const all = keys.split(",").map((key) => key.trim());
    const keydown = (event: KeyboardEvent) => {
      if (event.defaultPrevented || event.isComposing) return;
      if (!inInputs && typing(event.target)) return;
      if (!inModals && modalOpen()) return;
      if (!all.some((key) => matches(event, key))) return;
      event.preventDefault();
      latest.current(event);
    };
    window.addEventListener("keydown", keydown);
    return () => window.removeEventListener("keydown", keydown);
  }, [keys, inInputs, inModals, enabled]);
}
