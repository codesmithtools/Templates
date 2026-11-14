# Script a table's data

An `INSERT` script of a table's rows: seed data, or a lookup table copied to another database. The
script is T-SQL (`SET IDENTITY_INSERT` around tables with an identity column). It scripts the PetShop sample
database's `Category` table (`PetShop.db3`, SQLite, in this folder).

```sh
cs generate ScriptTableData.csp
```

The script goes to `Generated/Category.sql`. Pick another table for `SourceTable` in `ScriptTableData.csp`, or in VS
Code's Template Properties.
