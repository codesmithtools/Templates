# Database documentation (DBDocumenter)

A documentation site for a database: an overview with a relationship diagram, and a page for each table, view and
stored procedure. It documents the PetShop sample database (`PetShop.db3`, SQLite) in this folder.

```sh
cs generate Database.csp
```

Open `Docs/index.html` in a browser. The pages open straight from disk and work offline: no server, no downloads.

- **Overview:** the database's counts, its relationship diagram, and a summary of its tables, relationships, views
  and procedures. The diagram is SVG drawn while generating. Drag it to pan, Ctrl and the mouse wheel zoom, and
  hovering a table traces its keys.
- **A table's page:** its columns (type, nullability, default, key and identity badges, description), the foreign
  keys it holds and the ones that point at it, a diagram of its neighbors, its indexes, and the views and procedures
  that use it.
- **A view's or procedure's page:** its columns or parameters, the tables it uses, and its SQL, highlighted.
- **Search:** press `/` or Ctrl+K to find any table, view, procedure, column or parameter.
- Light and dark themes (the system's, or the toggle's), a sidebar that becomes a drawer on a phone, and pages
  that print cleanly.

Databases: any `cs` reads (SQL Server, SQLite, PostgreSQL, MySQL, Oracle, and SQL scripts). For SQL Server it adds a
server page (version, edition, collation).

To document your own database, change the data source in `Database.csp` (or pick one in VS Code's Template
Properties). Optional:

- `OutputDirectory`: where the pages go (default: a `Docs` folder beside the template).
- `DocumentationTitle`: the documentation's title (default: the database's name).
- `ServerName`: the server's name, shown on the pages (default: the server the connection string names, with its
  port, or the file it opens: `PetShop.db3`).

Descriptions are the database's own (`CS_Description`: SQL Server's `MS_Description`, the comments of PostgreSQL,
MySQL and Oracle).

| File | What it is |
| --- | --- |
| `master.cst` | The master: renders each page's template to its file (`<%@ Register %>`, `RenderToFile`), and copies `assets/`. |
| `overview.cst`, `table.cst`, `view.cst`, `procedure.cst`, `server.cst` | The pages: `index.html`, `tables/`, `views/`, `procedures/` and `server.html`. |
| `header.cst`, `footer.cst` | What every page renders around itself: the head, the sidebar, the top bar, the search. |
| `DBDocumenterTemplate.cs` | The code-behind every template inherits (`Inherits=`): the output folder, links, icons, badges. |
| `Site.cs` | The model the pages share: the objects in order, their pages, the keys both ways, what each view's and procedure's SQL uses, the search index. |
| `ErDiagram.cs` | The relationship diagram: a layered layout, drawn as SVG. |
| `SqlHighlighter.cs` | SQL highlighting, done while generating. |
| `assets/docs.css`, `assets/docs.js` | The stylesheet and script, copied beside the pages. |

DBDocumenter was written by Jason Alexander and Eric J. Smith. Its icons are [Lucide](https://lucide.dev)'s (ISC
license). [MarkdownDocs](../MarkdownDocs/README.md) documents a database in Markdown, for a repository.
