// The app's shell: the sidebar (the tables, with how many rows each has), the top bar (where you are, and the page's
// actions), and the page the address names: the overview, or a table's list, row, new row or edit.
import { useEffect, useState } from "react";
import { routes } from "../generated/ui/index.ts";
import {
  CommandPalette, Dashboard, EmptyState, Icon, Kbd, Link, Overlays, ShortcutsHelp, SlotsContext, appName, customize, cx, formatCount,
  match, modKey, rowCounts, tables, theDatabase, useLocation, useQuery, useShortcut, useTheme, type RowCount, type ThemeChoice,
} from "./kit/index.ts";

function Sidebar({ open, onClose, onSearch, onHelp }: { open: boolean; onClose: () => void; onSearch: () => void; onHelp: () => void }) {
  const location = useLocation();
  const counts = useQuery<RowCount[]>("counts", rowCounts, 5000);
  const [theme, setTheme] = useTheme();
  const at = (path: string) => location.path === path || location.path.startsWith(path + "/");
  useEffect(onClose, [location.path]);
  const themes: [ThemeChoice, string, string][] = [["light", "sun", "Light"], ["dark", "moon", "Dark"], ["system", "monitor", "System"]];
  return (
    <aside className={cx("sidebar", open && "is-open")} aria-label="Navigation">
      <div className="sidebar__brand">
        <Link href="/" className="brand">
          <span className="brand__mark"><Icon name="database" size={17} strokeWidth={2} /></span>
          <span className="brand__text">
            <span className="brand__name">{appName()}</span>
            <span className="brand__sub">{theDatabase().provider} · Admin</span>
          </span>
        </Link>
        <button type="button" className="sidebar__close" aria-label="Close the menu" onClick={onClose}><Icon name="x" size={18} /></button>
      </div>
      <button type="button" className="sidebar__search" onClick={onSearch}>
        <Icon name="search" size={15} />
        <span>Search</span>
        <Kbd>{modKey} K</Kbd>
      </button>
      <nav className="nav">
        <Link href="/" className={cx("nav__item", location.path === "/" && "is-active")} aria-current={location.path === "/" ? "page" : undefined}>
          <Icon name="grid" size={16} className="nav__icon" />
          <span className="nav__label">Overview</span>
        </Link>
        <div className="nav__heading">
          <span>Tables</span>
          <span className="nav__heading-count">{tables().length}</span>
        </div>
        {tables().map((table) => {
          const count = counts.shown?.find((c) => c.table === table.name)?.rows;
          const active = at("/" + table.name);
          return (
            <Link key={table.name} href={`/${table.name}`} className={cx("nav__item", active && "is-active")} aria-current={active ? "page" : undefined}>
              <Icon name={table.icon} size={16} className="nav__icon" />
              <span className="nav__label">{table.title}</span>
              <span className={cx("nav__count", count === 0 && "is-zero")}>{count === undefined ? "" : formatCount(count)}</span>
            </Link>
          );
        })}
      </nav>
      <div className="sidebar__footer">
        <div className="theme-switch" role="radiogroup" aria-label="Theme">
          {themes.map(([value, icon, label]) => (
            <button key={value} type="button" role="radio" aria-checked={theme === value} title={label} aria-label={label} className={cx("theme-switch__option", theme === value && "is-on")} onClick={() => setTheme(value)}>
              <Icon name={icon} size={15} />
            </button>
          ))}
        </div>
        <div className="sidebar__links">
          <button type="button" className="sidebar__link" onClick={onHelp} title="Keyboard shortcuts" aria-label="Keyboard shortcuts"><Icon name="keyboard" size={16} /></button>
          <a className="sidebar__link" href="/openapi/v1.json" target="_blank" rel="noreferrer" title="The API's OpenAPI document" aria-label="The API's OpenAPI document"><Icon name="code" size={16} /></a>
        </div>
      </div>
    </aside>
  );
}

function NotFound() {
  return (
    <div className="page">
      <div className="card">
        <EmptyState icon="search" title="There's no page here" actions={<Link href="/" className="btn btn--secondary"><Icon name="arrow-left" size={16} /><span className="btn__label">The overview</span></Link>}>
          Its address may have changed, or its table isn't in the database any more.
        </EmptyState>
      </div>
    </div>
  );
}

export function App() {
  const location = useLocation();
  const [crumbs, setCrumbs] = useState<HTMLElement | null>(null);
  const [actions, setActions] = useState<HTMLElement | null>(null);
  const [menu, setMenu] = useState(false);
  const [palette, setPalette] = useState(false);
  const [help, setHelp] = useState(false);
  const [, setTheme] = useTheme();
  useShortcut("mod+k", () => setPalette((open) => !open), { inInputs: true });
  useShortcut("?", () => setHelp(true));

  // the page: the overview, the first route whose path matches (a page of yours, where customize.tsx has one), or none
  let page = <NotFound />;
  if (location.path === "/") page = <Dashboard />;
  else
    for (const route of routes) {
      const params = match(route.path, location.path);
      if (!params) continue;
      const Page = customize().pages?.[`${route.table.name}.${route.page}`] ?? route.component;
      page = <Page key={route.path + JSON.stringify(params)} params={params} />;
      break;
    }

  return (
    <div className="app">
      <a href="#content" className="skip-link">Skip to the content</a>
      <Sidebar open={menu} onClose={() => setMenu(false)} onSearch={() => setPalette(true)} onHelp={() => setHelp(true)} />
      {menu && <div className="sidebar-backdrop" onClick={() => setMenu(false)} aria-hidden="true" />}
      <div className="main">
        <header className="topbar">
          <button type="button" className="topbar__menu" aria-label="Open the menu" onClick={() => setMenu(true)}><Icon name="menu" size={18} /></button>
          <nav ref={setCrumbs} className="crumbs" aria-label="Breadcrumb" />
          <div ref={setActions} className="topbar__actions" />
        </header>
        <main id="content" className="content">
          <SlotsContext.Provider value={{ crumbs, actions }}>{page}</SlotsContext.Provider>
        </main>
      </div>
      <Overlays />
      {palette && (
        <CommandPalette onClose={() => setPalette(false)} extra={[
          { group: "Theme", icon: "sun", label: "Light theme", run: () => { setTheme("light"); setPalette(false); } },
          { group: "Theme", icon: "moon", label: "Dark theme", run: () => { setTheme("dark"); setPalette(false); } },
          { group: "Theme", icon: "monitor", label: "System theme", run: () => { setTheme("system"); setPalette(false); } },
          { group: "Help", icon: "keyboard", label: "Keyboard shortcuts", run: () => { setPalette(false); setHelp(true); } },
        ]} />
      )}
      {help && <ShortcutsHelp onClose={() => setHelp(false)} />}
    </div>
  );
}
