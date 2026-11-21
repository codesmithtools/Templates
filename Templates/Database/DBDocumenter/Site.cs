// The documentation's model, which every page shares: the database's tables, views and procedures in order, each
// one's page, the foreign keys both ways, the objects a view's or procedure's SQL uses, and the search index.
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using SchemaExplorer;

public sealed class DocSite
{
    // a database's default schemas: names in them go without it
    private static readonly string[] DefaultOwners = { "", "dbo", "public", "main" };

    // types whose size is part of the type (varchar(80)), and those with a precision and scale (decimal(10,2))
    private static readonly string[] SizedTypes =
        { "char", "varchar", "nchar", "nvarchar", "binary", "varbinary", "character", "character varying", "bpchar", "varchar2", "nvarchar2", "raw" };
    private static readonly string[] DecimalTypes = { "decimal", "numeric", "number", "dec" };

    private static readonly ConditionalWeakTable<DatabaseSchema, DocSite> Sites = new ConditionalWeakTable<DatabaseSchema, DocSite>();

    private readonly Dictionary<string, string> _files = new Dictionary<string, string>(StringComparer.Ordinal);
    private readonly Dictionary<string, List<SchemaObjectWithOwnerBase>> _uses = new Dictionary<string, List<SchemaObjectWithOwnerBase>>(StringComparer.Ordinal);
    private readonly Dictionary<string, List<SchemaObjectWithOwnerBase>> _usedBy = new Dictionary<string, List<SchemaObjectWithOwnerBase>>(StringComparer.Ordinal);

    /// <summary>The site of a database: made once, then shared by every page that documents it.</summary>
    public static DocSite Of(DatabaseSchema database) => Sites.GetValue(database, d => new DocSite(d));

    private DocSite(DatabaseSchema database)
    {
        Database = database;
        Tables = database.Tables.Cast<TableSchema>().OrderBy(Title, StringComparer.OrdinalIgnoreCase).ToList();
        Views = database.Views.Cast<ViewSchema>().OrderBy(Title, StringComparer.OrdinalIgnoreCase).ToList();
        Commands = database.Commands.Cast<CommandSchema>().OrderBy(Title, StringComparer.OrdinalIgnoreCase).ToList();
        AddPages("tables", Tables);
        AddPages("views", Views);
        AddPages("procedures", Commands);
        Relationships = Tables.SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>()).Where(k => Find(k.PrimaryKeyTable) != null).ToList();

