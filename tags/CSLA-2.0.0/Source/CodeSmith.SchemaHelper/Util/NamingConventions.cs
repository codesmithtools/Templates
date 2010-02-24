using System;
using System.Text.RegularExpressions;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper.Util
{
    public static class NamingConventions
    {
        private static readonly Regex CleanNumberPrefix = new Regex(@"^\d+");

        public static string PropertyName(string value)
        {
            return PropertyName(value, string.Empty);
        }

        public static string PropertyName(string value, string suffix)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            if (string.IsNullOrEmpty(suffix))
                suffix = string.Empty;

            return string.Format("{0}{1}", StringUtil.ToPascalCase(CleanName(value.Trim())), suffix.Trim());
        }

        public static string PrivateMemberVariableName(string value)
        {
            return PrivateMemberVariableName(value, string.Empty);
        }

        public static string PrivateMemberVariableName(string value, string suffix)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            if (string.IsNullOrEmpty(suffix))
                suffix = string.Empty;

            return string.Format("_{0}{1}", StringUtil.ToCamelCase(CleanName(value.Trim())), suffix.Trim());
        }

        public static string VariableName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            value = StringUtil.ToCamelCase(CleanName(value.Trim()));

            // Lookup in mapping file for overrides.
            value = CodeSmith.SchemaHelper.Configuration.Instance.KeywordRenameAlias[value, value];

            // Replace system keywords..
            value = CodeSmith.SchemaHelper.Configuration.Instance.SystemTypeEscape[value, value];

            return value;
        }

        private static string CleanName(string value)
        {
            // Lookup in mapping file for overrides.
            value = CodeSmith.SchemaHelper.Configuration.Instance.KeywordRenameAlias[value, value];

            return CleanNumberPrefix.Replace(value, string.Empty, 1);
        }
    }
}
