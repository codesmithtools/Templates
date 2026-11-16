// What the templates share: the C# names of tables and columns, and their SQL names, quoted for the database.
using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.Engine;
using SchemaExplorer;

public static class Names
{
    // a database's default schemas: the SQL leaves them out
    private static readonly string[] DefaultOwners = { "", "dbo", "public", "main" };

    // C#'s keywords: a parameter with one's name gets an @
    private static readonly HashSet<string> Keywords = new HashSet<string>
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
        "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
    };

    // ---------------------------------------------------------------- C#

    /// <summary>The record's name: the table's in Pascal case, singular (Orders: Order); with its owner's in front when
    /// another owner has a table that would have the same name.</summary>
    public static string Entity(TableSchema table)
    {
        string name = Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(table.Name)));
        bool twice = table.Database.Tables.Any(t => t != table && Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(t.Name))) == name);
        return twice ? Identifier(StringUtil.ToPascalCase(table.Owner)) + name : name;
    }

    /// <summary>The repository's name: OrderRepository.</summary>
    public static string Repository(TableSchema table) => Entity(table) + "Repository";

    /// <summary>A column's property: its name in Pascal case (order_id: OrderId), not the record's own name, and
    /// once.</summary>
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

    /// <summary>A method parameter: the name in camel case (OrderId: orderId, LineItem: lineItem), @class for a
    /// keyword, and not a name the repository's methods use themselves.</summary>
    public static string Parameter(string name)
    {
        string camel = Identifier(StringUtil.ToCamelCase(name));
        if (camel == "connection" || camel == "cancellationToken" || camel == "sql")
            return camel + "Value";
        return Keywords.Contains(camel) ? "@" + camel : camel;
    }

    /// <summary>"required " for a column a new record must set: one that can't be null and that the database doesn't
    /// number itself.</summary>
    public static string Required(ColumnSchema column) => !column.AllowDBNull && !IsIdentity(column) ? "required " : "";

    // ---------------------------------------------------------------- SQL

    /// <summary>The database's provider, by name: for SQL scripts (DdlSchemaProvider), the provider of the scripts'
    /// dialect (CS_Dialect), so the SQL fits the database the scripts make.</summary>
    public static string ProviderName(DatabaseSchema database)
    {
        string name = database.Provider?.Name ?? "";
        if (name != "DdlSchemaProvider")
            return name;
        return System.Convert.ToString(database.ExtendedProperties["CS_Dialect"]?.Value) switch
        {
            "SqlServer" => "SqlSchemaProvider",
            "PostgreSQL" => "PostgreSQLSchemaProvider",
            "MySQL" => "MySQLSchemaProvider",
            "SQLite" => "SQLiteSchemaProvider",
            _ => name,
        };
    }

    /// <summary>An identifier quoted for the database: [x] (SQL Server), "x" (PostgreSQL, SQLite), `x`
    /// (MySQL).</summary>
    public static string Quote(DatabaseSchema database, string name)
    {
        switch (ProviderName(database))
        {
            case "SqlSchemaProvider":
                return "[" + name.Replace("]", "]]") + "]";
            case "MySQLSchemaProvider":
                return "`" + name.Replace("`", "``") + "`";
            case "PostgreSQLSchemaProvider":
            case "SQLiteSchemaProvider":
                return "\"" + name.Replace("\"", "\"\"") + "\"";
            default:
                throw new NotSupportedException(ProviderName(database) +
                    ": the templates write SQL for SQL Server, PostgreSQL, MySQL and SQLite.");
        }
    }

    /// <summary>The table's SQL name, with its schema unless it's the database's default (dbo, public, MySQL's
    /// database).</summary>
    public static string Table(TableSchema table)
    {
        string name = Quote(table.Database, table.Name);
        bool owned = !DefaultOwners.Contains(table.Owner ?? "") && table.Owner != table.Database.Name;
        return owned ? Quote(table.Database, table.Owner) + "." + name : name;
    }

    /// <summary>A column's SQL name.</summary>
    public static string Column(ColumnSchema column) => Quote(column.Table.Database, column.Name);

    /// <summary>A column in a SELECT, aliased to its property where the names differ ("order_id" AS "OrderId"; not
    /// just in case, which Dapper ignores).</summary>
    public static string Selected(ColumnSchema column) =>
        string.Equals(column.Name, Property(column), StringComparison.OrdinalIgnoreCase)
            ? Column(column)
            : Column(column) + " AS " + Quote(column.Table.Database, Property(column));

    /// <summary>"x" = @X: the column, and the parameter Dapper fills from its property.</summary>
    public static string Assignment(ColumnSchema column) => Column(column) + " = @" + Property(column);

    /// <summary>Whether the database numbers the column itself (an identity, serial, auto_increment or SQLite's
    /// INTEGER PRIMARY KEY AUTOINCREMENT): CS_IsIdentity, which a provider may not give.</summary>
    public static bool IsIdentity(ColumnSchema column) => Flag(column, "CS_IsIdentity");

    /// <summary>Whether an INSERT or UPDATE writes the column: not one the database writes (an identity, a computed
    /// column, a SQL Server rowversion).</summary>
    public static bool Written(ColumnSchema column) =>
        !IsIdentity(column) && !Flag(column, "CS_IsComputed") &&
        !((column.NativeType == "timestamp" || column.NativeType == "rowversion") &&
          column.GetClrType(TypeMapping.Modern) == typeof(byte[]));

    // A true/false extended property; false where the provider doesn't give it.
    private static bool Flag(ColumnSchema column, string name) =>
        bool.TryParse(Convert.ToString(column.ExtendedProperties[name]?.Value), out bool value) && value;

    /// <summary>The table's key columns (none without a primary key).</summary>
    public static List<ColumnSchema> Key(TableSchema table) =>
        table.HasPrimaryKey
            ? table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns[c.Name]).ToList()
            : new List<ColumnSchema>();

    /// <summary>The key column the database numbers, which an insert returns: a one-column primary key that's an
    /// identity; else null.</summary>
    public static ColumnSchema GeneratedKey(TableSchema table)
    {
        var key = Key(table);
        return key.Count == 1 && IsIdentity(key[0]) ? key[0] : null;
    }

    /// <summary>What follows an INSERT's VALUES (...) to return the key the database numbered: RETURNING, or a second
    /// statement that reads it.</summary>
    public static string NewKey(ColumnSchema key)
    {
        string indent = new string(' ', 12);
        switch (ProviderName(key.Table.Database))
        {
            case "SqlSchemaProvider":
                return ";" + Environment.NewLine + indent + "SELECT SCOPE_IDENTITY();";
            case "MySQLSchemaProvider":
                return ";" + Environment.NewLine + indent + "SELECT LAST_INSERT_ID();";
            default: // PostgreSQL, SQLite (3.35 and later)
                return Environment.NewLine + indent + "RETURNING " + Column(key);
        }
    }

    /// <summary>A line of SQL: <paramref name="start"/>, the items joined with ", ", and <paramref name="end"/>,
    /// wrapped before an item that would pass column 120. The SQL is indented 12 spaces (a raw string literal in a
    /// method), and a line that wraps goes on at 16.</summary>
    public static string Clause(string start, IEnumerable<string> items, string end = "")
    {
        var text = new System.Text.StringBuilder(start);
        int width = 12 + start.Length;
        bool first = true;
        foreach (string item in items)
        {
            if (!first)
            {
                text.Append(',');
                width++;
                if (width + 1 + item.Length > 120)
                {
                    text.Append(Environment.NewLine).Append(' ', 16);
                    width = 16;
                }
                else
                {
                    text.Append(' ');
                    width++;
                }
            }
            first = false;
            text.Append(item);
            width += item.Length;
        }
        return text.Append(end).ToString();
    }

    // A C# identifier: a name that starts with a digit gets an underscore; other characters go.
    private static string Identifier(string name)
    {
        var chars = (name ?? "").Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        string identifier = new string(chars);
        return identifier.Length == 0 ? "_" : char.IsDigit(identifier[0]) ? "_" + identifier : identifier;
    }
}
