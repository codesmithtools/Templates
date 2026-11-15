// The kit: what the generated UI and the app's shell use. Yours to change: the pages, the components, the styles
// (../theme.css, ../app.css).
export * from "./types.ts";
export { configure, tables, tableNamed, resolve, customize, appName, theDatabase, rowCounts, type RowCount } from "./config.ts";
export { useQuery, invalidateTable, useReferences } from "./data.ts";
export { Value, toneOf, type ValueProps } from "./cells.tsx";
export { FieldInput, FormField } from "./fields.tsx";
export { Combobox } from "./Combobox.tsx";
export { Icon } from "./icons.tsx";
export { Link, navigate, match, useLocation } from "./router.tsx";
export { useShortcut } from "./shortcuts.ts";
export { toast, confirm, Modal, Overlays } from "./overlays.tsx";
export { useTheme, type ThemeChoice } from "./theme.ts";
export * from "./format.ts";
export * from "./ui.tsx";
export { ListPage, deleteRow } from "./ListPage.tsx";
export { DetailPage } from "./DetailPage.tsx";
export { FormPage } from "./FormPage.tsx";
export { Dashboard } from "./Dashboard.tsx";
export { CommandPalette, ShortcutsHelp } from "./CommandPalette.tsx";
