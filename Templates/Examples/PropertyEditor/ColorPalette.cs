// The type of Palette.cst's Palette property, and what CodeSmith needs to read and write it:
//   - ColorPaletteConverter: the value as text. The property grid shows it, the web editor (palette-editor.js) reads
//     and sets it, and so do -p:Palette=... on the command line and the Property directive's Default.
//   - ColorPaletteSerializer: the value as XML, in a .csp and in the values the grid remembers.
// Both check the value the same way (ColorPalette.Parse and its constructor), so a bad value is refused wherever it
// comes from, with the same message.
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CodeSmith.Engine;

namespace CodeSmith.Samples
{
    /// <summary>A named color: its value in a light theme and in a dark one (#rrggbb).</summary>
    public sealed class PaletteColor
    {
        public PaletteColor(string name, string light, string dark)
        {
            Name = name;
            Light = light;
            Dark = dark;
        }

        /// <summary>Lowercase letters, digits and dashes: primary, text-muted.</summary>
        public string Name { get; }
        public string Light { get; }
        public string Dark { get; }
    }

    /// <summary>The colors of a theme, in order, each with a light and a dark value.</summary>
    [TypeConverter(typeof(ColorPaletteConverter))]
    [PropertySerializer(typeof(ColorPaletteSerializer))]
    public sealed class ColorPalette : IReadOnlyList<PaletteColor>
    {
        private static readonly Regex NamePattern = new Regex("^[a-z][a-z0-9]*(-[a-z0-9]+)*$");
        private static readonly Regex ColorPattern = new Regex("^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$");
        private readonly List<PaletteColor> _colors;

        /// <summary>A palette of these colors. Throws FormatException when one isn't valid.</summary>
        public ColorPalette(IEnumerable<PaletteColor> colors)
        {
            _colors = new List<PaletteColor>();
            foreach (PaletteColor color in colors)
            {
                if (color.Name == null || !NamePattern.IsMatch(color.Name))
                    throw new FormatException("\"" + color.Name + "\" isn't a color's name: lowercase letters, digits and dashes, starting with a letter (primary, text-muted).");
                if (_colors.Any(c => c.Name == color.Name))
                    throw new FormatException("Two colors are named " + color.Name + ".");
                _colors.Add(new PaletteColor(color.Name, Normalize(color.Name, "light", color.Light), Normalize(color.Name, "dark", color.Dark)));
            }
            if (_colors.Count == 0)
                throw new FormatException("A palette has at least one color.");
        }

        /// <summary>
        /// A palette from its text: "name: #light / #dark" for each color, separated by semicolons; a color the same in
        /// both themes needs only one value ("name: #rrggbb").
        /// </summary>
        public static ColorPalette Parse(string text)
        {
            var colors = new List<PaletteColor>();
            foreach (string entry in (text ?? "").Split(';').Select(e => e.Trim()).Where(e => e.Length > 0))
            {
                int colon = entry.IndexOf(':');
                string[] values = colon < 0 ? new string[0] : entry.Substring(colon + 1).Split('/').Select(v => v.Trim()).ToArray();
                if (colon < 0 || values.Length > 2)
                    throw new FormatException("\"" + entry + "\" isn't a color: name: #light / #dark (or name: #rrggbb for both).");
                colors.Add(new PaletteColor(entry.Substring(0, colon).Trim(), values[0], values.Length == 2 ? values[1] : values[0]));
            }
            return new ColorPalette(colors);
        }

        /// <summary>The palette as text, as Parse reads it.</summary>
        public override string ToString() =>
            string.Join("; ", _colors.Select(c => c.Name + ": " + (c.Light == c.Dark ? c.Light : c.Light + " / " + c.Dark)));

        public PaletteColor this[int index] => _colors[index];
        public int Count => _colors.Count;
        public IEnumerator<PaletteColor> GetEnumerator() => _colors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // #abc is #aabbcc; letters are lowercase
        private static string Normalize(string name, string theme, string value)
        {
            string color = (value ?? "").Trim();
            if (!ColorPattern.IsMatch(color))
                throw new FormatException(name + "'s " + theme + " color, \"" + value + "\", isn't a color: #rgb or #rrggbb.");
            if (color.Length == 4)
                color = "#" + color[1] + color[1] + color[2] + color[2] + color[3] + color[3];
            return color.ToLowerInvariant();
        }
    }

    /// <summary>The palette as text (ColorPalette.Parse and ToString).</summary>
    public sealed class ColorPaletteConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value is string text ? ColorPalette.Parse(text) : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string) && value is ColorPalette palette ? palette.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// The palette as XML, a color per element, in a .csp's property and in the values the grid remembers:
    /// <![CDATA[<property name="Palette"><color name="primary" light="#0b5cad" dark="#6cb6ff" /></property>]]>
    /// </summary>
    public sealed class ColorPaletteSerializer : IPropertySerializer
    {
        // The value as it is while the template compiles: nothing to do.
        public object SaveProperty(PropertySerializerContext context, object propertyValue) => propertyValue;

        // A value set on the template (CodeTemplate.SetProperty): the palette, or its text (cs -p:Palette=...).
        public object LoadProperty(PropertySerializerContext context, object propertyValue) =>
            propertyValue is string text ? ColorPalette.Parse(text) : propertyValue;

        public void WritePropertyXml(PropertySerializerContext context, XmlWriter writer, object propertyValue)
        {
            if (!(propertyValue is ColorPalette palette))
                return;
            foreach (PaletteColor color in palette)
            {
                writer.WriteStartElement("color");
                writer.WriteAttributeString("name", color.Name);
                writer.WriteAttributeString("light", color.Light);
                writer.WriteAttributeString("dark", color.Dark);
                writer.WriteEndElement();
            }
        }

        public object ReadPropertyXml(PropertySerializerContext context, XmlNode propertyValue)
        {
            var colors = propertyValue.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "color").Select(e =>
                new PaletteColor(e.GetAttribute("name"), e.GetAttribute("light"), e.HasAttribute("dark") ? e.GetAttribute("dark") : e.GetAttribute("light")));
            return new ColorPalette(colors);
        }

        // The Property directive's Default: the palette's text.
        public object ParseDefaultValue(PropertySerializerContext context, string defaultValue) => ColorPalette.Parse(defaultValue);
    }
}
