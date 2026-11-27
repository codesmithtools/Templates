# Legacy samples

CodeSmith Generator 8.x's framework templates and projects, kept as they were so that projects built on them still
generate. They run in CodeSmith Generator 9.0 on Windows, Linux and macOS, and write what 8.x wrote. What they
generate targets .NET Framework and Visual Studio 2010–2015 (and SQL Server, mostly): it runs on Windows, and builds
with the .NET SDK anywhere (see
[framework templates in CI](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/moving-from-8x.md#framework-templates-plinqo-csla-nhibernate-in-ci)).

| Folder | What it generates |
| --- | --- |
| `Frameworks/Csla` | CSLA .NET 3.8–4.5 business objects (C# and VB). |
| `Frameworks/PLINQO` | LINQ to SQL (PLINQO): see its [README](Frameworks/PLINQO/README.md). |
| `Frameworks/PLINQO-EF-DbContext` | Entity Framework 6 code first. |
| `Frameworks/PLINQO-NH` | NHibernate 3.2. |
| `Database/StoredProcedures` | T-SQL CRUD stored procedures. |
| `Projects` | 8.x's API sample (replaced by [Hosting](../Examples/Hosting/README.md)) and custom property editors (WinForms, replaced by [PropertyEditor](../Examples/PropertyEditor/README.md)), .NET Framework. |

They get fixes, not new features. For new work, start from the samples outside this folder.

PLINQO's entity files (`Product.cs`, `Product.vb`: the partial class whose `Metadata` class keeps the attributes you add)
can use today's C# and VB: `=>` members, file-scoped namespaces, `init`, nullable annotations, and `nameof`,
interpolated and raw strings in the attributes; in VB, auto-implemented properties, attributes on their own line
without ` _`, and `NameOf`. Entities.cst keeps each C# `Metadata` member as written (in VB, its attributes), with the
`#if` lines around its attributes, at the class's indentation: see
[PLINQO's README](Frameworks/PLINQO/README.md#the-metadata-class). 8.x read these files as C# 3 and VB 9 only, and kept
only the attributes.

`Database/StoredProcedures` writes what 8.x wrote, mistakes too: for a table whose last column is a `rowversion`
(`timestamp`), a procedure's parameters end with a comma before `AS`; a table whose columns are all its key gets an
update with an empty `SET`; and names with quotes aren't escaped. Fix those procedures by hand, or generate them with
a template of your own.
