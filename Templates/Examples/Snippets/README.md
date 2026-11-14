# Snippets

Small templates for code you'd otherwise type by hand. 8.x ran them as ActiveSnippets in Visual Studio's editor.

| Template | What it writes |
| --- | --- |
| `TableEnum.cst` | An enum with a member for each table of a database. |
| `TableProperties.cst` | A property for each column of a table. |
| `CustomException.cst` | An exception class with the usual constructors. |

```sh
cs generate Snippets.csp
```

The files go to `Generated/`, from the PetShop sample database (`PetShop.db3`, SQLite) in this folder. Render one on
its own with `cs render`, giving its properties with `-p`.
