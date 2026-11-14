# Markdown docs

Markdown documentation of a database, with a Mermaid diagram of its tables. A page for each table, view and
procedure, to keep in a repository, where GitHub shows them. It documents the PetShop sample database (`PetShop.db3`,
SQLite) in this folder.

```sh
cs generate MarkdownDocs.csp
```

Open `Generated/README.md`: the database's tables, views and procedures, and an ER diagram (Mermaid's `erDiagram`) of
the tables, their columns and the relationships their foreign keys make. GitHub and VS Code's Markdown preview draw it.

- A table's page (`Generated/tables/`) has its columns (type, nullable, default, key, description), primary key,
  foreign keys (linked to the tables they refer to), indexes, and the foreign keys of other tables that refer to it.
- A view's page has its columns; a procedure's, its parameters. PetShop's SQLite database has no procedures, so
  there are none here.
- Descriptions are the database's own (`CS_Description`: SQL Server's `MS_Description`, Oracle's comments).

| File | What it is |
| --- | --- |
| `MarkdownDocs.cst` | The master: renders `Readme.cst` once, and a page for each table, view and procedure, into `OutputDirectory`. |
| `Readme.cst` | `README.md`: the contents and the diagram. |
| `Table.cst`, `View.cst`, `Procedure.cst` | A table's, a view's and a procedure's page. |
| `Docs.cs` | What the templates share (`<%@ Assembly Src="Docs.cs" %>`): pages and links, Markdown text, the diagram's names. |
| `codesmith.options.json` | `langVersion: "latest"` for the templates' own code; warnings reported. |
| `MarkdownDocs.csp` | Generates from the PetShop sample database in this folder (`PetShop.db3`, SQLite). |

To document your own database, change the data source in `MarkdownDocs.csp` (or pick one in VS Code's Template
Properties), and `OutputDirectory` to a folder of your repository (`docs/database`).

[DBDocumenter](../DBDocumenter/README.md) documents a database in HTML.
