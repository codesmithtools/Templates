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
            return StringUtil.ToSingular(StringUtil.ToCamelCase(name));
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
            return StringUtil.ToPlural(StringUtil.ToCamelCase(name));
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

            return StringUtil.ToSingular(StringUtil.ToPascalCase(className));
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

        private static Random random = new Random();
        public static string GetUnitTestInitialization(ColumnSchema column)
        {
            string result;

            if (column.SystemType.Equals(typeof(String)))
            {
                StringBuilder sb = new StringBuilder();

                int size = (column.Size > 0 && column.Size < 100) ? random.Next(1, column.Size) : 10;

                sb.Append("\"");
                for (int x = 0; x < size; x++)
                {
                    switch (x % 5)
                    {
                        case 0:
                            sb.Append("T");
                            break;
                        case 1:
                            sb.Append("e");
                            break;
                        case 2:
                            sb.Append("s");
                            break;
                        case 3:
                            sb.Append("t");
                            break;
                        case 4:
                            sb.Append(" ");
                            break;
                    }
                }
                sb.Append("\"");

                result = sb.ToString();
            }
            else if (column.SystemType.Equals(typeof(DateTime)))
                result = "DateTime.Now";
            else if (column.SystemType.Equals(typeof(Decimal)))
                result = Convert.ToDecimal(random.Next(1, 100)).ToString();
            else if (column.SystemType.Equals(typeof(Int32)))
                result = random.Next(1, 100).ToString();
            else if (column.SystemType.Equals(typeof(Boolean)))
                result = (random.Next(1, 2).Equals(1)).ToString().ToLower();
            else if (column.SystemType.Equals(typeof(Guid)))
                result = "Guid.Empty";
            else if (column.SystemType.IsPrimitive)
                result = String.Format("default({0})", column.SystemType.Name.ToString());
            else
                result = "null";

            return result;
        }

        #endregion

        private MapCollection _keyWords;
        public MapCollection KeyWords
        {
            get
            {
                if (_keyWords == null)
                {
                    string path;
                    Map.TryResolvePath("CSharpKeyWordEscape", this.CodeTemplateInfo.DirectoryName, out path);
                    _keyWords = Map.Load(path);
                }
                return _keyWords;
            }
        }

        public string GetMethodParameters(List<MemberColumnSchema> mcsList, bool isDeclaration)
        {
            StringBuilder result = new StringBuilder();
            bool isFirst = true;
            foreach (MemberColumnSchema mcs in mcsList)
            {
                if (isFirst)
                    isFirst = false;
                else
                    result.Append(", ");
                if (isDeclaration)
                {
                    result.Append(mcs.SystemType.ToString());
                    result.Append(" ");
                }
                result.Append(KeyWords[GetVariableName(mcs)]);
            }
            return result.ToString();
        }
        public string GetMethodParameters(MemberColumnSchemaCollection mcsc, bool isDeclaration)
        {
            List<MemberColumnSchema> mcsList = new List<MemberColumnSchema>();
            for (int x = 0; x < mcsc.Count; x++)
                mcsList.Add(mcsc[x]);
            return GetMethodParameters(mcsList, isDeclaration);
        }
        public string GetMethodDeclaration(SearchCriteria sc)
        {
            StringBuilder result = new StringBuilder();
            result.Append(sc.MethodName);
            result.Append("(");
            result.Append(GetMethodParameters(sc.Items, true));
            result.Append(")");
            return result.ToString();
        }
        public string GetPrimaryKeyCallParameters(List<MemberColumnSchema> mcsList)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int x = 0; x < mcsList.Count; x++)
            {
                if (x > 0)
                    result.Append(", ");

                if (mcsList[x].SystemType == typeof(Guid))
                    result.AppendFormat("new {0}(keys[{1}])", mcsList[x].SystemType, x);
                else if (mcsList[x].SystemType == typeof(string))
                    result.AppendFormat("keys[{0}]", x);
                else
                    result.AppendFormat("{0}.Parse(keys[{1}])", mcsList[x].SystemType, x);
            }
            return result.ToString();
        }
    }
}
