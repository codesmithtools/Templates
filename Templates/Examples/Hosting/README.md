# Hosting

Generating from a .NET program of your own: a console app that hosts CodeSmith Generator's engine, and a project whose
build generates its code. It replaces 8.x's API sample.

```sh
dotnet run --project Host
dotnet run --project App
```

Both need `cs` on your `PATH`, and CodeSmith's NuGet packages. Until those are on nuget.org, set `CODESMITH_PACKAGES`
first: see [Before the packages are on nuget.org](#before-the-packages-are-on-nugetorg).

## Host: the engine in your program

`Host/Program.cs` uses `TemplateGenerator` (`CodeSmith.Engine.Hosting`), the
[hosting API](https://github.com/codesmithtools/Generator/blob/master/Documentation/CrossPlatformCli/guides/hosting.md):

- `RenderAsync` renders `Record.cst` with property values set in code, as `cs render -p:Name=Value` would, and prints
  the record it wrote (`Address`).
- A property the template doesn't have is a diagnostic in the result, not an exception. So are a template's compile
  errors and an exception in its code, at the template's line.
- `GenerateAsync` generates `App/Records.csp`, as `cs generate` would, and lists each file: `Written`, or `Unchanged`
  when it's already what the template writes (run it twice).
- `GeneratorOptions` holds the options for this generator's calls: line endings, warnings, the C# version of template
  code, the culture, and property values for every output.

The results are what `cs --json` reports: each output with its status and files, and diagnostics with their file,
line and code.

Where the engine comes from:

- **To compile:** the `CodeSmith.Generator.Sdk` package, the engine's reference assemblies. Nothing of it is copied to
  the output.
- **To run:** `cs`'s own files. `Host/CodeSmithEngine.cs` loads them, before anything uses them, from a `cs` folder
  beside the program, else from the `cs` on your `PATH` (`cs info` shows where its engine is). To ship a program that
  generates, copy a `cs` install's files into a `cs` folder beside it.
- Template code runs in your process, and its helper DLLs stay loaded. A program that runs for days and compiles many
  versions of its templates grows; run `cs --json generate` from it instead, and read the report.

## App: generate on build

`App/App.csproj` uses `CodeSmith.Generator.MSBuild`:

```xml
<PackageReference Include="CodeSmith.Generator.MSBuild" Version="9.0.0-*" PrivateAssets="all" />
<CodeSmithProject Include="Records.csp" />
```

Its build runs `cs generate Records.csp` before the compiler, and compiles what it writes (`App/Generated/`, a record
for each table of `PetShop.db3`). `App/Program.cs` uses the records, which are in no file of the project until it
builds.

- The build generates again only when the `.csp`, a template, or anything else generating read changed. Otherwise it
  skips the step (`dotnet build -v:n` says so).
- A database's schema isn't a file it reads: after one changes, `dotnet build -p:CodeSmithForce=true`.
- A template's error is a build error at the template's line (`Record.cst(12,5): error CS0103: ...`).
- `CodeSmithCommand` says how to run `cs`: `cs` on the `PATH`, or `dotnet cs` where a `.config/dotnet-tools.json`
  above the project has the `CodeSmith.Generator.Cli` tool. The package's README has the rest.

## Before the packages are on nuget.org

The packages (`CodeSmith.Generator.Sdk`, `CodeSmith.Generator.MSBuild`, and `CodeSmith.Generator.Cli`, the `cs` .NET
tool) come from a CodeSmith Generator release, or from a CI run's `tool` artifact. Put their `.nupkg` files in a
folder, and:

```sh
export CODESMITH_PACKAGES=/path/to/packages     # PowerShell: $env:CODESMITH_PACKAGES = "C:\path\to\packages"
dotnet tool install -g CodeSmith.Generator.Cli --prerelease --add-source "$CODESMITH_PACKAGES"
```

Both projects add that folder to their package sources (`RestoreAdditionalProjectSources`). From a Generator checkout,
`dotnet pack src/CodeSmith.Generator.Sdk -o <folder>` (and `src/CodeSmith.Generator.MSBuild`, `src/cs`) makes them.

Once they're on nuget.org:

- delete the `RestoreAdditionalProjectSources` line from `Host/Host.csproj` and `App/App.csproj`, and don't set
  `CODESMITH_PACKAGES`;
- install `cs` with `dotnet tool install -g CodeSmith.Generator.Cli`;
- pin the packages' version (`Version="9.0.0"`) instead of the newest preview (`9.0.0-*`).

| File | What it is |
| --- | --- |
| `Record.cst` | A C# record from a name and its properties (`string Street`, `int? Floor`). |
| `Records.cst` | `Record.cst` for each table of a database, a file each. |
| `Host/` | The console app: `Program.cs` (render and generate) and `CodeSmithEngine.cs` (where the engine loads from). |
| `App/` | The project that generates on build: `App.csproj`, `Records.csp` (`PetShop.db3`'s records, into `Generated/`) and `Program.cs`. |
| `PetShop.db3` | The PetShop sample database (SQLite). |
