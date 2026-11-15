// What the web templates share: each root's pages (their routes, titles and icons), each table's fields (which input,
// how wide, in which section) and its list's columns. Model.cs says what the business objects are; this says how their
// pages show them.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SchemaExplorer;

public static class Web
{
    // ---- pages ----

    /// <summary>A root's pages' address: its set's name in kebab case (Orders: orders, LineItems: line-items).</summary>
    public static string Route(TableSchema table) => Kebab(Names.Set(table));

    /// <summary>A root's title: its set's name in words (Orders, Line items).</summary>
    public static string Title(TableSchema table) => Words(Names.Set(table));

    /// <summary>One of them: Order, Line item.</summary>
    public static string Singular(TableSchema table) => Words(Model.Class(table));

    public static string ListPage(TableSchema table) => Model.Class(table) + "ListPage";
    public static string EditPage(TableSchema table) => Model.Class(table) + "EditPage";
    public static string Grid(TableSchema table) => Model.ChildList(table) + "Grid";

    /// <summary>The root's RootPage in Site.g.cs: Site.Orders.</summary>
    public static string SiteProperty(TableSchema table)
    {
        string name = Names.Set(table);
        return name is "Name" or "Database" or "Roots" ? name + "Pages" : name;
    }

    private static readonly (string Pattern, string Icon)[] Icons =
    {
        ("status|history|log|audit|event", "history"),
        ("line|detail", "list"),
        ("order|invoice|receipt|sale", "receipt"),
        ("cart|basket", "cart"),
        ("product|catalog", "package"),
        ("categor|tag|genre|type", "tag"),
        ("supplier|vendor|shipper|courier|carrier", "truck"),
        ("profile|user|customer|person|people|employee|member|account|contact|author", "user"),
        ("inventor|stock|warehouse", "warehouse"),
        ("item|part|sku", "box"),
        ("address|location|region|territor|countr|city", "map-pin"),
        ("payment|card|transaction", "credit-card"),
        ("review|rating", "star"),
        ("status|state|history|log|audit|event", "history"),
        ("line|detail", "list"),
        ("mail|message", "mail"),
        ("date|calendar|schedule|appointment", "calendar"),
        ("file|document|note", "file"),
    };

    /// <summary>A table's icon, by what its name says it holds.</summary>
    public static string Icon(TableSchema table)
    {
        string name = table.Name.ToLowerInvariant();
        foreach (var (pattern, icon) in Icons)
            if (Regex.IsMatch(name, pattern))
                return icon;
        return "table";
    }

    // ---- fields ----

    public enum Kind { Text, LongText, Integer, Decimal, Money, Date, DateTime, Time, Bool, Lookup, Unsupported }

    /// <summary>How a column's edited: its type, its size, its name.</summary>
    public static Kind KindOf(Model model, ColumnSchema column)
    {
        if (LookupOf(model, column) != null)
            return Kind.Lookup;
        string type = Model.Type(column).TrimEnd('?');
        switch (type)
        {
            case "string":
                // long text in a text area; a long name, address or path on a line still
                return (column.Size >= 200 || column.Size <= 0) && !Regex.IsMatch(column.Name, "name|title|user|login|mail|url|code|path|file|image|key", RegexOptions.IgnoreCase)
                    ? Kind.LongText : Kind.Text;
            case "int": case "long": case "short": case "byte": case "sbyte": case "uint": case "ulong": case "ushort":
                return Kind.Integer;
            case "decimal": case "double": case "float":
                return Money(column) ? Kind.Money : Kind.Decimal;
            case "DateTime": case "DateTimeOffset":
                return Regex.IsMatch(Names.Property(column), "(Date|Day|Birthday|Dob)$") ? Kind.Date : Kind.DateTime;
            case "DateOnly":
                return Kind.Date;
            case "TimeOnly": case "TimeSpan":
                return Kind.Time;
            case "bool":
                return Kind.Bool;
            default:
                return Kind.Unsupported; // bytes, a Guid, ...: not on the form
        }
    }

