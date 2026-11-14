# Database documentation (DBDocumenter)

HTML documentation of a database. A page for each table, view and stored procedure, with their columns, keys,
indexes, parameters and SQL. It documents the PetShop sample database (`PetShop.db3`, SQLite) in this folder.

```sh
cs generate Database.csp
```

Open `Docs/index.html`. The system information page is for SQL Server databases.

To document your own database, change the data source in `Database.csp` (or pick one in VS Code's Template
Properties), with:

- `OutputDirectory`: where the pages go (default: an `output` folder beside the template).
- `ServerName`: the server's name, shown on the pages.
- `DocumentationTitle`: the title of every page.

It shows a larger template set: a master template (`master.cst`) rendering sub-templates (`<%@ Register %>`) to
files, a code-behind base class (`DBDocumenterTemplate.cs`), extended properties, and a helper DLL (the SQL
highlighter in `bin/`).

DBDocumenter was written by Jason Alexander and Eric J. Smith. Its SQL highlighting uses Actipro's CodeHighlighter
(<http://www.codehighlighter.com/>).
