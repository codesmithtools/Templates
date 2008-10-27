using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using SchemaExplorer;

namespace NHibernateHelper
{
    public class NHibernateHelper : CodeTemplate
    {
        #region Constants

        private const string _extendedPropertyName = "cs_alias";

        #endregion

        #region HelperInit

        /// <summary>
        /// Should be called the first thing every time the master template executes.
        /// </summary>
        /// <param name="tablePrefix">TablePrefix Property</param>
        public static void HelperInit(string tablePrefix)
        {
            _tablePrefix = tablePrefix;
        }
        private static string _tablePrefix = String.Empty;

        #endregion

        #region Variable & Class Name Methods

        public static string GetPropertyName(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetPropertyName(column);
            else
            {
                string className = GetClassName(table);
                string name = GetPropertyName(className);
                return (name == className) ? String.Concat(name, "Member") : name;
            }
        }
        public static string GetPropertyName(ColumnSchema column)
        {
            return GetPropertyName(GetNameFromColumn(column));
        }
        private static string GetPropertyName(string name)
        {
            return StringUtil.ToSingular(StringUtil.ToPascalCase(name));
        }

        public static string GetPropertyNamePlural(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetPropertyNamePlural(GetNameFromColumn(column));
            else
            {
                string className = GetClassName(table);
                string name = GetPropertyNamePlural(className);
                return name == className ? className + "List" : name;
            }
        }
        public static string GetPropertyNamePlural(ColumnSchema column)
        {
            return GetPropertyNamePlural(GetNameFromColumn(column));
        }
        private static string GetPropertyNamePlural(string name)
        {
            return StringUtil.ToPlural(StringUtil.ToPascalCase(name));
        }

        public static string GetPrivateVariableName(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetPrivateVariableName(GetNameFromColumn(column));
            else
                return GetPrivateVariableName(GetClassName(table));
        }
        public static string GetPrivateVariableName(ColumnSchema column)
        {
            return GetPrivateVariableName(GetNameFromColumn(column));
        }
        private static string GetPrivateVariableName(string name)
        {
            return  String.Concat("_", GetVariableName(name));
        }

        public static string GetPrivateVariableNamePlural(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetPrivateVariableNamePlural(GetNameFromColumn(column));
            else
                return GetPrivateVariableNamePlural(GetClassName(table));
        }
        public static string GetPrivateVariableNamePlural(ColumnSchema column)
        {
            return GetPrivateVariableNamePlural(GetNameFromColumn(column));
        }
        private static string GetPrivateVariableNamePlural(string name)
        {
            return String.Concat("_", GetVariableNamePlural(name));
        }

        public static string GetVariableName(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetVariableName(GetNameFromColumn(column));
            else
                return GetVariableName(GetClassName(table));
        }
        public static string GetVariableName(ColumnSchema column)
        {
            return GetVariableName(GetNameFromColumn(column));
        }
        private static string GetVariableName(string name)
        {
            return StringUtil.ToCamelCase(StringUtil.ToSingular(name));
        }

        public static string GetVariableNamePlural(TableSchema table, ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetVariableNamePlural(GetNameFromColumn(column));
            else
                return GetVariableNamePlural(GetClassName(table));
        }
        public static string GetVariableNamePlural(ColumnSchema column)
        {
            return GetVariableNamePlural(GetNameFromColumn(column));
        }
        private static string GetVariableNamePlural(string name)
        {
            return StringUtil.ToCamelCase(StringUtil.ToPlural(name));
        }

        public static string GetClassName(TableSchema table)
        {
            string className;
            if (table.ExtendedProperties.Contains(_extendedPropertyName))
                className = table.ExtendedProperties[_extendedPropertyName].Value.ToString();
            else
            {
                className = table.Name;

                if (!String.IsNullOrEmpty(_tablePrefix) && className.StartsWith(_tablePrefix))
                    className = className.Remove(0, _tablePrefix.Length);
            }

            return StringUtil.ToPascalCase(StringUtil.ToSingular(className));
        }
        
        private static bool ColumnHasAlias(ColumnSchema column)
        {
            return column.ExtendedProperties.Contains(_extendedPropertyName);
        }
        private static string GetNameFromColumn(ColumnSchema column)
        {
            string name = (ColumnHasAlias(column))
                ? column.ExtendedProperties[_extendedPropertyName].Value.ToString()
                : column.Name;

            return (String.Compare(GetClassName(column.Table), name, true) == 0)
                ? String.Concat(name, "Member")
                : name;
        }

