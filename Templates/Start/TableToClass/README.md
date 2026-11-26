# Table to class

A C# record for each table of a database: the most common thing to generate. It reads the PetShop sample database
(`PetShop.db3`, SQLite) in this folder.

Databases: any `cs` reads (SQL Server, SQLite, PostgreSQL, MySQL, Oracle, and SQL scripts).

```sh
cs generate TableToClass.csp
```

`Records.cst` is the master template: it renders `Record.cst` (a sub-template, registered with `<%@ Register %>`) once
per table, each to its own file in `Generated/`, and lists them in `Generated/Files.txt` (its output). A table that's
gone takes its file with it: the file the last run listed and this one doesn't write is removed (`RemoveOutput`; `cs
check` says it's out of date). `Record.cst` writes a property for each column, with the column's C#
type (`column.GetCSharpTypeName()`: `int`, `string?`, `DateOnly`), `required` where the column can't be null. Two
tables of the same name in two schemas are `DboPerson` and `SalesPerson`, in `dbo.Person.cs` and `sales.Person.cs`.
It's a starting point: [ModernEntities](../../Database/ModernEntities/README.md) also names a column that starts with a
digit, or is named like its table.

To use your own database, change the data source in `TableToClass.csp`, or, in VS Code, pick one for `SourceDatabase`
in Template Properties (*Manage Data Sources* adds it).

Next: [ModernEntities](../../Database/ModernEntities/README.md), EF Core entities and a `DbContext`.
