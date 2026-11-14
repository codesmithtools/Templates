# Maps

Type maps: the `<%@ Map %>` directive looks values up in a `.csmap` file. Here, SQL Server types to C# types
(`Sql-CSharp`, one of the maps CodeSmith Generator comes with), and a map of the sample's own (`MapSample.csmap`).
It shows a default value for keys that aren't there, a default for one lookup, and the exception otherwise.

```sh
cs render MapSample.cst
```

In VS Code, `.csmap` files open in the map editor.
