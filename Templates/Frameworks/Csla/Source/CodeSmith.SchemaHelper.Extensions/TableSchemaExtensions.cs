using System;
using System.Linq;
using System.Text.RegularExpressions;

using CodeSmith.SchemaHelper.Util;

using SchemaExplorer;

using CodeSmith.Engine;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for TableSchema
    /// </summary>
    public static class TableSchemaExtensions
    {
        #region Constant(s)

        private const string CS_Description = "CS_Description";

        #endregion

        private static readonly Regex CleanNumberPrefix = new Regex(@"^\d+");

        #region Public Method(s)

        public static bool ContainsCompositeKeys(this TableSchema table)
        {
            bool result = false;

            IEnumerable<TableKeySchema> keys = table.PrimaryKeys.ToArray().Union(table.ForeignKeys.ToArray());
            foreach (TableKeySchema tks in keys)
            {
                if (tks.ForeignKeyMemberColumns.Count > 1 || tks.PrimaryKeyMemberColumns.Count > 1)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the class name of the current table.
        /// </summary>
        /// <param name="table">Then current table.</param>
        /// <returns>Returns properly formatted class name based off the name of the table.</returns>
        public static string ClassName(this TableSchema table)
        {
            string className;

            if (table.ExtendedProperties.Contains(Configuration.Instance.AliasExtendedProperty))
                className = table.ExtendedProperties[Configuration.Instance.AliasExtendedProperty].Value.ToString();
            else
            {
                className = CleanNumberPrefix.Replace(table.Name, string.Empty, 1);

                if (!String.IsNullOrEmpty(Configuration.Instance.TablePrefix) && className.StartsWith(Configuration.Instance.TablePrefix))
                    className = className.Remove(0, Configuration.Instance.TablePrefix.Length);

                if (Configuration.Instance.NamingProperty.EntityNaming == EntityNaming.Plural && Configuration.Instance.NamingProperty.TableNaming != TableNaming.Plural)
                    className = StringUtil.ToPlural(className);
                else if (Configuration.Instance.NamingProperty.EntityNaming == EntityNaming.Singular && Configuration.Instance.NamingProperty.TableNaming != TableNaming.Singular)
                    className = StringUtil.ToSingular(className);

                if (Configuration.Instance.CleanExpressions.Count > 0)
                {
                    foreach (Regex regex in Configuration.Instance.CleanExpressions)
                    {
                        if (regex.IsMatch(className))
                        {
                            return regex.Replace(className, "");
                        }
                    }
                }
            }

            return Configuration.Instance.ValidateName(className);
        }

        public static string Namespace(this TableSchema table)
        {
            return NamingConventions.PropertyName(table.Database.Name);
        }

        /// <summary>
        /// Checks to see if a Table is part of a ManyToMany relationship. It checks for the following:
        /// 
        /// 1) Table must have Two ForeignKeys.
        /// 2) All columns must be either...
        ///    a) Member of the Primary Key.
        ///    b) Member of a Foreign Key.
        ///    c) A DateTime stamp (CreateDate, EditDate, etc).
        ///    d) Name matches Version Regex.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool IsManyToMany(this TableSchema table)
        {
            // Bypass logic if table contains Extended Property for ManyToMany
            if (table.ExtendedProperties.Contains(Configuration.Instance.ManyToManyExtendedProperty))
            {
                bool manyToMany;
                if (Boolean.TryParse(table.ExtendedProperties[Configuration.Instance.ManyToManyExtendedProperty].Value.ToString(), out manyToMany))
                    return manyToMany;
            }

            // 1) Table must have Two ForeignKeys.
            // 2) All columns must be either...
            //    a) Member of the Primary Key.
            //    b) Member of a Foreign Key.
            //    c) A DateTime stamp (CreateDate, EditDate, etc).
            //    d) Name matches Version Regex.

            // has to be at least 2 columns
            if (table.Columns.Count < 2)
                return false;

            if (table.ForeignKeys.Count != 2)
                return false;

            foreach (ColumnSchema column in table.Columns)
            {
                bool isManyToMany = (column.IsForeignKeyMember || column.IsPrimaryKeyMember || column.SystemType.Equals(typeof(DateTime)) || Configuration.Instance.RowVersionColumnRegex.IsMatch(column.Name));
                if (!isManyToMany)
                    return false;
            }

            return true;
        }

        public static string BuildFKDataBaseColumns(this TableSchema table)
        {
            string columnNames = string.Empty;

            foreach (ColumnSchema column in table.ForeignKeyColumns)
            {
                columnNames += string.Format(", [{0}]", column.Name);
            }

            return columnNames;
        }
		
		public static string ResolveDescription(this TableSchema column)
        {
            if (column.ExtendedProperties.Contains(CS_Description))
                return column.ExtendedProperties[CS_Description].Value.ToString().Trim();

            return string.Empty;
        }

        #endregion
    }
}