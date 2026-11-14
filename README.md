# CodeSmith Generator samples

Templates and projects for [CodeSmith Generator](https://github.com/codesmithtools/Generator) 9.0: the `cs` command
line and the VS Code extension, on Windows, Linux and macOS. Each sample runs as it is, with the one command in its
README; the database samples bring their own SQLite database.

Get one with `cs sample new <name>` (`cs sample list` lists them), or *CodeSmith: New from Sample...* in VS Code. Or
clone this repository.

## Start here

| Sample | What it does |
| --- | --- |
| [HelloWorld](Templates/Start/HelloWorld) | A template that writes a class, and a project that generates two. |
| [TableToClass](Templates/Start/TableToClass) | A C# record for each table of a database. |

## Database

| Sample | What it does |
| --- | --- |
| [ModernEntities](Templates/Database/ModernEntities) | EF Core entities, their configurations and a `DbContext`. |
| [DapperRepositories](Templates/Database/DapperRepositories) | A record and an async Dapper repository for each table. |
| [MarkdownDocs](Templates/Database/MarkdownDocs) | Markdown documentation of a database, with a Mermaid diagram of its tables. |
| [DBDocumenter](Templates/Database/DBDocumenter) | HTML documentation of a database's tables, views and procedures. |
| [ScriptTableData](Templates/Database/ScriptTableData) | An `INSERT` script of a table's rows. |
| [LookupEnums](Templates/Database/LookupEnums) | A C# enum and a TypeScript const from a lookup table's rows. |

## Web

| Sample | What it does |
| --- | --- |
| [FullStack](Templates/Web/FullStack) | EF Core entities, an ASP.NET Core minimal API and a TypeScript client from one database. |

## Examples

| Sample | What it shows |
| --- | --- |
| [BasicSamples](Templates/Examples/BasicSamples) | The template language, a feature at a time (C# and VB). |
| [Merge](Templates/Examples/Merge) | Regenerating into files with hand-written code (PreserveRegions, InsertRegion). |
| [Xml](Templates/Examples/Xml) | XML input typed by a schema (`<%@ XmlProperty %>`). |
| [JsonToClasses](Templates/Examples/JsonToClasses) | Classes from a JSON document. |
| [Maps](Templates/Examples/Maps) | Type maps (`<%@ Map %>`). |
| [Snippets](Templates/Examples/Snippets) | Small templates: a table enum, table properties, an exception class. |
| [ExtendedProperties](Templates/Examples/ExtendedProperties) | Everything a schema provider gives about a database. |

## Also here

- [Legacy](Templates/Legacy): 8.x's framework templates (CSLA 3.8–4.5, PLINQO, Entity Framework 6, NHibernate 3.2) and
  projects, unchanged, for projects built on them.
- `Maps/`: type maps for SQL Server, MySQL, PostgreSQL, Oracle, SQLite, DbType, C#, VB, TypeScript and JavaScript.
- `Data/`: the PetShop sample database for SQLite (`PetShop.db3`), SQL Server, PostgreSQL, MySQL and Oracle, and the
  Tracker database for SQL Server.

Contributions are welcome: a sample should run with one command, against `PetShop.db3` where it needs a database.
