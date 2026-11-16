// Words for the labels of names that are all in one case, as PostgreSQL folds them (productid, listprice) and Oracle
// keeps them (PRODUCTID): there's no capital or underscore to split them at. The words come from the schema itself
// (its tables' names, its other names' parts where they have case, its columns of one word) and a few common ones
// (id, name, date, price...). A name is split into the longest of them: productid is Product ID, listprice List price.
// A name with case of its own (ProductId) or underscores (product_id) keeps its words. The Csla and FullStack samples
// have the same copy of this file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CodeSmith.Engine;
using SchemaExplorer;

public static class NameWords
{
    // short words a name ends with (productid, linenum)
    private static readonly HashSet<string> Suffixes = new HashSet<string> { "id", "ids", "num", "nbr", "qty", "amt" };

    // common words in names, anywhere in them
    private static readonly string[] Common =
    {
        "name", "date", "time", "code", "type", "price", "cost", "total", "amount", "count", "status", "city", "state", "zip",
        "country", "address", "addr", "phone", "email", "number", "user", "ship", "bill", "unit", "list", "line", "first",
        "last", "desc", "descn", "key", "value", "rate", "quantity", "created", "modified", "updated", "title", "level",
        "group", "size",
    };

    private static readonly ConditionalWeakTable<DatabaseSchema, Lexicon> Lexicons = new ConditionalWeakTable<DatabaseSchema, Lexicon>();

    /// <summary>Whether a name is all in one case: its letters all lower case, or all upper case, with digits at most.</summary>
    public static bool OneCase(string name) =>
        !string.IsNullOrEmpty(name) && name.All(char.IsLetterOrDigit) && name.Any(char.IsLetter) &&
        (!name.Any(char.IsUpper) || !name.Any(char.IsLower));

