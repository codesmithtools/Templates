// The command palette (Ctrl+K or ⌘K): go to a table, make a row, or find a row in any table that searches (as you
// type, a few from each). And the shortcuts' help (?).
import { useEffect, useMemo, useRef, useState, type KeyboardEvent } from "react";
import { tables } from "./config.ts";
import { keyText, rowLabel, rowPath } from "./format.ts";
import { Icon } from "./icons.tsx";
import { Modal } from "./overlays.tsx";
import { navigate } from "./router.tsx";
import type { Row, Table } from "./types.ts";
import { Kbd, Spinner, cx, modKey } from "./ui.tsx";

interface Command {
  id: string;
  group: string;
  icon: string;
  label: string;
  hint?: string;
  run: () => void;
}

export function CommandPalette({ onClose, extra = [] }: { onClose: () => void; extra?: Omit<Command, "id">[] }) {
  const [text, setText] = useState("");
  const [active, setActive] = useState(0);
  const [found, setFound] = useState<{ table: Table; rows: Row[] }[]>([]);
  const [searching, setSearching] = useState(false);
  const list = useRef<HTMLDivElement>(null);
  const query = text.trim().toLowerCase();

  // rows: a few of each table that searches, a moment after typing stops
  useEffect(() => {
    if (query.length < 2) {
      setFound([]);
      setSearching(false);
      return;
    }
    let current = true;
    setSearching(true);
    const timer = window.setTimeout(async () => {
      const results = await Promise.all(tables().filter((table) => table.search).map(async (table) => {
        try {
          return { table, rows: (await table.api.list({ search: text.trim(), pageSize: 3 })).items };
        } catch {
          return { table, rows: [] };
        }
      }));
      if (!current) return;
      setFound(results.filter((result) => result.rows.length > 0));
      setSearching(false);
    }, 200);
    return () => {
      current = false;
      window.clearTimeout(timer);
    };
  }, [query]);

  const go = (path: string) => () => {
    onClose();
    void navigate(path);
  };

  const commands = useMemo(() => {
    const all: Command[] = [];
    const matches = (label: string) => !query || label.toLowerCase().includes(query);
    for (const table of tables())
      if (matches(table.title) || matches(table.table)) all.push({ id: "go:" + table.name, group: "Tables", icon: table.icon, label: table.title, hint: table.table, run: go(`/${table.name}`) });
    if (matches("overview") || matches("dashboard") || matches("home")) all.push({ id: "go:/", group: "Tables", icon: "grid", label: "Overview", run: go("/") });
    for (const table of tables())
      if (table.api.create && (matches(`new ${table.singular}`) || matches(`create ${table.singular}`) || (query.length > 1 && matches(table.title))))
        all.push({ id: "new:" + table.name, group: "Make", icon: "plus", label: `New ${table.singular.toLowerCase()}`, hint: table.title, run: go(`/${table.name}/new`) });
    extra.forEach((command, i) => matches(command.label) && all.push({ ...command, id: "extra:" + i }));
    for (const { table, rows } of found)
      for (const row of rows)
        all.push({ id: `row:${table.name}:${keyText(table, row)}`, group: `${table.title}`, icon: table.icon, label: rowLabel(table, row), hint: keyText(table, row), run: table.key.length ? go(rowPath(table, row)) : go(`/${table.name}?q=${encodeURIComponent(text.trim())}`) });
    return all;
  }, [query, found]);

  useEffect(() => setActive(0), [query, found]);
  useEffect(() => {
    list.current?.querySelector<HTMLElement>(`[data-index="${active}"]`)?.scrollIntoView({ block: "nearest" });
  }, [active]);

  const keydown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "ArrowDown") {
      event.preventDefault();
      setActive((i) => Math.min(i + 1, commands.length - 1));
    } else if (event.key === "ArrowUp") {
      event.preventDefault();
      setActive((i) => Math.max(i - 1, 0));
    } else if (event.key === "Enter") {
      event.preventDefault();
      commands[active]?.run();
    }
  };

  let index = -1;
  const groups = [...new Set(commands.map((command) => command.group))];
  return (
    <Modal onClose={onClose} className="palette" label="Command palette" top>
      <div className="palette__search">
        <Icon name="search" size={17} />
        <input value={text} onChange={(event) => setText(event.target.value)} onKeyDown={keydown} placeholder="Go to a table, make a row, or find one…"
          aria-label="Search" role="combobox" aria-expanded="true" aria-controls="palette-list" aria-activedescendant={commands[active] ? `palette-${active}` : undefined} data-autofocus="" />
        {searching ? <Spinner size={15} /> : <Kbd>Esc</Kbd>}
      </div>
      <div ref={list} className="palette__list" id="palette-list" role="listbox" aria-label="Commands">
        {commands.length === 0 && !searching && <div className="palette__empty">Nothing matches “{text}”.</div>}
        {groups.map((group) => (
          <div key={group} className="palette__group" role="group" aria-label={group}>
            <div className="palette__heading">{group}</div>
            {commands.filter((command) => command.group === group).map((command) => {
              index++;
              const i = index;
              return (
                <div key={command.id} id={`palette-${i}`} data-index={i} role="option" aria-selected={i === active}
                  className={cx("palette__item", i === active && "is-active")} onMouseMove={() => setActive(i)} onClick={command.run}>
                  <span className="palette__icon"><Icon name={command.icon} size={15} /></span>
                  <span className="palette__label">{command.label}</span>
                  {command.hint && <span className="palette__hint is-mono">{command.hint}</span>}
                  {i === active && <Icon name="enter" size={14} className="palette__enter" />}
                </div>
              );
            })}
          </div>
        ))}
      </div>
      <div className="palette__footer">
        <span><Kbd>↑</Kbd><Kbd>↓</Kbd> to move</span>
        <span><Kbd>↵</Kbd> to go</span>
        <span className="palette__footer-end">Type two letters to search every table</span>
      </div>
    </Modal>
  );
}

const shortcuts: [string, [string, string][]][] = [
  ["Anywhere", [["Search, go, make", `${modKey} K`], ["These shortcuts", "?"]]],
  ["A list", [["Search it", "/"], ["New row", "N"], ["Next or previous row", "J K"], ["Open the row", "↵"], ["Edit the row", "E"]]],
  ["A row", [["Edit it", "E"], ["Delete it", "⌫"]]],
  ["A form", [["Save", `${modKey} S`], ["Cancel", "Esc"]]],
];

export function ShortcutsHelp({ onClose }: { onClose: () => void }) {
  return (
    <Modal onClose={onClose} className="help" label="Keyboard shortcuts">
      <div className="help__header">
        <Icon name="keyboard" size={18} />
        <h2>Keyboard shortcuts</h2>
        <button type="button" className="help__close" aria-label="Close" onClick={onClose} data-autofocus=""><Icon name="x" size={16} /></button>
      </div>
      <div className="help__groups">
        {shortcuts.map(([group, keys]) => (
          <section key={group} className="help__group">
            <h3>{group}</h3>
            <dl>
              {keys.map(([what, key]) => (
                <div key={what} className="help__row"><dt>{what}</dt><dd>{key.split(" ").map((k) => <Kbd key={k}>{k}</Kbd>)}</dd></div>
              ))}
            </dl>
          </section>
        ))}
      </div>
    </Modal>
  );
}
