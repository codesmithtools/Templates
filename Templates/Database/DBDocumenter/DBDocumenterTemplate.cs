//-------------------------------------------------------------
// CodeSmith DBDocumenter Templates
// Author:  Jason Alexander, Eric J. Smith
//-------------------------------------------------------------
// The templates' code-behind (Inherits="DBDocumenterTemplate"): the output folder, and what every page writes the
// same way: links, icons, badges, section headings and SQL. DocSite (Site.cs) is the model the pages share.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CodeSmith.Engine;
using SchemaExplorer;

public class DBDocumenterTemplate : CodeTemplate
{
    private string _outputDirectory = "";

    [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [Optional]
    [Category("Output")]
    [Description("Where the pages go (default: a Docs folder beside the template). Open index.html there.")]
    public string OutputDirectory
    {
        get => _outputDirectory.Length == 0 ? Path.Combine(CodeTemplateInfo.DirectoryName, "Docs") : _outputDirectory;
        set => _outputDirectory = value ?? "";
    }

    /// <summary>The output folder, from this page's folder: "" (index.html) or "../" (tables/Account.html).</summary>
    protected virtual string Root => "";

    /// <summary>The documentation's model: the database's (every template here has a SourceDatabase).</summary>
    protected DocSite Site => DocSite.Of((DatabaseSchema)GetType().GetProperty("SourceDatabase").GetValue(this, null));

    /// <summary>Renders a template here (a page's header and footer), with this one's properties.</summary>
    public void OutputTemplate(CodeTemplate template)
    {
        CopyPropertiesTo(template);
        template.Render(Response);
    }

    // HTML

    /// <summary>Text as HTML.</summary>
    public static string H(object value) => WebUtility.HtmlEncode(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "");

    public static string Plural(int count, string one, string many = null) => count + " " + (count == 1 ? one : many ?? one + "s");

    /// <summary>A link to an object's page from this page, or null.</summary>
    public string Href(SchemaObjectBase item)
    {
        string page = Site.Page(item);
        return page == null ? null : Root + page;
    }

    /// <summary>An object's name, linked to its page (its schema in a quieter tone).</summary>
    public string Link(SchemaObjectWithOwnerBase item, string css = "ref")
    {
        string href = Href(item);
        string schema = DocSite.Schema(item);
        string name = (schema.Length > 0 ? "<span class=\"ref-schema\">" + H(schema) + ".</span>" : "") + H(DocSite.Name(item));
        return href == null ? "<span class=\"" + css + "\">" + name + "</span>" : "<a class=\"" + css + "\" href=\"" + H(href) + "\">" + name + "</a>";
    }

    /// <summary>A column of a table, linked to its row on the table's page: Profiles.UniqueID.</summary>
    public string ColumnLink(TableSchema table, string column)
    {
        string href = Href(table);
        string text = "<span class=\"ref-table\">" + H(DocSite.Title(table)) + "</span>.<span class=\"ref-column\">" + H(column) + "</span>";
        return href == null ? "<span class=\"ref\">" + text + "</span>" : "<a class=\"ref\" href=\"" + H(href + "#" + DocSite.Anchor(column)) + "\">" + text + "</a>";
    }

    /// <summary>A list of names: Name, Email, Phone.</summary>
    /// <summary>A table's primary key columns, in the table's column order: a provider may give them in another (the
    /// PostgreSQL providers give them by name).</summary>
    public static IEnumerable<MemberColumnSchema> KeyColumns(TableSchema table) =>
        table.PrimaryKey.MemberColumns.Cast<MemberColumnSchema>().OrderBy(c => table.Columns.IndexOf(table.Columns[c.Name]));

    public static string Names(IEnumerable<MemberColumnSchema> columns) =>
        string.Join("<span class=\"sep\">, </span>", columns.Select(c => "<code>" + H(c.Name) + "</code>"));

    /// <summary>A small pill: pk (amber), fk (indigo), identity (emerald), unique (sky), muted.</summary>
    public static string Badge(string text, string kind, string title = null) =>
        "<span class=\"badge badge-" + kind + "\"" + (title != null ? " title=\"" + H(title) + "\"" : "") + ">" + H(text) + "</span>";

    /// <summary>A section's heading, with its anchor and a count.</summary>
    public static string Heading(string id, string text, int? count = null) =>
        "<h2 class=\"section-title\" id=\"" + id + "\"><a class=\"anchor\" href=\"#" + id + "\" aria-label=\"Link to " + H(text) + "\">" + Icon("hash", "icon anchor-icon") + "</a>" +
        H(text) + (count.HasValue ? " <span class=\"count\">" + count.Value + "</span>" : "") + "</h2>";

    /// <summary>SQL in a code block: highlighted, numbered, with a copy button.</summary>
    public static string CodeBlock(string sql, string label)
    {
        sql = SqlHighlighter.Layout(sql);
        int lines = SqlHighlighter.LineCount(sql);
        return "<figure class=\"code-block\">" +
            "<figcaption class=\"code-head\"><span class=\"code-title\">" + Icon("file-code", "icon") + H(label) + "</span>" +
            "<span class=\"code-meta\">SQL · " + Plural(lines, "line") + "</span>" +
            "<button type=\"button\" class=\"code-copy\" data-copy-code aria-label=\"Copy the SQL\">" + Icon("copy", "icon") + Icon("check", "icon icon-done") + "<span>Copy</span></button></figcaption>" +
            "<pre class=\"code\"><code>" + SqlHighlighter.ToHtml(sql) + "</code></pre></figure>";
    }

    /// <summary>A note in a box (no SQL, no relationships): an icon and a sentence or two.</summary>
    public static string Empty(string icon, string text) =>
        "<div class=\"empty\">" + Icon(icon, "icon") + "<p>" + text + "</p></div>";

    // Icons: Lucide's (ISC license), inline so the pages need nothing else.

    private static readonly Dictionary<string, string> Icons = new Dictionary<string, string>
    {
        ["database"] = "<ellipse cx=\"12\" cy=\"5\" rx=\"9\" ry=\"3\"/><path d=\"M3 5V19A9 3 0 0 0 21 19V5\"/><path d=\"M3 12A9 3 0 0 0 21 12\"/>",
        ["table"] = "<path d=\"M12 3v18\"/><rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"/><path d=\"M3 9h18\"/><path d=\"M3 15h18\"/>",
        ["view"] = "<path d=\"M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
        ["procedure"] = "<rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"/><path d=\"M9 17c2 0 2.8-1 2.8-2.8V10c0-2 1-3.3 3.2-3\"/><path d=\"M9 11.2h5.7\"/>",
        ["columns"] = "<rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"/><path d=\"M9 3v18\"/><path d=\"M15 3v18\"/>",
        ["link"] = "<path d=\"M9 17H7A5 5 0 0 1 7 7h2\"/><path d=\"M15 7h2a5 5 0 1 1 0 10h-2\"/><line x1=\"8\" x2=\"16\" y1=\"12\" y2=\"12\"/>",
        ["index"] = "<path d=\"M10 12h11\"/><path d=\"M10 18h11\"/><path d=\"M10 6h11\"/><path d=\"M4 10h2\"/><path d=\"M4 6h1v4\"/><path d=\"M6 18H4c0-1 2-2 2-3s-1-1.5-2-1\"/>",
        ["key"] = "<path d=\"M2.586 17.414A2 2 0 0 0 2 18.828V21a1 1 0 0 0 1 1h3a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h1a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h.172a2 2 0 0 0 1.414-.586l.814-.814a6.5 6.5 0 1 0-4-4z\"/><circle cx=\"16.5\" cy=\"7.5\" r=\".5\" fill=\"currentColor\"/>",
        ["search"] = "<circle cx=\"11\" cy=\"11\" r=\"8\"/><path d=\"m21 21-4.3-4.3\"/>",
        ["sun"] = "<circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 2v2\"/><path d=\"M12 20v2\"/><path d=\"m4.93 4.93 1.41 1.41\"/><path d=\"m17.66 17.66 1.41 1.41\"/><path d=\"M2 12h2\"/><path d=\"M20 12h2\"/><path d=\"m6.34 17.66-1.41 1.41\"/><path d=\"m19.07 4.93-1.41 1.41\"/>",
        ["moon"] = "<path d=\"M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z\"/>",
        ["menu"] = "<path d=\"M4 12h16\"/><path d=\"M4 6h16\"/><path d=\"M4 18h16\"/>",
        ["x"] = "<path d=\"M18 6 6 18\"/><path d=\"m6 6 12 12\"/>",
        ["copy"] = "<rect width=\"14\" height=\"14\" x=\"8\" y=\"8\" rx=\"2\" ry=\"2\"/><path d=\"M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2\"/>",
        ["check"] = "<path d=\"M20 6 9 17l-5-5\"/>",
        ["hash"] = "<line x1=\"4\" x2=\"20\" y1=\"9\" y2=\"9\"/><line x1=\"4\" x2=\"20\" y1=\"15\" y2=\"15\"/><line x1=\"10\" x2=\"8\" y1=\"3\" y2=\"21\"/><line x1=\"16\" x2=\"14\" y1=\"3\" y2=\"21\"/>",
        ["chevron"] = "<path d=\"m9 18 6-6-6-6\"/>",
        ["overview"] = "<rect width=\"7\" height=\"9\" x=\"3\" y=\"3\" rx=\"1\"/><rect width=\"7\" height=\"5\" x=\"14\" y=\"3\" rx=\"1\"/><rect width=\"7\" height=\"9\" x=\"14\" y=\"12\" rx=\"1\"/><rect width=\"7\" height=\"5\" x=\"3\" y=\"16\" rx=\"1\"/>",
        ["diagram"] = "<rect x=\"16\" y=\"16\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"2\" y=\"16\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"9\" y=\"2\" width=\"6\" height=\"6\" rx=\"1\"/><path d=\"M5 16v-3a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v3\"/><path d=\"M12 12V8\"/>",
        ["server"] = "<rect width=\"20\" height=\"8\" x=\"2\" y=\"2\" rx=\"2\" ry=\"2\"/><rect width=\"20\" height=\"8\" x=\"2\" y=\"14\" rx=\"2\" ry=\"2\"/><line x1=\"6\" x2=\"6.01\" y1=\"6\" y2=\"6\"/><line x1=\"6\" x2=\"6.01\" y1=\"18\" y2=\"18\"/>",
        ["zoom-in"] = "<circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" x2=\"16.65\" y1=\"21\" y2=\"16.65\"/><line x1=\"11\" x2=\"11\" y1=\"8\" y2=\"14\"/><line x1=\"8\" x2=\"14\" y1=\"11\" y2=\"11\"/>",
        ["zoom-out"] = "<circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" x2=\"16.65\" y1=\"21\" y2=\"16.65\"/><line x1=\"8\" x2=\"14\" y1=\"11\" y2=\"11\"/>",
        ["fit"] = "<path d=\"M3 7V5a2 2 0 0 1 2-2h2\"/><path d=\"M17 3h2a2 2 0 0 1 2 2v2\"/><path d=\"M21 17v2a2 2 0 0 1-2 2h-2\"/><path d=\"M7 21H5a2 2 0 0 1-2-2v-2\"/><rect width=\"8\" height=\"8\" x=\"8\" y=\"8\" rx=\"1\"/>",
        ["expand"] = "<path d=\"M15 3h6v6\"/><path d=\"m21 3-7 7\"/><path d=\"m3 21 7-7\"/><path d=\"M9 21H3v-6\"/>",
        ["shrink"] = "<path d=\"m15 15 6 6\"/><path d=\"m15 9 6-6\"/><path d=\"M21 16v5h-5\"/><path d=\"M21 8V3h-5\"/><path d=\"M3 16v5h5\"/><path d=\"m3 21 6-6\"/><path d=\"M3 8V3h5\"/><path d=\"M9 9 3 3\"/>",
        ["arrow-right"] = "<path d=\"M5 12h14\"/><path d=\"m12 5 7 7-7 7\"/>",
        ["arrow-left"] = "<path d=\"m12 19-7-7 7-7\"/><path d=\"M19 12H5\"/>",
        ["arrow-in"] = "<path d=\"m15 10-5 5 5 5\"/><path d=\"M4 4v7a4 4 0 0 0 4 4h12\"/>",
        ["arrow-out"] = "<path d=\"m15 10 5 5-5 5\"/><path d=\"M4 4v7a4 4 0 0 0 4 4h12\"/>",
        ["file-code"] = "<path d=\"M10 12.5 8 15l2 2.5\"/><path d=\"m14 12.5 2 2.5-2 2.5\"/><path d=\"M14 2v4a2 2 0 0 0 2 2h4\"/><path d=\"M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7z\"/>",
        ["info"] = "<circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 16v-4\"/><path d=\"M12 8h.01\"/>",
        ["alert"] = "<path d=\"m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3\"/><path d=\"M12 9v4\"/><path d=\"M12 17h.01\"/>",
        ["parameter"] = "<path d=\"M8 21s-4-3-4-9 4-9 4-9\"/><path d=\"M16 3s4 3 4 9-4 9-4 9\"/><line x1=\"15\" x2=\"9\" y1=\"9\" y2=\"15\"/><line x1=\"9\" x2=\"15\" y1=\"9\" y2=\"15\"/>",
        ["calendar"] = "<path d=\"M8 2v4\"/><path d=\"M16 2v4\"/><rect width=\"18\" height=\"18\" x=\"3\" y=\"4\" rx=\"2\"/><path d=\"M3 10h18\"/>",
        ["external"] = "<path d=\"M15 3h6v6\"/><path d=\"M10 14 21 3\"/><path d=\"M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6\"/>",
    };

    /// <summary>An icon (inline SVG, 24×24, drawn in the text's color).</summary>
    public static string Icon(string name, string css = "icon") =>
        "<svg class=\"" + css + "\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\">" +
        (Icons.TryGetValue(name, out string paths) ? paths : "") + "</svg>";

    /// <summary>The icon of an object's kind: table, view or procedure.</summary>
    public static string KindIcon(SchemaObjectBase item) => Icon(item is TableSchema ? "table" : item is ViewSchema ? "view" : "procedure");
}
