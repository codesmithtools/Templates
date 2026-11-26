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
    // it, the record's name or member gives way (From: FromEntity), as a navigation does; a number after that. A
    // record's name gives way to a table's entity too (a table named ProductResponse: Product's is ProductResponseRecord).
    private static string Own(TableSchema table, params string[] names) => Own(table, false, names);

    private static string Own(TableSchema table, bool type, params string[] names)
    {
        var taken = new HashSet<string>(Names.Columns(table).Select(Names.Property));
        if (type)
            taken.UnionWith(Entities(table.Database));
        foreach (string name in names)
            if (!taken.Contains(name))
                return name;
        string last = names[names.Length - 1];
        for (int i = 2; ; i++)
            if (!taken.Contains(last + i))
                return last + i;
    }

    /// <summary>The record a row is returned as: ProductResponse (ProductResponseRecord where a column or a table's
    /// entity has that name).</summary>
    public static string Response(TableSchema table) => Own(table, true, Names.Entity(table) + "Response", Names.Entity(table) + "ResponseRecord");

    /// <summary>The record a new row is sent as: CreateProductRequest (or ...Record).</summary>
    public static string CreateRequest(TableSchema table) => Own(table, true, "Create" + Names.Entity(table) + "Request", "Create" + Names.Entity(table) + "RequestRecord");

    /// <summary>The record a row's changes are sent as: UpdateProductRequest (or ...Record).</summary>
    public static string UpdateRequest(TableSchema table) => Own(table, true, "Update" + Names.Entity(table) + "Request", "Update" + Names.Entity(table) + "RequestRecord");

    // The names of the database's tables' entities, worked out once (a schema read again is new tables).
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, HashSet<string>> EntityNames =
        new System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, HashSet<string>>();

    /// <summary>Every table's entity's name.</summary>
    public static HashSet<string> Entities(DatabaseSchema database) =>
        EntityNames.GetValue(database.Tables, tables => new HashSet<string>(tables.Cast<TableSchema>().Select(Names.Entity), StringComparer.Ordinal));

    /// <summary>A type the API or the client has of its own (SaveProblem, Problem, RowCount): Record after it where a
    /// table's entity has its name, so the entity's is never hidden.</summary>
    public static string Own(DatabaseSchema database, string name) => Entities(database).Contains(name) ? name + "Record" : name;

    /// <summary>What a table's entity is named like, of the types FullStack generates, and what that type is named
    /// instead (a warning each): another table's records, the API's and the client's own types.</summary>
    public static List<string> Clashes(TableSchema table, string apiName)
    {
        string entity = Names.Entity(table);
        var clashes = new List<string>();
        void Clash(string what, string instead) =>
            clashes.Add(table.FullName + "'s entity is " + entity + ", the name of " + what + ", so that one is " + instead + ".");
        foreach (TableSchema other in table.Database.Tables)
        {
            string name = Names.Entity(other);
            if (other == table)
                continue;
            if (name + "Configuration" == entity)
                Clash(other.FullName + "'s EF Core configuration", Names.Configuration(other));
            if (name + "Response" == entity)
                Clash(other.FullName + "'s response record", Response(other));
            if ("Create" + name + "Request" == entity)
                Clash(other.FullName + "'s create request", CreateRequest(other));
            if ("Update" + name + "Request" == entity)
                Clash(other.FullName + "'s update request", UpdateRequest(other));
            foreach (string helper in new[] { "Sort", "Filter", "ListOptions" })
                if (name + helper == entity)
                    Clash("the client's " + name + helper + " type", Own(table.Database, name + helper));
        }
        foreach (string own in new[] { "SaveProblem", "SaveRefusal", apiName + "RowCount" })
            if (own == entity)
                Clash("the API's own " + own, Own(table.Database, own));
        foreach (string own in new[] { "Paged", "ListOptions", "RowCount", "Problem", "ApiError" })
            if (own == entity)
                Clash("the client's own " + own, Own(table.Database, own));
        return clashes;
    }

    // the .NET types the API and the client know: text, numbers, bools, dates and times, GUIDs and bytes
    private static readonly HashSet<string> Known = new HashSet<string>
    {
        "string", "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal",
        "DateTime", "DateTimeOffset", "DateOnly", "TimeOnly", "TimeSpan", "Guid", "byte[]", "char",
    };

    /// <summary>Whether the API and the UI take a column as text without knowing it is (PostgreSQL's inet, a range,
    /// hstore): its .NET type isn't one they know, nor an array of one.</summary>
    public static bool Unknown(ColumnSchema column) =>
        Names.Mapped(column) && BitString(column) == null && !Known.Contains(IsArray(column) ? Element(column) : Names.Type(column).TrimEnd('?'));

    // ---- tables left out ----

    /// <summary>Whether a table is left out (Exclude: sysdiagrams, __EFMigrationsHistory, aspnet_*): Names.Excluded, as
    /// the entities have it.</summary>
    public static bool Excluded(TableSchema table, string patterns) => Names.Excluded(table, patterns);

    /// <summary>The tables the app has: all but those left out.</summary>
    public static List<TableSchema> Tables(DatabaseSchema database, string exclude) => Names.Tables(database, exclude);

    /// <summary>Whether a foreign key is a relationship of the app's: both its tables are in it, with keys.</summary>
    public static bool Related(TableKeySchema key, string exclude) => Names.Related(key, exclude);

    /// <summary>The app's tables' foreign keys that refer to a table.</summary>
    public static List<TableKeySchema> ReferencesTo(TableSchema table, string exclude) => Names.ReferencesTo(table, exclude).ToList();

    /// <summary>The response's factory from an entity: From (FromEntity where a column is named From).</summary>
    public static string From(TableSchema table) => Own(table, "From", "FromEntity");

    /// <summary>The new row's request's entity: ToEntity (CreateEntity where a column is named ToEntity).</summary>
    public static string ToEntity(TableSchema table) => Own(table, "ToEntity", "CreateEntity");

    /// <summary>What applies the changes to an entity: ApplyTo (ApplyToEntity where a column is named ApplyTo).</summary>
    public static string ApplyTo(TableSchema table) => Own(table, "ApplyTo", "ApplyToEntity");

    /// <summary>The key's route template: "{orderId}/{lineNum}". A last column of text takes the rest of the path
    /// ({**key}), so a key with a / in it (a b/c) can be read: the client sends its / as it is.</summary>
    public static string KeyRoute(TableSchema table) => string.Join("/", Key(table).Select(c => "{" + (CatchAll(c) ? "**" : "") + Parameter(c) + "}"));

    /// <summary>Whether a key's column is its route's catch-all: the last, and text.</summary>
    public static bool CatchAll(ColumnSchema column)
    {
        var key = Key(column.Table);
        return key.Count > 0 && key[key.Count - 1] == column && Names.Type(column).TrimEnd('?') == "string";
    }

    /// <summary>A key's column in a row's address, in C# (the Location of a new row): text escaped, a catch-all's /
    /// kept.</summary>
    public static string Location(ColumnSchema column, string value) =>
        Names.Type(column).TrimEnd('?') != "string" ? "{" + value + "}"
            : "{Uri.EscapeDataString(" + value + ")" + (CatchAll(column) ? ".Replace(\"%2F\", \"/\")" : "") + "}";

    /// <summary>A key's column in a row's address, in TypeScript: encoded, a catch-all's / kept.</summary>
    public static string EncodeTs(ColumnSchema column) =>
        "${encodeURIComponent(" + Parameter(column) + ")" + (CatchAll(column) ? ".replace(/%2F/gi, \"/\")" : "") + "}";

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
    /// whose size is the storage's, nor an enum, whose values say it).</summary>
    public static string Validation(ColumnSchema column) =>
        Names.Type(column).TrimEnd('?') == "string" && column.Size > 0 && column.Size < 1_000_000 && !Blob(column) && EnumValues(column) == null
            ? "[MaxLength(" + column.Size + ")]" : null;

    /// <summary>A request property's checks, as attributes (minimal APIs' validation: a 400 that names the field):
    /// its length, an enum's values, a SET's, a bit string's 0s and 1s. A new row's column with a default may be left
    /// out (null).</summary>
    public static List<string> Checks(ColumnSchema column, bool create)
    {
        var checks = new List<string>();
        if (Validation(column) != null)
            checks.Add(Validation(column));
        var values = EnumValues(column);
        if (values != null && IsArray(column))
            checks.Add("[AllowedItems(" + string.Join(", ", values.Select(Cs)) + ")]");
        else if (values != null && IsSet(column))
            checks.Add("[RegularExpression(" + Cs("^(?:(?:" + Alternatives(values) + ")(?:,(?:" + Alternatives(values) + "))*)?$") + ")]");
        else if (values != null)
            checks.Add("[AllowedValues(" + string.Join(", ", values.Select(Cs)) + (column.AllowDBNull || create && Optional(column) ? ", null" : "") + ")]");
        if (BitString(column) != null)
            checks.Add("[RegularExpression(" + Cs(BitPattern(column)) + ")]");
        return checks;
    }

    private static string Alternatives(IEnumerable<string> values) => string.Join("|", values.Select(Regex.Escape));

    /// <summary>A C# string literal: "text", escaped.</summary>
    public static string Cs(string text) => "@\"" + (text ?? "").Replace("\"", "\"\"") + "\"";

    // ---- columns whose JSON isn't their .NET type's: arrays, bit strings, enums ----

    /// <summary>Whether a column is an array (PostgreSQL's text[], int4[]): a list in JSON. Bytes (byte[]) aren't.</summary>
    public static bool IsArray(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        return type.EndsWith("[]") && type != "byte[]";
    }

    /// <summary>An array's element's C# type: string, int.</summary>
    public static string Element(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        return type.Substring(0, type.Length - 2).TrimEnd('?');
    }

    /// <summary>A bit string, which the API sends as text of 0s and 1s: "BitArray" for PostgreSQL's bit(n) and bit
    /// varying (as Npgsql reads them), "ulong" for MySQL's BIT(n) (n over 1: BIT(1) is a bool); null for any other
    /// column.</summary>
    public static string BitString(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        if (type is "System.Collections.BitArray" or "BitArray")
            return "BitArray";
        return type == "ulong" && Native(column) == "bit" ? "ulong" : null;
    }

    /// <summary>A bit string's most bits (bit(8): 8; 0 for a bit varying without a limit).</summary>
    public static int BitLength(ColumnSchema column)
    {
        if (column.Size > 0)
            return column.Size;
        var size = Regex.Match(Convert.ToString(column.ExtendedProperties["CS_ColumnType"]?.Value) ?? column.NativeType ?? "", @"\((\d+)\)");
        return size.Success ? int.Parse(size.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture) : 0;
    }

    /// <summary>Whether a bit string has just so many bits: PostgreSQL's bit(n) (bit varying and MySQL's BIT take fewer).</summary>
    public static bool BitExact(ColumnSchema column) => BitString(column) == "BitArray" && Native(column) == "bit" && BitLength(column) > 0;

    /// <summary>A bit string's pattern: ^[01]{8}$.</summary>
    public static string BitPattern(ColumnSchema column) =>
        BitLength(column) == 0 ? "^[01]*$" : "^[01]{" + (BitExact(column) ? "" : BitString(column) == "ulong" ? "1," : "0,") + BitLength(column) + "}$";

    /// <summary>An enum's values (CS_EnumValues: MySQL's ENUM and SET, PostgreSQL's enums and their arrays), in order;
    /// null for a column without.</summary>
    public static List<string> EnumValues(ColumnSchema column)
    {
        string text = Convert.ToString(column.ExtendedProperties["CS_EnumValues"]?.Value);
        if (string.IsNullOrEmpty(text))
            return null;
        return Regex.Matches(text, "'((?:[^']|'')*)'").Cast<Match>().Select(m => m.Groups[1].Value.Replace("''", "'")).ToList();
    }

    /// <summary>A MySQL SET: its value is some of its values, separated by commas.</summary>
    public static bool IsSet(ColumnSchema column) => Native(column) == "set" && EnumValues(column) != null;

    /// <summary>A record's property's type: the column's, but text for a bit string.</summary>
    public static string ApiType(ColumnSchema column) => BitString(column) != null ? (column.AllowDBNull ? "string?" : "string") : Names.Type(column);

    /// <summary>The record's value of a column, from the entity's (entity.Flags): a bit string's as text.</summary>
    public static string FromEntity(ColumnSchema column, string value, string api)
    {
        string bang = column.AllowDBNull ? "" : "!";
        switch (BitString(column))
        {
            case "BitArray": return api + ".Bits(" + value + ")" + bang;
            case "ulong": return api + ".Bits(" + value + ", " + BitLength(column) + ")" + bang;
            default: return value;
        }
    }

    /// <summary>The entity's value of a column, from a request's (Flags): a bit string's from its text.</summary>
    public static string EntityValue(ColumnSchema column, string value, string api)
    {
        switch (BitString(column))
        {
            case "BitArray": return api + ".BitArray(" + value + ")" + (column.AllowDBNull ? "" : "!");
            case "ulong": return api + ".BitNumber(" + value + ")" + (column.AllowDBNull ? "" : " ?? 0");
            default: return value;
        }
    }

    // ---- defaults ----

    /// <summary>The column's default, as the database gives it (CS_Default: 'en-US'::character varying, now(),
    /// ((0)), CURRENT_TIMESTAMP); null for none.</summary>
    public static string Default(ColumnSchema column)
    {
        string text = Convert.ToString(column.ExtendedProperties["CS_Default"]?.Value, System.Globalization.CultureInfo.InvariantCulture)?.Trim();
        return string.IsNullOrEmpty(text) || text.Equals("NULL", StringComparison.OrdinalIgnoreCase) ? null : text;
    }

    /// <summary>Whether a new row may leave a column out: it has a default (CreatedAt DEFAULT now(), Severity DEFAULT
    /// 'low'), which the database gives it. Its create request's property is optional, set only when it's given.</summary>
    /// A list that can't be null (text[] NOT NULL DEFAULT '{}') is optional only where its default is a list of values: EF
    /// Core never leaves a required list to the database, so a new row is given that list.
    public static bool Optional(ColumnSchema column) =>
        Default(column) != null && !Generated(column) && !(IsArray(column) && !column.AllowDBNull && ArrayDefault(column) == null);

    /// <summary>An array's default as a list in C# and TypeScript: '{}'::text[] is [], '{a,"b c"}' ["a", "b c"], '{1,2}'
    /// [1, 2]; null for one that isn't such a list (an expression, a null item, an array of arrays, of another type).</summary>
    public static string ArrayDefault(ColumnSchema column)
    {
        string text = Default(column);
        if (text == null || !IsArray(column))
            return null;
        while (text.Length > 1 && text[0] == '(' && text[text.Length - 1] == ')')
            text = text.Substring(1, text.Length - 2).Trim();
        text = Regex.Replace(text, @"::[\w\s""-]+(\[\])?$", "").Trim();
        if (text.StartsWith("E'"))
            text = text.Substring(1);
        if (text.Length < 4 || text[0] != '\'' || text[text.Length - 1] != '\'')
            return null;
        string literal = text.Substring(1, text.Length - 2).Replace("''", "'");
        string element = Element(column);
        bool number = element is "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "float" or "double" or "decimal";
        if (literal.Length < 2 || literal[0] != '{' || literal[literal.Length - 1] != '}' || !number && element != "string")
            return null;
        var items = new List<string>();
        var item = new System.Text.StringBuilder();
        bool quoted = false, wasQuoted = false;
        string Item()
        {
            string value = wasQuoted ? item.ToString() : item.ToString().Trim();
            if (!wasQuoted && value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                return null;
            if (!number)
                return Ts(value);
            return decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal parsed)
                ? parsed.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
        }
        for (int i = 1; i < literal.Length - 1; i++)
        {
            char c = literal[i];
            if (quoted && c == '\\' && i + 1 < literal.Length - 1)
                item.Append(literal[++i]);
            else if (c == '"')
            {
                quoted = !quoted;
                wasQuoted = true;
            }
            else if (c == '{' && !quoted)
                return null;
            else if (c == ',' && !quoted)
            {
                items.Add(Item());
                item.Clear();
                wasQuoted = false;
            }
            else
                item.Append(c);
        }
        if (item.Length > 0 || wasQuoted || items.Count > 0)
            items.Add(Item());
        return items.Contains(null) ? null : "[" + string.Join(", ", items) + "]";
    }

    /// <summary>A create request's property's type: nullable where the column is, or where it may be left out.</summary>
    public static string CreateType(ColumnSchema column)
    {
        string type = ApiType(column);
        return Optional(column) && !type.EndsWith("?") ? type + "?" : type;
    }

    /// <summary>The value EF Core leaves a column out of an insert at, where it isn't .NET's default: a value type's
    /// literal default (HasSentinel(true) for DEFAULT true; Names.Default). A new row that leaves the column out is given
    /// it, and the database writes its default.</summary>
    public static string Sentinel(ColumnSchema column)
    {
        string sentinel = Names.Default(column).FirstOrDefault(s => s.StartsWith("HasSentinel(", StringComparison.Ordinal));
        return sentinel?.Substring("HasSentinel(".Length, sentinel.Length - "HasSentinel(".Length - 1);
    }

    /// <summary>"required " for a request's property that must be given: text (or a list, or a bit string) that can't
    /// be null. A number is 0 until it's set.</summary>
    public static string Required(ColumnSchema column) => !column.AllowDBNull && (BitString(column) != null || Names.Required(column) != "") ? "required " : "";

    /// <summary>"required " for a create request's property that must be given: as Required, but one the database
    /// has a default for may be left out.</summary>
    public static string CreateRequired(ColumnSchema column) => Optional(column) ? "" : Required(column);

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

    /// <summary>What escapes the search's text for LIKE (its wildcards, and SQL Server's [), with ! as the escape: a
    /// character that's plain in every dialect's string literals (MySQL's provider wrote \ as ESCAPE '\', which MySQL
    /// reads as an escaped quote: a syntax error).</summary>
    public static string LikeEscapes(TableSchema table) =>
        ".Replace(\"!\", \"!!\").Replace(\"%\", \"!%\").Replace(\"_\", \"!_\")" + (Provider(table.Database) == "SqlSchemaProvider" ? ".Replace(\"[\", \"![\")" : "");

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

    /// <summary>A column's Zod schema: z.string(), z.number(), z.boolean(), z.enum([...]) for an enum, z.array(...)
    /// for an array; nullable where the column is (or, a new row's, where it has a default and may be left out).</summary>
    public static string Zod(ColumnSchema column, bool request, bool create = false)
    {
        var values = EnumValues(column);
        string schema;
        if (BitString(column) != null)
            schema = request ? "z.string().regex(/" + BitPattern(column) + "/)" : "z.string()";
        else if (IsArray(column))
            // an array's text may hold nulls (PostgreSQL's arrays do; a number's can't, as Npgsql reads them)
            schema = "z.array(" + (values != null ? ZodEnum(values) : ZodOf(Element(column))) + (ZodOf(Element(column)) == "z.string()" ? ".nullable()" : "") + ")";
        else if (values != null && IsSet(column))
            schema = request
                ? "z.string().refine((value) => value === \"\" || value.split(\",\").every((item) => " + TsArray(values) + ".includes(item)))"
                : "z.string()";
        else if (values != null)
            schema = ZodEnum(values);
        else
            schema = ZodOf(Names.Type(column).TrimEnd('?'));
        if (request && Validation(column) != null)
            schema += ".max(" + column.Size + ")";
        // a request can leave out what may be null (null on the C# side), and a new row what has a default; a response
        // always has it
        if (request && create && Optional(column))
            return schema + ".nullish()";
        return !column.AllowDBNull ? schema : request ? schema + ".nullish()" : schema + ".nullable()";
    }

    private static string ZodOf(string type) => type switch
    {
        "bool" => "z.boolean()",
        "byte" or "sbyte" or "short" or "ushort" or "int" or "uint" or "long" or "ulong" or "float" or "double" or "decimal" => "z.number()",
        _ => "z.string()", // text, dates and times (ISO 8601), Guids, bytes (base64)
    };

    // z.enum(["a", "b"]); an enum without values (PostgreSQL allows one) takes none
    private static string ZodEnum(List<string> values) => values.Count == 0 ? "z.never()" : "z.enum(" + TsArray(values) + ")";

    private static string TsArray(IEnumerable<string> values) => "[" + string.Join(", ", values.Select(Ts)) + "]";

    /// <summary>A TypeScript string literal: "text", escaped.</summary>
    public static string Ts(string text)
    {
        var builder = new System.Text.StringBuilder("\"");
        foreach (char c in text ?? "")
        {
            switch (c)
            {
                case '"': builder.Append("\\\""); break;
                case '\\': builder.Append("\\\\"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                default:
                    if (c < ' ' || c == (char)0x2028 || c == (char)0x2029)
                        builder.Append("\\u").Append(((int)c).ToString("x4"));
                    else
                        builder.Append(c);
                    break;
            }
        }
        return builder.Append('"').ToString();
    }

    /// <summary>A key or filter parameter's TypeScript type.</summary>
    public static string TypeScript(ColumnSchema column) =>
        Zod(column, false).StartsWith("z.number") ? "number" : Zod(column, false).StartsWith("z.boolean") ? "boolean" : "string";

    /// <summary>The client's types of a table (its entity's name; its schema's is Schema after it): its list's sort,
    /// filter and options, and its requests (CreateProductRequest, with CreateProductRequestSchema), each with Record
    /// after it where a table's entity has its name.</summary>
    public static string TsSort(TableSchema table) => Own(table.Database, Names.Entity(table) + "Sort");

    public static string TsFilter(TableSchema table) => Own(table.Database, Names.Entity(table) + "Filter");

    public static string TsListOptions(TableSchema table) => Own(table.Database, Names.Entity(table) + "ListOptions");

    public static string TsCreate(TableSchema table) => Own(table.Database, "Create" + Names.Entity(table) + "Request");

    public static string TsUpdate(TableSchema table) => Own(table.Database, "Update" + Names.Entity(table) + "Request");

    /// <summary>The client's property for a table's calls: its set in camel case (lineItems).</summary>
    public static string ClientProperty(TableSchema table) => char.ToLowerInvariant(Names.Set(table)[0]) + Names.Set(table).Substring(1);
}
