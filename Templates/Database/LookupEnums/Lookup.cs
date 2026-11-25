// What the two templates share: the lookup table's columns, the enum's name, and the table's rows as members, named
// so that a name is an identifier in C# and TypeScript alike.
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using SchemaExplorer;

/// <summary>A row of the lookup table: the member's name, its value (a code, or a whole number), its display text.</summary>
public sealed record LookupMember(string Name, object Value, string Text);

/// <summary>A lookup table, read for an enum.</summary>
public sealed class Lookup
{
    /// <summary>Reads the table; an empty valueColumn, nameColumn or enumName is the default.</summary>
    public Lookup(TableSchema table, string valueColumn, string nameColumn, string enumName)
    {
        Table = table ?? throw new ArgumentNullException(nameof(table));
        ValueColumn = string.IsNullOrWhiteSpace(valueColumn) ? PrimaryKey(table) : Column(table, valueColumn);
        NameColumn = string.IsNullOrWhiteSpace(nameColumn) ? FirstText(table, ValueColumn) : Column(table, nameColumn);
        EnumName = Name(table, enumName);
        IsInteger = IsWholeNumber(ValueColumn);
        Members = Read();
    }

    /// <summary>The enum's name: enumName, else the table's, singular, in Pascal case (for a template's GetFileName too,
    /// without reading the table).</summary>
    public static string Name(TableSchema table, string enumName) =>
        string.IsNullOrWhiteSpace(enumName)
            ? Identifier(StringUtil.ToSingular(StringUtil.ToPascalCase(table.Name))) ?? "Lookup"
            : enumName.Trim();

    public TableSchema Table { get; }

    /// <summary>The column the values come from: the one named, or the primary key.</summary>
    public ColumnSchema ValueColumn { get; }

    /// <summary>The column the names and display text come from: the one named, or the first text column that isn't
    /// the key; the value column when there's none (BIRDS: Birds).</summary>
    public ColumnSchema NameColumn { get; }

    /// <summary>The enum's name: the one given, or the table's, singular, in Pascal case (Categories: Category).</summary>
    public string EnumName { get; }

    /// <summary>Whether the values are whole numbers, the members' own (Birds = 3); else they're codes, strings.</summary>
    public bool IsInteger { get; }

    /// <summary>The rows, in value order: each value once, and none without one.</summary>
    public List<LookupMember> Members { get; }

    /// <summary>The enum's base type: none (int), or " : long" for a value an int can't hold.</summary>
    public string BaseType => IsInteger && Members.Any(m => (long)m.Value is < int.MinValue or > int.MaxValue) ? " : long" : "";

    /// <summary>A member's value as C# and TypeScript write it: 3, or "BIRDS".</summary>
    public string Literal(LookupMember member) =>
        IsInteger ? ((long)member.Value).ToString(CultureInfo.InvariantCulture) : Quote((string)member.Value);

    /// <summary>A string literal for C# and TypeScript alike, with quotes, backslashes and control characters escaped.</summary>
    public static string Quote(string value)
    {
        var builder = new StringBuilder("\"");
        foreach (char c in value)
        {
            switch (c)
            {
                case '"': builder.Append("\\\""); break;
                case '\\': builder.Append("\\\\"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                default:
                    // U+2028 and U+2029 end a line in C#
                    if (char.IsControl(c) || char.GetUnicodeCategory(c) is UnicodeCategory.LineSeparator or UnicodeCategory.ParagraphSeparator)
                        builder.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else
                        builder.Append(c);
                    break;
            }
        }
        return builder.Append('"').ToString();
    }

    // The rows: each value once, in order; a member's name is its text's (else its value's), made unique (Fish, Fish2).
    private List<LookupMember> Read()
    {
        var rows = Table.GetTableData().Rows.Cast<DataRow>()
            .Where(row => row[ValueColumn.Name] != DBNull.Value)
            .Select(row => (Value: IsInteger
                    ? (object)Convert.ToInt64(row[ValueColumn.Name], CultureInfo.InvariantCulture)
                    : Convert.ToString(row[ValueColumn.Name], CultureInfo.InvariantCulture),
                Text: (Convert.ToString(row[NameColumn.Name], CultureInfo.InvariantCulture) ?? "").Trim()))
            .GroupBy(row => row.Value)
            .Select(values => values.First());
        rows = IsInteger ? rows.OrderBy(row => (long)row.Value) : rows.OrderBy(row => (string)row.Value, StringComparer.Ordinal);

        var members = new List<LookupMember>();
        var taken = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (value, text) in rows)
        {
            string code = Convert.ToString(value, CultureInfo.InvariantCulture);
            string name = Identifier(StringUtil.ToPascalCase(text)) ?? Identifier(StringUtil.ToPascalCase(code)) ?? "Value";
            string unique = name;
            for (int i = 2; !taken.Add(unique); i++)
                unique = name + i;
            members.Add(new LookupMember(unique, value, text.Length > 0 ? text : code));
        }
        return members;
    }

    // An identifier: letters and digits only, with _ before a leading digit (3DModels: _3DModels); null if none are left.
    private static string Identifier(string name)
    {
        string identifier = new string((name ?? "").Where(char.IsLetterOrDigit).ToArray());
        return identifier.Length == 0 ? null : char.IsDigit(identifier[0]) ? "_" + identifier : identifier;
    }

    private static ColumnSchema Column(TableSchema table, string name) =>
        table.Columns.Cast<ColumnSchema>().FirstOrDefault(c => string.Equals(c.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException(table.FullName + " has no column " + name.Trim() + ".");

    private static ColumnSchema PrimaryKey(TableSchema table) =>
        table.HasPrimaryKey && table.PrimaryKey.MemberColumns.Count == 1
            ? Column(table, table.PrimaryKey.MemberColumns[0].Name)
            : throw new ArgumentException(table.FullName + " has no primary key of one column: set ValueColumn.");

    private static ColumnSchema FirstText(TableSchema table, ColumnSchema valueColumn) =>
        table.Columns.Cast<ColumnSchema>().FirstOrDefault(c =>
            !c.IsPrimaryKeyMember && c.Name != valueColumn.Name && c.GetClrType(TypeMapping.Modern) == typeof(string)) ?? valueColumn;

    private static bool IsWholeNumber(ColumnSchema column) =>
        Type.GetTypeCode(column.GetClrType(TypeMapping.Modern)) is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16
            or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
}
