// The app's keyboard and a few browser things Blazor asks for (window.ui). Csla.cst wrote this file once: it's yours.
//
// Shortcuts click what the page marks with data-shortcut, so a page that has no Save button has no Ctrl+S:
//   Ctrl/Cmd+S  save          Esc  cancel (or close what's open)       /  search
//   Ctrl/Cmd+K  the palette   n    new                                  ?  the shortcuts
(() => {
    const typing = el => el && (el.isContentEditable || /^(INPUT|TEXTAREA|SELECT)$/.test(el.tagName));
    const find = name => document.querySelector(`[data-shortcut="${name}"]:not([disabled])`);
    const open = () => document.querySelector('.overlay, .listbox, .menu');

    document.addEventListener('keydown', e => {
        const mod = e.ctrlKey || e.metaKey;
        const key = e.key.length === 1 ? e.key.toLowerCase() : e.key;

        if (mod && key === 's') {
            const save = find('save');
            if (!save) return;
            e.preventDefault();
            // what's being typed is committed first (a number's input commits on change)
            const active = document.activeElement;
            if (typing(active)) active.dispatchEvent(new Event('change', { bubbles: true }));
            setTimeout(() => save.click(), 0);
            return;
        }
        if (mod && key === 'k') {
            const palette = find('palette');
            if (palette) { e.preventDefault(); palette.click(); }
            return;
        }
        if (mod || e.altKey || e.defaultPrevented) return;

        if (key === 'Escape') {
            if (open()) return; // the dialog, list or menu closes itself
            if (typing(document.activeElement)) { document.activeElement.blur(); return; }
            find('cancel')?.click();
            return;
        }
        if (typing(e.target)) return;
        if (key === '/') {
            const search = find('search');
            if (search) { e.preventDefault(); search.focus(); search.select?.(); }
        } else if (key === 'n') {
            const add = find('new');
            if (add) { e.preventDefault(); add.click(); }
        } else if (key === '?') {
            find('help')?.click();
        } else if (key === 'ArrowDown' || key === 'ArrowUp') {
            // a list's rows: up and down, Enter opens
            const row = e.target.closest?.('tr.row');
            if (!row) return;
            const next = key === 'ArrowDown' ? row.nextElementSibling : row.previousElementSibling;
            if (next?.matches('tr.row')) { e.preventDefault(); next.focus(); }
        }
    });

    window.ui = {
        focus(selector) {
            const el = document.querySelector(selector);
            if (!el) return;
            el.scrollIntoView({ block: 'center', behavior: matchMedia('(prefers-reduced-motion: reduce)').matches ? 'auto' : 'smooth' });
            el.focus({ preventScroll: true });
        },
        // the first field that says what's wrong
        focusFirstError() {
            const el = document.querySelector('.field.error input, .field.error textarea, .field.error button');
            if (el) { el.scrollIntoView({ block: 'center' }); el.focus({ preventScroll: true }); }
        },
        setCookie(name, value) {
            document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=31536000; samesite=lax`;
        },
        theme(value) {
            try { value === 'system' ? localStorage.removeItem('theme') : localStorage.setItem('theme', value); } catch { }
            if (value === 'system') delete document.documentElement.dataset.theme;
            else document.documentElement.dataset.theme = value;
        },
        currentTheme() {
            try { return localStorage.getItem('theme') || 'system'; } catch { return 'system'; }
        },
        scrollIntoView(selector) {
            document.querySelector(selector)?.scrollIntoView({ block: 'nearest' });
        },
        // a grid's new row: its first empty field (or its first field)
        focusLastRow(grid) {
            const rows = document.querySelectorAll(`[data-grid="${grid}"] tr.grid-row`);
            const inputs = [...(rows[rows.length - 1]?.querySelectorAll('input:not([readonly]), textarea:not([readonly])') ?? [])];
            (inputs.find(i => !i.value) ?? inputs[0])?.focus();
        }
    };
})();
