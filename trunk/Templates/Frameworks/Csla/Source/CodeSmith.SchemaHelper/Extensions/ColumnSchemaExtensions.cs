using System;
using SchemaExplorer;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for ColumnSchema
    /// </summary>
    public static class ColumnSchemaExtensions
    {
        #region Constant(s)

        private const string CS_Description = "CS_Description";
        private const string IS_IDENTITY = "CS_IsIdentity";
        private const string IS_COMPUTED = "CS_IsComputed";

        #endregion

        #region Public Method(s)

        public static bool IsRowVersion(this ColumnSchema column)
        {
            if (column.NativeType.ToLower() == "timestamp" || column.NativeType.ToLower() == "rowversion")
                return true;

            if (Configuration.Instance.UseRowVersionRegex && Configuration.Instance.RowVersionColumnRegex.IsMatch(column.Name))
                return true;

            return false;
        }

        public static bool HasAlias(this ColumnSchema column)
        {
            return column.ExtendedProperties.Contains(Configuration.Instance.AliasExtendedProperty);
        }

        public static string ResolveSystemType(this ColumnSchema column)
        {
            return column.ResolveSystemType(true);
        }

        public static string ResolveSystemType(this ColumnSchema column, bool canAppendNullable)
        {
            string result = column.SystemType.ToString();

            if (result.Equals("System.DateTime", StringComparison.InvariantCultureIgnoreCase))
                result = "SmartDate";

            if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                result = result.Replace("[]", "()");
            bool appendNull = column.AllowDBNull && column.SystemType.IsValueType && canAppendNullable;

            if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                return (appendNull) ? string.Format("System.Nullable(Of {0})", result) : result;

            return (appendNull) ? string.Format("{0}?", result) : result;
        }

        #region GetName

        public static string GetName(this ColumnSchema column)
        {
            string name = (column.HasAlias()) ? column.ExtendedProperties[Configuration.Instance.AliasExtendedProperty].Value.ToString() : column.Name;

            return Configuration.Instance.ValidateName(column, name);
        }

        public static string GetName(this ColumnSchema column, TableSchema table, AssociationType associationType)
        {
            if (column.HasAlias())
                return column.GetName();

            bool plural = (associationType != AssociationType.ManyToOne && associationType != AssociationType.OneToZeroOrOne);

            string name = (Configuration.Instance.NamingProperty.AssociationNaming == AssociationNaming.Column)
                              ? column.GetName()
                              : table.ClassName();

            if (plural)
            {
                if (Configuration.Instance.NamingProperty.AssociationSuffix == AssociationSuffix.List)
                    name = String.Concat(name, Configuration.Instance.ListSuffix);
                else if (Configuration.Instance.NamingProperty.AssociationSuffix == AssociationSuffix.Plural)
                    name = StringUtil.ToPlural(name);
            }

            return Configuration.Instance.ValidateName(column, name);
        }

        #endregion

        public static string ResolveDescription(this ColumnSchema column)
        {
            if (column.ExtendedProperties.Contains(CS_Description))
                 return column.ExtendedProperties[CS_Description].Value.ToString().Trim();

            return string.Empty;
        }

        public static bool IsIdentity(this ColumnSchema column)
        {
            return (column.ExtendedProperties.Contains(IS_IDENTITY) && ((bool) column.ExtendedProperties[IS_IDENTITY].Value));
        }

        public static bool IsComputed(this ColumnSchema column)
        {
            return (column.ExtendedProperties.Contains(IS_COMPUTED) && ((bool)column.ExtendedProperties[IS_COMPUTED].Value));
        }

        #endregion
    }
}