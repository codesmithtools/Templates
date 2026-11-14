# XML input

Templates whose input is an XML document, typed by its schema (`<%@ XmlProperty %>`):

- `PurchaseOrder.cst` reads `SamplePurchaseOrder.xml` (`PurchaseOrder.xsd`).
- `EntityMaster.cst` renders `Entity.cst` once for each entity of `NorthwindMap.xml` (`EntityMap.xsd`), a file each.
- `GenerateEntityMap.cst` writes such a map from a database.

```sh
cd CSharp
cs render PurchaseOrder.cst
cs generate EntityMaster.csp
```

The classes go to `output/`. C# and VB.
