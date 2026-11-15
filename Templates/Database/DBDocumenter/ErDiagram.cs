// The relationship diagram, drawn while generating as inline SVG (it works offline, and prints). Tables are boxes with
// their key columns; a foreign key is a curve from the column that refers to the column it refers to, with crow's-foot
// ends. The layout is layered: a table's parents (the tables it refers to) are left of it, its children right. Each
// group of related tables is laid out on its own, and the groups are packed into rows.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using SchemaExplorer;

public sealed class ErDiagram
{
    private const double HeaderHeight = 38, RowHeight = 22, PadY = 6, FooterHeight = 26;
    private const double GapY = 26, ComponentGapX = 64, ComponentGapY = 56, Margin = 28;
    private const double NameAdvance = 6.7; // 12px sans, on average (generous)
    private const double TypeAdvance = 6.35; // 10.5px monospace
    private const double TitleAdvance = 7.9; // 13px semibold sans, on average
    private const double MinWidth = 184, MaxWidth = 320;
    private const int MaxRows = 6, MaxPerColumn = 8;

    private sealed class Node
    {
        public TableSchema Table;
        public string Key, Title, Href;
        public int Columns, More, Rank;
        public bool Current;
        public double X, Y, W, H, Order;
        public List<Row> Rows = new List<Row>();
        public List<Edge> Edges = new List<Edge>();
    }

    private sealed class Row
    {
        public string Name, Type;
        public bool Primary, Foreign;
    }

    private sealed class Edge
    {
        public Node Child, Parent;
        public string ChildColumn, ParentColumn, Name;
        public bool Optional, OneToOne;
        public Node Other(Node n) => n == Child ? Parent : Child;
    }

    private readonly DocSite _site;
    private readonly string _root;
    private readonly List<Node> _nodes = new List<Node>();
    private readonly List<Edge> _edges = new List<Edge>();
    private double _width, _height;

    private ErDiagram(DocSite site, string root)
    {
        _site = site;
        _root = root;
    }

    /// <summary>The whole database's diagram, packed to show largest in a view of about targetWidth by targetHeight.</summary>
    public static string Database(DocSite site, string root, double targetWidth, double targetHeight)
    {
        var diagram = new ErDiagram(site, root);
        diagram.Build(site.Tables, null);
        diagram.LayoutAll(targetWidth, targetHeight);
        return diagram.Svg("er-diagram", "Relationship diagram of " + site.Database.Name);
    }

    /// <summary>A table with the tables it refers to (left) and those that refer to it (right); "" when it has none.</summary>
    public static string Neighborhood(DocSite site, TableSchema table, string root)
    {
        string key = DocSite.Key(table);
        var parents = table.ForeignKeys.Cast<TableKeySchema>().Select(k => site.Find(k.PrimaryKeyTable)).Where(t => t != null && DocSite.Key(t) != key);
        var children = site.ReferencedBy(table).Select(k => site.Find(k.ForeignKeyTable)).Where(t => t != null && DocSite.Key(t) != key);
        bool self = table.ForeignKeys.Cast<TableKeySchema>().Any(k => DocSite.Key(k.PrimaryKeyTable) == key);
        var parentKeys = new HashSet<string>(parents.Select(DocSite.Key));
        var tables = new List<TableSchema> { site.Find(table) ?? table };
        tables.AddRange(parents.GroupBy(DocSite.Key).Select(g => g.First()));
        tables.AddRange(children.Where(t => !parentKeys.Contains(DocSite.Key(t))).GroupBy(DocSite.Key).Select(g => g.First()));
        if (tables.Count == 1 && !self)
            return "";

        var diagram = new ErDiagram(site, root);
        diagram.Build(tables, key);
        foreach (var node in diagram._nodes)
            node.Rank = node.Key == key ? 1 : parentKeys.Contains(node.Key) ? 0 : 2;
        diagram.LayoutComponent(diagram._nodes, fixedRanks: true);
        diagram.Normalize(diagram._nodes, 8, 8);
        diagram._width = diagram._nodes.Max(n => n.X + n.W) + 8 + (self ? 64 : 0);
        diagram._height = diagram._nodes.Max(n => n.Y + n.H) + 8;
        return diagram.Svg("er-mini", "Relationships of " + DocSite.Title(table));
    }

