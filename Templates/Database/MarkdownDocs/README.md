# Markdown docs

Markdown documentation of a database, with Mermaid diagrams of its tables. A page for each table, view and
procedure, to keep in a repository, where GitHub shows them. It documents the PetShop sample database (`PetShop.db3`,
SQLite) in this folder.

Databases: any `cs` reads (SQL Server, SQLite, PostgreSQL, MySQL, Oracle, and SQL scripts).

```sh
cs generate MarkdownDocs.csp
```

Open `Generated/README.md`. GitHub and VS Code's Markdown preview draw the diagrams.

- **The README:** the database at a glance (its counts), an ER diagram (Mermaid's `erDiagram`) of the tables and
  their keys, and tables of its tables, relationships, views and procedures, each linked to its page.
- **A table's page** (`Generated/tables/`): its columns (type, nullable, key, default, the column a foreign key
  refers to, description), a diagram of the table and its neighbors, its foreign keys both ways, its indexes, and
  the views and procedures that use it.
- **A view's or procedure's page:** its columns or parameters, the tables it uses, and its SQL (GitHub highlights
  it). PetShop's SQLite database has neither, so there are none here.
- Each page starts with breadcrumbs and ends with links to the pages before and after it.
- Descriptions are the database's own (`CS_Description`: SQL Server's `MS_Description`, the comments of PostgreSQL,
  MySQL and Oracle).

The diagrams lay out left to right (`direction LR`), so a database's groups of tables stack instead of shrinking
side by side. GitHub ignores Mermaid's older `layoutDirection` setting.

| File | What it is |
| --- | --- |
| `MarkdownDocs.cst` | The master: renders `Readme.cst` once, and a page for each table, view and procedure, into `OutputDirectory`. |
| `Readme.cst` | `README.md`: the counts, the diagram, and the lists. |
| `Table.cst`, `View.cst`, `Procedure.cst` | A table's, a view's and a procedure's page. |
| `Docs.cs` | What the templates share (`<%@ Assembly Src="Docs.cs" %>`): pages and links, Markdown text, keys both ways, the tables a view's SQL uses, the diagrams' names. |
| `codesmith.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
| `MarkdownDocs.csp` | Generates from the PetShop sample database in this folder (`PetShop.db3`, SQLite). |

To document your own database, change the data source in `MarkdownDocs.csp` (or pick one in VS Code's Template
Properties), and `OutputDirectory` to a folder of your repository (`docs/database`).

[DBDocumenter](../DBDocumenter/README.md) documents a database as a site of HTML pages.
