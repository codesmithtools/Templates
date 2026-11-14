# Extended properties

Everything a schema provider says about a database, as extended properties. It lists the database's, and each
table's, column's, view's, key's, index's, procedure's and parameter's. The quickest way to see what a template can read from yours
(`CS_Default`, `CS_IsIdentity`, `CS_Description`, and a database's own). It reads the PetShop sample database
(`PetShop.db3`, SQLite) in this folder.

```sh
cs generate ExtendedProperties.csp
```

The list goes to `Generated/ExtendedProperties.txt`. Point `SourceDatabase` at your own database to see its.
