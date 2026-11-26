// What the templates share: the names and types the generated code uses for tables, columns and relationships, as EF
// Core's scaffolding would write them.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CodeSmith.Engine;
using SchemaExplorer;

public static class Names
{
    // a database's default schemas: ToTable leaves them out
    private static readonly string[] DefaultOwners = { "", "dbo", "public", "main" };

    /// <summary>Whether a table is left out (Exclude: tables separated by commas, sysdiagrams, aspnet_*), by its name or
    /// its schema and name (dbo.sysdiagrams); * and ? match any characters, in any case.</summary>
    public static bool Excluded(TableSchema table, string exclude) =>
        Globs(exclude).Any(glob => glob.IsMatch(table.Name) || glob.IsMatch(table.Owner + "." + table.Name));

    // An Exclude's patterns, made once (every relationship asks)
    private static Regex[] Globs(string exclude)
    {
        lock (GlobsOf)
        {
            if (!GlobsOf.TryGetValue(exclude ?? "", out var globs))
                GlobsOf[exclude ?? ""] = globs = (exclude ?? "").Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).Where(p => p.Length > 0)
                    .Select(p => new Regex("^" + Regex.Escape(p).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase))
                    .ToArray();
            return globs;
        }
    }

    private static readonly Dictionary<string, Regex[]> GlobsOf = new Dictionary<string, Regex[]>();

    /// <summary>A database's tables, but those excluded: the ones with an entity.</summary>
    public static List<TableSchema> Tables(DatabaseSchema database, string exclude) =>
        database.Tables.Cast<TableSchema>().Where(t => !Excluded(t, exclude)).ToList();

    /// <summary>The database's dialect: SqlServer, PostgreSQL, MySQL or SQLite (SQL scripts': theirs; a schema snapshot's:
    /// the provider's it was taken with), or another provider's name.</summary>
    public static string Dialect(DatabaseSchema database)
    {
        string provider = database.Provider?.Name ?? "";
        if (provider == "DdlSchemaProvider")
            provider = System.Convert.ToString(database.ExtendedProperties["CS_Dialect"]?.Value) ?? provider;
        if (provider.Contains("PostgreSQL"))
            return "PostgreSQL";
        if (provider.Contains("MySQL"))
            return "MySQL";
        if (provider.Contains("SQLite"))
            return "SQLite";
        if (provider == "SqlSchemaProvider" || provider == "SqlServer")
            return "SqlServer";
        return provider;
    }

    /// <summary>The entity's class: the table's name in Pascal case, singular (Orders: Order); with its owner's in front
    /// when another owner has a table that would have the same name; with Entity after it when it's the name of a type
    /// the generated code uses (Task: TaskEntity, Guid: GuidEntity).</summary>
    public static string Entity(TableSchema table)
    {
        string name = Name(table);
        return Taken(name) ? name + "Entity" : name;
    }

    // The table's name for its class, before Entity() makes it one no type has.
    private static string Name(TableSchema table)
    {
        string name = Bare(table);
        bool twice = BareNames.GetValue(table.Database.Tables, tables => tables.Cast<TableSchema>().GroupBy(Bare).ToDictionary(g => g.Key, g => g.Count()))
            .TryGetValue(name, out int count) && count > 1;
        return twice ? Identifier(StringUtil.ToPascalCase(table.Owner)) + name : name;
    }

    private static string Bare(TableSchema table) => Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(table.Name)));

    // How many of a database's tables have each name, worked out once (Name asked every table each time: a database of
    // 600 tables took minutes); a schema read again is a new collection.
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, Dictionary<string, int>> BareNames =
        new System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, Dictionary<string, int>>();

    /// <summary>The DbSet: plural (Order: Orders; Task: Tasks).</summary>
    public static string Set(TableSchema table) =>
        Taken(StringUtil.ToPlural(Name(table))) ? StringUtil.ToPlural(Entity(table)) : StringUtil.ToPlural(Name(table));

    /// <summary>The entity's configuration class: OrderConfiguration, or OrderEntityConfiguration where a table's entity
    /// has that name (a ProductConfiguration table: Product's is ProductEntityConfiguration).</summary>
    public static string Configuration(TableSchema table)
    {
        var entities = EntityNames.GetValue(table.Database.Tables, tables => new HashSet<string>(tables.Cast<TableSchema>().Select(Entity)));
        string name = Entity(table) + "Configuration";
        if (entities.Contains(name))
            name = Entity(table) + "EntityConfiguration";
        for (int i = 2; entities.Contains(name); i++)
            name = Entity(table) + "Configuration" + i;
        return name;
    }

    // The entities' names, worked out once for a database
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, HashSet<string>> EntityNames =
        new System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, HashSet<string>>();

    // The types an entity's name mustn't be: in its own namespace it would hide one the generated code uses there
    // (Task: async Task), and elsewhere it would be ambiguous with one a file imports (Guid, with ImplicitUsings).
    // Those of the namespaces every generated project imports (System, System.IO, System.Linq, System.Threading.Tasks,
    // ...), found by name, and the frameworks' the generated code uses (EF Core, ASP.NET Core, CSLA) or imports.
    private static readonly string[] Imports = { "System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Net.Http", "System.Threading", "System.Threading.Tasks" };
    private static readonly string[] Assemblies = { "System.Private.CoreLib", "System.Runtime", "System.Collections", "System.Console", "System.Linq", "System.Net.Http", "System.Threading" };
    private static readonly HashSet<string> Frameworks = new HashSet<string>
    {
        "DbContext", "DbContextOptions", "DbContextOptionsBuilder", "DbSet", "DbUpdateException", "DbUpdateConcurrencyException", "EF",
        "EntityState", "EntityTypeBuilder", "ModelBuilder",
        "Accepted", "Conflict", "Created", "Endpoint", "Host", "HttpContext", "Logger", "NoContent", "NotFound", "Ok", "Results", "Route",
        "RouteData", "RouteGroupBuilder", "StatusCodes", "TypedResults", "WebApplication",
        "ComponentBase", "Dispatcher", "EventCallback", "LayoutComponentBase", "MarkupString", "NavigationManager",
        "ApplicationContext", "BusinessBase", "BusinessListBase", "BusinessRule", "BusinessRules", "CommandBase", "DataPortal", "PropertyInfo",
        "ReadOnlyBase", "ReadOnlyListBase", "Rule", "Utilities", "ViewModel",
        "Validator", "ValidationException",
    };
    private static readonly Dictionary<string, bool> Seen = new Dictionary<string, bool>();

    private static bool Taken(string name)
    {
        lock (Seen)
        {
            if (!Seen.TryGetValue(name, out bool taken))
                Seen[name] = taken = Frameworks.Contains(name) || Imports.Any(n => new[] { "", "`1", "`2", "`3" }.Any(arity =>
                    Assemblies.Any(assembly => System.Type.GetType(n + "." + name + arity + ", " + assembly, false) is { IsPublic: true })));
            return taken;
        }
    }

    /// <summary>The schema for ToTable, or null for a database's default (dbo, public, SQLite's, MySQL's database).</summary>
    public static string Schema(TableSchema table) =>
        DefaultOwners.Contains(table.Owner ?? "") || table.Owner == table.Database.Name ? null : table.Owner;

    /// <summary>The columns the entity has a property for: those with a .NET type. A column without one (SQL Server's
    /// geography, hierarchyid or sql_variant, PostgreSQL's point) would be an object, which EF Core can't map: the whole
    /// model would fail, so the entity leaves it out (EF Core's scaffolding does too).</summary>
    public static List<ColumnSchema> Columns(TableSchema table) => table.Columns.Cast<ColumnSchema>().Where(Mapped).ToList();

    /// <summary>Whether a column has a .NET type, so a property (see Columns).</summary>
    public static bool Mapped(ColumnSchema column) => column.GetClrType(TypeMapping.Modern) != typeof(object);

    // What a class has already, or a record (DTOs): a column of one's name (Equals) gets Value after it, as one of the
    // class's own name does (a record's second Equals is CS0102; a class's hides object's)
    private static readonly string[] Inherited =
        { "Equals", "GetHashCode", "GetType", "ToString", "Finalize", "MemberwiseClone", "EqualityContract", "PrintMembers", "Deconstruct" };

    /// <summary>A column's property: its name in Pascal case (order_id: OrderId), not the class's own name nor one it
    /// has already (Equals: EqualsValue), and once.</summary>
    public static string Property(ColumnSchema column)
    {
        var names = new Dictionary<ColumnSchema, string>();
        var taken = new HashSet<string>(Inherited) { Entity(column.Table) };
        foreach (ColumnSchema c in column.Table.Columns)
        {
            string name = Identifier(StringUtil.ToPascalCase(c.Name));
            if (taken.Contains(name))
                name += "Value";
            for (int i = 2; taken.Contains(name); i++)
                name = Identifier(StringUtil.ToPascalCase(c.Name)) + i;
            taken.Add(name);
            names[c] = name;
        }
        return names[column];
    }

    /// <summary>
    /// The property's C# type: the column's (GetCSharpTypeName), except that a foreign key's column has the type of the
    /// key it refers to (an SQLite integer key is a long; the column referring to it may say int).
    /// </summary>
    public static string Type(ColumnSchema column)
    {
        foreach (TableKeySchema key in column.Table.ForeignKeys)
        {
            int index = key.ForeignKeyMemberColumns.IndexOf(column);
            if (index >= 0 && index < key.PrimaryKeyMemberColumns.Count)
            {
                string referenced = key.PrimaryKeyMemberColumns[index].Column.GetCSharpTypeName().TrimEnd('?');
                return column.AllowDBNull ? referenced + "?" : referenced;
            }
        }
        return column.GetCSharpTypeName();
    }

    /// <summary>"required " for a column a new row must set: one that can't be null and has no default of .NET's
    /// (a string; a number is 0 until set).</summary>
    public static string Required(ColumnSchema column) =>
        !column.AllowDBNull && !Type(column).EndsWith("?") && !column.GetClrType(TypeMapping.Modern).IsValueType ? "required " : "";

    /// <summary>A table's primary key columns, in the table's column order: a provider may give them in another (the
    /// PostgreSQL providers give them by name), and a composite foreign key's columns have to match the key's order.</summary>
    public static IEnumerable<MemberColumnSchema> KeyColumns(TableSchema table) =>
        table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().OrderBy(c => table.Columns.IndexOf(table.Columns[c.Name])).ToList();

    /// <summary>Whether a key refers to its table's primary key (else HasPrincipalKey names the columns it refers to).</summary>
    public static bool RefersToPrimaryKey(TableKeySchema key) =>
        key.PrimaryKeyTable.HasPrimaryKey &&
        key.PrimaryKeyMemberColumns.Select(c => c.Name).SequenceEqual(KeyColumns(key.PrimaryKeyTable).Select(c => c.Name));

    /// <summary>Whether a foreign key is a relationship in the model: EF Core needs a key on both of its tables.</summary>
    public static bool Related(TableKeySchema key) => key.ForeignKeyTable.HasPrimaryKey && key.PrimaryKeyTable.HasPrimaryKey;

    /// <summary>Whether a foreign key is a relationship in the model, with tables excluded: neither of its tables is.</summary>
    public static bool Related(TableKeySchema key, string exclude) =>
        Related(key) && !Excluded(key.ForeignKeyTable, exclude) && !Excluded(key.PrimaryKeyTable, exclude);

    /// <summary>A foreign key's navigation, on its table: the table it refers to, unless a column or another navigation
    /// has that name. A key of a table to itself is named for its column, as EF Core's scaffolding names it: ParentId's
    /// is Parent, and ReferredBy's ReferredByNavigation (the column has that name).</summary>
    public static string Navigation(TableKeySchema key) => Navigation(key, "");

    /// <summary>A foreign key's navigation, with tables excluded (which have none).</summary>
    public static string Navigation(TableKeySchema key, string exclude) => Navigations(key.ForeignKeyTable, exclude)[(key, false)];

    /// <summary>The navigation back, on the table referred to: the referring table's set (LineItems), unless taken; for a
    /// key of a table to itself, Inverse and its navigation (InverseParent), as EF Core's scaffolding names it.</summary>
    public static string Inverse(TableKeySchema key) => Inverse(key, "");

    /// <summary>The navigation back, with tables excluded (which have none).</summary>
    public static string Inverse(TableKeySchema key, string exclude) => Navigations(key.PrimaryKeyTable, exclude)[(key, true)];

    // A table's navigation names, to the tables its keys refer to and back from the tables that refer to it (a key of
    // the table to itself has both), each once.
    private static Dictionary<(TableKeySchema Key, bool Back), string> Navigations(TableSchema table, string exclude)
    {
        var taken = new HashSet<string>(table.Columns.Cast<ColumnSchema>().Select(Property)) { Entity(table) };
        var names = new Dictionary<(TableKeySchema, bool), string>();
        string Unique(string name, TableKeySchema key)
        {
            if (!taken.Contains(name))
                return name;
            if (!taken.Contains(name + "Navigation"))
                return name + "Navigation"; // as EF Core's scaffolding names it
            string by = name + "By" + string.Concat(key.ForeignKeyMemberColumns.Select(c => Identifier(StringUtil.ToPascalCase(c.Name))));
            for (int i = 2; taken.Contains(by); i++)
                by = name + i;
            return by;
        }
        foreach (TableKeySchema key in table.ForeignKeys.Cast<TableKeySchema>().Where(k => Related(k, exclude)))
            taken.Add(names[(key, false)] = Unique(SelfReference(key) ? ColumnsName(key) ?? Entity(key.PrimaryKeyTable) : Entity(key.PrimaryKeyTable), key));
        foreach (TableKeySchema key in ReferencesTo(table, exclude))
            taken.Add(names[(key, true)] = Unique(SelfReference(key) ? "Inverse" + names[(key, false)] : Set(key.ForeignKeyTable), key));
        return names;
    }

    private static bool SelfReference(TableKeySchema key) =>
        key.ForeignKeyTable.Owner == key.PrimaryKeyTable.Owner && key.ForeignKeyTable.Name == key.PrimaryKeyTable.Name;

    // A key's navigation by its columns, as EF Core's scaffolding has it: their properties' common start, without an Id
    // (or Guid) at its end (ParentId: Parent; ReferredBy: ReferredBy); null where they have none.
    private static string ColumnsName(TableKeySchema key)
    {
        var properties = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => Property(key.ForeignKeyTable.Columns[c.Name])).ToList();
        string name = properties[0];
        foreach (string property in properties.Skip(1))
        {
            int i = 0;
            while (i < name.Length && i < property.Length && name[i] == property[i])
                i++;
            name = name.Substring(0, i);
        }
        if (name.Length >= 3 && name.EndsWith("id", StringComparison.OrdinalIgnoreCase))
        {
            int strip = name.Length > 4 && name.EndsWith("guid", StringComparison.OrdinalIgnoreCase) ? 4 : 2;
            int end = name.Length - strip - 1;
            while (end >= 0 && !char.IsLetterOrDigit(name[end]))
                end--;
            if (end != 0)
                name = name.Substring(0, end + 1);
        }
        return name.Length == 0 ? null : name;
    }

    /// <summary>The foreign keys of the database's tables that refer to this one (and are relationships): its
    /// collections. Read from every table, as a provider's PrimaryKeys may not have them (SQLite's).</summary>
    public static IEnumerable<TableKeySchema> ReferencesTo(TableSchema table) => ReferencesTo(table, "");

    /// <summary>The foreign keys that refer to a table, with tables excluded (whose keys aren't relationships).</summary>
    public static IEnumerable<TableKeySchema> ReferencesTo(TableSchema table, string exclude) =>
        Referring.GetValue(table.Database.Tables, tables => tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
                .ToLookup(k => (k.PrimaryKeyTable.Owner, k.PrimaryKeyTable.Name)))[(table.Owner, table.Name)]
            .Where(k => Related(k, exclude));

    // A database's foreign keys by the table they refer to, gathered once (from every table's, each time, took minutes)
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, ILookup<(string, string), TableKeySchema>> Referring =
        new System.Runtime.CompilerServices.ConditionalWeakTable<TableSchemaCollection, ILookup<(string, string), TableKeySchema>>();

    /// <summary>e.Id, or new { e.A, e.B } for several columns of the table.</summary>
    public static string Members(string parameter, TableSchema table, IEnumerable<MemberColumnSchema> columns)
    {
        var names = columns.Select(c => parameter + "." + Property(table.Columns[c.Name])).ToList();
        return names.Count == 1 ? names[0] : "new { " + string.Join(", ", names) + " }";
    }

    /// <summary>The key a table's rows are identified by (none: the entity has no key).</summary>
    public static bool HasKey(TableSchema table) => table.HasPrimaryKey;

    /// <summary>A single integer key the database doesn't number itself: EF Core would otherwise leave it to it.</summary>
    public static bool NumberedByTheApplication(ColumnSchema column) =>
        column.Table.HasPrimaryKey && column.Table.PrimaryKey.MemberColumns.Count == 1 && column.IsPrimaryKeyMember &&
        column.GetClrType(TypeMapping.Modern) is var type && (type == typeof(int) || type == typeof(long) || type == typeof(short)) &&
        bool.TryParse(System.Convert.ToString(column.ExtendedProperties["CS_IsIdentity"]?.Value), out bool identity) && !identity;

    /// <summary>A SQL Server rowversion: EF Core checks it for concurrency.</summary>
    public static bool IsRowVersion(ColumnSchema column) =>
        (column.NativeType == "timestamp" || column.NativeType == "rowversion") && column.GetClrType(TypeMapping.Modern) == typeof(byte[]);

    /// <summary>A computed column: the database works it out, and EF Core never writes it.</summary>
    public static bool IsComputed(ColumnSchema column) =>
        bool.TryParse(System.Convert.ToString(column.ExtendedProperties["CS_IsComputed"]?.Value), out bool computed) && computed;

    /// <summary>A column type EF Core has to be told, where its provider's own for the property's type isn't the column's:
    /// json and jsonb (a string would be sent as text); and PostgreSQL's timestamp (Npgsql's DateTime is timestamptz),
    /// bit(n) (a bool is boolean, a BitArray bit varying) and an enum or another type of its own (a string is text).</summary>
    public static string ColumnType(ColumnSchema column)
    {
        if (column.NativeType == "json" || column.NativeType == "jsonb")
            return column.NativeType;
        if (Dialect(column.Table.Database) != "PostgreSQL")
            return null;
        var type = column.GetClrType(TypeMapping.Modern);
        if ((column.NativeType == "timestamp" || column.NativeType == "timestamp without time zone") && type == typeof(DateTime))
            return "timestamp without time zone";
        if (column.NativeType == "bit")
            return "bit(" + Math.Max(1, column.Size) + ")";
        if (StoreTypeString(column))
            return column.NativeType;
        return null;
    }

    /// <summary>A string column of a PostgreSQL type of the database's own (an enum, citext): Npgsql sends a string as
    /// text, which the column won't take, so its parameters have its type (Context.cst's StoreTypeStringMapping).</summary>
    public static bool StoreTypeString(ColumnSchema column) =>
        Dialect(column.Table.Database) == "PostgreSQL" && column.GetClrType(TypeMapping.Modern) == typeof(string) &&
        System.Convert.ToString(column.ExtendedProperties["CS_SystemType"]?.Value) == "USER-DEFINED";

    /// <summary>The converter a column's property needs, where its provider can't read or write its type as it is: its
    /// field in the configuration (Converters has them), or null. PostgreSQL's timestamptz takes UTC (Utc), a bit(1) is a
    /// bool (Bit); MySql.EntityFrameworkCore reads a DATE as a DateTime (Date) and a TIME as a TimeSpan (Time).</summary>
    public static string Converter(ColumnSchema column)
    {
        string dialect = Dialect(column.Table.Database);
        var type = column.GetClrType(TypeMapping.Modern);
        if (dialect == "PostgreSQL" && type == typeof(DateTime) && (column.NativeType == "timestamptz" || column.NativeType == "timestamp with time zone"))
            return "Utc";
        if (dialect == "PostgreSQL" && type == typeof(bool) && column.NativeType == "bit")
            return "Bit";
        if (dialect == "MySQL" && type == typeof(DateOnly))
            return "Date";
        if (dialect == "MySQL" && type == typeof(TimeOnly))
            return "Time";
        return null;
    }

    /// <summary>The converters' fields, for a configuration's columns: each one's declaration, with what it's for.</summary>
    public static List<string> Converters(IEnumerable<ColumnSchema> columns)
    {
        var used = new HashSet<string>(columns.Select(Converter).Where(c => c != null));
        var fields = new List<string>();
        if (used.Contains("Utc"))
            fields.Add("// timestamptz takes UTC: a local time is made UTC, and one of no kind is taken as UTC; it reads as UTC\n" +
                "    private static readonly ValueConverter<System.DateTime, System.DateTime> Utc = new(\n" +
                "        v => v.Kind == System.DateTimeKind.Local ? v.ToUniversalTime() : System.DateTime.SpecifyKind(v, System.DateTimeKind.Utc),\n" +
                "        v => System.DateTime.SpecifyKind(v, System.DateTimeKind.Utc));");
        if (used.Contains("Bit"))
            fields.Add("// bit(1): Npgsql's is a BitArray\n" +
                "    private static readonly ValueConverter<bool, System.Collections.BitArray> Bit = new(v => new System.Collections.BitArray(new[] { v }), v => v[0]);");
        if (used.Contains("Date"))
            fields.Add("// a DATE: MySql.EntityFrameworkCore reads a DateTime\n" +
                "    private static readonly ValueConverter<System.DateOnly, System.DateTime> Date = new(v => v.ToDateTime(System.TimeOnly.MinValue), v => System.DateOnly.FromDateTime(v));");
        if (used.Contains("Time"))
            fields.Add("// a TIME: MySql.EntityFrameworkCore reads a TimeSpan\n" +
                "    private static readonly ValueConverter<System.TimeOnly, System.TimeSpan> Time = new(v => v.ToTimeSpan(), v => System.TimeOnly.FromTimeSpan(v));");
        return fields;
    }

    /// <summary>
    /// A column's default, as EF Core's scaffolding writes it: HasDefaultValue(5) for a literal of the property's type
    /// (not .NET's own default: 0, false), HasDefaultValueSql("now()") for anything else, and nothing for an identity, a
    /// computed column or a rowversion. EF Core then leaves a property at its sentinel (.NET's default) out of an insert,
    /// and the database's default is written. A value type's literal default is its sentinel too (HasSentinel), so that
    /// .NET's default can still be written (false, where the default is true).
    /// </summary>
    public static List<string> Default(ColumnSchema column)
    {
        var settings = new List<string>();
        string text = (System.Convert.ToString(column.ExtendedProperties["CS_Default"]?.Value, CultureInfo.InvariantCulture) ?? "").Trim();
        string dialect = Dialect(column.Table.Database);
        // MySQL's default is its value, unquoted ('' too: CS_ColumnDefaultIsNull tells it from none), unless
        // DEFAULT_GENERATED says it's an expression (CURRENT_TIMESTAMP, (curdate()))
        bool value = dialect == "MySQL" &&
            !(System.Convert.ToString(column.ExtendedProperties["CS_ColumnExtra"]?.Value) ?? "").ToUpperInvariant().Contains("DEFAULT_GENERATED") &&
            !Regex.IsMatch(text, @"^current_timestamp(\(\d*\))?$", RegexOptions.IgnoreCase);
        bool none = text.Length == 0 && !(value && Flag(column, "CS_ColumnDefaultIsNull") == false);
        if (none || IsRowVersion(column) || IsComputed(column) || Flag(column, "CS_IsIdentity") == true)
            return settings;
        var type = column.GetClrType(TypeMapping.Modern);
        var (literal, clrDefault) = DefaultLiteral(value ? text : Unwrap(text), type, value);
        if (literal == null)
            settings.Add("HasDefaultValueSql(" + CSharpString(value ? "'" + text.Replace("'", "''") + "'" : text) + ")");
        else if (!clrDefault) // .NET's default (0, false): the scaffolding writes none
        {
            settings.Add("HasDefaultValue(" + literal + ")");
            if (type.IsValueType && !Type(column).EndsWith("?"))
                settings.Add("HasSentinel(" + literal + ")");
        }
        return settings;
    }

    private static bool? Flag(ColumnSchema column, string name) =>
        bool.TryParse(System.Convert.ToString(column.ExtendedProperties[name]?.Value), out bool flag) ? flag : null;

    // A default without the parentheses SQL Server keeps around it: ((0)), (N'x'), (getdate())
    private static string Unwrap(string text)
    {
        while (text.Length > 1 && text[0] == '(' && text[text.Length - 1] == ')' && Balanced(text.Substring(1, text.Length - 2)))
            text = text.Substring(1, text.Length - 2).Trim();
        return text;
    }

    private static bool Balanced(string text)
    {
        int depth = 0;
        foreach (char c in text)
            if (c == '(')
                depth++;
            else if (c == ')' && --depth < 0)
                return false;
        return depth == 0;
    }

    // A default as a C# literal of the property's type, and whether it's .NET's default for it: a number, true or false,
    // a string ('x', N'x'; MySQL's unquoted). Null for anything else: an expression, a cast ('x'::character varying), a date.
    private static (string Literal, bool ClrDefault) DefaultLiteral(string text, Type type, bool value)
    {
        var invariant = CultureInfo.InvariantCulture;
        if (type == typeof(string))
        {
            if (value)
                return (CSharpString(text), false);
            var quoted = Regex.Match(text, @"^N?'((?:[^']|'')*)'$");
            return (quoted.Success ? CSharpString(quoted.Groups[1].Value.Replace("''", "'")) : null, false);
        }
        if (type == typeof(bool))
        {
            var bit = Regex.Match(text, @"^(?:[Bb]?'([01])'(?:::""?bit""?)?|([01]))$");
            string truth = bit.Success ? (bit.Groups[1].Value + bit.Groups[2].Value == "1" ? "true" : "false")
                : text.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : text.Equals("false", StringComparison.OrdinalIgnoreCase) ? "false" : null;
            return (truth, truth == "false");
        }
        // MySQL's bit(n) as a number: b'101'
        var bits = Regex.Match(text, @"^[Bb]'([01]+)'$");
        if (bits.Success)
            text = System.Convert.ToUInt64(bits.Groups[1].Value, 2).ToString(invariant);
        if (!Regex.IsMatch(text, @"^-?\d+(\.\d+)?$") || !decimal.TryParse(text, NumberStyles.Number, invariant, out decimal number))
            return (null, false);
        bool whole = number == decimal.Truncate(number) && !text.Contains(".");
        string literal =
            type == typeof(int) && whole && int.TryParse(text, NumberStyles.AllowLeadingSign, invariant, out _) ? text :
            type == typeof(long) && whole && long.TryParse(text, NumberStyles.AllowLeadingSign, invariant, out _) ? text + "L" :
            type == typeof(short) && whole && short.TryParse(text, NumberStyles.AllowLeadingSign, invariant, out _) ? "(short)" + text :
            type == typeof(byte) && whole && byte.TryParse(text, NumberStyles.None, invariant, out _) ? "(byte)" + text :
            type == typeof(sbyte) && whole && sbyte.TryParse(text, NumberStyles.AllowLeadingSign, invariant, out _) ? "(sbyte)" + text :
            type == typeof(ushort) && whole && ushort.TryParse(text, NumberStyles.None, invariant, out _) ? "(ushort)" + text :
            type == typeof(uint) && whole && uint.TryParse(text, NumberStyles.None, invariant, out _) ? text + "u" :
            type == typeof(ulong) && whole && ulong.TryParse(text, NumberStyles.None, invariant, out _) ? text + "ul" :
            type == typeof(decimal) ? text + "m" :
            type == typeof(double) ? text + "d" :
            type == typeof(float) ? text + "f" : null;
        return (literal, literal != null && number == 0);
    }

    // A C# string literal (a database's names and defaults can have quotes, backslashes and line breaks)
    private static string CSharpString(string text) =>
        "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") + "\"";

    // A C# identifier: a name that starts with a digit gets an underscore; other characters go.
    private static string Identifier(string name)
    {
        var chars = (name ?? "").Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        string identifier = new string(chars);
        return identifier.Length == 0 ? "_" : char.IsDigit(identifier[0]) ? "_" + identifier : identifier;
    }
}
