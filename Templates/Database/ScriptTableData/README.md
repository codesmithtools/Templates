# Script a table's data

An `INSERT` script of a table's rows: seed data, or a lookup table copied to another database. The
script is T-SQL, for SQL Server: it deletes the table's rows and inserts these, in one transaction, with
`SET IDENTITY_INSERT` around a table with an identity column. It scripts the PetShop sample database's `Category`
table (`PetShop.db3`, SQLite, in this folder).

Databases: it reads the rows from any database `cs` reads, and writes T-SQL: the script runs on SQL Server. A table
with a `hierarchyid`, `geometry` or `geography` column can't be read: .NET has no type for their values without
`Microsoft.SqlServer.Types`.

```sh
cs generate ScriptTableData.csp
```

The script goes to `Generated/Category.sql`, named for the table (`OutputDirectory` is the folder). Pick another table
for `SourceTable` in `ScriptTableData.csp`, or in VS Code's Template Properties, or on the command line, with a data
source from `cs datasource list`:

```sh
cs generate ScriptTableData.csp -p:SourceTable=<data source>.dbo.Product
```

The script names the table as its database does, with its schema: `[dbo].[Product]`. A database's default schema
that SQL Server doesn't have, PostgreSQL's `public` and SQLite's `main`, is left out: `[product]`. From Oracle, the
schema is the table's owner (`[PETSHOP].[PRODUCT]`) and the `USE` line names Oracle's data source: change both to the
SQL Server database's names before running the script, or create a schema named for the owner there.
