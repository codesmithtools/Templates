// What the API and client templates share: routes, key parameters, request shapes, JSON names and TypeScript types.
// Names.cs (the entities' templates') names the C# side.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeSmith.Engine;
using SchemaExplorer;

public static class Web
{
    private static readonly HashSet<string> Keywords = new HashSet<string>
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
        "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
        "using", "virtual", "void", "volatile", "while",
        // and TypeScript's, which the client's parameters can't be either
        "delete", "export", "extends", "function", "import", "let", "super", "var", "with", "yield", "await",
    };

    /// <summary>A table's endpoints: its DbSet's name in kebab case (LineItems: /api/line-items).</summary>
    public static string Route(TableSchema table) =>
        "/api/" + Regex.Replace(Names.Set(table), "(?<=[a-z0-9])([A-Z])|(?<=[A-Z])([A-Z])(?=[a-z])", "-$1$2").ToLowerInvariant();

    /// <summary>The columns a row is found by: its primary key's (none without one).</summary>
    public static List<ColumnSchema> Key(TableSchema table) =>
        table.HasPrimaryKey
            ? table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns[c.Name]).ToList()
            : new List<ColumnSchema>();

    /// <summary>A key column's parameter, in C# and TypeScript: its property in camel case (OrderId: orderId).</summary>
    public static string Parameter(ColumnSchema column)
    {
        string name = Names.Property(column);
        name = char.ToLowerInvariant(name[0]) + name.Substring(1);
        return Keywords.Contains(name) ? name + "Value" : name;
    }

    /// <summary>The key's route template: "{orderId}/{lineNum}".</summary>
    public static string KeyRoute(TableSchema table) => string.Join("/", Key(table).Select(c => "{" + Parameter(c) + "}"));

    /// <summary>A column's JSON name, as ASP.NET Core writes it (System.Text.Json's camel case: AccountID is accountID).</summary>
    public static string Json(ColumnSchema column) => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(Names.Property(column));

    /// <summary>A column the database fills in itself (an identity, SQLite's INTEGER PRIMARY KEY, a rowversion).</summary>
    public static bool Generated(ColumnSchema column) =>
        Names.IsRowVersion(column) ||
        bool.TryParse(Convert.ToString(column.ExtendedProperties["CS_IsIdentity"]?.Value), out bool identity) && identity;

    /// <summary>What a new row is given: every column the database doesn't fill in.</summary>
    public static List<ColumnSchema> Creates(TableSchema table) => table.Columns.Cast<ColumnSchema>().Where(c => !Generated(c)).ToList();

    /// <summary>What a change can set: the new row's columns but its key.</summary>
    public static List<ColumnSchema> Updates(TableSchema table) => Creates(table).Where(c => !c.IsPrimaryKeyMember).ToList();

    /// <summary>A request property's validation: [MaxLength(80)] for a text column with a size.</summary>
    public static string Validation(ColumnSchema column) =>
        Names.Type(column).TrimEnd('?') == "string" && column.Size > 0 && column.Size < 1_000_000 ? "[MaxLength(" + column.Size + ")]" : null;

    /// <summary>A column's Zod schema: z.string(), z.number(), z.boolean(); nullable where the column is.</summary>
    public static string Zod(ColumnSchema column, bool request)
    {
        string type = Names.Type(column).TrimEnd('?');
        string schema = type switch
        {
            "bool" => "z.boolean()",
            "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "float" or "double" or "decimal" => "z.number()",
            _ => "z.string()", // text, dates and times (ISO 8601), Guids, bytes (base64)
        };
        if (request && Validation(column) != null)
            schema += ".max(" + column.Size + ")";
        // a request can leave out what may be null (null on the C# side); a response always has it
        return !column.AllowDBNull ? schema : request ? schema + ".nullish()" : schema + ".nullable()";
    }

    /// <summary>A key parameter's TypeScript type.</summary>
    public static string TypeScript(ColumnSchema column) => Zod(column, false).StartsWith("z.number") ? "number" : "string";
}