    private static bool Money(ColumnSchema column) =>
        column.Scale == 2 && Regex.IsMatch(column.Name, "price|cost|total|amount|balance|salary|fee|tax|payment|charge", RegexOptions.IgnoreCase);

    /// <summary>A decimal's places.</summary>
    public static int Scale(ColumnSchema column) => Math.Max(0, column.Scale);

    /// <summary>The foreign key's name/value list a column picks from: a single-column key to a table that has one.</summary>
    public static TableSchema LookupOf(Model model, ColumnSchema column)
    {
        foreach (TableKeySchema key in column.Table.ForeignKeys)
        {
            if (key.ForeignKeyMemberColumns.Count != 1 || key.ForeignKeyMemberColumns[0].Name != column.Name || !Names.RefersToPrimaryKey(key))
                continue;
            var target = model.Tables.FirstOrDefault(t => t.Owner == key.PrimaryKeyTable.Owner && t.Name == key.PrimaryKeyTable.Name);
            if (target != null && model.HasNameValueList(target))
                return target;
        }
        return null;
    }

    /// <summary>A lookup's field in a page: _categories.</summary>
    public static string LookupField(TableSchema table) => "_" + Camel(Names.Set(table));

    /// <summary>How many of a section's six columns a field takes: a third, or all of them for long text.</summary>
    public static int Span(Model model, ColumnSchema column) => KindOf(model, column) == Kind.LongText ? 6 : 2;

    /// <summary>The input's type for a text: email, tel, url, or text.</summary>
    public static string TextType(ColumnSchema column) =>
        Regex.IsMatch(column.Name, "e_?mail", RegexOptions.IgnoreCase) ? "email" :
        Regex.IsMatch(column.Name, "phone|fax|mobile", RegexOptions.IgnoreCase) ? "tel" :
        Regex.IsMatch(column.Name, "url|website|homepage", RegexOptions.IgnoreCase) ? "url" : "text";

    /// <summary>A code or a key, monospaced: a key's text, or a short text called ...Id or ...Code.</summary>
    public static bool Mono(ColumnSchema column) =>
        column.IsPrimaryKeyMember || (column.Size > 0 && column.Size <= 20 && Regex.IsMatch(Names.Property(column), "(Id|ID|Code|Sku|SKU)$"));

    /// <summary>The columns a form shows: all but the parent's key (the parent sets it) and what it can't edit or show.</summary>
    public static List<ColumnSchema> FormColumns(Model model, TableSchema table) =>
        model.Columns(table).Where(c => !model.FromParent(c) && KindOf(model, c) != Kind.Unsupported).ToList();

    public sealed class Section
    {
        public string Title;
        public string Icon;
        public List<ColumnSchema> Columns;
    }

    /// <summary>A form's sections: the table's own columns first, then a section for each group of three or more columns
    /// that start with the same word (ShipAddr1, ShipCity, ShipZip: Ship), their names first.</summary>
    public static List<Section> Sections(Model model, TableSchema table)
    {
        var columns = FormColumns(model, table);
        var groups = Groups(model, table);
        // long text last, below the rest
        columns = columns.OrderBy(c => KindOf(model, c) == Kind.LongText ? 1 : 0).ToList();
        var sections = new List<Section> { new Section { Title = Singular(table), Icon = Icon(table), Columns = columns.Where(c => !groups.Any(g => g.Value.Contains(c))).ToList() } };
        foreach (var group in groups)
            sections.Add(new Section
            {
                Title = group.Key,
                Icon = GroupIcon(group.Key),
                Columns = group.Value.OrderBy(c => Regex.IsMatch(Names.Property(c), "Name$") ? 0 : 1).ThenBy(c => columns.IndexOf(c)).ToList(),
            });
        return sections.Where(s => s.Columns.Count > 0).ToList();
    }