        // what each view's and procedure's SQL uses, and so what uses each table and view
        foreach (var item in Views.Cast<SchemaObjectWithOwnerBase>().Concat(Commands))
        {
            var uses = SqlReferences.Find(Sql(item), this).Where(o => Key(o) != Key(item)).ToList();
            _uses[Key(item)] = uses;
            foreach (var used in uses)
            {
                if (!_usedBy.TryGetValue(Key(used), out var list))
                    _usedBy[Key(used)] = list = new List<SchemaObjectWithOwnerBase>();
                list.Add(item);
            }
        }
    }

    public DatabaseSchema Database { get; }
    public IReadOnlyList<TableSchema> Tables { get; }
    public IReadOnlyList<ViewSchema> Views { get; }
    public IReadOnlyList<CommandSchema> Commands { get; }

    /// <summary>The foreign keys between the database's tables.</summary>
    public IReadOnlyList<TableKeySchema> Relationships { get; }

    public int ColumnCount => Tables.Sum(t => t.Columns.Count);
    public int IndexCount => Tables.Sum(t => t.Indexes.Count);

    /// <summary>The provider's product: SQL Server, PostgreSQL, MySQL, SQLite, Oracle.</summary>
    public string Product
    {
        get
        {
            string name = Database.Provider?.Name ?? "";
            if (name == "DdlSchemaProvider") // SQL scripts: their dialect (SqlServer, PostgreSQL, MySQL, SQLite)
                name = Convert.ToString(Database.ExtendedProperties["CS_Dialect"]?.Value) ?? name;
            if (name.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0) return "SQLite";
            if (name.IndexOf("PostgreSQL", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Npgsql", StringComparison.OrdinalIgnoreCase) >= 0) return "PostgreSQL";
            if (name.IndexOf("MySQL", StringComparison.OrdinalIgnoreCase) >= 0) return "MySQL";
            if (name.IndexOf("Oracle", StringComparison.OrdinalIgnoreCase) >= 0) return "Oracle";
            if (name.IndexOf("Sql", StringComparison.OrdinalIgnoreCase) >= 0) return "SQL Server";
            return name.Length > 0 ? name : "Database";
        }
    }

    public bool IsSqlServer => Database.Provider?.Name == "SqlSchemaProvider";

    // Pages

    /// <summary>An object's key: its kind and full name (another TableSchema of the same table has the same key), and a
    /// procedure's parameter types (PostgreSQL's overloads share a name).</summary>
    public static string Key(SchemaObjectBase item) =>
        (item is TableSchema ? "table:" : item is ViewSchema ? "view:" : item is CommandSchema ? "procedure:" : "object:") + item.FullName +
        (item is CommandSchema command ? "(" + Signature(command) + ")" : "");

    /// <summary>The name a page shows: the object's, with its schema in front unless that's the default (dbo, public).</summary>
    public static string Title(SchemaObjectWithOwnerBase item) =>
        IsDefaultOwner(item) ? Name(item) : item.Owner + "." + Name(item);

    /// <summary>The object's name, and a procedure's parameter types where another has its name (PostgreSQL's
    /// overloads: f(integer), f(text)).</summary>
    public static string Name(SchemaObjectWithOwnerBase item) =>
        item.Name + (item is CommandSchema command && Overloaded(command) ? "(" + Signature(command) + ")" : "");

    // A procedure's parameter types, the return value's aside: integer, text.
    private static readonly ConditionalWeakTable<CommandSchema, string> Signatures = new ConditionalWeakTable<CommandSchema, string>();

    private static string Signature(CommandSchema command) => Signatures.GetValue(command, c =>
        string.Join(", ", c.Parameters.Cast<ParameterSchema>().Where(p => p.Direction != ParameterDirection.ReturnValue).Select(Type)));

    // Whether another procedure of the database has this one's schema and name (an overload).
    private static readonly ConditionalWeakTable<DatabaseSchema, HashSet<string>> Overloads = new ConditionalWeakTable<DatabaseSchema, HashSet<string>>();

    private static bool Overloaded(CommandSchema command) => Overloads.GetValue(command.Database, database =>
        new HashSet<string>(database.Commands.Cast<CommandSchema>().GroupBy(c => c.FullName).Where(g => g.Count() > 1).Select(g => g.Key)))
        .Contains(command.FullName);

    /// <summary>The schema to show beside a name: "" for the default one.</summary>
    public static string Schema(SchemaObjectWithOwnerBase item) => IsDefaultOwner(item) ? "" : item.Owner;

    private static bool IsDefaultOwner(SchemaObjectWithOwnerBase item) =>
        DefaultOwners.Contains(item.Owner ?? "") || string.Equals(item.Owner, item.Database?.Name, StringComparison.OrdinalIgnoreCase);

    /// <summary>The object's page file, from the output folder: tables/Account.html. Null for one that isn't documented.</summary>
    public string File(SchemaObjectBase item) => _files.TryGetValue(Key(item), out string file) ? file : null;

    /// <summary>A link to the object's page, from the output folder (File, as a URL), or null.</summary>
    public string Page(SchemaObjectBase item)
    {
        string file = File(item);
        return file == null ? null : string.Join("/", file.Split('/').Select(Uri.EscapeDataString));
    }

    /// <summary>The documented object a schema object is (a foreign key's table), or null.</summary>
    public TableSchema Find(TableSchema table) => Tables.FirstOrDefault(t => Key(t) == Key(table));

    private void AddPages(string folder, IEnumerable<SchemaObjectWithOwnerBase> items)
    {
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in items)
        {
            string name = Regex.Replace(Title(item), @"[\x00-\x1f<>:""/\\|?*#%\s]", "_").TrimEnd('.');
            if (name.Length == 0)
                name = "_";
            string file = name;
            for (int i = 2; !taken.Add(file); i++)
                file = name + "-" + i;
            _files[Key(item)] = folder + "/" + file + ".html";
        }
    }

    /// <summary>A column's anchor on its table's page: col-OrderId.</summary>
    public static string Anchor(string column) => "col-" + Regex.Replace(column ?? "", @"[^A-Za-z0-9_\-]", "_");

    // Relationships

    /// <summary>The foreign keys of the database's tables that refer to this one (from every table: the same on each provider).</summary>
    public IEnumerable<TableKeySchema> ReferencedBy(TableSchema table) =>
        Relationships.Where(k => Key(k.PrimaryKeyTable) == Key(table));

    /// <summary>The foreign key a column is the first of (its table's), if any.</summary>
    public static TableKeySchema ForeignKeyOf(ColumnSchema column) =>
        column.Table.ForeignKeys.Cast<TableKeySchema>().FirstOrDefault(k => k.ForeignKeyMemberColumns.Any(c => c.Name == column.Name));

    /// <summary>The column a foreign key's column refers to.</summary>
    public static string ReferencedColumn(TableKeySchema key, string column)
    {
        var columns = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().ToList();
        var referenced = key.PrimaryKeyMemberColumns.Cast<MemberColumnSchema>().ToList();
        int i = columns.FindIndex(c => c.Name == column);
        return i >= 0 && i < referenced.Count ? referenced[i].Name : referenced.Select(c => c.Name).FirstOrDefault() ?? "";
    }

    /// <summary>A foreign key that is also its table's primary key: one row each (TaskExtended's TaskId).</summary>
    public static bool IsOneToOne(TableKeySchema key)
    {
        var table = key.ForeignKeyTable;
        if (!table.HasPrimaryKey)
            return false;
        var primary = table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => c.Name).OrderBy(n => n, StringComparer.Ordinal);
        var foreign = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => c.Name).OrderBy(n => n, StringComparer.Ordinal);
        return primary.SequenceEqual(foreign);
    }

    /// <summary>The tables and views a view's or procedure's SQL uses.</summary>
    public IReadOnlyList<SchemaObjectWithOwnerBase> Uses(SchemaObjectWithOwnerBase item) =>
        _uses.TryGetValue(Key(item), out var list) ? list : new List<SchemaObjectWithOwnerBase>();

    /// <summary>The views and procedures whose SQL uses a table or view.</summary>
    public IReadOnlyList<SchemaObjectWithOwnerBase> UsedBy(SchemaObjectWithOwnerBase item) =>
        _usedBy.TryGetValue(Key(item), out var list) ? list : new List<SchemaObjectWithOwnerBase>();

    /// <summary>The tables and views a name in SQL may be: owner.name, or name in any schema.</summary>
    internal SchemaObjectWithOwnerBase Resolve(string owner, string name)
    {
        IEnumerable<SchemaObjectWithOwnerBase> all = Tables.Cast<SchemaObjectWithOwnerBase>().Concat(Views);
        var named = all.Where(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase)).ToList();
        if (owner != null)
            return named.FirstOrDefault(o => string.Equals(o.Owner, owner, StringComparison.OrdinalIgnoreCase)) ?? (named.Count == 1 ? named[0] : null);
        return named.FirstOrDefault(IsDefaultOwner) ?? named.FirstOrDefault();
    }

    // What the schema says

    /// <summary>A view's or procedure's SQL, or "".</summary>
    public static string Sql(SchemaObjectWithOwnerBase item)
    {
        try
        {
            return (item is ViewSchema view ? view.ViewText : item is CommandSchema command ? command.CommandText : "") ?? "";
        }
        catch (Exception)
        {
            return ""; // a provider that can't read it (permissions)
        }
    }

    /// <summary>A description (CS_Description), or "".</summary>
    public static string Description(SchemaObjectBase item) => Property(item, "CS_Description");

    /// <summary>A column's or parameter's type as its database writes it: varchar(80), decimal(10,2), MySQL's whole
    /// column type (CS_ColumnType: int unsigned).</summary>
    public static string Type(DataObjectBase item)
    {
        string whole = Property(item, "CS_ColumnType");
        if (whole.Length > 0)
            return whole.ToLowerInvariant();
        string native = (item.NativeType ?? "").Trim();
        string type = native.ToLowerInvariant();
        if (native.Contains("("))
            return native;
        if (SizedTypes.Contains(type) && item.Size > 0)
            return native + "(" + item.Size + ")";
        if (SizedTypes.Contains(type) && item.Size == -1)
            return native + "(max)";
        if (DecimalTypes.Contains(type) && item.Precision > 0)
            return native + "(" + item.Precision + "," + item.Scale + ")";
        return native;
    }

    /// <summary>A column's or parameter's default (CS_Default), or "".</summary>
    public static string Default(DataObjectBase item)
    {
        if (item is ParameterSchema parameter && parameter.Direction == ParameterDirection.ReturnValue)
            return "";
        return Property(item, "CS_Default");
    }

    public static bool IsIdentity(DataObjectBase item) => bool.TryParse(Property(item, "CS_IsIdentity"), out bool identity) && identity;

    /// <summary>A column with a unique index or constraint of its own (not its table's primary key).</summary>
    public static bool IsUnique(ColumnSchema column)
    {
        if (column.IsPrimaryKeyMember)
            return false;
        if (bool.TryParse(Property(column, "CS_IsUnique"), out bool unique) && unique)
            return true;
        return column.Table.Indexes.Cast<IndexSchema>().Any(i => i.IsUnique && !i.IsPrimaryKey && i.MemberColumns.Count == 1 && i.MemberColumns[0].Name == column.Name);
    }

    /// <summary>A parameter's direction: in, out, in/out, return.</summary>
    public static string Direction(ParameterSchema parameter) => parameter.Direction switch
    {
        ParameterDirection.Output => "out",
        ParameterDirection.InputOutput => "in/out",
        ParameterDirection.ReturnValue => "return",
        _ => "in",
    };

    public static string Property(SchemaObjectBase item, string name)
    {
        try
        {
            return Convert.ToString(item.ExtendedProperties[name]?.Value, CultureInfo.InvariantCulture)?.Trim() ?? "";
        }
        catch (Exception)
        {
            return "";
        }
    }

    // The search index: every table, view, procedure, column and parameter, for the search box (assets/search-index.js).

    public string SearchIndex()
    {
        var json = new StringBuilder();
        json.Append("window.DBDOCS_INDEX=[");
        bool first = true;
        void Add(string kind, string name, string parent, string url, string detail)
        {
            json.Append(first ? "\n" : ",\n");
            first = false;
            json.Append("[").Append(Json(kind)).Append(',').Append(Json(name)).Append(',').Append(Json(parent)).Append(',')
                .Append(Json(url)).Append(',').Append(Json(detail)).Append(']');
        }
        foreach (var table in Tables)
        {
            string page = Page(table);
            Add("table", Title(table), "", page, Summary(Description(table), table.Columns.Count + " columns"));
            foreach (ColumnSchema column in table.Columns)
                Add("column", column.Name, Title(table), page + "#" + Anchor(column.Name), Type(column));
        }
        foreach (var view in Views)
        {
            string page = Page(view);
            Add("view", Title(view), "", page, Summary(Description(view), view.Columns.Count + " columns"));
            foreach (ViewColumnSchema column in view.Columns)
                Add("column", column.Name, Title(view), page + "#" + Anchor(column.Name), Type(column));
        }
        foreach (var command in Commands)
        {
            string page = Page(command);
            int parameters = command.Parameters.Cast<ParameterSchema>().Count(p => p.Direction != ParameterDirection.ReturnValue);
            Add("procedure", Title(command), "", page, Summary(Description(command), parameters == 1 ? "1 parameter" : parameters + " parameters"));
            foreach (ParameterSchema parameter in command.Parameters)
                if (parameter.Direction != ParameterDirection.ReturnValue)
                    Add("parameter", parameter.Name, Title(command), page + "#" + Anchor(parameter.Name), Type(parameter));
        }
        json.Append("\n];\n");
        return json.ToString();
    }

    private static string Summary(string description, string otherwise)
    {
        string text = Regex.Replace(description ?? "", @"\s+", " ").Trim();
        return text.Length == 0 ? otherwise : text.Length > 120 ? text.Substring(0, 117).TrimEnd() + "…" : text;
    }

    public static string Json(string value)
    {
        var json = new StringBuilder("\"");
        foreach (char c in value ?? "")
        {
            switch (c)
            {
                case '"': json.Append("\\\""); break;
                case '\\': json.Append("\\\\"); break;
                case '\n': json.Append("\\n"); break;
                case '\r': json.Append("\\r"); break;
                case '\t': json.Append("\\t"); break;
                case '<': json.Append("\\u003c"); break; // never </script>
                default:
                    if (c < ' ' || c == '\u2028' || c == '\u2029')
                        json.Append("\\u").Append(((int)c).ToString("x4"));
                    else
                        json.Append(c);
                    break;
            }
        }
        return json.Append('"').ToString();
    }
}

