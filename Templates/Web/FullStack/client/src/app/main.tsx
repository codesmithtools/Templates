// The app's start: the theme and styles, the generated tables (with your customizations), then the shell.
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { database, tables } from "../generated/ui/index.ts";
import { api } from "./api.ts";
import { App } from "./App.tsx";
import { customize } from "./customize.tsx";
import { configure } from "./kit/index.ts";
import "./theme.css";
import "./app.css";

configure({ tables, database, customize, rowCounts: () => api.rowCounts() });

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