    // The graph

    private void Build(IEnumerable<TableSchema> tables, string current)
    {
        var byKey = new Dictionary<string, Node>(StringComparer.Ordinal);
        foreach (var table in tables)
        {
            var node = new Node { Table = table, Key = DocSite.Key(table), Title = DocSite.Title(table), Href = _root + _site.Page(table), Columns = table.Columns.Count };
            node.Current = node.Key == current;
            byKey[node.Key] = node;
            _nodes.Add(node);
        }
        foreach (var node in _nodes)
        {
            foreach (TableKeySchema key in node.Table.ForeignKeys)
            {
                if (!byKey.TryGetValue(DocSite.Key(key.PrimaryKeyTable), out var parent))
                    continue;
                // the neighborhood shows the current table's keys only
                if (current != null && node.Key != current && parent.Key != current)
                    continue;
                var columns = key.ForeignKeyMemberColumns.Cast<MemberColumnSchema>().ToList();
                var referenced = key.PrimaryKeyMemberColumns.Cast<MemberColumnSchema>().ToList();
                var edge = new Edge
                {
                    Child = node,
                    Parent = parent,
                    Name = key.Name,
                    ChildColumn = columns.Select(c => c.Name).FirstOrDefault(),
                    ParentColumn = referenced.Select(c => c.Name).FirstOrDefault(),
                    Optional = columns.Any(c => c.AllowDBNull),
                    OneToOne = DocSite.IsOneToOne(key),
                };
                _edges.Add(edge);
                node.Edges.Add(edge);
                if (parent != node)
                    parent.Edges.Add(edge);
            }
        }
        foreach (var node in _nodes)
            AddRows(node);
    }

    // A box's rows: its key columns (those its edges join), then its other columns up to MaxRows, and how many more.
    private void AddRows(Node node)
    {
        var joined = new HashSet<string>(node.Edges.SelectMany(e => new[] { e.Child == node ? e.ChildColumn : null, e.Parent == node ? e.ParentColumn : null }).Where(c => c != null));
        var columns = node.Table.Columns.Cast<ColumnSchema>().ToList();
        var keys = columns.Where(c => c.IsPrimaryKeyMember).Concat(columns.Where(c => !c.IsPrimaryKeyMember && (c.IsForeignKeyMember || joined.Contains(c.Name)))).ToList();
        var shown = new List<ColumnSchema>(keys);
        foreach (var column in columns)
        {
            if (shown.Count >= Math.Max(MaxRows, keys.Count))
                break;
            if (!shown.Contains(column))
                shown.Add(column);
        }
        if (columns.Count - shown.Count == 1)
            shown.Add(columns.First(c => !shown.Contains(c)));
        foreach (var column in shown)
            node.Rows.Add(new Row { Name = column.Name, Type = DocSite.Type(column), Primary = column.IsPrimaryKeyMember, Foreign = column.IsForeignKeyMember });
        node.More = columns.Count - shown.Count;

        double title = Measure(node.Title, TitleAdvance) + 34 + 18 + Measure(node.Columns.ToString(CultureInfo.InvariantCulture), 7) + 14;
        double rows = node.Rows.Count == 0 ? 0 : node.Rows.Max(r => 30 + Measure(r.Name, NameAdvance) + 20 + Measure(r.Type, TypeAdvance) + 12);
        node.W = Math.Round(Math.Min(MaxWidth, Math.Max(MinWidth, Math.Max(title, rows))));
        node.H = HeaderHeight + (node.Rows.Count > 0 ? PadY * 2 + node.Rows.Count * RowHeight : 0) + (node.More > 0 ? FooterHeight - (node.Rows.Count > 0 ? PadY : 0) : 0);
        if (node.Rows.Count == 0 && node.More == 0)
            node.H = HeaderHeight + 10;
    }