    /// <summary>A name all in one case, in Pascal case at its words (productid: ProductId, SHIPADDR1: ShipAddr1); null
    /// for a name with case of its own or underscores, or one that doesn't split.</summary>
    public static string Pascal(string name, DatabaseSchema database)
    {
        if (!OneCase(name) || database == null)
            return null;
        var lexicon = Lexicons.GetValue(database, Build);
        bool split = false;
        string pascal = Regex.Replace(name.ToLowerInvariant(), "[a-z]+", m =>
        {
            var words = lexicon.Split(m.Value);
            split |= words.Count > 1;
            return string.Concat(words.Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));
        });
        return split ? pascal : null;
    }

    /// <summary>A column's property for its label: its name's words (productid: ProductId), else Names.Property's.</summary>
    public static string Property(ColumnSchema column) => Pascal(column.Name, column.Table.Database) ?? Names.Property(column);

    /// <summary>A table's entity for its labels: its name's words, singular (lineitem: LineItem), else Names.Entity's.</summary>
    public static string Entity(TableSchema table)
    {
        string entity = Names.Entity(table);
        string pascal = Pascal(table.Name, table.Database);
        if (pascal == null)
            return entity;
        // with the owner's name in front, where Names.Entity puts it
        string plain = StringUtil.ToSingular(StringUtil.ToPascalCase(table.Name));
        string words = StringUtil.ToSingular(pascal);
        return entity.EndsWith(plain, StringComparison.Ordinal) ? entity.Substring(0, entity.Length - plain.Length) + words : words;
    }

    /// <summary>A table's set for its labels: its entity's plural (LineItems).</summary>
    public static string Set(TableSchema table) => Pascal(table.Name, table.Database) == null ? Names.Set(table) : StringUtil.ToPlural(Entity(table));

    // The database's words: the common ones, the parts of its names with case (ShipCity: ship, city), and its names in
    // one case that don't split (city, supplier; a table's singular and plural too), shortest first.
    private static Lexicon Build(DatabaseSchema database)
    {
        var lexicon = new Lexicon(Common);
        var names = new List<string>();
        void Add(string name, bool table)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (!OneCase(name))
            {
                foreach (string part in Regex.Split(name, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|[^A-Za-z]+"))
                    if (part.Length >= 3)
                        lexicon.Words.Add(part.ToLowerInvariant());
                return;
            }
            foreach (Match word in Regex.Matches(name.ToLowerInvariant(), "[a-z]+"))
            {
                names.Add(word.Value);
                if (table)
                {
                    names.Add(StringUtil.ToSingular(word.Value).ToLowerInvariant());
                    names.Add(StringUtil.ToPlural(word.Value).ToLowerInvariant());
                }
            }
        }
        foreach (TableSchema table in database.Tables)
        {
            Add(table.Name, true);
            foreach (ColumnSchema column in table.Columns)
                Add(column.Name, false);
        }
        foreach (ViewSchema view in database.Views)
        {
            Add(view.Name, true);
            foreach (ViewColumnSchema column in view.Columns)
                Add(column.Name, false);
        }
        foreach (string name in names.Where(n => n.Length >= 3).Distinct().OrderBy(n => n.Length).ThenBy(n => n, StringComparer.Ordinal))
            if (lexicon.Split(name).Count == 1)
                lexicon.Words.Add(name);
        lexicon.Splits.Clear(); // split again with every word
        return lexicon;
    }

    /// <summary>Known words, and the words a lower-case run of letters splits into.</summary>
    public sealed class Lexicon
    {
        public Lexicon(IEnumerable<string> words) => Words = new HashSet<string>(words, StringComparer.Ordinal);

        public HashSet<string> Words { get; }

        internal Dictionary<string, List<string>> Splits { get; } = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        /// <summary>
        /// A word (lower-case letters) in the longest known words: the fewest letters that aren't in one, then the fewest
        /// words. One run of three letters or more may be unknown (listprice: list, price), where what follows it is
        /// known and it's no longer than what is (candidate stays whole), or where an id-like suffix ends the word
        /// (suppid: supp, id). The word itself, as one, where it doesn't split so, or is known whole (status); a word
        /// never splits into itself, its singular or its plural.
        /// </summary>
        public List<string> Split(string word)
        {
            if (Splits.TryGetValue(word, out var known))
                return known;
            var result = Find(word) ?? new List<string> { word };
            Splits[word] = result;
            return result;
        }

        private List<string> Find(string word)
        {
            if (Words.Contains(word) || Suffixes.Contains(word))
                return null;
            string singular = StringUtil.ToSingular(word).ToLowerInvariant(), plural = StringUtil.ToPlural(word).ToLowerInvariant();
            bool Known(int start, int end)
            {
                string part = word.Substring(start, end - start);
                if (part == word || part == singular || part == plural)
                    return false;
                return Words.Contains(part) && part.Length >= 3 || end == word.Length && Suffixes.Contains(part);
            }

            int n = word.Length;
            // best[i, u]: word[i..] split into words, starting with a known one; u: the unknown run is still to use
            var best = new (int Unknown, int Count, List<(string Part, bool Known)> Parts)?[n + 1, 2];
            var done = new bool[n + 1, 2];
            (int, int, List<(string, bool)>)? Best(int i, int u)
            {
                if (i == n)
                    return (0, 0, new List<(string, bool)>());
                if (done[i, u])
                    return best[i, u];
                (int Unknown, int Count, List<(string Part, bool Known)> Parts)? found = null;
                void Consider(int unknown, int count, List<(string, bool)> parts)
                {
                    if (found == null || unknown < found.Value.Unknown || unknown == found.Value.Unknown && count < found.Value.Count)
                        found = (unknown, count, parts);
                }
                for (int end = n; end > i; end--)
                {
                    if (!Known(i, end))
                        continue;
                    // a known word, then a known word or the unknown run (if it's still to use)
                    var rest = Best(end, u);
                    if (rest != null)
                        Consider(rest.Value.Item1, rest.Value.Item2 + 1, new[] { (word.Substring(i, end - i), true) }.Concat(rest.Value.Item3).ToList());
                    if (u == 1)
                        for (int stop = end + 3; stop < n; stop++)
                            if (Best(stop, 0) is { } after)
                                Consider(after.Item1 + stop - end, after.Item2 + 2,
                                    new[] { (word.Substring(i, end - i), true), (word.Substring(end, stop - end), false) }.Concat(after.Item3).ToList());
                }
                done[i, u] = true;
                best[i, u] = found;
                return found;
            }

            var splits = new List<(int Unknown, int Count, List<(string Part, bool Known)> Parts)>();
            if (Best(0, 1) is { } first)
                splits.Add(first);
            // the unknown run first: listprice
            for (int stop = 3; stop < n; stop++)
                if (Best(stop, 0) is { } after)
                    splits.Add((after.Item1 + stop, after.Item2 + 1, new[] { (word.Substring(0, stop), false) }.Concat(after.Item3).ToList()));
            foreach (var split in splits.OrderBy(s => s.Unknown).ThenBy(s => s.Count))
            {
                if (split.Parts.Count < 2)
                    continue;
                int unknown = split.Unknown, knownLetters = n - unknown;
                if (unknown <= knownLetters || Suffixes.Contains(split.Parts[split.Parts.Count - 1].Part))
                    return split.Parts.Select(p => p.Part).ToList();
            }
            return null;
        }
    }
}
