// What the templates share: the names and types the generated code uses for tables, columns and relationships, as EF
// Core's scaffolding would write them.
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Engine;
using SchemaExplorer;

public static class Names
{
    // a database's default schemas: ToTable leaves them out
    private static readonly string[] DefaultOwners = { "", "dbo", "public", "main" };

    /// <summary>The entity's class: the table's name in Pascal case, singular (Orders: Order); with its owner's in front
    /// when another owner has a table that would have the same name.</summary>
    public static string Entity(TableSchema table)
    {
        string name = Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(table.Name)));
        bool twice = table.Database.Tables.Any(t => t != table && Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(t.Name))) == name);
        return twice ? Identifier(StringUtil.ToPascalCase(table.Owner)) + name : name;
    }

    /// <summary>The DbSet: plural (Order: Orders).</summary>
    public static string Set(TableSchema table) => StringUtil.ToPlural(Entity(table));

    /// <summary>The schema for ToTable, or null for a database's default (dbo, public, SQLite's, MySQL's database).</summary>
    public static string Schema(TableSchema table) =>
        DefaultOwners.Contains(table.Owner ?? "") || table.Owner == table.Database.Name ? null : table.Owner;

    /// <summary>A column's property: its name in Pascal case (order_id: OrderId), not the class's own name, and once.</summary>
    public static string Property(ColumnSchema column)
    {
        var names = new Dictionary<ColumnSchema, string>();
        var taken = new HashSet<string> { Entity(column.Table) };
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

    /// <summary>Whether a key refers to its table's primary key (else HasPrincipalKey names the columns it refers to).</summary>
    public static bool RefersToPrimaryKey(TableKeySchema key) =>
        key.PrimaryKeyTable.HasPrimaryKey &&
        key.PrimaryKeyMemberColumns.Select(c => c.Name).SequenceEqual(key.PrimaryKeyTable.PrimaryKey.MemberColumns.Select(c => c.Name));

    /// <summary>Whether a foreign key is a relationship in the model: EF Core needs a key on both of its tables.</summary>
    public static bool Related(TableKeySchema key) => key.ForeignKeyTable.HasPrimaryKey && key.PrimaryKeyTable.HasPrimaryKey;

    /// <summary>A foreign key's navigation, on its table: the table it refers to, unless a column or another navigation
    /// has that name.</summary>
    public static string Navigation(TableKeySchema key) => Navigations(key.ForeignKeyTable)[(key, false)];

    /// <summary>The navigation back, on the table referred to: the referring table's set (LineItems), unless taken.</summary>
    public static string Inverse(TableKeySchema key) => Navigations(key.PrimaryKeyTable)[(key, true)];

    // A table's navigation names, to the tables its keys refer to and back from the tables that refer to it (a key of
    // the table to itself has both), each once.
    private static Dictionary<(TableKeySchema Key, bool Back), string> Navigations(TableSchema table)
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
        foreach (TableKeySchema key in table.ForeignKeys.Cast<TableKeySchema>().Where(Related))
            taken.Add(names[(key, false)] = Unique(Entity(key.PrimaryKeyTable), key));
        foreach (TableKeySchema key in ReferencesTo(table))
            taken.Add(names[(key, true)] = Unique(Set(key.ForeignKeyTable), key));
        return names;
    }

    /// <summary>The foreign keys of the database's tables that refer to this one (and are relationships): its
    /// collections. Read from every table, as a provider's PrimaryKeys may not have them (SQLite's).</summary>
    public static IEnumerable<TableKeySchema> ReferencesTo(TableSchema table) =>
        table.Database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
            .Where(k => k.PrimaryKeyTable.Owner == table.Owner && k.PrimaryKeyTable.Name == table.Name && Related(k));

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

    /// <summary>A column type EF Core has to be told (PostgreSQL's jsonb, which a string would otherwise be sent as text).</summary>
    public static string ColumnType(ColumnSchema column) =>
        column.NativeType == "json" || column.NativeType == "jsonb" ? column.NativeType : null;

    // A C# identifier: a name that starts with a digit gets an underscore; other characters go.
    private static string Identifier(string name)
    {
        var chars = (name ?? "").Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        string identifier = new string(chars);
        return identifier.Length == 0 ? "_" : char.IsDigit(identifier[0]) ? "_" + identifier : identifier;
    }
}
