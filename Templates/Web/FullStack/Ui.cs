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

    /// <summary>A name in words, in sentence case: ShipAddr1 is "Ship address 1", ProductId "Product ID" (NameWords.Label,
    /// which Csla's labels use too).</summary>
    public static string Words(string name) => NameWords.Label(name);

    /// <summary>A table's name in the navigation and its pages' titles: its set in words (Line items; lineitem's too,
    /// NameWords.cs).</summary>
    public static string Title(TableSchema table) => Words(NameWords.Set(table));

    /// <summary>One of its rows: its entity in words (Line item).</summary>
    public static string Singular(TableSchema table) => Words(NameWords.Entity(table));

    /// <summary>A column's label: its name in words (listprice too: List price, NameWords.cs); a bool's without its Is
    /// (IsShoppingCart: Shopping cart). A foreign key's is the table it refers to (UniqueID refers to Profiles:
    /// Profile), where its column is named for it (ProductId, GroupId to Group's Id) and it's the only one to it;
    /// else its column's, without its ID (ParentId: Parent, ReferredBy: Referred by).</summary>
    public static string Label(ColumnSchema column)
    {
        var key = Reference(column);
        if (key != null)
        {
            if (NamedFor(column, key) && column.Table.ForeignKeys.Cast<TableKeySchema>().Count(k => Same(k.PrimaryKeyTable, key.PrimaryKeyTable)) == 1)
                return Singular(key.PrimaryKeyTable);
            string words = NameWords.Label(column);
            return words.EndsWith(" ID", StringComparison.Ordinal) && words.Length > 3 ? words.Substring(0, words.Length - 3) : words;
        }
        return NameWords.Label(column);
    }

    // Whether a foreign key's column is named for the table it refers to: as the column it refers to (ProductId), or
    // the table's name and that column's or an ID (GroupId to Group's Id; CategoryCode to Category's Code).
    private static bool NamedFor(ColumnSchema column, TableKeySchema key)
    {
        string Plain(string name) => Regex.Replace(name ?? "", "[^A-Za-z0-9]", "").ToLowerInvariant();
        string own = Plain(column.Name), referred = Plain(key.PrimaryKeyMemberColumns[0].Name);
        if (own == referred)
            return true;
        foreach (string table in new[] { Plain(NameWords.Entity(key.PrimaryKeyTable)), Plain(key.PrimaryKeyTable.Name), Plain(Names.Entity(key.PrimaryKeyTable)) })
            if (own.StartsWith(table, StringComparison.Ordinal) && own.Substring(table.Length) is var rest &&
                (rest == referred || rest is "id" or "key" or "code" or "no" or "num"))
                return true;
        return false;
    }

    // ---- kinds of input ----

    private static readonly Regex Money = new Regex("(price|cost|amount|total|balance|salary|fee|subtotal|tax|payment|charge|revenue)$", RegexOptions.IgnoreCase);
    private static readonly Regex LongText = new Regex("(desc|descn|description|comment|comments|note|notes|body|details|summary|bio|about|message|content|text|remarks)$", RegexOptions.IgnoreCase);

    /// <summary>How a column is shown and edited: text, multiline, email, phone, url, integer, decimal, money, boolean,
    /// date, datetime, time, uuid, binary, json, bits, list, enum or set.</summary>
    public static string Kind(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        string native = Web.Native(column);
        string name = Names.Property(column);
        // bit strings (text of 0s and 1s), arrays (a list), enums (a select) and MySQL's SETs (some of its values)
        if (Web.BitString(column) != null)
            return "bits";
        if (Web.IsArray(column))
            return "list";
        if (Web.IsSet(column))
            return "set";
        if (Web.EnumValues(column) != null)
            return "enum";
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

    /// <summary>A column whose few values read as a badge: an enum's, or a short Status, Type, Kind, Level, Priority or
    /// Stage.</summary>
    public static bool Badge(ColumnSchema column) =>
        Kind(column) == "enum" ||
        Names.Type(column).TrimEnd('?') == "string" && column.Size > 0 && column.Size <= 20 && Kind(column) is not ("set" or "list") &&
        Regex.IsMatch(Names.Property(column), "(Status|Type|Kind|Level|Priority|Stage|Category|Role)$") && !column.IsPrimaryKeyMember && !column.IsForeignKeyMember;

    /// <summary>The element's kind of a list (an array): text, integer, decimal, boolean, uuid, date, datetime or time.</summary>
    public static string Items(ColumnSchema column)
    {
        switch (Web.Element(column))
        {
            case "bool": return "boolean";
            case "byte": case "sbyte": case "short": case "ushort": case "int": case "uint": case "long": case "ulong": return "integer";
            case "float": case "double": case "decimal": return "decimal";
            case "Guid": return "uuid";
            case "DateOnly": return "date";
            case "DateTime": case "DateTimeOffset": return "datetime";
            case "TimeOnly": case "TimeSpan": return "time";
            default: return "text";
        }
    }

    /// <summary>A date and time that's an instant (PostgreSQL's timestamptz, MySQL's TIMESTAMP, SQL Server's
    /// datetimeoffset): the API sends it in UTC (or with its offset), and the form shows and sends it in the browser's
    /// time.</summary>
    public static bool Zoned(ColumnSchema column) =>
        Kind(column) == "datetime" && (Names.Type(column).TrimEnd('?') == "DateTimeOffset" || Web.Native(column) is "timestamptz" or "timestamp with time zone" ||
            Web.Native(column) == "timestamp" && Web.Provider(column.Table.Database) == "MySQLSchemaProvider");

    /// <summary>The type a field shows: the database's, with its size (varchar(80), numeric(10,2)); an enum's or a SET's
    /// as enum or set (MySQL's) or its own name (PostgreSQL's).</summary>
    public static string Type(ColumnSchema column)
    {
        if (Web.EnumValues(column) != null && Web.Native(column) is "enum" or "set")
            return Web.Native(column);
        if (Web.BitString(column) != null)
            return Web.Native(column) + (Web.BitLength(column) > 0 ? "(" + Web.BitLength(column) + ")" : "");
        return column.NativeType + (Web.Validation(column) != null ? "(" + column.Size + ")" : Exact(column) && Web.Native(column) is "decimal" or "numeric" ? "(" + column.Precision + "," + column.Scale + ")" : "");
    }

    /// <summary>A column that reads better in a monospaced font: a key's, a GUID, bytes, JSON, bits.</summary>
    public static bool Mono(ColumnSchema column) =>
        column.IsPrimaryKeyMember || column.IsForeignKeyMember || Kind(column) is "uuid" or "binary" or "json" or "bits";

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

    /// <summary>What a new row starts with: the column's default, where it's a value (a number, text, true or false,
    /// an enum's value, a list) or the time ("now"); null otherwise (the database gives a new row the rest).</summary>
    public static string Default(ColumnSchema column)
    {
        string text = Web.Default(column);
        if (text == null || Web.Generated(column))
            return null;
        while (text.Length > 1 && text[0] == '(' && text[text.Length - 1] == ')')
            text = text.Substring(1, text.Length - 2).Trim();
        text = Regex.Replace(text, @"::[\w\s""-]+(\[\])?$", "").Trim(); // PostgreSQL's 'en-US'::character varying
        string kind = Kind(column);
        string lower = text.ToLowerInvariant();
        if (kind is "date" or "datetime" or "time" &&
            Regex.IsMatch(lower, @"^(getdate|getutcdate|sysdatetime|sysutcdatetime|sysdatetimeoffset|now|current_timestamp|current_date|current_time|localtimestamp|localtime|datetime\('now'\)|date\('now'\))(\(\d*\))?$"))
            return "\"now\"";
        // MySQL's literal default is as it is (free, 1, 0.00); an expression is DEFAULT_GENERATED (uuid())
        if (Web.Provider(column.Table.Database) == "MySQLSchemaProvider" && !text.StartsWith("'") &&
            (Convert.ToString(column.ExtendedProperties["CS_ColumnExtra"]?.Value) ?? "").IndexOf("DEFAULT_GENERATED", StringComparison.OrdinalIgnoreCase) < 0)
            text = "'" + text.Replace("'", "''") + "'";
        if (text.StartsWith("N'") || text.StartsWith("E'"))
            text = text.Substring(1); // SQL Server's N'text', PostgreSQL's E'text'
        if (text.Length >= 2 && text[0] == '\'' && text[text.Length - 1] == '\'')
        {
            string value = text.Substring(1, text.Length - 2).Replace("''", "'");
            switch (kind)
            {
                case "boolean": return value is "1" or "true" or "t" or "b'1'" ? "true" : value is "0" or "false" or "f" or "b'0'" ? "false" : null;
                case "integer": case "decimal": case "money":
                    return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal quoted) ? quoted.ToString(CultureInfo.InvariantCulture) : null;
                case "text": case "multiline": case "email": case "phone": case "url": case "bits": return Ts(value);
                case "enum": return Web.EnumValues(column).Contains(value) ? Ts(value) : null;
                case "set": return value.Split(',').All(Web.EnumValues(column).Contains) || value == "" ? Ts(value) : null;
                case "list": return Web.ArrayDefault(column);
                default: return null;
            }
        }
        if (kind == "boolean")
            return lower is "1" or "true" or "b'1'" ? "true" : lower is "0" or "false" or "b'0'" ? "false" : null;
        if (kind is "integer" or "decimal" or "money" && decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
            return number.ToString(CultureInfo.InvariantCulture);
        return null;
    }

    /// <summary>Whether a new row must be given a column: one that can't be null, the database doesn't fill in, and
    /// has no default (of any kind: now() too).</summary>
    public static bool Required(ColumnSchema column) => !column.AllowDBNull && !Web.Generated(column) && !Web.Optional(column);

    // ---- rows ----

    private static readonly string[] DisplayNames = { "name", "title", "display_name", "displayname", "label", "user_name", "username", "login", "full_name", "fullname" };

    /// <summary>The columns a row is shown by: a Name or a Title (or the like), a first and last name, an Email, a
    /// unique text column (a Key, a Code); none for a row that's best shown by its key (an order: #1001).</summary>
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
        if (email != null)
            return new List<ColumnSchema> { email };
        // a unique text column (Group's Key), as Csla's lists show it: a Key, a Code or the like, else the first
        var unique = text.Where(c => Kind(c) is "text" or "email" or "url" or "phone" && table.Indexes.Cast<IndexSchema>()
            .Any(i => i.IsUnique && i.MemberColumns.Count == 1 && i.MemberColumns[0].Name == c.Name)).ToList();
        var shown = unique.FirstOrDefault(c => Regex.IsMatch(c.Name, "^(key|code|slug|handle|sku|number|ref|reference|login|email)$", RegexOptions.IgnoreCase))
            ?? unique.FirstOrDefault();
        return shown != null ? new List<ColumnSchema> { shown } : new List<ColumnSchema>();
    }

    /// <summary>A list's columns: the key, what the row is shown by, then the foreign keys, badges, money and dates, and
    /// the rest (not long text, bytes or JSON), six at most, in the table's order after the first two.</summary>
    public static List<ColumnSchema> Columns(TableSchema table, int most = 6)
    {
        var all = Names.Columns(table).Where(c => !Names.IsRowVersion(c) && Kind(c) is not ("binary" or "json")).ToList();
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
    public static string Ts(string text) => Web.Ts(text);

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
            or "extends" or "implements" or "instanceof" or "null" or "true" or "false" or "api" or "kit" or "database" or "tables" or "routes"
            ? name + "Table" : name;
    }

    /// <summary>Whether a table is left out (Exclude: aspnet_*, sysdiagrams): Web.Excluded.</summary>
    public static bool Excluded(TableSchema table, string patterns) => Web.Excluded(table, patterns);
}
