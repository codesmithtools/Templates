# Custom property editor

A property of your own type, with its own editor in VS Code's Template Properties: a color palette, edited a color
at a time, that a template turns into CSS custom properties for a light theme and a dark one.

Run it from this folder:

```sh
cs generate Palette.csp
cs check Palette.csp
```

`Generated/palette.css` has the `Brand` palette of `Palette.csp`. Give the template a palette of your own as text:

```sh
cs render Palette.cst "-p:Palette=ink: #1f2328 / #e6edf3; paper: #ffffff / #0d1117" -p:Prefix=theme
```

In VS Code, open `Palette.cst`. In Template Properties, the Palette row opens the palette editor below it: a row for
each color, with its name, its light value and its dark value. Type a color or pick one. Open `Palette.csp` to edit the
`Brand` output's palette in the project editor.

| File | What it is |
| --- | --- |
| `Palette.cst` | The template: CSS custom properties from the palette, light by default and dark when the system's theme is dark (or `data-theme="dark"`). |
| `ColorPalette.cs` | The type (`ColorPalette`, a list of `PaletteColor`), its converter and its serializer (`<%@ Assembly Src="ColorPalette.cs" %>`). |
| `palette-editor.js` | The editor: a web component in an ES module. |
| `codesmith.json` | Names the editor (`propertyEditors`), so the property grid loads it for the templates here. |
| `Palette.csp` | A palette of eight colors, written to `Generated/palette.css`. |

This replaces 8.x's CustomPropertiesSample (in `Legacy/Projects`). Its WinForms editors (`UITypeEditor`) don't run in
9.0: the property grid is a web page in VS Code, and its editors are web components.

## Writing your own editor

### The value is text

The grid shows a property's value as its type's `TypeConverter` writes it, and sets it from text the converter reads.
An editor does the same: it reads `property.value` and calls `host.setValue(text)`. So a type of your own needs:

- **A converter** (`[TypeConverter]`): `ConvertTo` string and `ConvertFrom` string. `ColorPaletteConverter` writes
  `primary: #0b5cad / #4493f8; ...`. The same text works with `-p:` on the command line and in the Property directive's
  `Default`, and without the editor the grid edits it as text.
- **A serializer** (`[PropertySerializer]`, an `IPropertySerializer`): the value as XML in a `.csp`, and in the values
  the grid remembers. `ColorPaletteSerializer` writes an element per color. Its `LoadProperty` gets the text from `-p:`,
  so it parses a string as well.
- **Checks in one place.** `ColorPalette` throws `FormatException` for a value that isn't valid. The session refuses it
  with that message, the grid shows it on the property's row and in its description, and `cs` prints it for `-p:`.
  The editor checks the same rules as you type, to mark the field.

For a value only .NET can make or check (a lookup, a database), give the property an editor service: see the
extension's README, "Custom property editors".

### The editor is a web component

The module exports `activate(grid)`, which defines the element and registers it:

```js
export function activate(grid) {
  customElements.define("sample-palette-editor", PaletteEditor);
  grid.registerEditor({ tag: "sample-palette-editor", types: ["CodeSmith.Samples.ColorPalette"], style: "dropdown" });
}
```

- **Which properties:** `types` matches the property's type (or a base type or interface) by its full name. A Property
  directive can name an editor itself with `WebEditor="sample-palette-editor"`; `editorTypes` matches an `[Editor]`
  type's name; `kinds` matches the built-in kinds.
- **Where:** `style` is `"inline"` (in the value cell), `"dropdown"` (below the row, as here) or `"dialog"`.
- **What it gets:** the grid sets `property` (name, type, `value`, `isNull`, `readOnly`, `required`, `error`,
  `attributes`...) before it adds the element, and again whenever the values change. `host` has `setValue`,
  `reverted` (it dropped a value the session refused), `close` (its dropdown or dialog), file and folder pickers, and
  `service` (the editor service). The types are in the CodeSmith extension's
  [`api/property-editor.d.ts`](https://github.com/codesmithtools/Generator/blob/master/editors/vscode/api/property-editor.d.ts).
- **The theme:** VS Code's CSS variables (`--vscode-foreground`, `--vscode-input-border`, `--vscode-errorForeground`,
  `--vscode-focusBorder`...) follow the theme, light or dark. The page also defines the VSCode Elements components the
  grid uses (`vscode-textfield`, `vscode-button`, `vscode-toolbar-button`...), so an editor that uses them looks like
  the built-in ones. This one does both.
- **The keyboard:** every control is a field or a button, so Tab reaches it. Here, Enter or leaving a field sets the
  value, Alt+Up and Alt+Down move a color, and Escape takes back a value the session refused, then closes the editor.
  `delegatesFocus` on the shadow root lets the grid's Enter move into the editor.
- **Errors:** a refused `setValue` rejects its promise, and the grid marks the row and shows why. Keep what was typed,
  to correct, and mark the field (`invalid` on a `vscode-textfield`). Call `host.reverted()` when you show the
  property's value again.

### Where the script lives, and how it's loaded

Put the module beside the templates and name it in a `codesmith.json` there:

```json
{ "propertyEditors": ["palette-editor.js"] }
```

Paths are relative to the options file, and an options file applies to its folder and the folders below it. When a
template opens, the template session reports the modules its options files name. The extension loads them into
Template Properties and the project editor, in a trusted workspace only (they're code). Saving the module loads it
again, so you can change the editor and see it at once.

- **A template folder:** the module and the options file travel with the templates, as here. `cs sample new` and
  *CodeSmith: New from Sample...* copy them.
- **A helper DLL:** put the type, its converter and its serializer in the DLL, and the module beside it. The options
  file of the folder that uses the DLL names the module, by a path relative to it (`"tools/lib/palette-editor.js"`
  beside `"assemblyPaths": ["tools/lib"]`).
- **For every workspace:** the `codesmith.propertyEditors` setting, or a VS Code extension that contributes modules
  (`"contributes": { "codesmith": { "propertyEditors": ["./dist/editors.js"] } }`).
