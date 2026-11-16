// A program that generates with CodeSmith Generator's engine: TemplateGenerator renders a template with properties set
// here, and generates a project (.csp) as cs generate does. The results are cs --json's: outputs, files, diagnostics.
using CodeSmith.Engine.Hosting;

var generator = new TemplateGenerator(new GeneratorOptions { Warnings = "report" });
string here = Environment.CurrentDirectory;

// A template, with property values from code (as cs render's -p:Name=Value): its text comes back
var address = await generator.RenderAsync("Record.cst", new Dictionary<string, string>
{
    ["Namespace"] = "Acme.Shipping",
    ["Name"] = "Address",
    ["Properties"] = "string Street, string City, string? Region, string PostalCode",
    ["Summary"] = "Where an order goes.",
});
Report(address);
Console.WriteLine(address.Text);

// A property the template doesn't have: a diagnostic, not an exception
Report(await generator.RenderAsync("Record.cst", new Dictionary<string, string> { ["Colour"] = "Blue" }));

// A project: each output's files, written, or unchanged where they're the same (run it twice)
var records = await generator.GenerateAsync("App/Records.csp");
Report(records);
foreach (var output in records.Outputs)
    foreach (var file in output.Files)
        Console.WriteLine($"  {output.Name}: {Path.GetRelativePath(here, file.Path)} {file.Status}");
return records.Success ? 0 : 1;

static void Report(GenerationResult result)
{
    Console.WriteLine(result.Success ? "Succeeded" : "Failed");
    foreach (var diagnostic in result.Diagnostics)
        Console.WriteLine($"  {diagnostic}");
}
