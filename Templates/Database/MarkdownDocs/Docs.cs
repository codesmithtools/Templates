// What the templates share: each page's file and link, Markdown text, and the words the Mermaid diagram is written in.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SchemaExplorer;

public static class Docs
{
    // a database's default schemas: names in them go without it
    private static readonly string[] DefaultOwners = { "", "dbo", "public", "main" };

    // types whose size is part of the type (varchar(80)), and those with a precision and scale (decimal(10,2))
    private static readonly string[] SizedTypes =
        { "char", "varchar", "nchar", "nvarchar", "binary", "varbinary", "character", "character varying", "bpchar", "varchar2", "nvarchar2", "raw" };
    private static readonly string[] DecimalTypes = { "decimal", "numeric", "number", "dec" };

    // Pages

    /// <summary>The name a page shows: the table's (or view's, or procedure's), with its schema in front unless that's
    /// the database's default (dbo, public).</summary>
    public static string Title(SchemaObjectWithOwnerBase item) =>
        DefaultOwners.Contains(item.Owner ?? "") || item.Owner == item.Database.Name ? item.Name : item.Owner + "." + item.Name;

    /// <summary>A page's path in the output folder: tables/Account.md, views/..., procedures/....</summary>
    public static string Page(SchemaObjectWithOwnerBase item) => Folder(item) + "/" + Files(item)[item];

    /// <summary>A Markdown link to a page: from the README, or from a page in the same folder.</summary>
    public static string Link(SchemaObjectWithOwnerBase item, bool sameFolder = false)
    {
        if (!Files(item).TryGetValue(item, out string file))
            return Text(Title(item)); // not one of the database's (no page)
        return "[" + Text(Title(item)) + "](" + (sameFolder ? "" : Folder(item) + "/") + Uri.EscapeDataString(file) + ")";
    }

    private static string Folder(SchemaObjectWithOwnerBase item) =>
        item is TableSchema ? "tables" : item is ViewSchema ? "views" : "procedures";

    // The files of a folder's pages: each title, without what a file name can't have, once (case aside, as on Windows).
    private static Dictionary<SchemaObjectWithOwnerBase, string> Files(SchemaObjectWithOwnerBase item)
    {
        DatabaseSchema database = item.Database;
        IEnumerable<SchemaObjectWithOwnerBase> items = item is TableSchema ? database.Tables.Cast<SchemaObjectWithOwnerBase>()
            : item is ViewSchema ? database.Views.Cast<SchemaObjectWithOwnerBase>() : database.Commands.Cast<SchemaObjectWithOwnerBase>();
        var files = new Dictionary<SchemaObjectWithOwnerBase, string>();
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (SchemaObjectWithOwnerBase each in items)
        {
            string name = Regex.Replace(Title(each), @"[\x00-\x1f<>:""/\\|?*\s]", "_").TrimEnd('.');
            if (name.Length == 0)
                name = "_";
            string file = name + ".md";
            for (int i = 2; !taken.Add(file); i++)
                file = name + "-" + i + ".md";
            files[each] = file;
        }
        return files;
    }

    // What the schema says

    /// <summary>A description (CS_Description), or "" where there's none.</summary>
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

    /// <summary>A column's or parameter's default for a table cell (CS_Default, as code); "identity" for a column the
    /// database numbers.</summary>
    public static string Default(DataObjectBase item)
    {
        if (item is ParameterSchema parameter && parameter.Direction == ParameterDirection.ReturnValue)
            return "";
        string value = Property(item, "CS_Default");
        if (value.Length > 0)
            return Code(value);
        return bool.TryParse(Property(item, "CS_IsIdentity"), out bool identity) && identity ? "identity" : "";
    }

    /// <summary>What keys a column is part of: PK, FK, or both.</summary>
    public static string Keys(ColumnSchema column) =>
        string.Join(", ", new[] { column.IsPrimaryKeyMember ? "PK" : null, column.IsForeignKeyMember ? "FK" : null }.Where(k => k != null));

    /// <summary>A key's or an index's columns: OrderId, LineNum.</summary>
    public static string Columns(IEnumerable<MemberColumnSchema> columns) => string.Join(", ", columns.Select(c => Text(c.Name)));

    /// <summary>The foreign keys of the database's tables that refer to this one. Read from every table: a provider's
    /// PrimaryKeys may not have them (SQLite's).</summary>
    public static IEnumerable<TableKeySchema> ReferencedBy(TableSchema table) =>
        table.Database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
            .Where(k => k.PrimaryKeyTable.Owner == table.Owner && k.PrimaryKeyTable.Name == table.Name);

    /// <summary>A parameter's direction: in, out, in/out, return.</summary>
    public static string Direction(ParameterSchema parameter) => parameter.Direction switch
    {
        ParameterDirection.Output => "out",
        ParameterDirection.InputOutput => "in/out",
        ParameterDirection.ReturnValue => "return",
        _ => "in",
    };

    // An extended property's value as text: "" where the provider has none.
    private static string Property(SchemaObjectBase item, string name) =>
        Convert.ToString(item.ExtendedProperties[name]?.Value)?.Trim() ?? "";

    // Markdown

