// The small pieces every page is made of: buttons, badges, keys, a spinner, skeletons, empty states, cards, a page's
// header, and the top bar's slots (a page's breadcrumbs and actions go up there).
import { createContext, useContext, useEffect, type ButtonHTMLAttributes, type ReactNode } from "react";
import { createPortal } from "react-dom";
import { appName } from "./config.ts";
import { Icon } from "./icons.tsx";
import { Link } from "./router.tsx";

export const cx = (...names: (string | false | null | undefined)[]) => names.filter(Boolean).join(" ");

/** The key that goes with Ctrl on Windows and Linux, ⌘ on a Mac. */
export const modKey = /Mac|iPhone|iPad/.test(navigator.platform) ? "⌘" : "Ctrl";

type Variant = "primary" | "secondary" | "ghost" | "danger" | "danger-ghost";

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
  size?: "sm" | "md";
  icon?: string;
  /** Its shortcut, shown beside it. */
  shortcut?: string;
  busy?: boolean;
}

export function Button({ variant = "secondary", size = "md", icon, shortcut, busy, className, children, disabled, type = "button", ...rest }: ButtonProps) {
  return (
    <button type={type} className={cx("btn", `btn--${variant}`, size === "sm" && "btn--sm", !children && "btn--icon", busy && "is-busy", className)} disabled={disabled || busy} aria-busy={busy || undefined} {...rest}>
      {busy ? <Spinner size={14} /> : icon ? <Icon name={icon} size={size === "sm" ? 14 : 16} /> : null}
      {children && <span className="btn__label">{children}</span>}
      {shortcut && <Kbd className="btn__kbd">{shortcut}</Kbd>}
    </button>
  );
}

/** A link that looks like a button. */
export function ButtonLink({ href, variant = "secondary", size = "md", icon, iconEnd, shortcut, children, className, ...rest }: { href: string; variant?: Variant; size?: "sm" | "md"; icon?: string; iconEnd?: string; shortcut?: string; children?: ReactNode; className?: string; "aria-label"?: string; title?: string }) {
  return (
    <Link href={href} className={cx("btn", `btn--${variant}`, size === "sm" && "btn--sm", !children && "btn--icon", className)} {...rest}>
      {icon && <Icon name={icon} size={size === "sm" ? 14 : 16} />}
      {children && <span className="btn__label">{children}</span>}
      {iconEnd && <Icon name={iconEnd} size={size === "sm" ? 13 : 15} />}
      {shortcut && <Kbd className="btn__kbd">{shortcut}</Kbd>}
    </Link>
  );
}

export function Kbd({ children, className }: { children: ReactNode; className?: string }) {
  return <kbd className={cx("kbd", className)}>{children}</kbd>;
}

export type Tone = "neutral" | "accent" | "success" | "warning" | "danger";

export function Badge({ tone = "neutral", children, dot, mono, title }: { tone?: Tone; children: ReactNode; dot?: boolean; mono?: boolean; title?: string }) {
  return (
    <span className={cx("badge", `badge--${tone}`, mono && "is-mono")} title={title}>
      {dot && <span className="badge__dot" aria-hidden="true" />}
      {children}
    </span>
  );
}

export function Spinner({ size = 16, label }: { size?: number; label?: string }) {
  return <span className="spinner" style={{ width: size, height: size }} role={label ? "status" : undefined} aria-label={label} />;
}

export function Skeleton({ width, height = 12, radius, className }: { width?: number | string; height?: number; radius?: number; className?: string }) {
  return <span className={cx("skeleton", className)} style={{ width, height, borderRadius: radius }} aria-hidden="true" />;
}

export function EmptyState({ icon = "inbox", title, children, actions, compact }: { icon?: string; title: string; children?: ReactNode; actions?: ReactNode; compact?: boolean }) {
  return (
    <div className={cx("empty", compact && "empty--compact")}>
      <div className="empty__icon"><Icon name={icon} size={compact ? 18 : 22} /></div>
      <div className="empty__title">{title}</div>
      {children && <div className="empty__text">{children}</div>}
      {actions && <div className="empty__actions">{actions}</div>}
    </div>
  );
}

export function Card({ title, icon, actions, children, className, flush, count }: { title?: ReactNode; icon?: string; actions?: ReactNode; children: ReactNode; className?: string; flush?: boolean; count?: number }) {
  return (
    <section className={cx("card", className)}>
      {(title || actions) && (
        <header className="card__header">
          <h2 className="card__title">
            {icon && <Icon name={icon} size={15} />}
            {title}
            {count !== undefined && <span className="card__count">{count}</span>}
          </h2>
          {actions && <div className="card__actions">{actions}</div>}
        </header>
      )}
      <div className={cx("card__body", flush && "card__body--flush")}>{children}</div>
    </section>
  );
}

/** A page's header: its icon, title, what it is, and things beside the title. */
export function PageHeader({ icon, title, meta, description, aside }: { icon?: string; title: ReactNode; meta?: ReactNode; description?: ReactNode; aside?: ReactNode }) {
  return (
    <header className="page-header">
      {icon && <div className="page-header__icon"><Icon name={icon} size={20} /></div>}
      <div className="page-header__text">
        <div className="page-header__title-row">
          <h1 className="page-header__title">{title}</h1>
          {meta}
        </div>
        {description && <p className="page-header__description">{description}</p>}
      </div>
      {aside && <div className="page-header__aside">{aside}</div>}
    </header>
  );
}

// ---- the top bar's slots ----

interface Slots {
  crumbs: HTMLElement | null;
  actions: HTMLElement | null;
}

export const SlotsContext = createContext<Slots>({ crumbs: null, actions: null });

export interface Crumb {
  label: string;
  href?: string;
  mono?: boolean;
}

/** The page's place in the app, in the top bar (and the window's title). */
export function Crumbs({ items }: { items: Crumb[] }) {
  const { crumbs } = useContext(SlotsContext);
  const title = items.length ? items[items.length - 1].label : "";
  useEffect(() => {
    document.title = title ? `${title} · ${appName()}` : appName();
  }, [title]);
  if (!crumbs) return null;
  return createPortal(
    <ol className="crumbs__list">
      {items.map((item, i) => (
        <li key={i} className="crumbs__item">
          {i > 0 && <Icon name="chevron-right" size={14} className="crumbs__separator" />}
          {item.href && i < items.length - 1
            ? <Link href={item.href} className={cx("crumbs__link", item.mono && "is-mono")}>{item.label}</Link>
            : <span className={cx("crumbs__current", item.mono && "is-mono")} aria-current={i === items.length - 1 ? "page" : undefined}>{item.label}</span>}
        </li>
      ))}
    </ol>,
    crumbs,
  );
}

/** The page's actions, in the top bar. */
export function TopbarActions({ children }: { children: ReactNode }) {
  const { actions } = useContext(SlotsContext);
  return actions ? createPortal(children, actions) : null;
}
