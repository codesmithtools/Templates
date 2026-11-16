// What the web UI's templates (Ui.cst, UiTable.cst, UiIndex.cst) know of a table: its labels, the kind of input each
// column takes, the columns its row is shown by, its list's columns, its relationships and its icon. Web.cs has the
// routes, JSON names and lists (sorts, search, filters) it shares with the API and the client.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SchemaExplorer;

public static class Ui
{
    // ---- labels ----

    // words a column's name shortens, spelled out
    private static readonly Dictionary<string, string> Spelled = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["descn"] = "description", ["desc"] = "description", ["addr"] = "address", ["qty"] = "quantity", ["num"] = "number",
        ["amt"] = "amount", ["id"] = "ID", ["url"] = "URL", ["uri"] = "URI", ["ip"] = "IP", ["pin"] = "PIN", ["sku"] = "SKU",
        ["guid"] = "GUID", ["uuid"] = "UUID", ["api"] = "API",
    };

    /// <summary>A name in words, in sentence case: ShipAddr1 is "Ship address 1", ProductId "Product ID".</summary>
    public static string Words(string name)
    {
        var words = Regex.Split(name ?? "", "(?<=[a-z])(?=[A-Z0-9])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[0-9])(?=[A-Za-z])|_+")
            .Where(w => w.Length > 0).ToList();
        for (int i = 0; i < words.Count; i++)
        {
            if (Spelled.TryGetValue(words[i], out string spelled))
                words[i] = spelled;
            else if (i > 0 && words[i].Any(char.IsLower))
                words[i] = words[i].ToLowerInvariant();
        }
        string text = string.Join(" ", words);
        return text.Length == 0 ? name : char.ToUpperInvariant(text[0]) + text.Substring(1);
    }

    /// <summary>A table's name in the navigation and its pages' titles: its set in words (Line items; lineitem's too,
    /// NameWords.cs).</summary>
    public static string Title(TableSchema table) => Words(NameWords.Set(table));

    /// <summary>One of its rows: its entity in words (Line item).</summary>
    public static string Singular(TableSchema table) => Words(NameWords.Entity(table));

    /// <summary>A column's label: its name in words (listprice too: List price, NameWords.cs); a foreign key's, the table
    /// it refers to (UniqueID refers to Profiles: Profile), where it's the only one to it; a bool's without its Is
    /// (IsShoppingCart: Shopping cart).</summary>
    public static string Label(ColumnSchema column)
    {
        var key = Reference(column);
        if (key != null && column.Table.ForeignKeys.Cast<TableKeySchema>().Count(k => Same(k.PrimaryKeyTable, key.PrimaryKeyTable)) == 1)
            return Singular(key.PrimaryKeyTable);
        string property = NameWords.Property(column);
        if (Names.Type(column).TrimEnd('?') == "bool" && Regex.IsMatch(property, "^(Is|Has)[A-Z0-9]"))
            property = property.Substring(property.StartsWith("Is") ? 2 : 3);
        return Words(property);
    }

    // ---- kinds of input ----

    private static readonly Regex Money = new Regex("(price|cost|amount|total|balance|salary|fee|subtotal|tax|payment|charge|revenue)$", RegexOptions.IgnoreCase);
    private static readonly Regex LongText = new Regex("(desc|descn|description|comment|comments|note|notes|body|details|summary|bio|about|message|content|text|remarks)$", RegexOptions.IgnoreCase);

    /// <summary>How a column is shown and edited: text, multiline, email, phone, url, integer, decimal, money, boolean,
    /// date, datetime, time, uuid, binary or json.</summary>
    public static string Kind(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        string native = Web.Native(column);
        string name = Names.Property(column);
        switch (type)
        {
            case "bool": return "boolean";
            case "byte": case "sbyte": case "short": case "ushort": case "int": case "uint": case "long": case "ulong": return "integer";
            case "float": case "double": return "decimal";
            case "decimal": return native is "money" or "smallmoney" || Money.IsMatch(name) ? "money" : "decimal";
            case "DateTime": case "DateTimeOffset": return native == "date" ? "date" : "datetime";
            case "DateOnly": return "date";
            case "TimeOnly": case "TimeSpan": return "time";
            case "Guid": return "uuid";
            case "byte[]": return "binary";
            case "string": break;
            default: return "text";
        }
        if (native is "json" or "jsonb")
            return "json";
        string words = " " + Words(NameWords.Property(column)).ToLowerInvariant() + " ";
        if (words.Contains("email") || words.Contains(" e mail "))
            return "email";
        if (Regex.IsMatch(words, " (phone|fax|telephone|tel) "))
            return "phone";
        if (Regex.IsMatch(words, " (url|website|homepage|web site) "))
            return "url";
        bool unbounded = column.Size <= 0 || column.Size >= 1_000_000 || Web.Validation(column) == null;
        if (native == "xml" || unbounded && native != "" || column.Size > 255 || LongText.IsMatch(name) && column.Size >= 100)
            return "multiline";
        return "text";
    }

    /// <summary>A column whose few values read as a badge: a short Status, Type, Kind, Level, Priority or Stage.</summary>
    public static bool Badge(ColumnSchema column) =>
        Names.Type(column).TrimEnd('?') == "string" && column.Size > 0 && column.Size <= 20 &&
        Regex.IsMatch(Names.Property(column), "(Status|Type|Kind|Level|Priority|Stage|Category|Role)$") && !column.IsPrimaryKeyMember && !column.IsForeignKeyMember;

    /// <summary>A column that reads better in a monospaced font: a key's, a GUID, bytes, JSON.</summary>
    public static bool Mono(ColumnSchema column) =>
        column.IsPrimaryKeyMember || column.IsForeignKeyMember || Kind(column) is "uuid" or "binary" or "json";

    /// <summary>Whether a column is a decimal with a precision and scale the database keeps to (numeric(10,2); SQL
    /// Server's money is (19,4)), which the form checks.</summary>
    public static bool Exact(ColumnSchema column) =>
        Names.Type(column).TrimEnd('?') == "decimal" && column.Precision > 0 && column.Precision <= 38 && column.Scale >= 0 && column.Scale <= column.Precision;

    /// <summary>A whole number's limits: its type's (an int's), within what JavaScript counts exactly.</summary>
    public static (string Min, string Max) Range(ColumnSchema column)
    {
        switch (Names.Type(column).TrimEnd('?'))
        {
            case "byte": return ("0", "255");
            case "sbyte": return ("-128", "127");
            case "short": return ("-32768", "32767");
            case "ushort": return ("0", "65535");
            case "int": return ("-2147483648", "2147483647");
            case "uint": return ("0", "4294967295");
            case "ulong": return ("0", "Number.MAX_SAFE_INTEGER");
            default: return ("Number.MIN_SAFE_INTEGER", "Number.MAX_SAFE_INTEGER");
        }
    }

    /// <summary>What a new row starts with: the column's default, where it's a value (a number, text, true or false) or
    /// the time ("now"); null otherwise.</summary>
    public static string Default(ColumnSchema column)
    {
        string text = Convert.ToString(column.ExtendedProperties["CS_Default"]?.Value, CultureInfo.InvariantCulture)?.Trim();
        if (string.IsNullOrEmpty(text) || Web.Generated(column))
            return null;
        while (text.Length > 1 && text[0] == '(' && text[text.Length - 1] == ')')
            text = text.Substring(1, text.Length - 2).Trim();
        text = Regex.Replace(text, @"::[\w\s]+(\[\])?$", "").Trim(); // PostgreSQL's 'en-US'::character varying
        string kind = Kind(column);
        string lower = text.ToLowerInvariant();
        if (kind is "date" or "datetime" or "time" &&
            Regex.IsMatch(lower, @"^(getdate|getutcdate|sysdatetime|sysutcdatetime|sysdatetimeoffset|now|current_timestamp|current_date|current_time|localtimestamp|localtime|datetime\('now'\)|date\('now'\))(\(\))?$"))
            return "\"now\"";
        if (text.Length >= 2 && text[0] == '\'' && text[text.Length - 1] == '\'')
        {
            string value = text.Substring(1, text.Length - 2).Replace("''", "'");
            if (kind == "boolean")
                return value is "1" or "true" or "t" ? "true" : value is "0" or "false" or "f" ? "false" : null;
            if (kind is "integer" or "decimal" or "money")
                return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal quoted) ? quoted.ToString(CultureInfo.InvariantCulture) : null;
            return kind is "text" or "multiline" or "email" or "phone" or "url" ? Ts(value) : null;
        }
        if (kind == "boolean")
            return lower is "1" or "true" or "b'1'" ? "true" : lower is "0" or "false" or "b'0'" ? "false" : null;
        if (kind is "integer" or "decimal" or "money" && decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
            return number.ToString(CultureInfo.InvariantCulture);
        return null;
    }

    /// <summary>Whether a new row must be given a column: one that can't be null, the database doesn't fill in, and
    /// has no default.</summary>
    public static bool Required(ColumnSchema column) => !column.AllowDBNull && !Web.Generated(column) && Default(column) == null;

    // ---- rows ----

    private static readonly string[] DisplayNames = { "name", "title", "display_name", "displayname", "label", "user_name", "username", "login", "full_name", "fullname" };

    /// <summary>The columns a row is shown by: a Name or a Title (or the like), a first and last name, an Email; none
    /// for a row that's best shown by its key (an order: #1001).</summary>
    public static List<ColumnSchema> Display(TableSchema table)
    {
        var text = table.Columns.Cast<ColumnSchema>()
            .Where(c => !c.IsPrimaryKeyMember && !c.IsForeignKeyMember && Names.Type(c).TrimEnd('?') == "string" && Web.Sortable(c)).ToList();
        foreach (string name in DisplayNames)
            if (text.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) is ColumnSchema named)
                return new List<ColumnSchema> { named };
        var first = text.FirstOrDefault(c => Regex.IsMatch(c.Name, "^(first_?name|given_?name|forename)$", RegexOptions.IgnoreCase));
        var last = text.FirstOrDefault(c => Regex.IsMatch(c.Name, "^(last_?name|surname|family_?name)$", RegexOptions.IgnoreCase));
        if (first != null && last != null)
            return new List<ColumnSchema> { first, last };
        // RoleName in aspnet_Roles (its entity ends in Role)
        string entity = Names.Entity(table).ToLowerInvariant();
        var own = text.FirstOrDefault(c => c.Name.EndsWith("name", StringComparison.OrdinalIgnoreCase) && c.Name.Length > 4 &&
            entity.EndsWith(c.Name.Substring(0, c.Name.Length - 4).TrimEnd('_').ToLowerInvariant(), StringComparison.Ordinal));
        if (own != null)
            return new List<ColumnSchema> { own };
        var email = text.FirstOrDefault(c => Kind(c) == "email");
        return email != null ? new List<ColumnSchema> { email } : new List<ColumnSchema>();
    }

    /// <summary>A list's columns: the key, what the row is shown by, then the foreign keys, badges, money and dates, and
    /// the rest (not long text, bytes or JSON), six at most, in the table's order after the first two.</summary>
    public static List<ColumnSchema> Columns(TableSchema table, int most = 6)
    {
        var all = table.Columns.Cast<ColumnSchema>().Where(c => !Names.IsRowVersion(c) && Kind(c) is not ("binary" or "json")).ToList();
        var chosen = new List<ColumnSchema>();
        void Take(IEnumerable<ColumnSchema> columns)
        {
            foreach (var column in columns)
                if (chosen.Count < most && !chosen.Contains(column))
                    chosen.Add(column);
        }
        Take(Web.Key(table));
        Take(Display(table));
        Take(all.Where(c => c.IsForeignKeyMember));
        Take(all.Where(c => Badge(c) || Kind(c) is "money" or "datetime" or "date" or "boolean"));
        Take(all.Where(c => Kind(c) != "multiline" && !c.AllowDBNull));
        Take(all.Where(c => Kind(c) != "multiline"));
        Take(all);
        int head = Web.Key(table).Count + Display(table).Count;
        return chosen.Take(head).Concat(all.Where(c => chosen.Skip(head).Contains(c))).ToList();
    }

    /// <summary>What "recent" sorts by on the dashboard: a created, added or modified date (or a date), or a key the
    /// database numbers; null where there's neither.</summary>
    public static ColumnSchema Recent(TableSchema table)
    {
        var dates = table.Columns.Cast<ColumnSchema>().Where(c => Kind(c) is "datetime" or "date" && Web.Sortable(c)).ToList();
        return dates.FirstOrDefault(c => Regex.IsMatch(c.Name, "(created|added|inserted|placed|ordered)", RegexOptions.IgnoreCase))
            ?? dates.FirstOrDefault(c => Regex.IsMatch(c.Name, "(date|time|stamp|_at)$", RegexOptions.IgnoreCase))
            ?? dates.FirstOrDefault(c => Regex.IsMatch(c.Name, "(updated|modified|changed|activity)", RegexOptions.IgnoreCase))
            ?? Web.Key(table).Where(c => Web.Key(table).Count == 1 && Web.Generated(c) && Web.Sortable(c)).FirstOrDefault();
    }

    // ---- relationships ----

    private static bool Same(TableSchema a, TableSchema b) => a.Owner == b.Owner && a.Name == b.Name;

    /// <summary>The foreign key a column is alone in (the key a combobox picks), or null.</summary>
    public static TableKeySchema Reference(ColumnSchema column) =>
        column.Table.ForeignKeys.Cast<TableKeySchema>().FirstOrDefault(k =>
            k.ForeignKeyMemberColumns.Count == 1 && k.ForeignKeyMemberColumns[0].Name == column.Name);

    /// <summary>A foreign key's columns, paired: its own JSON name and the JSON name of the column it refers to.</summary>
    public static string Pairs(TableKeySchema key) =>
        "[" + string.Join(", ", key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select((c, i) =>
            "[" + Ts(Web.Json(key.ForeignKeyTable.Columns[c.Name])) + ", " +
            Ts(Web.Json(key.PrimaryKeyTable.Columns[key.PrimaryKeyMemberColumns[i].Name])) + "]")) + "]";

    /// <summary>Whether the UI follows a foreign key: both its tables are shown, and the table it refers to can be
    /// filtered by the columns it refers to (and the referring one by its own, for the rows that refer).</summary>
    public static bool Follows(TableKeySchema key, Func<TableSchema, bool> shown) =>
        shown(key.ForeignKeyTable) && shown(key.PrimaryKeyTable) &&
        key.ForeignKeyMemberColumns.Count == key.PrimaryKeyMemberColumns.Count &&
        key.PrimaryKeyMemberColumns.Cast<MemberColumnSchema>().All(c => Web.Filters(key.PrimaryKeyTable).Any(f => f.Name == c.Name)) &&
        key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().All(c => Web.Filters(key.ForeignKeyTable).Any(f => f.Name == c.Name));

    /// <summary>The foreign keys of every table that refer to this one: its rows' children.</summary>
    public static IEnumerable<TableKeySchema> Children(TableSchema table) =>
        table.Database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>()).Where(k => Same(k.PrimaryKeyTable, table));

    /// <summary>A child relationship's label: the referring table's (Items), or with its column where it refers twice
    /// (Orders by bill to).</summary>
    public static string ChildLabel(TableKeySchema key)
    {
        int twice = Children(key.PrimaryKeyTable).Count(k => Same(k.ForeignKeyTable, key.ForeignKeyTable));
        if (twice <= 1)
            return Title(key.ForeignKeyTable);
        string columns = string.Join(" and ", key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => Words(NameWords.Property(key.ForeignKeyTable.Columns[c.Name])).ToLowerInvariant()));
        return Title(key.ForeignKeyTable) + " by " + columns;
    }

    // ---- icons ----

    // a table's icon, by the words of its name (its last word first): one of the kit's icons
    private static readonly (string Pattern, string Icon)[] Icons =
    {
        ("^(status|state|log|history|event|audit|activity|change)", "activity"),
        ("^(account|user|member|customer|person|people|employee|contact|profile|author|client|owner|staff)", "user"),
        ("^(role|permission|right|group|team)", "shield"),
        ("^(order|invoice|receipt|sale|purchase|quote)", "receipt"),
        ("^(cart|basket|bag)", "cart"),
        ("^(product|merchandise|good)", "package"),
        ("^(item|sku|article|part|asset|unit)", "box"),
        ("^(categor|tag|type|genre|label|kind|class)", "tag"),
        ("^(supplier|vendor|shipper|carrier|delivery|shipment|shipping)", "truck"),
        ("^(inventor|stock|warehouse|bin|lot)", "layers"),
        ("^(address|location|city|countr|region|place|area|zone|territor)", "map-pin"),
        ("^(payment|card|transaction|billing|charge|refund)", "credit-card"),
        ("^(company|organi|org|department|store|shop|branch|office|business)", "building"),
        ("^(message|comment|review|note|post|feedback|mail)", "message"),
        ("^(application|app|setting|config|path|schema|version|option|preference|personali)", "settings"),
        ("^(event|calendar|schedule|appointment|booking|reservation)", "calendar"),
        ("^(file|document|attachment|image|photo|media)", "file"),
    };

    /// <summary>The table's icon in the navigation (user, receipt, package, tag, ...; table where none fits).</summary>
    public static string Icon(TableSchema table)
    {
        var words = Words(NameWords.Entity(table)).ToLowerInvariant().Split(' ').Where(w => w.Length > 0).ToList();
        if (words.Contains("line"))
            return "list"; // an order's lines
        foreach (string word in Enumerable.Reverse(words).Take(1).Concat(words))
            foreach (var (pattern, icon) in Icons)
                if (Regex.IsMatch(word, pattern))
                    return icon;
        return "table";
    }

    // ---- TypeScript ----

    /// <summary>A TypeScript string literal: "text", escaped.</summary>
    public static string Ts(string text)
    {
        var builder = new StringBuilder("\"");
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

    /// <summary>A list of names as TypeScript: ["productId", "name"].</summary>
    public static string Ts(IEnumerable<string> names) => "[" + string.Join(", ", names.Select(Ts)) + "]";

    /// <summary>The table's route, its name in the UI: products, line-items.</summary>
    public static string Route(TableSchema table) => Web.Route(table).Substring("/api/".Length);

    /// <summary>The variable the table's definition is: its entity in camel case (lineItem).</summary>
    public static string Variable(TableSchema table)
    {
        string entity = Names.Entity(table);
        string name = char.ToLowerInvariant(entity[0]) + entity.Substring(1);
        return name is "delete" or "default" or "function" or "class" or "new" or "this" or "import" or "export" or "var" or "let"
            or "const" or "enum" or "in" or "case" or "switch" or "with" or "yield" or "await" or "package" or "interface"
            or "public" or "private" or "protected" or "static" or "super" or "typeof" or "void" or "while" or "do" or "for"
            or "if" or "else" or "return" or "throw" or "try" or "catch" or "finally" or "break" or "continue" or "debugger"
            or "extends" or "implements" or "instanceof" or "null" or "true" or "false" or "api" ? name + "Table" : name;
    }

    /// <summary>Whether a table's name matches one of a list's patterns (aspnet_*, sysdiagrams), as its owner and name
    /// or its name alone; * and ? match any characters.</summary>
    public static bool Excluded(TableSchema table, string patterns) =>
        (patterns ?? "").Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Where(p => p.Length > 0)
            .Any(p =>
            {
                var regex = new Regex("^" + Regex.Escape(p).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
                return regex.IsMatch(table.Name) || regex.IsMatch(table.Owner + "." + table.Name);
            });
}