    /// <summary>Text on one line of Markdown (a heading, a list item, a table cell): what Markdown would read as
    /// formatting escaped, | too, and line breaks as spaces.</summary>
    public static string Text(string value)
    {
        string text = Regex.Replace(value ?? "", @"\s+", " ").Trim();
        var markdown = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            bool escape = "\\`*~[<|$".IndexOf(c) >= 0 // emphasis, code, links, HTML, table cells, math
                || (c == '_' && !InWord(text, i)) // order_id and PK__Orders are text; _order_ is emphasis
                || (c == '#' && (i == 0 || i == text.Length - 1)); // a heading's
            if (escape)
                markdown.Append('\\');
            markdown.Append(c);
        }
        return markdown.ToString();
    }

    // Whether the run of _ at i is inside a word, where Markdown doesn't read it as emphasis.
    private static bool InWord(string text, int i)
    {
        int start = i, end = i;
        while (start > 0 && text[start - 1] == '_')
            start--;
        while (end + 1 < text.Length && text[end + 1] == '_')
            end++;
        return start > 0 && end + 1 < text.Length && char.IsLetterOrDigit(text[start - 1]) && char.IsLetterOrDigit(text[end + 1]);
    }

    /// <summary>A table cell's text that may have several lines: kept, with &lt;br&gt; between them.</summary>
    public static string Cell(string value) =>
        string.Join("<br>", Regex.Split(value ?? "", @"\r\n|\r|\n").Select(Text).Where(line => line.Length > 0));

    /// <summary>A description's paragraphs (blank lines part them), each a line of text.</summary>
    public static IEnumerable<string> Paragraphs(string value) =>
        Regex.Split((value ?? "").Trim(), @"(?:\r\n|\r|\n)\s*(?:\r\n|\r|\n)").Select(Text).Where(p => p.Length > 0)
            .Select(p => Regex.Replace(p, @"^(?:[>+-]|\d+(?=[.)]))", m => char.IsDigit(m.Value[0]) ? m.Value + "\\" : "\\" + m.Value)); // not a quote or a list

    /// <summary>The first paragraph of a description, after a colon, for a list: ": Customers' orders.", or "".</summary>
    public static string Summary(SchemaObjectBase item) => Paragraphs(Description(item)).Select(p => ": " + p).FirstOrDefault() ?? "";

    /// <summary>Code in a table cell (an expression: a default): `'en-US'`.</summary>
    public static string Code(string value)
    {
        string text = Regex.Replace(value ?? "", @"\s+", " ").Trim();
        if (text.Length == 0)
            return "";
        int longest = Regex.Matches(text, "`+").Select(m => m.Length).DefaultIfEmpty(0).Max();
        string ticks = new string('`', longest + 1);
        string space = text.StartsWith("`") || text.EndsWith("`") ? " " : "";
        return ticks + space + text.Replace("|", "\\|") + space + ticks;
    }

    // The diagram (Mermaid's erDiagram)

    // words an erDiagram reads as its own, not as an entity's name (to, many: a relationship's)
    private static readonly string[] Keywords =
        { "erdiagram", "style", "classdef", "class", "subgraph", "end", "direction", "one", "only", "zero", "many", "to", "optionally" };

    /// <summary>A table's entity: a word to Mermaid (letters, digits and _, from a letter), once in the diagram.</summary>
    public static string Entity(TableSchema table) => Entities(table.Database)[table];

    /// <summary>The entity's declaration: its title as an alias where the name had to change (Order_Details["Order Details"]).</summary>
    public static string EntityDeclaration(TableSchema table)
    {
        string entity = Entity(table), title = Title(table);
        return entity == title ? entity : entity + "[\"" + Regex.Replace(title, @"[""%\\]", "'") + "\"]"; // an alias can't have " % \
    }

    private static Dictionary<TableSchema, string> Entities(DatabaseSchema database)
    {
        var entities = new Dictionary<TableSchema, string>();
        var taken = new HashSet<string>();
        foreach (TableSchema table in database.Tables)
        {
            string name = Word(Title(table), "table");
            if (Keywords.Contains(name.ToLowerInvariant()))
                name += "_";
            string entity = name;
            for (int i = 2; !taken.Add(entity); i++)
                entity = name + "_" + i;
            entities[table] = entity;
        }
        return entities;
    }

    /// <summary>A column of an entity: type, name and keys, each a word (varchar(80) is varchar; Unit Price is Unit_Price).</summary>
    public static string Attribute(ColumnSchema column)
    {
        string type = Word(Regex.Replace(column.NativeType ?? "", @"\(.*?\)", ""), "any");
        string name = Word(column.Name, "column");
        if (name.ToUpperInvariant() is "PK" or "FK" or "UK")
            name += "_"; // Mermaid's key markers
        string keys = Keys(column);
        return type + " " + name + (keys.Length > 0 ? " " + keys : "");
    }

    /// <summary>The foreign keys the diagram draws: those to a table of the database.</summary>
    public static IEnumerable<TableKeySchema> Relationships(DatabaseSchema database)
    {
        var entities = Entities(database);
        return database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
            .Where(k => entities.ContainsKey(k.PrimaryKeyTable) && entities.ContainsKey(k.ForeignKeyTable));
    }

    /// <summary>A foreign key as a relationship: one row of the table it refers to (||), or none where its columns can
    /// be null (|o), to any number of the table's (o{), labeled with its columns.</summary>
    public static string Relationship(TableKeySchema key)
    {
        bool optional = key.ForeignKeyMemberColumns.Any(c => c.AllowDBNull);
        string label = string.Join(", ", key.ForeignKeyMemberColumns.Select(c => c.Name)).Replace("\"", "'");
        return Entity(key.PrimaryKeyTable) + (optional ? " |o--o{ " : " ||--o{ ") + Entity(key.ForeignKeyTable) + " : \"" + label + "\"";
    }

    // A word to Mermaid: letters, digits and _ (others become _), starting with a letter.
    private static string Word(string value, string fallback)
    {
        string word = Regex.Replace(value ?? "", "[^A-Za-z0-9_]+", "_").Trim('_');
        return word.Length == 0 ? fallback : char.IsLetter(word[0]) ? word : fallback + "_" + word;
    }
}
