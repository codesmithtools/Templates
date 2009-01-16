using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeSmith.Engine;
using SchemaExplorer;
using System.Text.RegularExpressions;

namespace NHibernateHelper
{
    public class NHibernateHelper : CodeTemplate
    {
        #region Constants

        private const string ExtendedPropertyName = "cs_alias";
        private const string ExtendedPropertyManyToMany = "cs_ManyToMany";

        private static Regex _versionRegex = null;
        public static Regex VersionRegex
        {
            get
            {
                if(_versionRegex == null)
                    _versionRegex = new Regex("(V|v)ersion", RegexOptions.Compiled);
                return _versionRegex;
            }
        }

        private const string SingularMemberSuffix = "Member";
        private const string ListSuffix = "List";

        #endregion

        #region HelperInit

        /// <summary>
        /// Should be called the first thing every time the master template executes.
        /// </summary>
        /// <param name="tablePrefix">TablePrefix Property</param>
        /// <param name="namingProperty">NamingContentions Property</param>
        public static void HelperInit(string tablePrefix, MapCollection systemCSharpAliasMap, MapCollection csharpKeyWordEscapeMap) //, NamingProperty namingConventions)
        {
            _tablePrefix = tablePrefix;

            _systemCSharpAliasMap = systemCSharpAliasMap;
            _csharpKeyWordEscapeMap = csharpKeyWordEscapeMap;

            //_tableNaming = namingConventions.TableNaming;
            //_entityNaming = namingConventions.EntityNaming;
            //_associationNaming = namingConventions.AssociationNaming;
            //_associationSuffix = namingConventions.AssociationSuffix;
        }

        private static string _tablePrefix = String.Empty;

        private static MapCollection _csharpKeyWordEscapeMap { get; set; }
        private static MapCollection _systemCSharpAliasMap { get; set; }

        private static TableNamingEnum _tableNaming = TableNamingEnum.Mixed;
        private static EntityNamingEnum _entityNaming = EntityNamingEnum.Singular;
        private static AssociationNamingEnum _associationNaming = AssociationNamingEnum.Table;
        private static AssociationSuffixEnum _associationSuffix = AssociationSuffixEnum.Plural;

        #endregion

        #region Variable & Class Name Methods

        public static string GetGenericName(TableSchema table, ColumnSchema column, AssociationTypeEnum associationType)
        {
            return GetGenericName(table, column, (associationType != AssociationTypeEnum.ManyToOne));
        }
        private static string GetGenericName(TableSchema table, ColumnSchema column, bool plural)
        {
            if (ColumnHasAlias(column))
                return GetNameFromColumn(column);

            string genericName = GetAssociationName(table, column, plural);
            return ValidateName(column, genericName);
        }
        public static string GetGenericName(ColumnSchema column)
        {
            if (ColumnHasAlias(column))
                return GetNameFromColumn(column);

            string genericName = GetNameFromColumn(column);
            return ValidateName(column, genericName);
        }

        internal static string GetPropertyName(string name)
        {
            return StringUtil.ToPascalCase(name);
        }
        internal static string GetPrivateVariableName(string name)
        {
            return String.Concat("_", GetVariableName(name));
        }
        internal static string GetVariableName(string name)
        {
            return StringUtil.ToCamelCase(name);
        }

        public static string GetClassName(TableSchema table)
        {
            if (table.ExtendedProperties.Contains(ExtendedPropertyName))
                return table.ExtendedProperties[ExtendedPropertyName].Value.ToString();

            string className = table.Name;

            if (!String.IsNullOrEmpty(_tablePrefix) && className.StartsWith(_tablePrefix))
                className = className.Remove(0, _tablePrefix.Length);

            if (_entityNaming == EntityNamingEnum.Plural && _tableNaming != TableNamingEnum.Plural)
                className = StringUtil.ToPlural(className);
            else if (_entityNaming == EntityNamingEnum.Singular && _tableNaming != TableNamingEnum.Singular)
                className = StringUtil.ToSingular(className);

            className = StringUtil.ToPascalCase(className);
            return ValidateName(className);
        }

        private static string ValidateName(string memberName)
        {
            return (_csharpKeyWordEscapeMap.ContainsKey(memberName))
                ? _csharpKeyWordEscapeMap[memberName]
                : memberName;
        }
        private static string ValidateName(ColumnSchema column, string memberName)
        {
            if (String.Compare(GetClassName(column.Table), memberName, true) == 0)
                memberName = String.Concat(memberName, SingularMemberSuffix);

            return ValidateName(memberName);
        }

        private static bool ColumnHasAlias(ColumnSchema column)
        {
            return column.ExtendedProperties.Contains(ExtendedPropertyName);
        }
        private static string GetNameFromColumn(ColumnSchema column)
        {
            string name = (ColumnHasAlias(column))
                ? column.ExtendedProperties[ExtendedPropertyName].Value.ToString()
                : column.Name;

            return ValidateName(name);
        }

        private static string GetAssociationName(TableSchema table, ColumnSchema column, bool plural)
        {
            string result = (_associationNaming == AssociationNamingEnum.Column)
                ? GetNameFromColumn(column)
                : GetClassName(table);

            if (plural)
            {
                if (_associationSuffix == AssociationSuffixEnum.List)
                    result = String.Concat(result, ListSuffix);
                else if (_associationSuffix == AssociationSuffixEnum.Plural)
                    result = StringUtil.ToPlural(result);
            }

            return result;
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
            // Bypass logic if table contains Extended Property for ManyToMany
            if (table.ExtendedProperties.Contains(ExtendedPropertyManyToMany))
                return true;

            // 1) Table must have Two ForeignKeys.
            // 2) All columns must be either...
            //    a) Member of the Primary Key.
            //    b) Member of a Foreign Key.
            //    c) A DateTime stamp (CreateDate, EditDate, etc).
            //    d) Name matches Version Regex.

            if(table.ForeignKeys.Count != 2)
                return false;

            bool result = true;

            foreach (ColumnSchema column in table.Columns)
            {
                if (!( column.IsForeignKeyMember
                    || column.IsPrimaryKeyMember
                    || column.SystemType.Equals(typeof(DateTime))
                    || VersionRegex.IsMatch(column.Name)))
                {
                    result = false;
                    break;
                }
            }

            return result;
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

        public static Type GetBusinessBaseIdType(TableSchema table)
        {
            if (IsMutliColumnPrimaryKey(table.PrimaryKey))
                return typeof(string);
            else
                return GetPrimaryKeyColumn(table.PrimaryKey).SystemType;
        }

        public static TableSchema GetForeignTable(MemberColumnSchema mcs, TableSchema table)
        {
            foreach (TableKeySchema tks in table.ForeignKeys)
                if (tks.ForeignKeyMemberColumns.Contains(mcs))
                    return tks.PrimaryKeyTable;
            throw new Exception(String.Format("Could not find Column {0} in Table {1}'s ForeignKeys.", mcs.Name, table.Name));
        }

        public static string GetCascade(ColumnSchema column)
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

        public static string GetSystemTypeName(ColumnSchema column)
        {
            string result = (_systemCSharpAliasMap.ContainsKey(column.SystemType.FullName))
                ? _systemCSharpAliasMap[column.SystemType.FullName]
                : column.SystemType.FullName;

            return (column.AllowDBNull && column.SystemType.IsValueType)
                ? String.Concat(result, "?")
                : result;
        }

        #endregion
    }
}
