# JSON to classes

C# classes for the shape of a JSON document: a class for each object, a property for each member, lists for
arrays. Its input is a file, not a database.

```sh
cs render JsonToClasses.cst
cs render JsonToClasses.cst -p:JsonFile=sample2.json -p:RootClassName=Order
```

`JsonFile` is relative to the template (`sample.json` by default).
