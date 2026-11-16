// The Palette property's editor in VS Code's Template Properties: a row for each color, with its name, its light
// value and its dark value, each picked or typed. It's a web component in an ES module, with no framework and no build.
//
// - Loaded: codesmith.json beside the template names it ("propertyEditors"), and the property grid loads it
//   in a trusted workspace and calls activate(grid), which registers it for ColorPalette properties.
// - The value: the property's text, as ColorPalette's converter writes and reads it ("primary: #0b5cad / #4493f8; ...").
//   The editor reads property.value and sets host.setValue(text). The template session checks what it sets
//   (ColorPalette.Parse): a value it refuses shows on the property's row and in its description, as for the built-in
//   editors, and stays here to correct. The editor checks it the same way as you type.
// - The look: VS Code's theme, through its CSS variables, and the VSCode Elements components the grid itself uses.
// - The keyboard: Tab between the fields, Enter (or leaving a field) sets the value, Alt+Up and Alt+Down move a color,
//   Escape takes back what the session refused, then closes the editor.
// The API is the CodeSmith extension's api/property-editor.d.ts.

const NamePattern = /^[a-z][a-z0-9]*(-[a-z0-9]+)*$/;
const ColorPattern = /^#([0-9a-f]{3}|[0-9a-f]{6})$/i;

/** The palette's text as colors ({ name, light, dark }), as ColorPalette.Parse reads it. */
export function parsePalette(text) {
  return (text ?? "").split(";").map((entry) => entry.trim()).filter(Boolean).map((entry) => {
    const colon = entry.indexOf(":");
    const name = colon < 0 ? entry : entry.slice(0, colon).trim();
    const [light = "", dark = light] = colon < 0 ? [] : entry.slice(colon + 1).split("/").map((v) => v.trim());
    return { name, light, dark };
  });
}

/** The colors as the palette's text, as ColorPalette.ToString writes it. */
export function formatPalette(colors) {
  return colors.map((c) => `${c.name.trim()}: ${c.light.trim() === c.dark.trim() ? c.light.trim() : `${c.light.trim()} / ${c.dark.trim()}`}`).join("; ");
}

/** What's wrong with the colors, as ColorPalette says it: [{ index, field, message }]. */
export function paletteProblems(colors) {
  const problems = [];
  if (colors.length === 0) {
    problems.push({ index: -1, field: "", message: "A palette has at least one color." });
  }
  colors.forEach((color, index) => {
    const name = color.name.trim();
    if (!NamePattern.test(name)) {
      problems.push({ index, field: "name", message: `"${name}" isn't a color's name: lowercase letters, digits and dashes, starting with a letter (primary, text-muted).` });
    } else if (colors.findIndex((c) => c.name.trim() === name) < index) {
      problems.push({ index, field: "name", message: `Two colors are named ${name}.` });
    }
    for (const theme of ["light", "dark"]) {
      if (!ColorPattern.test(color[theme].trim())) {
        problems.push({ index, field: theme, message: `${name}'s ${theme} color, "${color[theme]}", isn't a color: #rgb or #rrggbb.` });
      }
    }
  });
  return problems;
}

/** #abc as #aabbcc, for the color picker (which takes only that); undefined when it isn't a color. */
function pickerValue(text) {
  const color = text.trim();
  if (!ColorPattern.test(color)) {
    return undefined;
  }
  return (color.length === 4 ? "#" + [...color.slice(1)].map((c) => c + c).join("") : color).toLowerCase();
}

/** Focuses an element: the field inside it where it doesn't pass the focus on itself (vscode-toolbar-button). */
function focus(element) {
  element?.focus();
  if (element && !element.matches(":focus-within")) {
    element.shadowRoot?.querySelector("button, input")?.focus();
  }
}

