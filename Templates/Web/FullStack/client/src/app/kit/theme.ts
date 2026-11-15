// Light, dark, or the system's: set on <html> as data-theme (theme.css), and kept in the browser where it can be.
import { useSyncExternalStore } from "react";

export type ThemeChoice = "system" | "light" | "dark";

const read = (): ThemeChoice => {
  try {
    const saved = localStorage.getItem("theme");
    return saved === "light" || saved === "dark" ? saved : "system";
  } catch {
    return "system";
  }
};

let choice = read();
const listeners = new Set<() => void>();

const apply = () => {
  if (choice === "system") delete document.documentElement.dataset.theme;
  else document.documentElement.dataset.theme = choice;
};

// before the first render: no flash of the other theme
apply();

function choose(next: ThemeChoice) {
  choice = next;
  apply();
  try {
    if (next === "system") localStorage.removeItem("theme");
    else localStorage.setItem("theme", next);
  } catch {
    // private browsing: it lasts as long as the page
  }
  listeners.forEach((listener) => listener());
}

const subscribe = (listener: () => void) => {
  listeners.add(listener);
  return () => void listeners.delete(listener);
};

export function useTheme(): [ThemeChoice, (choice: ThemeChoice) => void] {
  return [useSyncExternalStore(subscribe, () => choice), choose];
}
