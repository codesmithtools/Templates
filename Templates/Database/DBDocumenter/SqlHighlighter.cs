// SQL syntax highlighting, done while generating: the pages need no script or library for it. SqlLexer splits SQL into
// tokens (T-SQL's, PostgreSQL's, MySQL's and SQLite's quoting and comments); SqlHighlighter writes them as HTML spans.
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum SqlTokenKind { Space, Comment, String, Number, Word, Quoted, Variable, Symbol }

public readonly struct SqlToken
{
    public SqlToken(SqlTokenKind kind, string text)
    {
        Kind = kind;
        Text = text;
    }

    public SqlTokenKind Kind { get; }
    public string Text { get; }
}

public static class SqlLexer
{
    public static List<SqlToken> Tokenize(string sql)
    {
        var tokens = new List<SqlToken>();
        string s = sql ?? "";
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            int start = i;
            if (char.IsWhiteSpace(c))
            {
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                tokens.Add(new SqlToken(SqlTokenKind.Space, s.Substring(start, i - start)));
            }
            else if (c == '-' && Next(s, i) == '-')
            {
                while (i < s.Length && s[i] != '\n' && s[i] != '\r') i++;
                tokens.Add(new SqlToken(SqlTokenKind.Comment, s.Substring(start, i - start)));
            }
            else if (c == '/' && Next(s, i) == '*')
            {
                int end = s.IndexOf("*/", i + 2, StringComparison.Ordinal);
                i = end < 0 ? s.Length : end + 2;
                tokens.Add(new SqlToken(SqlTokenKind.Comment, s.Substring(start, i - start)));
            }
            else if (c == '\'')
            {
                i = Quoted(s, i, '\'');
                tokens.Add(new SqlToken(SqlTokenKind.String, s.Substring(start, i - start)));
            }
            else if (c == '"' || c == '`')
            {
                i = Quoted(s, i, c);
                tokens.Add(new SqlToken(SqlTokenKind.Quoted, s.Substring(start, i - start)));
            }
            else if (c == '[' && BracketName(s, i, out int close))
            {
                i = close + 1;
                tokens.Add(new SqlToken(SqlTokenKind.Quoted, s.Substring(start, i - start)));
            }
            else if (char.IsDigit(c) || (c == '.' && char.IsDigit(Next(s, i))))
            {
                if (c == '0' && (Next(s, i) == 'x' || Next(s, i) == 'X'))
                    for (i += 2; i < s.Length && Uri.IsHexDigit(s[i]); i++) { }
                else
                {
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
                    if (i < s.Length && (s[i] == 'e' || s[i] == 'E') && (char.IsDigit(Next(s, i)) || ((Next(s, i) == '-' || Next(s, i) == '+') && i + 2 < s.Length && char.IsDigit(s[i + 2]))))
                        for (i += 2; i < s.Length && char.IsDigit(s[i]); i++) { }
                }
                tokens.Add(new SqlToken(SqlTokenKind.Number, s.Substring(start, i - start)));
            }
            else if (c == '@' || (c == '$' && char.IsDigit(Next(s, i))))
            {
                i++;
                if (i < s.Length && s[i] == '@') i++;
                while (i < s.Length && IsWordChar(s[i])) i++;
                tokens.Add(new SqlToken(SqlTokenKind.Variable, s.Substring(start, i - start)));
            }
            else if (c == '$' && DollarTag(s, i, out int tagEnd))
            {
                // PostgreSQL's $$ or $body$ around a function's body: a delimiter, the body inside is SQL
                i = tagEnd;
                tokens.Add(new SqlToken(SqlTokenKind.Symbol, s.Substring(start, i - start)));
            }
            else if (char.IsLetter(c) || c == '_' || c == '#')
            {
                while (i < s.Length && (IsWordChar(s[i]) || s[i] == '#')) i++;
                string word = s.Substring(start, i - start);
                // N'unicode', E'escapes', X'hex', B'bits'
                if (word.Length == 1 && "NnEeXxBb".IndexOf(word[0]) >= 0 && i < s.Length && s[i] == '\'')
                {
                    i = Quoted(s, i, '\'');
                    tokens.Add(new SqlToken(SqlTokenKind.String, s.Substring(start, i - start)));
                }
                else
                    tokens.Add(new SqlToken(SqlTokenKind.Word, word));
            }
            else
            {
                i++;
                if (i < s.Length && IsOperatorPair(c, s[i])) i++;
                tokens.Add(new SqlToken(SqlTokenKind.Symbol, s.Substring(start, i - start)));
            }
        }
        return tokens;
    }

    private static char Next(string s, int i) => i + 1 < s.Length ? s[i + 1] : '\0';

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '$';

    private static bool IsOperatorPair(char a, char b) =>
        (a == '<' && (b == '=' || b == '>')) || (a == '>' && b == '=') || (a == '!' && b == '=') || (a == ':' && (b == ':' || b == '=')) ||
        (a == '|' && b == '|') || (a == '-' && b == '>');

    // a quoted run, its quote doubled inside ('it''s'): the index after it
    private static int Quoted(string s, int i, char quote)
    {
        i++;
        while (i < s.Length)
        {
            if (s[i] == quote)
            {
                if (i + 1 < s.Length && s[i + 1] == quote) { i += 2; continue; }
                return i + 1;
            }
            if (s[i] == '\\' && quote == '\'' && i + 1 < s.Length && s[i + 1] == '\'') { i += 2; continue; } // MySQL's \'
            i++;
        }
        return s.Length;
    }

    // [a name] (T-SQL), not an array's [1]
    private static bool BracketName(string s, int i, out int close)
    {
        close = s.IndexOf(']', i + 1);
        if (close < 0) return false;
        int newline = s.IndexOf('\n', i + 1);
        if (newline >= 0 && newline < close) return false;
        string inside = s.Substring(i + 1, close - i - 1);
        return inside.Length > 0 && !int.TryParse(inside, out _) && (char.IsLetter(inside[0]) || inside[0] == '_' || inside[0] == ' ' || inside[0] == '#');
    }

    // $$ or $tag$
    private static bool DollarTag(string s, int i, out int end)
    {
        end = i + 1;
        while (end < s.Length && (char.IsLetterOrDigit(s[end]) || s[end] == '_')) end++;
        if (end < s.Length && s[end] == '$') { end++; return true; }
        return false;
    }
}

