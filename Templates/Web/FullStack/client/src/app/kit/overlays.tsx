// What shows over the page: toasts (toast.success, toast.error), a dialog that asks (confirm), and the modal they're
// in. <Overlays /> (in the app's shell) renders them.
import { useEffect, useRef, useState, useSyncExternalStore, type ReactNode } from "react";
import { createPortal } from "react-dom";
import { Icon } from "./icons.tsx";
import { Button, cx } from "./ui.tsx";

// ---- a store of what's showing ----

function store<T>(initial: T) {
  let value = initial;
  const listeners = new Set<() => void>();
  return {
    get: () => value,
    set: (next: T) => {
      value = next;
      listeners.forEach((listener) => listener());
    },
    subscribe: (listener: () => void) => {
      listeners.add(listener);
      return () => void listeners.delete(listener);
    },
  };
}

// ---- toasts ----

type ToastTone = "success" | "error" | "info";

interface ToastItem {
  id: number;
  tone: ToastTone;
  title: string;
  description?: string;
  leaving?: boolean;
}

const toasts = store<ToastItem[]>([]);
let nextToast = 1;

function show(tone: ToastTone, title: string, description?: string) {
  const id = nextToast++;
  toasts.set([...toasts.get(), { id, tone, title, description }].slice(-4));
  return id;
}

function dismiss(id: number) {
  toasts.set(toasts.get().map((item) => (item.id === id ? { ...item, leaving: true } : item)));
  window.setTimeout(() => toasts.set(toasts.get().filter((item) => item.id !== id)), 160);
}

export const toast = {
  success: (title: string, description?: string) => show("success", title, description),
  error: (title: string, description?: string) => show("error", title, description),
  info: (title: string, description?: string) => show("info", title, description),
  dismiss,
};

const toneIcons: Record<ToastTone, string> = { success: "check-circle", error: "alert-circle", info: "info" };

function Toast({ item }: { item: ToastItem }) {
  const [paused, setPaused] = useState(false);
  useEffect(() => {
    if (paused || item.leaving) return;
    const timer = window.setTimeout(() => dismiss(item.id), item.tone === "error" ? 8000 : 4500);
    return () => window.clearTimeout(timer);
  }, [paused, item]);
  return (
    <div
      className={cx("toast", `toast--${item.tone}`, item.leaving && "is-leaving")}
      role={item.tone === "error" ? "alert" : "status"}
      onMouseEnter={() => setPaused(true)}
      onMouseLeave={() => setPaused(false)}
    >
      <Icon name={toneIcons[item.tone]} size={18} className="toast__icon" />
      <div className="toast__text">
        <div className="toast__title">{item.title}</div>
        {item.description && <div className="toast__description">{item.description}</div>}
      </div>
      <button type="button" className="toast__close" aria-label="Dismiss" onClick={() => dismiss(item.id)}>
        <Icon name="x" size={14} />
      </button>
    </div>
  );
}

// ---- modals ----

/** A modal: over a backdrop, focus kept in it, Escape closes it, and focus goes back where it was. */
export function Modal({ onClose, children, className, label, top }: { onClose: () => void; children: ReactNode; className?: string; label: string; top?: boolean }) {
  const panel = useRef<HTMLDivElement>(null);
  const close = useRef(onClose);
  close.current = onClose;
  useEffect(() => {
    const before = document.activeElement as HTMLElement | null;
    const first = panel.current?.querySelector<HTMLElement>("[data-autofocus]") ?? panel.current?.querySelector<HTMLElement>("input, button:not([disabled])");
    first?.focus();
    const keydown = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        event.preventDefault();
        event.stopPropagation();
        close.current();
      } else if (event.key === "Tab" && panel.current) {
        const focusable = [...panel.current.querySelectorAll<HTMLElement>("a[href], button:not([disabled]), input, textarea, select, [tabindex]:not([tabindex='-1'])")];
        if (focusable.length === 0) return;
        const [head, tail] = [focusable[0], focusable[focusable.length - 1]];
        if (event.shiftKey && document.activeElement === head) {
          event.preventDefault();
          tail.focus();
        } else if (!event.shiftKey && document.activeElement === tail) {
          event.preventDefault();
          head.focus();
        }
      }
    };
    document.addEventListener("keydown", keydown, true);
    return () => {
      document.removeEventListener("keydown", keydown, true);
      before?.focus?.();
    };
  }, []);
  return createPortal(
    <div className={cx("modal", top && "modal--top")} data-modal="">
      <div className="modal__backdrop" onMouseDown={() => close.current()} />
      <div ref={panel} className={cx("modal__panel", className)} role="dialog" aria-modal="true" aria-label={label}>
        {children}
      </div>
    </div>,
    document.body,
  );
}

/** Whether a modal is open (shortcuts leave the page alone then). */
export const modalOpen = () => document.querySelector("[data-modal]") !== null;

// ---- confirm ----

interface Question {
  title: string;
  body?: ReactNode;
  confirm?: string;
  cancel?: string;
  tone?: "danger" | "primary";
  icon?: string;
  resolve: (answer: boolean) => void;
}

const questions = store<Question | null>(null);

/** Asks, in a dialog: true for yes. */
export function confirm(question: Omit<Question, "resolve">): Promise<boolean> {
  return new Promise((resolve) => questions.set({ ...question, resolve }));
}

function ConfirmDialog({ question }: { question: Question }) {
  const answer = (yes: boolean) => {
    questions.set(null);
    question.resolve(yes);
  };
  const danger = question.tone === "danger";
  return (
    <Modal onClose={() => answer(false)} className="dialog" label={question.title}>
      <div className="dialog__content">
        <div className={cx("dialog__icon", danger && "dialog__icon--danger")}>
          <Icon name={question.icon ?? (danger ? "alert-triangle" : "info")} size={20} />
        </div>
        <div>
          <h2 className="dialog__title">{question.title}</h2>
          {question.body && <div className="dialog__body">{question.body}</div>}
        </div>
      </div>
      <div className="dialog__actions">
        <Button onClick={() => answer(false)} data-autofocus={danger ? "" : undefined}>{question.cancel ?? "Cancel"}</Button>
        <Button variant={danger ? "danger" : "primary"} onClick={() => answer(true)} data-autofocus={danger ? undefined : ""}>
          {question.confirm ?? "OK"}
        </Button>
      </div>
    </Modal>
  );
}

/** The toasts and the dialog, wherever the app has them (once). */
export function Overlays() {
  const items = useSyncExternalStore(toasts.subscribe, toasts.get);
  const question = useSyncExternalStore(questions.subscribe, questions.get);
  return (
    <>
      {createPortal(
        <div className="toasts" aria-live="polite">
          {items.map((item) => <Toast key={item.id} item={item} />)}
        </div>,
        document.body,
      )}
      {question && <ConfirmDialog question={question} />}
    </>
  );
}
