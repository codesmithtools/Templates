// What the CSLA templates share: the object model a database gives (which tables are editable roots, which are children
// of which, their read-only lists and name/value lists), and the names, types and rules of what's generated. Names.cs
// (the entities' templates') names the EF Core side, and the business objects' names start from it.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CodeSmith.Engine;
using SchemaExplorer;

public sealed class Model
{
    private static readonly ConditionalWeakTable<DatabaseSchema, Dictionary<string, Model>> Models = new();

    /// <summary>A database's model with the master's settings: built once, for every template that asks.</summary>
    public static Model Of(DatabaseSchema database, string exclude, string children)
    {
        var models = Models.GetOrCreateValue(database);
        string key = (exclude ?? "") + "\n" + (children ?? "");
        lock (models)
        {
            if (!models.TryGetValue(key, out var model))
                models[key] = model = new Model(database, exclude ?? "", children ?? "");
            return model;
        }
    }

    private readonly string _exclude;
    private readonly List<Regex> _excluded;
    private readonly Dictionary<TableSchema, TableKeySchema> _parentKeys = new();
    private readonly Dictionary<TableSchema, string> _why = new();

    /// <summary>The tables that have business objects: those not excluded, with a primary key.</summary>
    public List<TableSchema> Tables { get; }

    /// <summary>What the model left out, and why (Model.md lists them).</summary>
    public List<string> Notes { get; } = new();

    private Model(DatabaseSchema database, string exclude, string children)
    {
        _exclude = exclude;
        _excluded = exclude.Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(glob => glob.Trim()).Where(glob => glob.Length > 0)
            .Select(glob => new Regex("^" + Regex.Escape(glob).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase))
            .ToList();

        Tables = new List<TableSchema>();
        foreach (TableSchema table in database.Tables)
        {
            if (Names.Excluded(table, exclude))
                Notes.Add($"`{table.Name}` is excluded (Exclude).");
            else if (!table.HasPrimaryKey)
                Notes.Add($"`{table.Name}` has no primary key: a business object couldn't save it.");
            else
            {
                foreach (MemberColumnSchema column in table.PrimaryKey.MemberColumns)
                    if (Excluded(table.Name + "." + column.Name))
                        throw new ArgumentException($"Exclude: {table.Name}.{column.Name} is in the table's primary key, which a business object needs.");
                Tables.Add(table);
            }
        }

        // the Children setting first ("Profiles: Account, Cart; ..."), then what the keys say
        foreach (string entry in children.Split(new[] { ';', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (entry.Trim().Length == 0)
                continue;
            string[] parts = entry.Split(':');
            if (parts.Length != 2)
                throw new ArgumentException($"Children: \"{entry.Trim()}\" isn't \"Parent: Child, Child\".");
            TableSchema parent = Find(parts[0].Trim(), "Children");
            foreach (string name in parts[1].Split(',').Select(n => n.Trim()).Where(n => n.Length > 0))
            {
                TableSchema child = Find(name, "Children");
                var key = ForeignKeys(child).FirstOrDefault(k => Same(k.PrimaryKeyTable, parent) && Names.RefersToPrimaryKey(k))
                    ?? throw new ArgumentException($"Children: {child.Name} has no foreign key to the primary key of {parent.Name}.");
                if (Same(child, parent) || AncestorsOf(parent).Any(t => Same(t, child)))
                    throw new ArgumentException($"Children: {child.Name} can't be a child of {parent.Name}, which is its own child.");
                _parentKeys[child] = key;
                _why[child] = "the Children setting";
            }
        }
        foreach (TableSchema table in Tables.Where(t => !_parentKeys.ContainsKey(t)))
        {
            // a child's key starts with its parent's (identifying: the longest such key, OrderLine's before Order's), or
            // its parent's deletes cascade to it; in the table's column order, the same from every provider (theirs
            // differ: by the keys' names, or as they were made)
            var candidates = ForeignKeys(table)
                .Where(k => !Same(k.PrimaryKeyTable, table) && Tables.Any(t => Same(t, k.PrimaryKeyTable)) && Names.RefersToPrimaryKey(k))
                .Select(k => (Key: k, Identifying: Identifying(k), Cascades: Cascades(k)))
                .Where(c => c.Identifying || c.Cascades)
                .OrderBy(c => c.Identifying ? 0 : 1)
                .ThenByDescending(c => c.Identifying ? c.Key.ForeignKeyMemberColumns.Count : 0);
            foreach (var candidate in candidates)
            {
                if (AncestorsOf(candidate.Key.PrimaryKeyTable).Any(t => Same(t, table)))
                    continue;
                _parentKeys[table] = candidate.Key;
                _why[table] = candidate.Identifying
                    ? $"its key starts with {Possessive(candidate.Key.PrimaryKeyTable.Name)}"
                    : $"deleting a {candidate.Key.PrimaryKeyTable.Name} row deletes its rows";
                break;
            }
        }
    }

    private TableSchema Find(string name, string setting) =>
        Tables.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase) || string.Equals(Names.Entity(t), name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"{setting}: there's no table {name} (or it's excluded, or has no primary key).");

    private static bool Same(TableSchema a, TableSchema b) => a.Owner == b.Owner && a.Name == b.Name;

    // A key of the primary key's first columns, in the table's order (GroupMember's GroupId of GroupId, CustomerId; not
    // its CustomerId), and not all of them (all of them is a one-to-one extension).
    private static bool Identifying(TableKeySchema key)
    {
        var primary = Names.KeyColumns(key.ForeignKeyTable).Select(c => c.Name).ToList();
        var columns = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => c.Name).ToList();
        return columns.Count < primary.Count && new HashSet<string>(columns).SetEquals(primary.Take(columns.Count));
    }

