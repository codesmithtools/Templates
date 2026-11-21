# JSON to classes

C# classes for the shape of a JSON document: a class for each object, a property for each member, lists for
arrays. Its input is a file, not a database.

```sh
cs render JsonToClasses.cst
cs render JsonToClasses.cst -p:JsonFile=sample2.json -p:RootClassName=Order
```

`JsonFile` is relative to the template (`sample.json` by default).

A member's name that isn't a C# name becomes one, as `cs new` makes them (`123abc`: `_123abc`), and one named like
its class gets `Value` after it. An array of values of different kinds (`[1, "two"]`) is a `List<object>`; a whole
number is an `int`, or a `long` or `decimal` when it's bigger. A file whose root is an array of objects (a list) makes
the root class from the first of them.