    private static Dictionary<string, List<ColumnSchema>> Groups(Model model, TableSchema table)
    {
        var groups = new Dictionary<string, List<ColumnSchema>>();
        foreach (var column in FormColumns(model, table).Where(c => !c.IsPrimaryKeyMember))
        {
            var words = Model.Friendly(column).Split(' ');
            if (words.Length < 2)
                continue;
            if (!groups.TryGetValue(words[0], out var list))
                groups[words[0]] = list = new List<ColumnSchema>();
            list.Add(column);
        }
        return groups.Where(g => g.Value.Count >= 3).ToDictionary(g => g.Key, g => g.Value);
    }

    private static string GroupIcon(string word)
    {
        string w = word.ToLowerInvariant();
        if (Regex.IsMatch(w, "^(ship|shipping|delivery|deliver)")) return "truck";
        if (Regex.IsMatch(w, "^(bill|billing|payment|pay|invoice)")) return "credit-card";
        if (Regex.IsMatch(w, "^(addr|address|home|mail|postal|street)")) return "map-pin";
        if (Regex.IsMatch(w, "^(contact|phone|email)")) return "mail";
        return "folder";
    }

    /// <summary>A list's columns: the key, then the table's own (not a group's, a second address line, nor more than one
    /// long text), seven at most.</summary>
    public static List<ColumnSchema> ListColumns(Model model, TableSchema table)
    {
        var all = model.Columns(table).Where(c => KindOf(model, c) != Kind.Unsupported).ToList();
        var grouped = Groups(model, table).SelectMany(g => g.Value).ToList();
        var columns = new List<ColumnSchema>();
        columns.AddRange(all.Where(c => c.IsPrimaryKeyMember));
        bool longText = false;
        foreach (var column in all.Where(c => !c.IsPrimaryKeyMember && !grouped.Contains(c)))
        {
            string property = Names.Property(column);
            if (property.EndsWith("2") && all.Any(c => Names.Property(c) == property.Substring(0, property.Length - 1) + "1"))
                continue;
            if (KindOf(model, column) == Kind.LongText)
            {
                if (longText)
                    continue;
                longText = true;
            }
            columns.Add(column);
        }
        return columns.Take(7).ToList();
    }

    /// <summary>A list's column's ValueKind.</summary>
    public static string ValueKind(Model model, ColumnSchema column)
    {
        if (column.IsPrimaryKeyMember && KindOf(model, column) is Kind.Text or Kind.Integer)
            return "Key";
        switch (KindOf(model, column))
        {
            case Kind.LongText: return "LongText";
            case Kind.Integer: case Kind.Decimal: return "Number";
            case Kind.Money: return "Money";
            case Kind.Date: return "Date";
            case Kind.DateTime: return "DateTime";
            case Kind.Bool: return "Bool";
            default: return "Text";
        }
    }

    // ---- a field's markup ----