public static class SqlHighlighter
{
    private static readonly HashSet<string> Keywords = Words(@"
        add all alter and any as asc authorization begin between break by call cascade case catch check close coalesce
        collate column comment commit concurrently constraint continue create cross current current_date current_time
        current_timestamp current_user cursor database deallocate declare default definer delete desc deterministic distinct
        do drop each else elsif end escape except exception exec execute exists exit fetch first for foreach foreign from
        full function go goto grant group having identity if ilike immutable in index inner inout insert instead intersect
        into invoker is isnull join key language last lateral leading left level like limit loop merge modifies natural
        next no nocount not notice notnull null nulls of off offset on only open option or order out outer output over
        partition perform pivot primary print proc procedure raise raiserror range reads recursive references replace
        restrict return returning returns revoke right rollback row rows rowcount schema security select set setof sql
        stable strict table then throw to top tran transaction trigger true false truncate try union unique unpivot update
        use using values view volatile when where while window with without xact_abort plpgsql query record found
        ");

    private static readonly HashSet<string> Types = Words(@"
        bigint bigserial binary bit blob bool boolean bpchar bytea char character clob cidr date datetime datetime2
        datetimeoffset dec decimal double enum float geography geometry hierarchyid image inet int int2 int4 int8 integer
        interval json jsonb longblob longtext mediumint mediumtext money nchar ntext number numeric nvarchar nvarchar2
        precision raw real rowversion serial smalldatetime smallint smallmoney smallserial sql_variant sysname text time
        timestamp timestamptz timetz tinyint tinytext uniqueidentifier unsigned uuid varbinary varchar varchar2 varying xml year
        ");

    /// <summary>SQL as the lines of a code block: each line a span (a block, which the stylesheet numbers), each token
    /// one of its classes (k keyword, t type, f function, s string, n number, c comment, v variable, q quoted name,
    /// p punctuation, o operator).</summary>
    public static string ToHtml(string sql)
    {
        string text = (sql ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Trim('\n');
        var tokens = SqlLexer.Tokenize(text);
        var html = new StringBuilder();
        html.Append("<span class=\"line\">");
        for (int t = 0; t < tokens.Count; t++)
        {
            var token = tokens[t];
            string css = Class(tokens, t);
            string[] lines = token.Text.Split('\n');
            for (int l = 0; l < lines.Length; l++)
            {
                if (l > 0)
                    html.Append("</span><span class=\"line\">");
                if (lines[l].Length == 0)
                    continue;
                string encoded = WebUtility.HtmlEncode(lines[l]);
                if (css == null)
                    html.Append(encoded);
                else
                    html.Append("<span class=\"").Append(css).Append("\">").Append(encoded).Append("</span>");
            }
        }
        html.Append("</span>");
        return html.ToString();
    }

    public static int LineCount(string sql) =>
        (sql ?? "").Replace("\r\n", "\n").Replace('\r', '\n').Trim('\n').Split('\n').Length;

    // the words a clause starts with: a one-line definition breaks before them
    private static readonly HashSet<string> Clauses = Words("from where group order having limit union except intersect join left right inner outer cross full window");
    private static readonly HashSet<string> JoinWords = Words("left right inner outer cross full join natural");

    /// <summary>SQL to show: as the database keeps it, but a long definition on one line (MySQL keeps views so) on a line
    /// per clause, and a line per column of its SELECT.</summary>
    public static string Layout(string sql)
    {
        string text = (sql ?? "").Trim();
        if (text.Length < 100 || text.IndexOf('\n') >= 0)
            return text;
        var tokens = SqlLexer.Tokenize(text);
        var lines = new StringBuilder();
        int depth = 0, selectDepth = -1;
        bool joinStarted = false;
        for (int t = 0; t < tokens.Count; t++)
        {
            var token = tokens[t];
            string word = token.Kind == SqlTokenKind.Word ? token.Text.ToLowerInvariant() : null;
            if (word != null && Clauses.Contains(word) && !(JoinWords.Contains(word) && joinStarted) && !((word == "left" || word == "right") && NextSignificant(tokens, t) == "("))
            {
                TrimEnd(lines);
                lines.Append('\n').Append(Indent(depth));
                joinStarted = JoinWords.Contains(word) && word != "join";
                if (word != "join" && !JoinWords.Contains(word))
                    selectDepth = -1;
            }
            else if (word != null && !JoinWords.Contains(word))
                joinStarted = false;
            if (token.Kind == SqlTokenKind.Space)
            {
                if (lines.Length > 0 && lines[lines.Length - 1] != '\n' && lines[lines.Length - 1] != ' ')
                    lines.Append(' ');
                continue;
            }
            lines.Append(token.Text);
            if (word == "select")
                selectDepth = depth;
            if (token.Text == "(") depth++;
            else if (token.Text == ")") depth = Math.Max(0, depth - 1);
            else if (token.Text == "," && depth == selectDepth)
            {
                lines.Append('\n').Append(Indent(depth + 1));
                while (t + 1 < tokens.Count && tokens[t + 1].Kind == SqlTokenKind.Space) t++;
            }
        }
        return lines.ToString().Trim();
    }

    private static string Indent(int depth) => new string(' ', Math.Min(depth, 6) * 4);

    private static void TrimEnd(StringBuilder text)
    {
        while (text.Length > 0 && text[text.Length - 1] == ' ')
            text.Length--;
    }

    private static string Class(List<SqlToken> tokens, int t)
    {
        var token = tokens[t];
        switch (token.Kind)
        {
            case SqlTokenKind.Comment: return "c";
            case SqlTokenKind.String: return "s";
            case SqlTokenKind.Number: return "n";
            case SqlTokenKind.Variable: return "v";
            case SqlTokenKind.Quoted: return "q";
            case SqlTokenKind.Symbol: return token.Text.Length == 1 && "(),;.".IndexOf(token.Text[0]) >= 0 ? "p" : "o";
            case SqlTokenKind.Word:
                string word = token.Text.ToLowerInvariant();
                bool call = NextSignificant(tokens, t) == "(";
                bool member = t > 0 && tokens[t - 1].Text == ".";
                if (!member && Types.Contains(word)) return "t";
                if (!member && Keywords.Contains(word) && !(call && !IsCallKeyword(word))) return "k";
                if (call) return "f";
                return null;
            default: return null;
        }
    }

    // keywords that are followed by ( and stay keywords (IN (...), VALUES (...))
    private static bool IsCallKeyword(string word) =>
        word is "in" or "values" or "exists" or "as" or "on" or "into" or "returns" or "table" or "over" or "and" or "or" or "not"
            or "using" or "from" or "join" or "select" or "where" or "when" or "then" or "else" or "return" or "if" or "while"
            or "primary" or "key" or "unique" or "check" or "references" or "foreign" or "with" or "option" or "all" or "any";

    private static string NextSignificant(List<SqlToken> tokens, int t)
    {
        for (int i = t + 1; i < tokens.Count; i++)
            if (tokens[i].Kind != SqlTokenKind.Space)
                return tokens[i].Text;
        return null;
    }

    private static HashSet<string> Words(string list) =>
        new HashSet<string>(list.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries), StringComparer.Ordinal);
}