        #endregion

        #region ManyToMany Table Methods

        public static TableSchema GetToManyTable(TableSchema manyToTable, TableSchema sourceTable)
        {
            TableSchema result = null;
            foreach (TableKeySchema key in manyToTable.ForeignKeys)
                if (!key.PrimaryKeyTable.Equals(sourceTable))
                {
                    result = key.PrimaryKeyTable;
                    break;
                }
            return result;
        }
        public static MemberColumnSchema GetToManyTableKey(TableSchema manyToTable, TableSchema foreignTable)
        {
            MemberColumnSchema result = null;
            foreach (TableKeySchema key in manyToTable.ForeignKeys)
                if (key.PrimaryKeyTable.Equals(foreignTable))
                {
                    result = key.ForeignKeyMemberColumns[0];
                    break;
                }
            return result;
        }
        public static bool IsManyToMany(TableSchema table)
        {
            // If there are 2 ForeignKeyColumns AND...
            // ...there are only two columns OR
            //    there are 3 columns and 1 is a primary key.
            return (table.ForeignKeyColumns.Count == 2
                && ((table.Columns.Count == 2)
                    || (table.Columns.Count == 3 && table.PrimaryKey != null)));
        }

        #endregion

        #region BusinessObject Methods

        public static string GetInitialization(Type type)
        {
            string result;

            if (type.Equals(typeof(String)))
                result = "String.Empty";
            else if (type.Equals(typeof(DateTime)))
                result = "new DateTime()";
            else if (type.Equals(typeof(Decimal)))
                result = "default(Decimal)";
            else if (type.Equals(typeof(Guid)))
                result = "Guid.Empty";
            else if (type.IsPrimitive)
                result = String.Format("default({0})", type.Name.ToString());
            else
                result = "null";
            return result;
        }
        public static Type GetBusinessBaseIdType(TableSchema table)
        {
            if (IsMutliColumnPrimaryKey(table.PrimaryKey))
                return typeof(string);
            else
                return GetPrimaryKeyColumn(table.PrimaryKey).SystemType;
        }

        #endregion

        #region PrimaryKey Methods

        public static MemberColumnSchema GetPrimaryKeyColumn(PrimaryKeySchema primaryKey)
        {
            if (primaryKey.MemberColumns.Count != 1)
                throw new System.ApplicationException("This method will only work on primary keys with exactly one member column.");
            return primaryKey.MemberColumns[0];
        }
        public static bool IsMutliColumnPrimaryKey(PrimaryKeySchema primaryKey)
        {
            if (primaryKey.MemberColumns.Count == 0)
                throw new System.ApplicationException("This template will only work on primary keys with exactly one member column.");

            return (primaryKey.MemberColumns.Count > 1);
        }
        public static string GetForeignKeyColumnClassName(MemberColumnSchema mcs, TableSchema table)
        {
            string result = String.Empty;
            foreach (TableKeySchema tks in table.ForeignKeys)
                if (tks.ForeignKeyMemberColumns.Contains(mcs))
                {
                    result = GetPropertyName(tks.PrimaryKeyTable.Name);
                    break;
                }
            return result;
        }

        #endregion

        #region Misc

        public static TableSchema GetForeignTable(MemberColumnSchema mcs, TableSchema table)
        {
            foreach (TableKeySchema tks in table.ForeignKeys)
                if (tks.ForeignKeyMemberColumns.Contains(mcs))
                    return tks.PrimaryKeyTable;
            throw new Exception(String.Format("Could not find Column {0} in Table {1}'s ForeignKeys.", mcs.Name, table.Name));
        }

        public static string GetCascade(MemberColumnSchema column)
        {
            return column.AllowDBNull ? "all" : "all-delete-orphan";
        }

        public static string GetCriterionNamespace(NHibernateVersion version)
        {
            switch (version)
            {
                case NHibernateVersion.v1_2:
                    return "NHibernate.Expression";

                case NHibernateVersion.v2_0:
                    return "NHibernate.Criterion";

                default:
                    throw new Exception("Invalid NHibernateVersion");
            }
        }

        public static bool ContainsForeignKey(SearchCriteria sc, TableSchemaCollection tsc)
        {
            foreach (TableSchema ts in tsc)
                foreach (TableKeySchema tks in ts.PrimaryKeys)
                    foreach (MemberColumnSchema mcs in sc.Items)
                        if (tks.PrimaryKeyMemberColumns.Contains(mcs))
                            return true;
            return false;
        }

        #endregion
    }
}
