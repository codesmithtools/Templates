// What the test data templates know: a Bogus rule for each column's values, the rows a table's foreign keys refer to,
// and the builders' and tests' names. Names.cs (the Dapper templates') has the records' names and the SQL.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeSmith.Engine;
using SchemaExplorer;

public static class Fakes
{
    // ---------------------------------------------------------------- names

    /// <summary>The builder's class: OrderBuilder.</summary>
    public static string Builder(TableSchema table) => Names.Entity(table) + "Builder";

    /// <summary>The builder's property on TestData: the record's plural (Orders, Categories).</summary>
    public static string Builders(TableSchema table) => StringUtil.ToPlural(Names.Entity(table));

    /// <summary>The test class: OrderRepositoryTests.</summary>
    public static string Tests(TableSchema table) => Names.Repository(table) + "Tests";

    /// <summary>The lambdas' parameter: the record's first letter (o => o.OrderDate); x where that's f, which the rules
    /// use for the Faker.</summary>
    public static string Lambda(TableSchema table)
    {
        char first = char.ToLowerInvariant(Names.Entity(table)[0]);
        return first >= 'a' && first <= 'z' && first != 'f' ? first.ToString() : "x";
    }

    // ---------------------------------------------------------------- what the repository can do

    /// <summary>Whether the repository has InsertAsync (Repository.cst): a primary key, and a column to write.</summary>
    public static bool CanInsert(TableSchema table) =>
        Names.Key(table).Count > 0 && table.Columns.Cast<ColumnSchema>().Any(Names.Written);

    /// <summary>Whether the repository has UpdateAsync: a column to write outside the primary key.</summary>
    public static bool CanUpdate(TableSchema table) =>
        Names.Key(table).Count > 0 && table.Columns.Cast<ColumnSchema>().Any(c => Names.Written(c) && !c.IsPrimaryKeyMember);

    /// <summary>The columns an insert writes, which a test checks were saved as they were given.</summary>
    public static List<ColumnSchema> Written(TableSchema table) => table.Columns.Cast<ColumnSchema>().Where(Names.Written).ToList();

    // ---------------------------------------------------------------- foreign keys

    /// <summary>A foreign key of a table: the row it refers to, and how a new row gets one.</summary>
    public sealed class Parent
    {
        public TableKeySchema Key { get; set; }

        /// <summary>The table it refers to.</summary>
        public TableSchema Table { get; set; }

        /// <summary>Each of this table's columns, with the column of the parent's it holds.</summary>
        public List<(ColumnSchema Column, ColumnSchema Referenced)> Columns { get; set; }

        /// <summary>The parameter that gives one: order.</summary>
        public string Parameter { get; set; }

        /// <summary>The parent's table leads back to this one (a manager who's an employee): a new row takes a row
        /// that's there, or none, and never makes one (which would make another, and so on).</summary>
        public bool Cyclic { get; set; }

        /// <summary>Its columns are one of the table's unique keys (a row per parent): a new row always gets a new
        /// parent, even with ReuseParents.</summary>
        public bool OnePerParent { get; set; }

        /// <summary>Its columns can all be null.</summary>
        public bool Nullable { get; set; }
    }