// The tables and views SQL names where it reads or writes them: after FROM, JOIN, UPDATE, INTO, TABLE, USING and MERGE,
// and after the commas of a FROM's list. Strings and comments don't count.
public static class SqlReferences
{
    private static readonly HashSet<string> Introducers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "from", "join", "update", "into", "table", "using", "merge", "apply" };

    // words that end a FROM list's item, rather than being its alias
    private static readonly HashSet<string> Clauses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "where", "group", "order", "having", "inner", "left", "right", "full", "outer", "cross", "join", "on", "union",
        "except", "intersect", "limit", "offset", "set", "values", "select", "natural", "window", "returning", "output",
        "with", "when", "then", "else", "end", "for", "option", "into", "go", "begin", "if", "while", "return", "fetch",
        "lateral", "apply", "pivot", "unpivot", "default", "as",
    };

    public static IEnumerable<SchemaObjectWithOwnerBase> Find(string sql, DocSite site)
    {
        var found = new List<SchemaObjectWithOwnerBase>();
        if (string.IsNullOrEmpty(sql))
            return found;
        var tokens = SqlLexer.Tokenize(sql).Where(t => t.Kind != SqlTokenKind.Space && t.Kind != SqlTokenKind.Comment).ToList();
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].Kind != SqlTokenKind.Word || !Introducers.Contains(tokens[i].Text))
                continue;
            bool list = string.Equals(tokens[i].Text, "from", StringComparison.OrdinalIgnoreCase) || string.Equals(tokens[i].Text, "join", StringComparison.OrdinalIgnoreCase);
            int j = i + 1;
            while (j < tokens.Count && tokens[j].Text == "(") // MySQL's from (a join b on ...)
                j++;
            while (true)
            {
                if (!ReadName(tokens, ref j, out string owner, out string name))
                    break;
                var item = site.Resolve(owner, name);
                if (item != null && !found.Any(f => DocSite.Key(f) == DocSite.Key(item)))
                    found.Add(item);
                if (!list)
                    break;
                // an alias (AS a, or a), then a comma for the list's next item
                if (j < tokens.Count && tokens[j].Kind == SqlTokenKind.Word && string.Equals(tokens[j].Text, "as", StringComparison.OrdinalIgnoreCase))
                    j += 2;
                else if (j < tokens.Count && (tokens[j].Kind == SqlTokenKind.Quoted || (tokens[j].Kind == SqlTokenKind.Word && !Clauses.Contains(tokens[j].Text))))
                    j++;
                if (j < tokens.Count && tokens[j].Text == ",")
                    j++;
                else
                    break;
            }
        }
        return found;
    }

    // owner.name, [owner].[name], "owner"."name", database.owner.name: the last two parts
    private static bool ReadName(List<SqlToken> tokens, ref int j, out string owner, out string name)
    {
        owner = null;
        name = null;
        var parts = new List<string>();
        while (j < tokens.Count && (tokens[j].Kind == SqlTokenKind.Word || tokens[j].Kind == SqlTokenKind.Quoted))
        {
            parts.Add(Unquote(tokens[j].Text));
            j++;
            if (j < tokens.Count && tokens[j].Text == "." && j + 1 < tokens.Count)
                j++;
            else
                break;
        }
        if (parts.Count == 0)
            return false;
        name = parts[parts.Count - 1];
        owner = parts.Count > 1 ? parts[parts.Count - 2] : null;
        return true;
    }

    private static string Unquote(string text)
    {
        if (text.Length >= 2 && (text[0] == '[' || text[0] == '"' || text[0] == '`'))
            return text.Substring(1, text.Length - 2);
        return text;
    }
}