    /// <summary>A table's foreign keys in the order of their columns in the table (then their names): a provider gives
    /// them in its own order (PostgreSQL's by name, MySQL's as they were made), which would change which is a child's
    /// parent and which fetch a list has.</summary>
    public static List<TableKeySchema> ForeignKeys(TableSchema table) =>
        table.ForeignKeys.Cast<TableKeySchema>()
            .OrderBy(k => string.Join(",", k.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns.IndexOf(table.Columns[c.Name]).ToString("D4"))), StringComparer.Ordinal)
            .ThenBy(k => k.Name, StringComparer.Ordinal)
            .ToList();

    private static bool Cascades(TableKeySchema key) =>
        bool.TryParse(Convert.ToString(key.ExtendedProperties["CS_CascadeDelete"]?.Value), out bool cascades) && cascades;

    private bool Excluded(string name) => _excluded.Any(r => r.IsMatch(name));

    // ---- the object model ----

    /// <summary>Whether a table is a child: an editable child in its parent's list, saved with its root.</summary>
    public bool IsChild(TableSchema table) => _parentKeys.ContainsKey(Table(table));

    /// <summary>A child's foreign key to its parent.</summary>
    public TableKeySchema ParentKey(TableSchema table) => _parentKeys[Table(table)];

    /// <summary>A child's parent's table.</summary>
    public TableSchema Parent(TableSchema table) => Table(ParentKey(table).PrimaryKeyTable);

    /// <summary>The tables whose rows are a table's children, in the database's order.</summary>
    public List<TableSchema> ChildrenOf(TableSchema table) =>
        Tables.Where(t => _parentKeys.TryGetValue(t, out var key) && Same(key.PrimaryKeyTable, table)).ToList();

    /// <summary>Every table below a table: its children, theirs, and so on (deepest last).</summary>
    public List<TableSchema> DescendantsOf(TableSchema table)
    {
        var descendants = new List<TableSchema>();
        foreach (var child in ChildrenOf(table))
        {
            descendants.Add(child);
            descendants.AddRange(DescendantsOf(child));
        }
        return descendants;
    }

    /// <summary>A child's parent, its parent's parent, and so on to the root.</summary>
    public List<TableSchema> AncestorsOf(TableSchema table)
    {
        var ancestors = new List<TableSchema>();
        for (var t = Table(table); t != null && _parentKeys.TryGetValue(t, out var key); t = Table(key.PrimaryKeyTable))
        {
            if (ancestors.Any(a => Same(a, key.PrimaryKeyTable)))
                break;
            ancestors.Add(Table(key.PrimaryKeyTable));
        }
        return ancestors;
    }

    /// <summary>Why a table is a child (Model.md).</summary>
    public string WhyChild(TableSchema table) => _why[Table(table)];

    /// <summary>The model's own instance of a table (a key's tables are other instances of the same table).</summary>
    public TableSchema Table(TableSchema table) => Tables.FirstOrDefault(t => Same(t, table)) ?? table;

    /// <summary>The columns a table's objects have: the entity's (one with a .NET type) but the excluded.</summary>
    public List<ColumnSchema> Columns(TableSchema table) =>
        Names.Columns(table).Where(c => !Excluded(table.Name + "." + c.Name)).ToList();

    /// <summary>The primary key's columns.</summary>
    public static List<ColumnSchema> Key(TableSchema table) =>
        table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns[c.Name]).ToList();

    /// <summary>A child's columns that hold its parent's key: the parent sets them.</summary>
    public bool FromParent(ColumnSchema column) =>
        IsChild(column.Table) && ParentKey(column.Table).ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Any(c => c.Name == column.Name);

    /// <summary>The parent's column a child's column takes its value from.</summary>
    public ColumnSchema ParentColumn(ColumnSchema column)
    {
        var key = ParentKey(column.Table);
        int index = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().ToList().FindIndex(c => c.Name == column.Name);
        return Parent(column.Table).Columns[key.PrimaryKeyMemberColumns[index].Name];
    }

    /// <summary>A column the database fills in: an identity, a computed column, a rowversion.</summary>
    public static bool Generated(ColumnSchema column) =>
        Names.IsRowVersion(column) || Names.IsComputed(column) || Identity(column);

    /// <summary>A column the database numbers (an identity): a new object's is 0 until it's saved.</summary>
    public static bool Identity(ColumnSchema column) => Flag(column, "CS_IsIdentity");

    private static bool Flag(ColumnSchema column, string name) =>
        bool.TryParse(Convert.ToString(column.ExtendedProperties[name]?.Value), out bool value) && value;

    /// <summary>A column the object's user sets: not the database's, nor the parent's.</summary>
    public bool Settable(ColumnSchema column) => !Generated(column) && !FromParent(column);

    /// <summary>A key column the application sets: it can change until the object's saved.</summary>
    public bool KeySetOnce(ColumnSchema column) => column.IsPrimaryKeyMember && Settable(column);

    // ---- read-only lists and name/value lists ----

    /// <summary>A read-only list's criteria: a foreign key's columns ("ByCategory", categoryId).</summary>
    public sealed class Criteria
    {
        public TableKeySchema Key;
        public string Name;
        public List<ColumnSchema> Columns;
    }

    /// <summary>A root's read-only list's fetches by a foreign key (all rows is the other), each with its own types.</summary>
    public List<Criteria> ListCriteria(TableSchema table)
    {
        var criteria = new List<Criteria>();
        var signatures = new HashSet<string> { "" };
        var included = Columns(table);
        foreach (TableKeySchema key in ForeignKeys(table))
        {
            var columns = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Select(c => table.Columns[c.Name]).ToList();
            if (columns.Any(c => !included.Contains(c)))
                continue;
            string signature = string.Join(",", columns.Select(ParameterType));
            string name = "By" + Names.Entity(key.PrimaryKeyTable);
            if (table.ForeignKeys.Cast<TableKeySchema>().Count(k => Same(k.PrimaryKeyTable, key.PrimaryKeyTable)) > 1)
                name = "By" + string.Concat(columns.Select(Property));
            if (!signatures.Add(signature))
            {
                Notes.Add($"`{ListName(table)}` has no fetch {name}: another fetch takes the same types ({signature}), and the data portal tells them apart by type.");
                continue;
            }
            criteria.Add(new Criteria { Key = key, Name = name, Columns = columns });
        }
        return criteria;
    }

    /// <summary>Whether a root has a name/value list: another table refers to it (a lookup), by its single-column key,
    /// and it has a column to show.</summary>
    public bool HasNameValueList(TableSchema table) =>
        !IsChild(table) && table.PrimaryKey.MemberColumns.Count == 1 && DisplayColumn(table) != null &&
        table.Database.Tables.Cast<TableSchema>().SelectMany(t => t.ForeignKeys.Cast<TableKeySchema>())
            .Any(k => Same(k.PrimaryKeyTable, table) && !(Tables.Any(t => Same(t, k.ForeignKeyTable)) && IsChild(k.ForeignKeyTable) && Same(Parent(k.ForeignKeyTable), table)));

    private static readonly Regex DisplayNames = new("^(name|title|display_?name|description|user_?name|login|code|label)$", RegexOptions.IgnoreCase);

    /// <summary>The column a name/value list shows: a text column called Name, Title, ..., or the first text column.</summary>
    public ColumnSchema DisplayColumn(TableSchema table)
    {
        var text = Columns(table).Where(c => !c.IsPrimaryKeyMember && Names.Type(c).TrimEnd('?') == "string").ToList();
        return text.FirstOrDefault(c => DisplayNames.IsMatch(c.Name)) ?? text.FirstOrDefault();
    }

    // ---- names ----

    /// <summary>The editable object: the entity's name (Orders: Order).</summary>
    public static string Class(TableSchema table) => Names.Entity(table);

    /// <summary>A child's list: plural (LineItem: LineItems).</summary>
    public static string ChildList(TableSchema table) =>
        GiveWay(table, Names.Set(table) != Names.Entity(table) ? Names.Set(table) : Names.Entity(table) + "Collection", Names.Entity(table) + "Collection");

    /// <summary>The parent's property that holds a child's list: the entity's navigation back, or the list's name where
    /// that's singular (a Plural-Overrides entry that keeps a word as it is, as "status" is).</summary>
    public string ChildListProperty(TableSchema table) =>
        Names.Inverse(ParentKey(table), _exclude) == Names.Entity(table) ? ChildList(table) : Names.Inverse(ParentKey(table), _exclude);

    // The names of the types each object has (OrderInfo, OrderList, OrderDto, ...) give way to a table's: a ProductInfo
    // table's object is ProductInfo, and Product's read-only object is ProductReadOnlyInfo. A name taken twice would be
    // one file written twice, and a type the other's code can't tell from its own.
    public static string InfoName(TableSchema table) => GiveWay(table, Class(table) + "Info", Class(table) + "ReadOnlyInfo");
    public static string ListName(TableSchema table) => GiveWay(table, Class(table) + "List", Class(table) + "ReadOnlyList");
    public static string CountName(TableSchema table) => GiveWay(table, ListName(table) + "Count", Class(table) + "RowCount");
    public static string NameValueListName(TableSchema table) => GiveWay(table, Class(table) + "NameValueList", Class(table) + "Lookup");
    public static string Dto(TableSchema table) => GiveWay(table, Class(table) + "Dto", Class(table) + "RowDto");
    public static string DalInterface(TableSchema table) => GiveWay(table, "I" + Class(table) + "Dal", "I" + Class(table) + "DataAccess");
    public static string Dal(TableSchema table) => GiveWay(table, Class(table) + "Dal", Class(table) + "EfDal");

    /// <summary>A type's name, or another where a table's class (its entity's) has it: the table's name wins.</summary>
    public static string GiveWay(TableSchema table, string name, string instead)
    {
        var classes = Classes.GetValue(table.Database.Tables, tables => new HashSet<string>(tables.Cast<TableSchema>().Select(Names.Entity)));
        if (name == Class(table) || !classes.Contains(name))
            return name;
        string other = instead;
        for (int i = 2; classes.Contains(other); i++)
            other = instead + i;
        return other;
    }

    private static readonly ConditionalWeakTable<TableSchemaCollection, HashSet<string>> Classes = new();

    // CSLA's own members, which a property can't be
    private static readonly HashSet<string> Reserved = new()
    {
        "ApplicationContext", "BrokenRulesCollection", "BusinessRules", "EditLevel", "FieldManager", "IsBusy", "IsChild",
        "IsDeleted", "IsDirty", "IsNew", "IsReadOnly", "IsSavable", "IsSelfBusy", "IsSelfDirty", "IsSelfValid", "IsValid",
        "Parent", "Identity", "DeletedList", "Count", "Item",
    };

    /// <summary>A column's property: the entity's (Names.Property), with Value after a name CSLA has.</summary>
    public static string Property(ColumnSchema column)
    {
        string name = Names.Property(column);
        return Reserved.Contains(name) ? name + "Value" : name;
    }

    /// <summary>A property's PropertyInfo: OrderIdProperty.</summary>
    public static string Info(ColumnSchema column) => Property(column) + "Property";

    /// <summary>A property's type: the entity's, but a text column's is a string either way (CSLA's default is "",
    /// for data binding; "" saves as null where the column takes it), and bytes may be null. An array (PostgreSQL's
    /// text[], int[]) is a read-only list, never null (CSLA keeps it as a MobileList, which its n-level undo can copy; an
    /// array it can't); and a bit string (bit(8)), a string of 0s and 1s.</summary>
    public static string Type(ColumnSchema column)
    {
        if (IsList(column))
            return "IReadOnlyList<" + ElementType(column) + ">";
        if (IsBits(column))
            return "string";
        string type = Names.Type(column);
        return type == "string?" ? "string" : type == "byte[]" ? "byte[]?" : type;
    }

    /// <summary>The PropertyInfo's type: the property's, but a list's is CSLA's MobileList.</summary>
    public static string FieldType(ColumnSchema column) => IsList(column) ? "Csla.Core.MobileList<" + ElementType(column) + ">" : Type(column);

    /// <summary>A property's value as its field has it: a list's as a MobileList.</summary>
    public static string ToField(ColumnSchema column, string value) => IsList(column) ? "new " + FieldType(column) + "(" + value + ")" : value;

    /// <summary>An array column (PostgreSQL's), but bytes.</summary>
    public static bool IsList(ColumnSchema column) =>
        column.GetClrType(TypeMapping.Modern) is { IsArray: true } type && type != typeof(byte[]);

    /// <summary>A bit string of more than one bit (PostgreSQL's bit(8), varbit): EF Core's BitArray.</summary>
    public static bool IsBits(ColumnSchema column) => column.GetClrType(TypeMapping.Modern) == typeof(System.Collections.BitArray);

    /// <summary>An array's item type: string, int, ...</summary>
    public static string ElementType(ColumnSchema column)
    {
        string type = Names.Type(column).TrimEnd('?');
        return type.EndsWith("[]") ? type.Substring(0, type.Length - 2) : type;
    }

    /// <summary>What GetProperty and ReadProperty need after them: "!" for a string or a list (CSLA's P? is never null
    /// for one).</summary>
    public static string NotNull(ColumnSchema column) => Type(column) == "string" || IsList(column) ? "!" : "";

    /// <summary>ReadProperty(OrderIdProperty), for the data portal's operations (no authorization rule hides it).</summary>
    public static string Read(ColumnSchema column) => "ReadProperty(" + Info(column) + ")" + NotNull(column);

    /// <summary>A DTO property's type: the column's, nullable where it is.</summary>
    public static string DtoType(ColumnSchema column) => Names.Type(column);

    /// <summary>A criteria parameter's type: the column's, never null.</summary>
    public static string ParameterType(ColumnSchema column) => Names.Type(column).TrimEnd('?');

    /// <summary>A column's parameter: its property in camel case (OrderId: orderId).</summary>
    public static string Parameter(ColumnSchema column)
    {
        string name = Property(column);
        name = char.ToLowerInvariant(name[0]) + name.Substring(1);
        return Keywords.Contains(name) ? "@" + name : name;
    }

    /// <summary>Parameters for columns: "long orderId, int lineNum".</summary>
    public static string Parameters(IEnumerable<ColumnSchema> columns) => string.Join(", ", columns.Select(c => ParameterType(c) + " " + Parameter(c)));

    /// <summary>Arguments for columns: "orderId, lineNum".</summary>
    public static string Arguments(IEnumerable<ColumnSchema> columns) => string.Join(", ", columns.Select(Parameter));

    /// <summary>A property's display name: its words (ShipAddr1: "Ship addr 1", UniqueID: "Unique ID"; listprice too:
    /// "List price", NameWords.cs).</summary>
    public static string Friendly(ColumnSchema column)
    {
        var words = Regex.Split(NameWords.Property(column), "(?<=[a-z])(?=[A-Z0-9])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[0-9])(?=[A-Za-z])")
            .Where(w => w.Length > 0).ToList();
        for (int i = 0; i < words.Count; i++)
            if (words[i] == "Id")
                words[i] = "ID";
            else if (i > 0 && words[i].Any(char.IsLower))
                words[i] = words[i].ToLowerInvariant();
        return string.Join(" ", words);
    }

    /// <summary>A property's attributes: its display name, and the rules its column gives (Required, StringLength,
    /// Range for a decimal's precision). CSLA makes rules of them.</summary>
    public List<string> Attributes(ColumnSchema column)
    {
        var attributes = new List<string> { $"Display(Name = \"{Friendly(column)}\")" };
        if (!Settable(column))
            return attributes;
        string type = Names.Type(column).TrimEnd('?');
        if (IsBits(column))
        {
            // bit(8) takes 8 of them, varbit(8) up to 8
            string count = column.NativeType == "bit" ? "{" + Math.Max(1, column.Size) + "}" : column.Size > 0 ? "{0," + column.Size + "}" : "*";
            string what = column.NativeType == "bit" ? Math.Max(1, column.Size) + " bits" : column.Size > 0 ? "up to " + column.Size + " bits" : "bits";
            if (!column.AllowDBNull)
                attributes.Add("Required");
            attributes.Add($"RegularExpression(\"^([01]{count})?$\", ErrorMessage = \"{{0}} is {what}: 0s and 1s.\")");
            return attributes;
        }
        if (type == "string" && !column.AllowDBNull)
            attributes.Add("Required");
        if (type == "string" && column.Size > 0 && column.Size < 1_000_000)
            attributes.Add($"StringLength({column.Size})");
        if (type == "decimal" && column.Precision > 0 && column.Precision <= 28 && column.Scale >= 0 && column.Scale < column.Precision)
        {
            string max = new string('9', column.Precision - column.Scale) + (column.Scale > 0 ? "." + new string('9', column.Scale) : "");
            attributes.Add($"Range(typeof(decimal), \"-{max}\", \"{max}\", ParseLimitsInInvariantCulture = true)");
        }
        return attributes;
    }

    /// <summary>What a DTO's property is given from the object: a text column that takes null gets null for "", and a
    /// list or bit string that does, null for none; a list is an array, a bit string a BitArray.</summary>
    public static string ToDto(ColumnSchema column, string value)
    {
        string type = Names.Type(column);
        if (IsList(column))
            return column.AllowDBNull ? $"{value}.Count == 0 ? null : {value}.ToArray()" : $"{value}.ToArray()";
        if (IsBits(column))
            return (column.AllowDBNull ? $"{value} is \"\" ? null : " : "") + $"new System.Collections.BitArray({value}.Select(bit => bit == '1').ToArray())";
        if (type == "string?")
            return $"{value} is \"\" ? null : {value}";
        if (type == "byte[]")
            return $"{value} ?? []";
        return value;
    }

    /// <summary>What a property loads from a DTO's: "" for a text column's null, no items for a list's, and a bit
    /// string's 0s and 1s.</summary>
    public static string FromDto(ColumnSchema column, string value)
    {
        if (IsList(column))
            return column.AllowDBNull ? value + " ?? []" : value;
        if (IsBits(column))
            return (column.AllowDBNull ? $"{value} is null ? \"\" : " : "") + $"string.Concat({value}.Cast<bool>().Select(bit => bit ? '1' : '0'))";
        return Names.Type(column) == "string?" ? value + " ?? \"\"" : value;
    }

    private static readonly HashSet<string> Keywords = new()
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
        "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
        "using", "virtual", "void", "volatile", "while",
    };

    /// <summary>A name's possessive: Orders', Order's.</summary>
    public static string Possessive(string name) => name.EndsWith("s") ? name + "'" : name + "'s";

    /// <summary>A sentence's list: "A", "A and B", "A, B and C".</summary>
    public static string And(IEnumerable<string> items)
    {
        var list = items.ToList();
        return list.Count <= 1 ? string.Concat(list) : string.Join(", ", list.Take(list.Count - 1)) + " and " + list[list.Count - 1];
    }

    /// <summary>The code's comment for a table: "main.Orders" without SQLite's main.</summary>
    public static string Source(TableSchema table) => string.IsNullOrEmpty(Names.Schema(table)) ? table.Name : Names.Schema(table) + "." + table.Name;

    /// <summary>A C# string literal.</summary>
    public static string Literal(string value) => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

    /// <summary>The name of the DI method: AddPetShopDal (the namespace without its dots).</summary>
    public static string ServicesName(string ns) => string.Concat(ns.Split('.').Select(p => p.Length == 0 ? p : char.ToUpperInvariant(p[0]) + p.Substring(1)));
}
