# Table to class

A C# record for each table of a database: the most common thing to generate. It reads the PetShop sample database
(`PetShop.db3`, SQLite) in this folder.

```sh
cs generate TableToClass.csp
```

`Records.cst` is the master template: it renders `Record.cst` (a sub-template, registered with `<%@ Register %>`) once
per table, each to its own file in `Generated/`. `Record.cst` writes a property for each column, with the column's C#
type (`column.GetCSharpTypeName()`: `int`, `string?`, `DateOnly`), `required` where the column can't be null.

To use your own database, change the data source in `TableToClass.csp`, or, in VS Code, pick one for `SourceDatabase`
in Template Properties (*Manage Data Sources* adds it).

Next: [ModernEntities](../../Database/ModernEntities/README.md), EF Core entities and a `DbContext`.