    /// <summary>A column's field: its input for its kind, bound to CSLA's PropertyInfo for it (on target, the page's Model or a row).</summary>
    public static string Field(Model model, ColumnSchema column, string target, string row, bool compact = false)
    {
        string property = target + "." + Model.Property(column);
        string info = row.Length == 0 ? "@Info(() => " + property + ")" : "@Info(() => " + property + ", " + row + ")";
        var attributes = new List<string> { "Info=\"" + info + "\"" };
        if (compact)
            attributes.Add("Compact");
        var kind = Web.KindOf(model, column);
        if (!model.Settable(column))
        {
            string valueKind = column.IsPrimaryKeyMember ? "Key" : Web.ValueKind(model, column);
            attributes.Add("Kind=\"ValueKind." + valueKind + "\"");
            if (kind is Web.Kind.Decimal or Web.Kind.Money && Web.Scale(column) > 0)
                attributes.Add("Scale=\"" + Web.Scale(column) + "\"");
            if (Model.Identity(column))
                attributes.Add("PendingText=\"Assigned when it's saved\"");
            else if (Names.IsComputed(column))
                attributes.Add("Hint=\"The database computes it.\"");
            return "<ReadOnlyField " + string.Join(" ", attributes) + " />";
        }
        if (!compact && Web.Span(model, column) != 2)
            attributes.Add("Span=\"" + Web.Span(model, column) + "\"");
        if (!column.AllowDBNull && kind != Web.Kind.Bool)
            attributes.Add("Required");
        if (model.KeySetOnce(column))
            attributes.Add("KeySetOnce");
        switch (kind)
        {
            case Web.Kind.Lookup:
                if (Model.Friendly(column).EndsWith(" ID"))
                    attributes.Insert(1, "Label=\"" + Singular(LookupOf(model, column)) + "\"");
                attributes.Add("Items=\"" + Web.LookupField(Web.LookupOf(model, column)) + "\"");
                return "<LookupField " + string.Join(" ", attributes) + " />";
            case Web.Kind.Integer:
                return "<NumberField " + string.Join(" ", attributes) + " />";
            case Web.Kind.Decimal:
            case Web.Kind.Money:
                if (kind == Web.Kind.Money)
                    attributes.Add("Money");
                if (Web.Scale(column) > 0)
                    attributes.Add("Scale=\"" + Web.Scale(column) + "\"");
                return "<NumberField " + string.Join(" ", attributes) + " />";
            case Web.Kind.Date:
                return "<DateField " + string.Join(" ", attributes) + " />";
            case Web.Kind.DateTime:
                attributes.Add("WithTime");
                return "<DateField " + string.Join(" ", attributes) + " />";
            case Web.Kind.Time:
                return "<DateField " + string.Join(" ", attributes) + " />";
            case Web.Kind.Bool:
                return "<SwitchField " + string.Join(" ", attributes) + " />";
            default:
                if (column.Size > 0 && column.Size < 1_000_000)
                    attributes.Add("MaxLength=\"" + column.Size + "\"");
                if (kind == Web.Kind.LongText)
                    attributes.Add("Multiline");
                else if (Web.Mono(column))
                    attributes.Add("Mono");
                if (Web.TextType(column) != "text")
                    attributes.Add("Type=\"" + Web.TextType(column) + "\"");
                return "<TextField " + string.Join(" ", attributes) + " />";
        }
    }

    // ---- keys in addresses ----

    /// <summary>A key column's route parameter: its constraint and its property's type (nullable: a new object has none).</summary>
    public static (string Constraint, string Type, string Cast) RouteParameter(ColumnSchema column)
    {
        switch (Model.ParameterType(column))
        {
            case "long": return (":long", "long?", "");
            case "int": return (":int", "int?", "");
            case "short": return (":int", "int?", "(short)");
            case "byte": return (":int", "int?", "(byte)");
            case "Guid": return (":guid", "Guid?", "");
            case "decimal": return (":decimal", "decimal?", "");
            case "double": return (":double", "double?", "");
            case "float": return (":float", "float?", "");
            case "bool": return (":bool", "bool?", "");
            case "DateTime": return (":datetime", "DateTime?", "");
            default: return ("", "string?", "");
        }
    }

    /// <summary>A key's route: {OrderId:long}/{LineNum:int}.</summary>
    public static string RouteTemplate(IEnumerable<ColumnSchema> key) =>
        string.Join("/", key.Select(c => "{" + Model.Property(c) + RouteParameter(c).Constraint + "}"));

    // ---- words ----

    public static string Words(string name)
    {
        var words = Regex.Split(name, "(?<=[a-z])(?=[A-Z0-9])|(?<=[A-Z])(?=[A-Z][a-z])").Where(w => w.Length > 0).ToList();
        for (int i = 1; i < words.Count; i++)
            if (words[i].Any(char.IsLower))
                words[i] = words[i].ToLowerInvariant();
        return string.Join(" ", words);
    }

    public static string Kebab(string name) => Words(name).ToLowerInvariant().Replace(' ', '-');

    public static string Camel(string name) => char.ToLowerInvariant(name[0]) + name.Substring(1);
}
