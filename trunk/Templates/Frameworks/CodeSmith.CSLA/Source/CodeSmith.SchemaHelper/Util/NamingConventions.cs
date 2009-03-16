using System;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper.Util
{
    public static class NamingConventions
    {
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

            return string.Format("{0}{1}", StringUtil.ToPascalCase(value), suffix.Trim());
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

            return string.Format("_{0}{1}", StringUtil.ToCamelCase(value.Trim()), suffix.Trim());
        }

        public static string VariableName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return String.Empty;

            return StringUtil.ToCamelCase(value.Trim());
        }
    }
}