const styles = `
  :host { display: block; container-type: inline-size; color: var(--vscode-foreground); }
  .head, .color { display: grid; grid-template-columns: minmax(7em, 1fr) 9.5em 9.5em 68px; gap: 4px 8px; align-items: center; }
  .head { color: var(--vscode-descriptionForeground); font-size: 0.9em; padding: 0 2px 2px; }
  .colors { display: flex; flex-direction: column; gap: 4px; }
  .color { padding: 2px; border-radius: 2px; }
  .color:focus-within { background: var(--vscode-list-inactiveSelectionBackground, transparent); }
  .value { display: flex; align-items: center; gap: 4px; min-width: 0; }
  vscode-textfield { width: 100%; min-width: 0; }
  .actions { display: flex; }
  input[type=color] {
    flex: none; inline-size: 26px; block-size: 26px; padding: 2px; margin: 0; cursor: pointer;
    background: var(--vscode-input-background); border: 1px solid var(--vscode-input-border, var(--vscode-panel-border, transparent)); border-radius: 2px;
  }
  input[type=color]::-webkit-color-swatch-wrapper { padding: 0; }
  input[type=color]::-webkit-color-swatch { border: none; border-radius: 1px; }
  input[type=color]:focus { outline: 1px solid var(--vscode-focusBorder); outline-offset: -1px; }
  input[type=color]:disabled { cursor: default; opacity: 0.6; }
  .footer { display: flex; align-items: center; gap: 12px; margin-top: 6px; }
  .hint { color: var(--vscode-descriptionForeground); font-size: 0.9em; }
  .problems { color: var(--vscode-errorForeground); margin: 4px 0 0; padding: 0; list-style: none; }
  .problems:empty { display: none; }
  .empty { color: var(--vscode-descriptionForeground); font-style: italic; padding: 2px; }
  /* narrow (the side bar): the name and the buttons on one line, the two colors below */
  @container (max-width: 520px) {
    .head, .color { grid-template-columns: 1fr 1fr 68px; }
    .head .name-head { display: none; }
    .color .name { grid-column: 1 / 3; }
    .color .actions { grid-column: 3; grid-row: 1; }
  }
`;

let ids = 0;

class PaletteEditor extends HTMLElement {
  #property;
  /** The colors being edited: { id, name, light, dark, typed }; typed: the fields typed in since the value was last set. */
  #rows = [];
  /** The value the rows were last read from (or that the session last took). */
  #value;
  /** setValue calls under way: the property the grid sets meanwhile is read when they're done. */
  #pending = 0;
  /** What the last setValue call set. */
  #sent;
  /** The session refused the last value set: the rows keep it to correct. */
  #refused = false;
  #elements = new Map();

  set property(property) {
    this.#property = property;
    if (this.#pending === 0) {
      this.#load();
    }
  }

  get property() {
    return this.#property;
  }

