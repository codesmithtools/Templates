# Merging into existing files

Regenerating without losing hand-written code. Each project (`MergeSamples.csp`) writes its outputs with a merge
strategy, set by the output's `mergeType` and `mergeInit`:

- **PreserveRegions**: the file's regions whose names match `RegionNameRegex` keep what's in them; the template
  replaces everything else. In C#, VB, JavaScript, SQL and XML.
- **InsertRegion**: the template's output replaces one region (`RegionName`) of a file that is otherwise yours. In C#,
  VB and JavaScript.

```sh
cd CSharp
cs generate MergeSamples.csp
```

Edit a custom region in `PreserveRegionsSample.cs`, generate again, and your edit stays.
