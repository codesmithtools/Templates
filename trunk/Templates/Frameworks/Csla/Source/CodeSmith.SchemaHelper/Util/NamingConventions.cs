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

        public static string PropertyName(string value, bool useKeywordRenameAlias)
        {
            return PropertyName(value, string.Empty, useKeywordRenameAlias);
        }

        public static string PropertyName(string value, string suffix)
        {
            return PropertyName(value, string.Empty, true);
        }

        public static string PropertyName(string value, string suffix, bool useKeywordRenameAlias)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            if (string.IsNullOrEmpty(suffix))
                suffix = string.Empty;

            return string.Format("{0}{1}", StringUtil.ToPascalCase(CleanName(value.Trim(), useKeywordRenameAlias)), suffix.Trim());
        }

        public static string PrivateMemberVariableName(string value)
        {
            return PrivateMemberVariableName(value, string.Empty);
        }

        public static string PrivateMemberVariableName(string value, bool useKeywordRenameAlias)
        {
            return PrivateMemberVariableName(value, string.Empty, useKeywordRenameAlias);
        }

        public static string PrivateMemberVariableName(string value, string suffix)
        {
            return PrivateMemberVariableName(value, string.Empty, true);
        }

        public static string PrivateMemberVariableName(string value, string suffix, bool useKeywordRenameAlias)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            if (string.IsNullOrEmpty(suffix))
                suffix = string.Empty;

            return string.Format("_{0}{1}", StringUtil.ToCamelCase(CleanName(value.Trim(), useKeywordRenameAlias)), suffix.Trim());
        }

        public static string VariableName(string value)
        {
            return VariableName(value, true);
        }

        public static string VariableName(string value, bool useKeywordRenameAlias)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            value = StringUtil.ToCamelCase(CleanName(value.Trim(), useKeywordRenameAlias));

            // Replace system keywords..
            value = Configuration.Instance.SystemTypeEscape[value, value];

            return value;
        }

        private static string CleanName(string value, bool useKeywordRenameAlias)
        {
            // Lookup in mapping file for overrides.
            if (useKeywordRenameAlias)
            {
                value = Configuration.Instance.KeywordRenameAlias[value, value];
            }

            return CleanNumberPrefix.Replace(value, string.Empty, 1);
        }
    }
}
