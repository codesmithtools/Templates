# Hello world

A template that writes a C# class, and a project that generates two classes from it. Start here.

Render the template, giving its properties values:

```sh
cs render Hello.cst -p:ClassName=Greeting -p:Fields=Name,Message
```

Generate the project (`Hello.csp`): each output is the template with its own values, written to `Generated/`.

```sh
cs generate Hello.csp
```

In VS Code, open `Hello.cst`: Template Properties shows its properties, and ▷ generates it with their values. Open
`Hello.csp` to edit the project's outputs.

| File | What it is |
| --- | --- |
| `Hello.cst` | The template: directives (`<%@ %>`), properties, text, and code in `<% %>` and `<%= %>`. |
| `Hello.csp` | The project: an output per class, with its values and file. |

Next: [TableToClass](../TableToClass/README.md), a class for each table of a database.