    /// <summary>The table's foreign keys to the database's tables, in the schema's order.</summary>
    public static List<Parent> Parents(TableSchema table)
    {
        var tables = new HashSet<string>(table.Database.Tables.Cast<TableSchema>().Select(t => t.FullName));
        var keys = table.ForeignKeys.Cast<TableKeySchema>()
            .Where(k => tables.Contains(k.PrimaryKeyTable.FullName) && k.ForeignKeyMemberColumns.Count == k.PrimaryKeyMemberColumns.Count)
            .ToList();
        var parents = new List<Parent>();
        // names the builder's methods use themselves: their parameters, locals and fields, and the row's
        var taken = new HashSet<string> { "change", "cancellationToken", "data", "random", "repository", "rows", Names.Parameter(Names.Entity(table)) };
        if (Names.GeneratedKey(table) is ColumnSchema newKey)
            taken.Add(Names.Parameter(Names.Property(newKey)));
        foreach (var key in keys)
        {
            var columns = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().Zip(key.PrimaryKeyMemberColumns.Cast<MemberColumnSchema>(),
                (c, r) => (table.Columns[c.Name], key.PrimaryKeyTable.Columns[r.Name])).ToList();
            // named for the table it refers to (order), or for its columns where that's not enough (a second key to the
            // same table, or one to its own: ManagerId is manager)
            bool byColumns = key.PrimaryKeyTable.FullName == table.FullName ||
                             keys.Count(k => k.PrimaryKeyTable.FullName == key.PrimaryKeyTable.FullName) > 1;
            string name = byColumns ? Trimmed(string.Concat(columns.Select(c => Names.Property(c.Item1)))) : Names.Entity(key.PrimaryKeyTable);
            string parameter = Names.Parameter(name);
            for (int i = 2; taken.Contains(parameter); i++)
                parameter = Names.Parameter(name) + i;
            taken.Add(parameter);
            var unique = Unique(table);
            parents.Add(new Parent
            {
                Key = key,
                Table = key.PrimaryKeyTable,
                Columns = columns,
                Parameter = parameter,
                Cyclic = Reaches(key.PrimaryKeyTable, table, new HashSet<string>()),
                OnePerParent = unique.Any(u => u.All(c => columns.Any(p => p.Item1.Name == c))),
                Nullable = columns.All(c => c.Item1.AllowDBNull),
            });
        }
        return parents;
    }

    // ManagerId: Manager; an Id at the end goes, unless that's all there is
    private static string Trimmed(string name) =>
        name.Length > 2 && name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ? name.Substring(0, name.Length - 2) : name;

    // whether a table's foreign keys lead to target (a cycle, when target is where they started)
    private static bool Reaches(TableSchema from, TableSchema target, HashSet<string> seen)
    {
        if (from.FullName == target.FullName)
            return true;
        if (!seen.Add(from.FullName))
            return false;
        var tables = new HashSet<string>(target.Database.Tables.Cast<TableSchema>().Select(t => t.FullName));
        return from.ForeignKeys.Cast<TableKeySchema>()
            .Where(k => tables.Contains(k.PrimaryKeyTable.FullName))
            .Any(k => Reaches(k.PrimaryKeyTable, target, seen));
    }

    // the table's unique keys, by their columns' names: the primary key and the unique indexes
    private static List<List<string>> Unique(TableSchema table)
    {
        var keys = table.Indexes.Cast<IndexSchema>().Where(i => i.IsUnique || i.IsPrimaryKey)
            .Select(i => i.MemberColumns.Cast<MemberColumnSchema>().Select(c => c.Name).ToList()).ToList();
        if (table.HasPrimaryKey)
            keys.Add(table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().Select(c => c.Name).ToList());
        return keys.Where(k => k.Count > 0).ToList();
    }

    /// <summary>The value for a foreign key's column from its parent's (order.OrderId), cast where the types differ
    /// ((int)order.OrderId, where one's an int and the other a long).</summary>
    public static string Value(ColumnSchema column, ColumnSchema referenced, string parent)
    {
        string type = column.GetCSharpTypeName(), from = referenced.GetCSharpTypeName();
        string value = parent + "." + Names.Property(referenced);
        return type.TrimEnd('?') == from.TrimEnd('?') ? value : "(" + type + ")" + value;
    }

    // ---------------------------------------------------------------- rules

    // A PostgreSQL enum (CREATE TYPE ... AS ENUM): a string to .NET, of a user-defined type.
    private static bool IsEnum(ColumnSchema column) =>
        column.Table.Database.Provider?.GetType().Name == "PostgreSQLSchemaProvider" &&
        System.Convert.ToString(column.ExtendedProperties["CS_SystemType"]?.Value) == "USER-DEFINED" &&
        column.GetClrType(TypeMapping.Modern) == typeof(string);