  connectedCallback() {
    if (!this.shadowRoot) {
      // delegatesFocus: focusing the editor focuses its first field, as the grid's own editors do
      const root = this.attachShadow({ mode: "open", delegatesFocus: true });
      root.innerHTML = `<style>${styles}</style>
        <div class="head" aria-hidden="true"><span class="name-head">Name</span><span>Light</span><span>Dark</span></div>
        <div class="colors" role="list" aria-label="Colors"></div>
        <ul class="problems" aria-live="polite"></ul>
        <div class="footer">
          <vscode-button secondary icon="add" class="add">Add color</vscode-button>
          <span class="hint">Alt+↑ and Alt+↓ move a color.</span>
        </div>`;
      root.querySelector(".add").addEventListener("click", () => this.#add());
      root.addEventListener("keydown", (e) => this.#key(e));
    }
    this.#load();
  }

  // ---------------------------------------------------------------- the value

  /** The rows from the property's value, unless they already show it. What's been typed since it was set stays. */
  #load() {
    if (!this.#property || !this.shadowRoot) {
      return;
    }
    const value = this.#property.isNull ? "" : this.#property.value ?? "";
    if (value !== this.#value) {
      // the same rows (and their fields) where there are as many: the one being typed in keeps its place
      const before = this.#rows;
      this.#rows = parsePalette(value).map((color, i) => {
        const row = { id: before[i]?.id ?? ++ids, ...color, typed: new Set(before[i]?.typed) };
        row.typed.forEach((field) => (row[field] = before[i][field]));
        return row;
      });
      this.#value = value;
      this.#refused = false;
    }
    this.#render();
  }

  /** Sets the property to the rows (when they changed): the session checks it, and the grid shows every value again. */
  async #commit() {
    const text = formatPalette(this.#rows);
    // what the property has, or will have once the calls under way are done
    if (text === (this.#pending > 0 ? this.#sent : this.#value) && !this.#refused) {
      return;
    }
    if (text === this.#value && this.#pending === 0) {
      // back to the value it has: the refused one's error goes
      this.#refused = false;
      this.host.reverted();
      this.#render();
      return;
    }
    this.#rows.forEach((row) => row.typed.clear());
    this.#sent = text;
    this.#pending++;
    try {
      await this.host.setValue(text);
      this.#refused = false;
    } catch {
      // the grid shows why on the property's row and in its description; the rows keep it to correct
      this.#refused = true;
    } finally {
      this.#pending--;
    }
    if (this.#pending === 0) {
      this.#refused ? this.#render() : this.#load();
    }
  }

  /** Escape: the value the property has, in place of one the session refused; else the editor closes. */
  #revert() {
    if (formatPalette(this.#rows) === this.#value && !this.#refused) {
      this.host.close();
      return;
    }
    this.#rows.forEach((row) => row.typed.clear());
    this.#value = undefined;
    this.#load();
    this.host.reverted();
  }

  // ---------------------------------------------------------------- editing

  async #add() {
    let n = this.#rows.length + 1;
    while (this.#rows.some((r) => r.name === `color-${n}`)) n++;
    this.#rows.push({ id: ++ids, name: `color-${n}`, light: "#808080", dark: "#808080", typed: new Set() });
    this.#render();
    const commit = this.#commit();
    // the new color's name, selected, to type over
    const name = this.#elements.get(this.#rows.at(-1).id).name;
    await name.updateComplete;
    name.focus();
    name.shadowRoot?.querySelector("input")?.select();
    await commit;
  }

  #remove(id) {
    const index = this.#rows.findIndex((r) => r.id === id);
    this.#rows.splice(index, 1);
    this.#render();
    // the next color's (or the previous one's) remove button, or Add
    const next = this.#rows[Math.min(index, this.#rows.length - 1)];
    focus(next ? this.#elements.get(next.id).remove : this.shadowRoot.querySelector(".add"));
    void this.#commit();
  }

  #move(id, by, focused) {
    const index = this.#rows.findIndex((r) => r.id === id);
    const to = index + by;
    if (to < 0 || to >= this.#rows.length) {
      return;
    }
    const [row] = this.#rows.splice(index, 1);
    this.#rows.splice(to, 0, row);
    this.#render();
    // the same field, in its new place (moving it took its focus)
    focus(focused);
    void this.#commit();
  }

  #key(e) {
    if (e.key === "Escape") {
      e.preventDefault();
      e.stopPropagation();
      this.#revert();
      return;
    }
    if (e.altKey && (e.key === "ArrowUp" || e.key === "ArrowDown")) {
      const row = e.composedPath().find((el) => el instanceof HTMLElement && el.classList.contains("color"));
      if (row) {
        e.preventDefault();
        e.stopPropagation();
        // the field itself (a vscode-textfield's, or a color picker)
        const focused = e.composedPath().find((el) => el instanceof HTMLElement && el.parentElement?.closest?.(".color") === row && (el.localName === "vscode-textfield" || el.localName === "input" || el.localName === "vscode-toolbar-button"));
        this.#move(Number(row.dataset.id), e.key === "ArrowUp" ? -1 : 1, focused);
      }
    }
  }

  // ---------------------------------------------------------------- the rows

  /** A row's elements, made once: they keep their focus and what's being typed while the value comes back. */
  #row(row) {
    let elements = this.#elements.get(row.id);
    if (elements) {
      return elements;
    }
    const element = document.createElement("div");
    element.className = "color";
    element.setAttribute("role", "listitem");
    element.dataset.id = String(row.id);
    element.innerHTML = `
      <vscode-textfield class="name"></vscode-textfield>
      <span class="value"><input type="color" class="light-picker"><vscode-textfield class="light"></vscode-textfield></span>
      <span class="value"><input type="color" class="dark-picker"><vscode-textfield class="dark"></vscode-textfield></span>
      <span class="actions">
        <vscode-toolbar-button icon="arrow-up" class="up" label="Move up" title="Move up (Alt+Up)"></vscode-toolbar-button>
        <vscode-toolbar-button icon="arrow-down" class="down" label="Move down" title="Move down (Alt+Down)"></vscode-toolbar-button>
        <vscode-toolbar-button icon="trash" class="remove" label="Remove" title="Remove"></vscode-toolbar-button>
      </span>`;
    const $ = (selector) => element.querySelector(selector);
    elements = { element, name: $(".name"), light: $(".light"), dark: $(".dark"), lightPicker: $(".light-picker"), darkPicker: $(".dark-picker"),
      up: $(".up"), down: $(".down"), remove: $(".remove") };
    const current = () => this.#rows.find((r) => r.id === row.id);
    elements.name.label = "Name";
    for (const theme of ["light", "dark"]) {
      const field = elements[theme];
      const picker = elements[theme + "Picker"];
      field.label = theme === "light" ? "Light" : "Dark";
      picker.setAttribute("aria-label", `Pick the ${theme} color`);
      // typing a color shows it in the picker; picking one types it
      field.addEventListener("input", () => {
        current()[theme] = field.value;
        current().typed.add(theme);
        picker.value = pickerValue(field.value) ?? picker.value;
        this.#mark();
      });
      field.addEventListener("change", () => void this.#commit());
      picker.addEventListener("input", () => {
        current()[theme] = picker.value;
        current().typed.add(theme);
        field.value = picker.value;
        this.#mark();
      });
      picker.addEventListener("change", () => void this.#commit());
    }
    elements.name.addEventListener("input", () => {
      current().name = elements.name.value;
      current().typed.add("name");
      element.setAttribute("aria-label", elements.name.value);
      this.#mark();
    });
    elements.name.addEventListener("change", () => void this.#commit());
    elements.up.addEventListener("click", () => this.#move(row.id, -1, elements.up));
    elements.down.addEventListener("click", () => this.#move(row.id, 1, elements.down));
    elements.remove.addEventListener("click", () => this.#remove(row.id));
    this.#elements.set(row.id, elements);
    return elements;
  }

  #render() {
    if (!this.shadowRoot || !this.#property) {
      return;
    }
    const list = this.shadowRoot.querySelector(".colors");
    const readOnly = this.#property.readOnly;
    const ids = new Set(this.#rows.map((r) => r.id));
    for (const [id, elements] of this.#elements) {
      if (!ids.has(id)) {
        elements.element.remove();
        this.#elements.delete(id);
      }
    }
    this.#rows.forEach((row, index) => {
      const elements = this.#row(row);
      if (list.children[index] !== elements.element) {
        list.insertBefore(elements.element, list.children[index] ?? null);
      }
      elements.element.setAttribute("aria-label", row.name);
      for (const field of ["name", "light", "dark"]) {
        if (elements[field].value !== row[field]) {
          elements[field].value = row[field];
        }
        elements[field].readonly = readOnly;
      }
      for (const theme of ["light", "dark"]) {
        // a color being typed that isn't one yet: the picker keeps the last one that was
        const picker = elements[theme + "Picker"];
        picker.value = pickerValue(row[theme]) ?? picker.value;
        picker.disabled = readOnly;
      }
      elements.up.disabled = readOnly || index === 0;
      elements.down.disabled = readOnly || index === this.#rows.length - 1;
      elements.remove.disabled = readOnly || this.#rows.length === 1;
    });
    let empty = list.querySelector(".empty");
    if (this.#rows.length === 0 && !empty) {
      empty = list.appendChild(document.createElement("div"));
      empty.className = "empty";
      empty.textContent = "No colors yet.";
    } else if (this.#rows.length > 0) {
      empty?.remove();
    }
    this.shadowRoot.querySelector(".add").disabled = readOnly;
    this.#mark();
  }

  /** Marks the fields with a problem, and says what it is, as you type. */
  #mark() {
    const problems = paletteProblems(this.#rows);
    for (const row of this.#rows) {
      const elements = this.#elements.get(row.id);
      for (const field of ["name", "light", "dark"]) {
        const problem = problems.find((p) => p.index === this.#rows.indexOf(row) && p.field === field);
        elements[field].invalid = !!problem;
        elements[field].title = problem?.message ?? "";
      }
    }
    const list = this.shadowRoot.querySelector(".problems");
    const messages = problems.map((p) => p.message);
    if ([...list.children].map((li) => li.textContent).join("\n") !== messages.join("\n")) {
      list.replaceChildren(...messages.map((message) => Object.assign(document.createElement("li"), { textContent: message })));
    }
  }
}

/** Called by the property grid when it loads this module. */
export function activate(grid) {
  if (!customElements.get("sample-palette-editor")) {
    customElements.define("sample-palette-editor", PaletteEditor);
  }
  // every ColorPalette property, below its row in the grid
  grid.registerEditor({ tag: "sample-palette-editor", types: ["CodeSmith.Samples.ColorPalette"], style: "dropdown" });
}
