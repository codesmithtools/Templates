# Basic samples

The template language, a feature at a time, in C# and VB:

| Template | What it shows |
| --- | --- |
| `CodeSmith101/sample1.cst` to `sample4.cst` | Text and `<%= %>`, `<% %>` code, properties, a database table property. |
| `SimpleTemplate.cst` | A small template, start to end. |
| `TemplateScript.cst` | Methods in `<script runat="template">`. |
| `CodeBehind.cst` | A base class in a code-behind file (`Src=` and `Inherits=`). |
| `PartialTemplate.cst` | A partial code-behind class. |
| `MasterTemplate.cst`, `SubTemplate.cst` | A master template rendering a sub-template. |
| `Includes/Includes.cst` | `<!-- #include file="..." -->`. |
| `UsingLinq.cst` | LINQ in template code. |
| `ProgressReporting.cst` | Reporting progress while generating. |

```sh
cd CSharp
cs render SimpleTemplate.cst
cs render CodeSmith101/sample3.cst -p:Person=Ada
cs render Includes/Includes.cst -p:SomeValue=a -p:SomeIncludeValue=b
```

Templates with required properties say which when they're missing; in VS Code, Template Properties asks for them.
`Includes/HeaderInclude.cst` is the file `Includes.cst` includes, not a template of its own.
