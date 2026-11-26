// What the API, client and UI templates share: routes, key parameters, request shapes, lists (their sorts, searches and
// filters), JSON names and TypeScript types. Names.cs (the entities' templates') names the C# side.
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
        // and TypeScript's, which the client's parameters can't be either (a module is strict: eval, arguments)
        "delete", "export", "extends", "function", "import", "let", "super", "var", "with", "yield", "await", "debugger",
        "instanceof", "implements", "package", "eval", "arguments",
    };

    // what a key's parameters are beside: the endpoints' other parameters and locals (db, request, environment, entity,
    // problem), and the client's (request, row, and what its calls call: call, found, encodeURIComponent, undefined)
    private static readonly HashSet<string> KeyNeighbours = new HashSet<string>
    {
        "db", "request", "environment", "entity", "problem", "routes", "group", "e",
        "row", "call", "found", "query", "paged", "options", "list", "z", "undefined", "encodeURIComponent",
    };

    /// <summary>A table's endpoints: its DbSet's name in kebab case (LineItems: /api/line-items).</summary>
    public static string Route(TableSchema table) =>
        "/api/" + Regex.Replace(Names.Set(table), "(?<=[a-z0-9])([A-Z])|(?<=[A-Z])([A-Z])(?=[a-z])", "-$1$2").ToLowerInvariant();

    /// <summary>The columns a row is found by: its primary key's (none without one).</summary>
    public static List<ColumnSchema> Key(TableSchema table) =>
        table.HasPrimaryKey
            ? table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns[c.Name]).ToList()
            : new List<ColumnSchema>();

    /// <summary>A key column's parameter, in C# and TypeScript: its property in camel case (OrderId: orderId), with
    /// Value after it where it's a keyword or a name the code around it has (class: classValue, db: dbValue), and a
    /// number where another column's has it.</summary>
    public static string Parameter(ColumnSchema column) => ParameterNames(column.Table).Parameters[column];

    // A property in camel case, as the parameters have it (not JSON's: OrderID is orderID).
    private static string Camel(ColumnSchema column)
    {
        string name = Names.Property(column);
        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    // The table's columns' parameters (Parameter) and filters (Filter), each once in the table; worked out once a table
    // (a schema read again is new tables).
    private static (Dictionary<ColumnSchema, string> Parameters, Dictionary<ColumnSchema, string> Filters) ParameterNames(TableSchema table) =>
        TableParameters.GetValue(table, ParameterNamesOf).Names;

    private sealed class TableNames
    {
        public (Dictionary<ColumnSchema, string>, Dictionary<ColumnSchema, string>) Names;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TableSchema, TableNames> TableParameters =
        new System.Runtime.CompilerServices.ConditionalWeakTable<TableSchema, TableNames>();

    private static TableNames ParameterNamesOf(TableSchema table)
    {
        var parameters = new Dictionary<ColumnSchema, string>();
        var filters = new Dictionary<ColumnSchema, string>();
        var parameterNames = new HashSet<string>();
        var filterNames = new HashSet<string>(ListParameters);
        string Once(string name, HashSet<string> taken)
        {
            string unique = name;
            for (int i = 2; !taken.Add(unique); i++)
                unique = name + i;
            return unique;
        }
        foreach (ColumnSchema column in table.Columns)
        {
            string name = Camel(column);
            parameters[column] = Once(Keywords.Contains(name) || KeyNeighbours.Contains(name) ? name + "Value" : name, parameterNames);
            filters[column] = Once(ListParameters.Contains(name) ? name + "Filter" : Keywords.Contains(name) ? name + "Value" : name, filterNames);
        }
        return new TableNames { Names = (parameters, filters) };
    }

    // ---- the records' and their members' names ----

    // FullStack's own names in a table's records (the response's From, the requests' ToEntity and ApplyTo, the records
    // themselves): a column's property can't be one of its record's members, nor its record's name, so where one has
    // it, the record's name or member gives way (From: FromEntity), as a navigation does; a number after that.
    private static string Own(TableSchema table, params string[] names)
    {
        var taken = new HashSet<string>(Names.Columns(table).Select(Names.Property));
        foreach (string name in names)
            if (!taken.Contains(name))
                return name;
        string last = names[names.Length - 1];
        for (int i = 2; ; i++)
            if (!taken.Contains(last + i))
                return last + i;
    }

    /// <summary>The record a row is returned as: ProductResponse (ProductResponseRecord where a column has that name).</summary>
    public static string Response(TableSchema table) => Own(table, Names.Entity(table) + "Response", Names.Entity(table) + "ResponseRecord");

    /// <summary>The record a new row is sent as: CreateProductRequest (or ...Record).</summary>
    public static string CreateRequest(TableSchema table) => Own(table, "Create" + Names.Entity(table) + "Request", "Create" + Names.Entity(table) + "RequestRecord");

    /// <summary>The record a row's changes are sent as: UpdateProductRequest (or ...Record).</summary>
    public static string UpdateRequest(TableSchema table) => Own(table, "Update" + Names.Entity(table) + "Request", "Update" + Names.Entity(table) + "RequestRecord");

    /// <summary>The response's factory from an entity: From (FromEntity where a column is named From).</summary>
    public static string From(TableSchema table) => Own(table, "From", "FromEntity");

    /// <summary>The new row's request's entity: ToEntity (CreateEntity where a column is named ToEntity).</summary>
    public static string ToEntity(TableSchema table) => Own(table, "ToEntity", "CreateEntity");

    /// <summary>What applies the changes to an entity: ApplyTo (ApplyToEntity where a column is named ApplyTo).</summary>
    public static string ApplyTo(TableSchema table) => Own(table, "ApplyTo", "ApplyToEntity");

    /// <summary>The key's route template: "{orderId}/{lineNum}".</summary>
    public static string KeyRoute(TableSchema table) => string.Join("/", Key(table).Select(c => "{" + Parameter(c) + "}"));

    /// <summary>A column's JSON name, as ASP.NET Core writes it (System.Text.Json's camel case: AccountID is accountID).</summary>
    public static string Json(ColumnSchema column) => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(Names.Property(column));

    /// <summary>A column the database fills in itself (an identity, SQLite's INTEGER PRIMARY KEY, a rowversion, a computed
    /// column).</summary>
    public static bool Generated(ColumnSchema column) =>
        Names.IsRowVersion(column) || Names.IsComputed(column) ||
        bool.TryParse(Convert.ToString(column.ExtendedProperties["CS_IsIdentity"]?.Value), out bool identity) && identity;

    /// <summary>What a new row is given: every column the database doesn't fill in.</summary>
    public static List<ColumnSchema> Creates(TableSchema table) => Names.Columns(table).Where(c => !Generated(c)).ToList();

    /// <summary>What a change can set: the new row's columns but its key.</summary>
    public static List<ColumnSchema> Updates(TableSchema table) => Creates(table).Where(c => !c.IsPrimaryKeyMember).ToList();

    /// <summary>A request property's validation: [MaxLength(80)] for a text column with a size (not text or ntext,
    /// whose size is the storage's).</summary>
    public static string Validation(ColumnSchema column) =>
        Names.Type(column).TrimEnd('?') == "string" && column.Size > 0 && column.Size < 1_000_000 && !Blob(column)
            ? "[MaxLength(" + column.Size + ")]" : null;

    // ---- lists: their order, search and filters ----

    // what a list can sort and filter by: the simple types
    private static readonly HashSet<string> Comparable = new HashSet<string>
    {
        "string", "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal",
        "DateTime", "DateTimeOffset", "DateOnly", "TimeOnly", "TimeSpan", "Guid",
    };

    // and what no database compares, though it's text to .NET
    private static readonly HashSet<string> Incomparable = new HashSet<string>
    {
        "json", "jsonb", "xml", "geography", "geometry", "hierarchyid", "sql_variant", "image", "tsvector", "inet", "cidr",
        "macaddr", "interval", "bit varying", "varbit",
    };

    // the parameters a list has already, its locals (rows, pattern, total, items; e, its lambdas'), and the UI's list's
    // own in its address (q, its search), which a filter's name can't be
    private static readonly HashSet<string> ListParameters = new HashSet<string>
    {
        "page", "pageSize", "sort", "desc", "search", "db", "rows", "pattern", "isNumber", "number", "total", "items", "e", "q",
    };

    /// <summary>The column's database type, in lower case and without its size: varchar, numeric, text.</summary>
    public static string Native(ColumnSchema column)
    {
        string native = (column.NativeType ?? "").ToLowerInvariant();
        int size = native.IndexOf('(');
        return (size >= 0 ? native.Substring(0, size) : native).Trim();
    }

    // SQL Server's text, ntext and image: LIKE reads them, but nothing sorts or compares them
    private static bool Blob(ColumnSchema column) =>
        Native(column) is "text" or "ntext" or "image" && Provider(column.Table.Database) == "SqlSchemaProvider";

    /// <summary>Whether a list sorts by a column: a number, text, a date or time, a Guid, a bool (not bytes, JSON, XML,
    /// a spatial type, nor SQL Server's text and ntext).</summary>
    public static bool Sortable(ColumnSchema column) =>
        Comparable.Contains(Names.Type(column).TrimEnd('?')) && !Incomparable.Contains(Native(column)) && !Blob(column);

    /// <summary>Whether a list's search looks in a column: its text (char, varchar, text and the like).</summary>
    public static bool Searchable(ColumnSchema column)
    {
        string native = Native(column);
        return Names.Type(column).TrimEnd('?') == "string" && !Incomparable.Contains(native) &&
            (native.Contains("char") || native.Contains("text") || native.Contains("clob") || native is "citext" or "name" or "string" or "");
    }

    /// <summary>The columns a list searches.</summary>
    public static List<ColumnSchema> Searches(TableSchema table) => table.Columns.Cast<ColumnSchema>().Where(Searchable).ToList();

    /// <summary>The columns a list can sort by.</summary>
    public static List<ColumnSchema> Sorts(TableSchema table) => table.Columns.Cast<ColumnSchema>().Where(Sortable).ToList();

    /// <summary>The columns a list filters by (rows whose column is one of the values asked for): those of a key, its
    /// own, a foreign key's, or one another table's foreign key refers to. A child's rows are found by them, and the rows
    /// a page refers to.</summary>
    public static List<ColumnSchema> Filters(TableSchema table) =>
        table.Columns.Cast<ColumnSchema>().Where(c => (c.IsPrimaryKeyMember || c.IsForeignKeyMember || Referred(c)) && Sortable(c)).ToList();

    // whether another table's foreign key refers to a column
    private static bool Referred(ColumnSchema column) =>
        column.Table.Database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
            .Any(k => k.PrimaryKeyTable.Owner == column.Table.Owner && k.PrimaryKeyTable.Name == column.Table.Name &&
                      k.PrimaryKeyMemberColumns.Cast<MemberColumnSchema>().Any(m => m.Name == column.Name));

    /// <summary>A filter's query parameter: the column's property in camel case (productId), with Filter after it where
    /// the list has one of that name already (pageFilter), Value where it's a keyword (classValue), and a number where
    /// another column's has it.</summary>
    public static string Filter(ColumnSchema column) => ParameterNames(column.Table).Filters[column];

    /// <summary>Whether a column's C# type is a value type (int, not string): a nullable one is read with .Value.</summary>
    public static bool IsValueType(ColumnSchema column) => Names.Type(column).TrimEnd('?') is not ("string" or "byte[]" or "object");

    /// <summary>Whether the search lowers its text first (PostgreSQL's LIKE minds case).</summary>
    public static bool SearchLowers(TableSchema table) => Provider(table.Database) == "PostgreSQLSchemaProvider";

    /// <summary>The database's provider, by name: for SQL scripts (DdlSchemaProvider), the provider of the scripts'
    /// dialect (CS_Dialect).</summary>
    public static string Provider(DatabaseSchema database)
    {
        string name = database.Provider?.Name ?? "";
        if (name != "DdlSchemaProvider")
            return name;
        return Convert.ToString(database.ExtendedProperties["CS_Dialect"]?.Value) switch
        {
            "SqlServer" => "SqlSchemaProvider",
            "PostgreSQL" => "PostgreSQLSchemaProvider",
            "MySQL" => "MySQLSchemaProvider",
            "SQLite" => "SQLiteSchemaProvider",
            _ => name,
        };
    }

    /// <summary>A whole-number column of the key: a search that's a number finds its row by it.</summary>
    public static bool NumberKey(ColumnSchema column) =>
        column.IsPrimaryKeyMember && Names.Type(column).TrimEnd('?') is "byte" or "short" or "int" or "long";

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

    /// <summary>A key or filter parameter's TypeScript type.</summary>
    public static string TypeScript(ColumnSchema column) =>
        Zod(column, false).StartsWith("z.number") ? "number" : Zod(column, false).StartsWith("z.boolean") ? "boolean" : "string";

    /// <summary>The client's property for a table's calls: its set in camel case (lineItems).</summary>
    public static string ClientProperty(TableSchema table) => char.ToLowerInvariant(Names.Set(table)[0]) + Names.Set(table).Substring(1);
}