    /// <summary>
    /// The Bogus rule for a column's value, as the C# after "f =>" (f is the Faker): a value that fits the column (its
    /// type, its length, its precision and scale), from its name where that says what it holds (Email, City, Phone),
    /// and one of its own where the column is in the primary key or a unique index. Null for a column the database
    /// writes (an identity, a computed column, a rowversion), and for a type with no rule (Configure can give one): a
    /// PostgreSQL enum's too, a string whose values only the database knows.
    /// </summary>
    public static string Rule(ColumnSchema column)
    {
        if (!Names.Written(column) || IsEnum(column))
            return null;
        string name = Plain(column.Name);
        bool unique = IsUnique(column);
        switch (column.GetCSharpTypeName().TrimEnd('?'))
        {
            case "string":
                return Text(name, Plain(column.Table.Name), column.Size, unique);
            case "char":
                return "f.Random.Char('A', 'Z')";
            case "bool":
                return "f.Random.Bool()";
            case "byte":
                return unique ? "f.Random.Byte(1)" : "f.Random.Byte(1, 100)";
            case "sbyte":
                return "f.Random.SByte(1, 100)";
            case "short":
                return unique ? "f.Random.Short(1)" : "f.Random.Short(1, " + Small(name, 10_000) + ")";
            case "ushort":
                return "f.Random.UShort(1, " + Small(name, 10_000) + ")";
            case "int":
                return unique ? "f.Random.Int(1)" : "f.Random.Int(1, " + Small(name, 10_000) + ")";
            case "uint":
                return "f.Random.UInt(1, " + Small(name, 10_000) + ")";
            case "long":
                return unique ? "f.Random.Long(1)" : "f.Random.Long(1, " + Small(name, 1_000_000) + ")";
            case "ulong":
                return "f.Random.ULong(1, " + Small(name, 1_000_000) + ")";
            case "decimal":
                string maximum = Maximum(column);
                return "f.Finance.Amount(" + (Money(name) && maximum != "0.9" ? "1" : "0") + ", " + maximum + (maximum.Contains('.') ? "m" : "") + ", " + Scale(column) + ")";
            case "double":
                return "Math.Round(f.Random.Double(0, " + Maximum(column) + "), " + Scale(column) + ")";
            case "float":
                return "(float)Math.Round(f.Random.Double(0, " + Maximum(column) + "), " + Scale(column) + ")";
            case "DateTime":
                return "Fake.Moment(" + Past(name) + ")";
            case "DateTimeOffset":
                return "new DateTimeOffset(Fake.Moment(" + Past(name) + "), TimeSpan.Zero)";
            case "DateOnly":
                return "DateOnly.FromDateTime(" + Past(name) + ")";
            case "TimeOnly":
                return "TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(f.Random.Int(0, 86_399)))";
            case "TimeSpan":
                return "TimeSpan.FromSeconds(f.Random.Int(0, 86_399))";
            case "Guid":
                return "f.Random.Guid()";
            case "byte[]":
                return "f.Random.Bytes(" + (column.Size > 0 ? Math.Min(column.Size, 16) : 16) + ")";
            default:
                return null;
        }
    }

    /// <summary>Whether a new row needs a value of its own: the column is in the primary key or a unique index, and
    /// isn't a foreign key's (its parent gives that).</summary>
    public static bool IsUnique(ColumnSchema column) =>
        !Parents(column.Table).Any(p => p.Columns.Any(c => c.Column.Name == column.Name)) &&
        Unique(column.Table).Any(k => k.Contains(column.Name));

