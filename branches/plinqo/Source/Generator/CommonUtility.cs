using System;
using System.Text;
using CodeSmith.Engine;

namespace LinqToSqlShared.Generator
{
    public static class CommonUtility
    {
        public const string TrueLiteral = "true";
        public const string FalseLiteral = "false";

        public static bool IsNullableType(string nativeType)
        {
            if (nativeType.StartsWith("System."))
            {
                System.Type myType = System.Type.GetType(nativeType, false);
                if (myType != null)
                {
                    return myType.IsValueType;
                }
            }
            return false;
        }

        public static string GetFullName(string classNamespace, string className)
        {
            return string.Format("{0}.{1}", classNamespace, className);
        }

        public static string GetClassName(string name)
        {
            if (name.IndexOf('.') < 0)
                return name;

            string[] namespaces = name.Split(new Char[] { '.' });
            return namespaces[namespaces.Length - 1];
        }

        public static string GetNamespace(string name)
        {
            if (name.IndexOf('.') < 0)
                return name;

            string[] namespaces = name.Split(new Char[] { '.' });
            return String.Join(".", namespaces, 0, namespaces.Length - 1);
        }

        public static string GetFieldName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            if (propertyName.Length > 1)
                return "_" + propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
            else
                return "_" + propertyName;
        }

        public static string GetParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("propertyName");

            if (name.Length > 1)
                return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
            else
                return name.Substring(0, 1).ToLowerInvariant();
        }

        public static string ToBooleanString(bool value)
        {
            return value ? TrueLiteral : FalseLiteral;
        }

        public static string ToSpaced(string name)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && sb.Length != 0)
                    sb.Append(' ');

                sb.Append(name[i]);
            }
            return sb.ToString();
        }

    }
}