    private static double Measure(string text, double advance) => (text ?? "").Length * advance;

    // The y of a column's row in its box (its middle), or the header's middle.
    private static double Port(Node node, string column)
    {
        int i = node.Rows.FindIndex(r => r.Name == column);
        return i < 0 ? HeaderHeight / 2 : HeaderHeight + PadY + i * RowHeight + RowHeight / 2;
    }

    // Layout

    private void LayoutAll(double targetWidth, double targetHeight)
    {
        // the groups of related tables (a table on its own is a group of one)
        var parent = _nodes.ToDictionary(n => n, n => n);
        Node FindRoot(Node n) { while (parent[n] != n) n = parent[n] = parent[parent[n]]; return n; }
        foreach (var edge in _edges)
            parent[FindRoot(edge.Child)] = FindRoot(edge.Parent);
        var components = _nodes.GroupBy(FindRoot).Select(g => g.ToList())
            .OrderByDescending(c => c.Count > 1).ThenByDescending(c => c.Count).ThenBy(c => c[0].Title, StringComparer.OrdinalIgnoreCase).ToList();

        var boxes = new List<(List<Node> Nodes, double W, double H)>();
        foreach (var component in components)
        {
            LayoutComponent(component, fixedRanks: false);
            Normalize(component, 0, 0);
            boxes.Add((component, component.Max(n => n.X + n.W), component.Max(n => n.Y + n.H)));
        }

        // Pack the groups into rows (shelves), trying widths from the widest group's to all of them in a row, and keep
        // the packing that the page's view (about targetWidth by targetHeight) shows largest: up to 1:1, then the
        // smallest. A row's groups sit in its middle.
        double widest = boxes.Max(b => b.W), all = boxes.Sum(b => b.W) + (boxes.Count - 1) * ComponentGapX;
        List<List<(List<Node> Nodes, double W, double H)>> best = null;
        double bestScale = -1, bestArea = double.MaxValue;
        for (int i = 0; i <= 48; i++)
        {
            double limit = widest + (all - widest) * i / 48.0;
            var shelves = Pack(boxes, limit);
            double w = shelves.Max(Used) + Margin * 2;
            double h = shelves.Sum(r => r.Max(b => b.H)) + (shelves.Count - 1) * ComponentGapY + Margin * 2;
            double scale = Math.Min(1, Math.Min(targetWidth / w, targetHeight / h));
            if (scale > bestScale + 0.004 || (Math.Abs(scale - bestScale) <= 0.004 && w * h < bestArea))
            {
                best = shelves;
                bestScale = scale;
                bestArea = w * h;
            }
        }
        double width = best.Max(Used);
        double y = Margin;
        foreach (var shelf in best)
        {
            double x = Margin + (width - Used(shelf)) / 2;
            double tallest = shelf.Max(b => b.H);
            foreach (var box in shelf)
            {
                Normalize(box.Nodes, x, y + (tallest - box.H) / 2);
                x += box.W + ComponentGapX;
            }
            y += tallest + ComponentGapY;
        }
        _width = width + Margin * 2;
        _height = Math.Max(y - ComponentGapY + Margin, Margin * 2);
    }

    // First fit: each group (the tallest first) on the first row with room for it.
    private static List<List<(List<Node> Nodes, double W, double H)>> Pack(List<(List<Node> Nodes, double W, double H)> boxes, double width)
    {
        var shelves = new List<List<(List<Node> Nodes, double W, double H)>>();
        foreach (var box in boxes.OrderByDescending(b => b.H).ThenByDescending(b => b.Nodes.Count))
        {
            var shelf = shelves.FirstOrDefault(s => Used(s) + ComponentGapX + box.W <= width + 0.5);
            if (shelf == null)
                shelves.Add(shelf = new List<(List<Node> Nodes, double W, double H)>());
            shelf.Add(box);
        }
        return shelves;
    }

