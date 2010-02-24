using System;
using SchemaExplorer;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for ColumnSchema
    /// </summary>
    public static class ColumnSchemaCollectionExtensions
    {
        #region GetName

        public static string GetName(this ColumnSchemaCollection columns, TableSchema table, AssociationType associationType)
        {
            string name = string.Empty;

            bool plural = (associationType != AssociationType.ManyToOne && associationType != AssociationType.OneToZeroOrOne);
            foreach (var column in columns)
            {
                string columnName = (column.HasAlias()) ? column.ExtendedProperties[Configuration.Instance.AliasExtendedProperty].Value.ToString() : column.GetName();
                name += (Configuration.Instance.NamingProperty.AssociationNaming == AssociationNaming.Column)
                              ? columnName
                              : table.ClassName();
            }

            if (plural)
            {
                if (Configuration.Instance.NamingProperty.AssociationSuffix == AssociationSuffix.List)
                    name = String.Concat(name, Configuration.Instance.ListSuffix);
                else if (Configuration.Instance.NamingProperty.AssociationSuffix == AssociationSuffix.Plural)
                    name = StringUtil.ToPlural(name);
            }

            return Configuration.Instance.ValidateName(name);
        }

        #endregion
    }
}