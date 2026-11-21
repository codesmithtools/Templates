# Legacy samples

CodeSmith Generator 8.x's framework templates and projects, kept as they were so that projects built on them still
generate. They run in CodeSmith Generator 9.0 on Windows, Linux and macOS, and write what 8.x wrote. What they
generate targets .NET Framework and Visual Studio 2010–2015 (and SQL Server, mostly), so it builds only there.

| Folder | What it generates |
| --- | --- |
| `Frameworks/Csla` | CSLA .NET 3.8–4.5 business objects (C# and VB). |
| `Frameworks/PLINQO` | LINQ to SQL (PLINQO). |
| `Frameworks/PLINQO-EF-DbContext` | Entity Framework 6 code first. |
| `Frameworks/PLINQO-NH` | NHibernate 3.2. |
| `Database/StoredProcedures` | T-SQL CRUD stored procedures. |
| `Projects` | 8.x's API sample (replaced by [Hosting](../Examples/Hosting/README.md)) and custom property editors (WinForms, replaced by [PropertyEditor](../Examples/PropertyEditor/README.md)), .NET Framework. |

They aren't changed any more. For new work, start from the samples outside this folder.

`Database/StoredProcedures` writes what 8.x wrote, mistakes too: for a table whose last column is a `rowversion`
(`timestamp`), a procedure's parameters end with a comma before `AS`; a table whose columns are all its key gets an
update with an empty `SET`; and names with quotes aren't escaped. Fix those procedures by hand, or generate them with
a template of your own.