    private static double Used(List<(List<Node> Nodes, double W, double H)> shelf) => shelf.Sum(b => b.W) + (shelf.Count - 1) * ComponentGapX;

    private void LayoutComponent(List<Node> nodes, bool fixedRanks)
    {
        if (!fixedRanks)
            Rank(nodes);
        int ranks = nodes.Max(n => n.Rank) + 1;

        // a rank with many tables is split into columns (a schema's lookup tables), so the diagram isn't one tall column
        var layers = new List<List<Node>>();
        for (int r = 0; r < ranks; r++)
            layers.Add(nodes.Where(n => n.Rank == r).OrderBy(n => n.Title, StringComparer.OrdinalIgnoreCase).ToList());
        layers = layers.Where(l => l.Count > 0).ToList(); // a neighborhood without parents (or children)
        for (int r = 0; r < layers.Count; r++)
            foreach (var node in layers[r])
                node.Rank = r;
        Order(nodes, layers);
        if (!fixedRanks && layers.Any(l => l.Count > MaxPerColumn))
        {
            var split = new List<List<Node>>();
            foreach (var layer in layers)
            {
                int columns = (layer.Count + MaxPerColumn - 1) / MaxPerColumn;
                int per = (layer.Count + columns - 1) / Math.Max(columns, 1);
                for (int c = 0; c < columns; c++)
                    split.Add(layer.Skip(c * per).Take(per).ToList());
            }
            layers = split.Where(l => l.Count > 0).ToList();
            for (int r = 0; r < layers.Count; r++)
                foreach (var node in layers[r])
                    node.Rank = r;
        }

        // x: a column per rank, as wide as its widest box; the gap grows with the edges that cross it
        double x = 0;
        for (int r = 0; r < layers.Count; r++)
        {
            double w = layers[r].Max(n => n.W);
            foreach (var node in layers[r])
            {
                node.W = w;
                node.X = x;
            }
            int crossing = _edges.Count(e => nodes.Contains(e.Child) && Math.Min(e.Child.Rank, e.Parent.Rank) <= r && Math.Max(e.Child.Rank, e.Parent.Rank) > r);
            x += w + 76 + Math.Min(56, crossing * 6);
        }

        // y: stacked, then moved toward the rows their edges join, keeping their order (isotonic regression)
        double tallest = layers.Max(l => l.Sum(n => n.H) + (l.Count - 1) * GapY);
        foreach (var layer in layers)
        {
            double y = (tallest - (layer.Sum(n => n.H) + (layer.Count - 1) * GapY)) / 2;
            foreach (var node in layer)
            {
                node.Y = y;
                y += node.H + GapY;
            }
        }
        for (int iteration = 0; iteration < 40; iteration++)
        {
            bool down = iteration % 2 == 0;
            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[down ? i : layers.Count - 1 - i];
                var desired = layer.Select(n =>
                {
                    var ports = n.Edges.Where(e => e.Other(n) != n && nodes.Contains(e.Other(n)))
                        .Select(e => e.Other(n).Y + Port(e.Other(n), e.Other(n) == e.Child ? e.ChildColumn : e.ParentColumn) - Port(n, n == e.Child ? e.ChildColumn : e.ParentColumn))
                        .ToList();
                    return ports.Count == 0 ? n.Y : ports.Average();
                }).ToList();
                Place(layer, desired);
            }
        }
    }

    // Ranks: a table right of every table it refers to (the longest path), cycles broken where they're found; a table
    // that refers to none sits just left of the first it's referred to by.
    private void Rank(List<Node> nodes)
    {
        var parentsOf = nodes.ToDictionary(n => n, n => n.Edges.Where(e => e.Child == n && e.Parent != n && nodes.Contains(e.Parent)).Select(e => e.Parent).Distinct().ToList());
        var childrenOf = nodes.ToDictionary(n => n, n => n.Edges.Where(e => e.Parent == n && e.Child != n && nodes.Contains(e.Child)).Select(e => e.Child).Distinct().ToList());
        var done = new HashSet<Node>();
        var order = new List<Node>();
        foreach (var node in nodes)
            node.Rank = 0;
        while (done.Count < nodes.Count)
        {
            // the next table whose parents are all ranked; in a cycle, the one with the fewest left
            var next = nodes.Where(n => !done.Contains(n)).OrderBy(n => parentsOf[n].Count(p => !done.Contains(p))).ThenBy(n => n.Title, StringComparer.OrdinalIgnoreCase).First();
            done.Add(next);
            order.Add(next);
            foreach (var parent in parentsOf[next].Where(done.Contains))
                if (parent != next && order.IndexOf(parent) < order.IndexOf(next))
                    next.Rank = Math.Max(next.Rank, parent.Rank + 1);
        }
        for (int i = order.Count - 1; i >= 0; i--)
        {
            var node = order[i];
            var children = childrenOf[node].Where(c => c.Rank > node.Rank).ToList();
            if (parentsOf[node].All(p => p.Rank >= node.Rank) && children.Count > 0)
                node.Rank = Math.Max(node.Rank, children.Min(c => c.Rank) - 1);
        }
        int min = nodes.Min(n => n.Rank);
        foreach (var node in nodes)
            node.Rank -= min;
    }

    // Each rank's order: by the average position of the tables each one's edges join (barycenters), swept both ways,
    // keeping the order with the fewest crossings.
    private void Order(List<Node> nodes, List<List<Node>> layers)
    {
        void Number()
        {
            foreach (var layer in layers)
                for (int i = 0; i < layer.Count; i++)
                    layer[i].Order = (i + 0.5) / layer.Count;
        }
        Number();
        var best = layers.Select(l => l.ToList()).ToList();
        int fewest = Crossings(nodes);
        for (int iteration = 0; iteration < 24; iteration++)
        {
            bool down = iteration % 2 == 0;
            for (int i = 0; i < layers.Count; i++)
            {
                int r = down ? i : layers.Count - 1 - i;
                var layer = layers[r];
                var centers = layer.ToDictionary(n => n, n =>
                {
                    var near = n.Edges.Select(e => e.Other(n)).Where(o => o != n && nodes.Contains(o) && (down ? o.Rank < r : o.Rank > r)).ToList();
                    if (near.Count == 0)
                        near = n.Edges.Select(e => e.Other(n)).Where(o => o != n && nodes.Contains(o)).ToList();
                    return near.Count == 0 ? n.Order : near.Average(o => o.Order);
                });
                layers[r] = layer.OrderBy(n => centers[n]).ThenBy(n => n.Order).ToList();
                for (int j = 0; j < layers[r].Count; j++)
                    layers[r][j].Order = (j + 0.5) / layers[r].Count;
            }
            int crossings = Crossings(nodes);
            if (crossings < fewest)
            {
                fewest = crossings;
                best = layers.Select(l => l.ToList()).ToList();
            }
        }
        for (int r = 0; r < layers.Count; r++)
            layers[r] = best[r];
        Number();
    }

    private int Crossings(List<Node> nodes)
    {
        var edges = _edges.Where(e => e.Child != e.Parent && nodes.Contains(e.Child) && nodes.Contains(e.Parent)).ToList();
        int count = 0;
        for (int i = 0; i < edges.Count; i++)
            for (int j = i + 1; j < edges.Count; j++)
            {
                var (a1, a2) = Ends(edges[i]);
                var (b1, b2) = Ends(edges[j]);
                if (a1.Rank == b1.Rank && a2.Rank == b2.Rank && (a1.Order - b1.Order) * (a2.Order - b2.Order) < 0)
                    count++;
            }
        return count;
    }

    private static (Node Left, Node Right) Ends(Edge e) => e.Parent.Rank <= e.Child.Rank ? (e.Parent, e.Child) : (e.Child, e.Parent);

    // Places a rank's boxes as near their desired tops as their order and the gaps between them allow: the closest
    // positions in the least-squares sense (pool adjacent violators).
    private static void Place(List<Node> layer, List<double> desired)
    {
        int n = layer.Count;
        var offset = new double[n];
        for (int i = 1; i < n; i++)
            offset[i] = offset[i - 1] + layer[i - 1].H + GapY;
        var blocks = new List<(double Sum, int Count)>();
        for (int i = 0; i < n; i++)
        {
            blocks.Add((desired[i] - offset[i], 1));
            while (blocks.Count > 1 && blocks[blocks.Count - 2].Sum / blocks[blocks.Count - 2].Count > blocks[blocks.Count - 1].Sum / blocks[blocks.Count - 1].Count)
            {
                var last = blocks[blocks.Count - 1];
                var before = blocks[blocks.Count - 2];
                blocks.RemoveAt(blocks.Count - 1);
                blocks[blocks.Count - 1] = (before.Sum + last.Sum, before.Count + last.Count);
            }
        }
        int k = 0;
        foreach (var block in blocks)
            for (int c = 0; c < block.Count; c++, k++)
                layer[k].Y = block.Sum / block.Count + offset[k];
    }

    private void Normalize(List<Node> nodes, double left, double top)
    {
        double minX = nodes.Min(n => n.X), minY = nodes.Min(n => n.Y);
        foreach (var node in nodes)
        {
            node.X = Math.Round(node.X - minX + left);
            node.Y = Math.Round(node.Y - minY + top);
        }
    }

    // SVG

    private string Svg(string css, string label)
    {
        var svg = new StringBuilder();
        string w = F(Math.Ceiling(_width)), h = F(Math.Ceiling(_height));
        svg.Append("<svg class=\"er ").Append(css).Append("\" viewBox=\"0 0 ").Append(w).Append(' ').Append(h).Append("\" width=\"").Append(w)
            .Append("\" height=\"").Append(h).Append("\" role=\"img\" aria-label=\"").Append(E(label)).Append("\" data-width=\"").Append(w)
            .Append("\" data-height=\"").Append(h).Append("\">\n");
        svg.Append("<g class=\"er-edges\">\n");
        foreach (var edge in _edges)
            AppendEdge(svg, edge);
        svg.Append("</g>\n<g class=\"er-nodes\">\n");
        foreach (var node in _nodes)
            AppendNode(svg, node);
        svg.Append("</g>\n</svg>");
        return svg.ToString();
    }

    private void AppendNode(StringBuilder svg, Node node)
    {
        double w = node.W, h = node.H;
        svg.Append("<a class=\"er-node").Append(node.Current ? " is-current" : "").Append("\" href=\"").Append(E(node.Href)).Append("\" data-key=\"").Append(E(node.Key))
            .Append("\" transform=\"translate(").Append(F(node.X)).Append(' ').Append(F(node.Y)).Append(")\">");
        svg.Append("<title>").Append(E(node.Title + ": " + node.Columns + (node.Columns == 1 ? " column" : " columns"))).Append("</title>");
        svg.Append("<rect class=\"er-box\" width=\"").Append(F(w)).Append("\" height=\"").Append(F(h)).Append("\" rx=\"10\"/>");
        svg.Append("<path class=\"er-head\" d=\"M0.5 10.5a10 10 0 0 1 10-10h").Append(F(w - 21)).Append("a10 10 0 0 1 10 10v").Append(F(HeaderHeight - 10.5))
            .Append("h-").Append(F(w - 1)).Append("z\"/>");
        if (node.Rows.Count > 0 || node.More > 0)
            svg.Append("<path class=\"er-rule\" d=\"M0 ").Append(F(HeaderHeight)).Append("h").Append(F(w)).Append("\"/>");
        svg.Append("<g class=\"er-glyph\" transform=\"translate(12 12) scale(0.583)\"><path d=\"M12 3v18\"/><rect width=\"18\" height=\"18\" x=\"3\" y=\"3\" rx=\"2\"/><path d=\"M3 9h18\"/><path d=\"M3 15h18\"/></g>");
        string count = node.Columns.ToString(CultureInfo.InvariantCulture);
        double countWidth = Math.Max(22, count.Length * 7 + 12);
        int titleChars = (int)((w - 34 - countWidth - 18) / TitleAdvance);
        svg.Append("<text class=\"er-title\" x=\"34\" y=\"24\">").Append(E(Clip(node.Title, titleChars))).Append("</text>");
        svg.Append("<rect class=\"er-pill\" x=\"").Append(F(w - 12 - countWidth)).Append("\" y=\"10\" width=\"").Append(F(countWidth)).Append("\" height=\"18\" rx=\"9\"/>");
        svg.Append("<text class=\"er-count\" x=\"").Append(F(w - 12 - countWidth / 2)).Append("\" y=\"23\" text-anchor=\"middle\">").Append(count).Append("</text>");
        double y = HeaderHeight + PadY;
        foreach (var row in node.Rows)
        {
            svg.Append("<g class=\"er-row").Append(row.Primary ? " is-pk" : row.Foreign ? " is-fk" : "").Append("\" transform=\"translate(0 ").Append(F(y)).Append(")\">");
            if (row.Primary)
                svg.Append("<g class=\"er-key\" transform=\"translate(11 5) scale(0.5)\"><path d=\"M2.586 17.414A2 2 0 0 0 2 18.828V21a1 1 0 0 0 1 1h3a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h1a1 1 0 0 0 1-1v-1a1 1 0 0 1 1-1h.172a2 2 0 0 0 1.414-.586l.814-.814a6.5 6.5 0 1 0-4-4z\"/><circle cx=\"16.5\" cy=\"7.5\" r=\".5\"/></g>");
            else if (row.Foreign)
                svg.Append("<g class=\"er-link\" transform=\"translate(11 5) scale(0.5)\"><path d=\"M9 17H7A5 5 0 0 1 7 7h2\"/><path d=\"M15 7h2a5 5 0 1 1 0 10h-2\"/><path d=\"M8 12h8\"/></g>");
            // the name and the type share the row: the type gets what the name leaves, and at least 40%
            double room = w - 30 - 12 - 14;
            double nameWidth = Math.Min(Measure(row.Name, NameAdvance), room * 0.6);
            int typeChars = (int)((room - nameWidth) / TypeAdvance);
            int nameChars = (int)((room - Math.Min(Measure(row.Type, TypeAdvance), room * 0.4)) / NameAdvance);
            svg.Append("<text class=\"er-col\" x=\"30\" y=\"15\">").Append(E(Clip(row.Name, nameChars))).Append("</text>");
            svg.Append("<text class=\"er-type\" x=\"").Append(F(w - 12)).Append("\" y=\"15\" text-anchor=\"end\">").Append(E(Clip(row.Type, typeChars))).Append("</text>");
            svg.Append("</g>");
            y += RowHeight;
        }
        if (node.More > 0)
        {
            double top = node.Rows.Count > 0 ? y + PadY : HeaderHeight;
            svg.Append("<path class=\"er-rule is-faint\" d=\"M12 ").Append(F(top)).Append("h").Append(F(w - 24)).Append("\"/>");
            svg.Append("<text class=\"er-more\" x=\"30\" y=\"").Append(F(top + 14)).Append("\">+ ").Append(node.More).Append(node.More == 1 ? " more column" : " more columns").Append("</text>");
        }
        svg.Append("</a>\n");
    }

    private void AppendEdge(StringBuilder svg, Edge edge)
    {
        Node child = edge.Child, parent = edge.Parent;
        double py = parent.Y + Port(parent, edge.ParentColumn), cy = child.Y + Port(child, edge.ChildColumn);
        double px, cx, pd, cd; // the ends, and which way each leaves its box (+1 right, -1 left)
        if (child == parent || child.Rank == parent.Rank)
        {
            px = parent.X + parent.W; cx = child.X + child.W; pd = 1; cd = 1;
            if (child == parent && Math.Abs(py - cy) < 1)
                cy += 8;
        }
        else if (parent.Rank < child.Rank)
        {
            px = parent.X + parent.W; cx = child.X; pd = 1; cd = -1;
        }
        else
        {
            px = parent.X; cx = child.X + child.W; pd = -1; cd = 1;
        }
        string path;
        if (pd == cd)
        {
            double reach = 44 + Math.Abs(py - cy) * 0.18;
            path = "M" + F(px) + " " + F(py) + "C" + F(px + pd * reach) + " " + F(py) + " " + F(cx + cd * reach) + " " + F(cy) + " " + F(cx) + " " + F(cy);
        }
        else
        {
            double dx = Math.Max(40, Math.Abs(cx - px) * 0.5);
            path = "M" + F(px) + " " + F(py) + "C" + F(px + pd * dx) + " " + F(py) + " " + F(cx + cd * dx) + " " + F(cy) + " " + F(cx) + " " + F(cy);
        }
        string tip = DocSite.Title(child.Table) + "." + edge.ChildColumn + " → " + DocSite.Title(parent.Table) + "." + edge.ParentColumn + (string.IsNullOrEmpty(edge.Name) ? "" : " (" + edge.Name + ")");
        svg.Append("<g class=\"er-edge\" data-from=\"").Append(E(child.Key)).Append("\" data-to=\"").Append(E(parent.Key)).Append("\"><title>").Append(E(tip)).Append("</title>");
        svg.Append("<path class=\"er-hit\" d=\"").Append(path).Append("\"/>");
        svg.Append("<path class=\"er-line\" d=\"").Append(path).Append("\"/>");

        // the parent's end: exactly one (||), or zero or one (o|) where the key's columns may be null
        var marks = new StringBuilder();
        marks.Append(Bar(px + pd * 7, py));
        if (edge.Optional)
            svg.Append(Circle(px + pd * 15, py));
        else
            marks.Append(Bar(px + pd * 12, py));
        // the child's end: zero or many (o<), or zero or one (o|) where the key is the child's primary key
        if (edge.OneToOne)
            marks.Append(Bar(cx + cd * 7, cy));
        else
            marks.Append("M").Append(F(cx + cd * 12)).Append(' ').Append(F(cy)).Append("L").Append(F(cx)).Append(' ').Append(F(cy - 6))
                .Append("M").Append(F(cx + cd * 12)).Append(' ').Append(F(cy)).Append("L").Append(F(cx)).Append(' ').Append(F(cy + 6));
        svg.Append(Circle(cx + cd * (edge.OneToOne ? 15 : 18), cy));
        svg.Append("<path class=\"er-mark\" d=\"").Append(marks).Append("\"/>");
        svg.Append("</g>\n");
    }

    private static string Bar(double x, double y) => "M" + F(x) + " " + F(y - 6) + "V" + F(y + 6);

    private static string Circle(double x, double y) => "<circle class=\"er-dot\" cx=\"" + F(x) + "\" cy=\"" + F(y) + "\" r=\"3.5\"/>";

    private static string Clip(string text, int chars)
    {
        text = text ?? "";
        chars = Math.Max(chars, 4);
        return text.Length <= chars ? text : text.Substring(0, chars - 1) + "…";
    }

    private static string F(double value) => Math.Round(value, 1).ToString(CultureInfo.InvariantCulture);

    private static string E(string text) => WebUtility.HtmlEncode(text ?? "");
}