    // a name in lower case, letters and digits only: ShipToFirstName is shiptofirstname, order_id is orderid
    private static string Plain(string name) => new string(name.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    // Text, from the column's name where that says what it holds (and a name column's from its table's: a supplier's is
    // a company's); cut to the column's length. A unique column's has a random part (Fake.Unique), or is only random
    // letters and digits where it's a code (CategoryId varchar(10)).
    private static string Text(string name, string table, int size, bool unique)
    {
        // a size the rules' text can't reach is no limit (text, varchar(max), a long varchar)
        if (size >= 1000)
            size = 0;
        bool code = name.EndsWith("id") || name.EndsWith("code") || name.EndsWith("key") || name.EndsWith("num") || name.EndsWith("no");
        string value =
            name.Contains("email") ? "f.Internet.Email()" :
            name.Contains("firstname") || name.Contains("givenname") ? "f.Name.FirstName()" :
            name.Contains("lastname") || name.Contains("surname") || name.Contains("familyname") ? "f.Name.LastName()" :
            name.Contains("fullname") || name.Contains("contactname") || name.Contains("customername") ? "f.Name.FullName()" :
            name.Contains("username") || name.Contains("userid") || name.Contains("login") ? "f.Internet.UserName()" :
            name.Contains("phone") || name.Contains("mobile") || name.Contains("fax") ? "f.Phone.PhoneNumber()" :
            name.Contains("city") || name.Contains("town") ? "f.Address.City()" :
            name.Contains("state") || name.Contains("province") || name.Contains("region") ? "f.Address.StateAbbr()" :
            name.Contains("zip") || name.Contains("postal") || name.Contains("postcode") ? "f.Address.ZipCode()" :
            name.Contains("country") ? "f.Address.CountryCode()" :
            name.Contains("addr") || name.Contains("street") ? "f.Address.StreetAddress()" :
            name.Contains("url") || name.Contains("website") || name.Contains("homepage") ? "f.Internet.Url()" :
            name.Contains("image") || name.Contains("photo") || name.Contains("picture") || name.Contains("avatar") ? "f.Image.PicsumUrl()" :
            name.Contains("company") || name.Contains("supplier") || name.Contains("vendor") ? "f.Company.CompanyName()" :
            name.Contains("locale") || name.Contains("culture") || name.Contains("language") ? "f.PickRandom(\"en-US\", \"en-GB\", \"fr-FR\", \"de-DE\")" :
            name.Contains("currency") ? "f.Finance.Currency().Code" :
            name.Contains("color") || name.Contains("colour") ? "f.Commerce.Color()" :
            name.Contains("desc") || name.Contains("comment") || name.Contains("note") || name.Contains("body") || name.Contains("summary") ? "f.Lorem.Sentence()" :
            name.Contains("title") || name.Contains("subject") ? "f.Lorem.Sentence(3)" :
            name.EndsWith("name") ? Named(table) :
            null;
        if (unique)
            return code || value == null ? "Fake.Code(f, " + Length(size, 10) + ")" : "Fake.Unique(f, " + value + ", " + Length(size, 0) + ")";
        if (value == null)
            value = code ? "Fake.Code(f, " + Length(size, 10) + ")" :
                    size > 0 && size <= 3 ? "Fake.Code(f, " + size + ")" :
                    size > 0 && size <= 20 ? "f.Lorem.Word()" :
                    "f.Lorem.Sentence()";
        return value.StartsWith("Fake.Code(") || size <= 0 ? value : "Fake.Fit(" + value + ", " + size + ")";
    }

    // what a name column of a table holds: a company's name, a person's, a department's, else a product's
    private static string Named(string table) =>
        new[] { "supplier", "company", "vendor", "manufacturer", "brand", "organization" }.Any(table.Contains) ? "f.Company.CompanyName()" :
        new[] { "customer", "person", "people", "employee", "user", "contact", "member", "author" }.Any(table.Contains) ? "f.Name.FullName()" :
        new[] { "category", "categories", "department", "group" }.Any(table.Contains) ? "f.Commerce.Department()" :
        "f.Commerce.ProductName()";

    // a length to make text of: the column's, up to most (0: no limit); unlimited columns (0, -1) get most
    private static int Length(int size, int most) => size <= 0 ? most : most == 0 ? size : Math.Min(size, most);

    // the largest number a count or a key-like number gets
    private static string Small(string name, int otherwise) =>
        (name.Contains("quantity") || name.Contains("qty") || name.Contains("count") ? 100 : otherwise).ToString("N0", CultureInfo.InvariantCulture).Replace(",", "_");

    private static bool Money(string name) =>
        name.Contains("price") || name.Contains("cost") || name.Contains("amount") || name.Contains("total") || name.Contains("fee");

    // the largest value that fits a decimal(p, s): 1000, or less where p - s leaves fewer digits
    private static string Maximum(ColumnSchema column)
    {
        int digits = column.Precision > 0 ? column.Precision - Math.Max(column.Scale, 0) : 4;
        return digits >= 4 ? "1000" : digits <= 0 ? "0.9" : (Math.Pow(10, digits) - 1).ToString(CultureInfo.InvariantCulture);
    }

    // the decimals a value has: the column's scale (2 where it has none)
    private static int Scale(ColumnSchema column) =>
        column.Precision > 0 && column.Scale >= 0 ? Math.Min(column.Scale, 4) : 2;

    // a moment in the past, around TestData.Today (so the same seed makes the same rows all day)
    private static string Past(string name) =>
        name.Contains("birth") || name.Contains("dob") ? "f.Date.Past(60, data.Today.AddYears(-18))" : "f.Date.Past(2, data.Today)";
}
