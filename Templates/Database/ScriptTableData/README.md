# Script a table's data

An `INSERT` statement for every row of a table: seed data, or a copy of a lookup table for another database. The
script is T-SQL (`SET IDENTITY_INSERT` around tables with an identity column). It scripts the PetShop sample
database's `Category` table (`PetShop.db3`, SQLite, in this folder).

```sh
cs generate ScriptTableData.csp
```

The script goes to `Generated/Category.sql`. Pick another table for `SourceTable` in `ScriptTableData.csp`, or in VS
Code's Template Properties.
