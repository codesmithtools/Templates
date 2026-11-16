// CodeSmith Generator's engine, for this program. CodeSmith.Generator.Sdk has only what to compile against: at run time
// the engine is cs's own files, loaded from a cs install (the hosting guide). This finds one:
//   1. cs/ beside the program: how you'd ship it (copy a cs install's folder there);
//   2. else the cs on the PATH, which says where its engine is (cs info --json).
// A module initializer runs it before any code here uses the engine's types.
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text.Json;

internal static class CodeSmithEngine
{
    /// <summary>The folder the engine's assemblies load from.</summary>
    public static string Folder { get; private set; } = "";

    [ModuleInitializer]
    internal static void Load()
    {
        Folder = Find() ?? Fail();
        // the engine's assemblies (CodeSmith.Engine, SchemaExplorer, the schema providers, Roslyn, the database
        // drivers) load into the default load context, where the engine expects them
        AssemblyLoadContext.Default.Resolving += (context, name) =>
        {
            string path = Path.Combine(Folder, name.Name + ".dll");
            return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
        };
    }

    private static string? Find()
    {
        string beside = Path.Combine(AppContext.BaseDirectory, "cs");
        if (IsEngine(beside))
            return beside;
        try
        {
            using var cs = Process.Start(new ProcessStartInfo("cs", "info --json") { RedirectStandardOutput = true })!;
            string output = cs.StandardOutput.ReadToEnd();
            cs.WaitForExit();
            using var info = JsonDocument.Parse(output);
            return info.RootElement.TryGetProperty("engine", out var engine) && IsEngine(engine.GetString()) ? engine.GetString() : null;
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception or JsonException)
        {
            return null; // no cs, or one older than its "engine"
        }
    }

    private static bool IsEngine(string? folder) => folder != null && File.Exists(Path.Combine(folder, "CodeSmith.Engine.dll"));

    private static string Fail()
    {
        Console.Error.WriteLine("CodeSmith Generator's engine wasn't found: install cs (CodeSmith.Generator.Cli), or put a cs install in cs/ beside this program.");
        Environment.Exit(1);
        return "";
    }
}
