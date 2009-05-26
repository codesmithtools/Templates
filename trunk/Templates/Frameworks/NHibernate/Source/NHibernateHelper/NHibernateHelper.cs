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

        internal const string ExtendedPropertyName = "cs_alias";
        internal const string ExtendedPropertyManyToMany = "cs_ManyToMany";

        private static Regex _versionRegex = null;
        public static Regex VersionRegex
        {
            get
            {
                if(_versionRegex == null)
                    _versionRegex = new Regex("^((R|r)ow)?(V|v)ersion$", RegexOptions.Compiled);
                return _versionRegex;
            }
        }

        private const string SingularMemberSuffix = "Member";
        internal const string ListSuffix = "List";

        #endregion

        #region HelperInit

        /// <summary>
        /// Should be called the first thing every time the master template executes.
        /// </summary>
        /// <param name="tablePrefix">TablePrefix Property</param>
        /// <param name="namingProperty">NamingContentions Property</param>
        public static void HelperInit(string tablePrefix, MapCollection systemCSharpAliasMap, MapCollection csharpKeyWordEscapeMap, string[] excludedColumns) //, NamingProperty namingConventions)
        {
            _tablePrefix = tablePrefix;

            SystemCSharpAliasMap = systemCSharpAliasMap;
            CsharpKeyWordEscapeMap = csharpKeyWordEscapeMap;

            _excludedColumns = new List<Regex>();
            foreach (var s in excludedColumns)
            {
                if (!String.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    _excludedColumns.Add(new Regex(s, RegexOptions.Compiled));
                }
            }

            //_tableNaming = namingConventions.TableNaming;
            //_entityNaming = namingConventions.EntityNaming;
            //_associationNaming = namingConventions.AssociationNaming;
            //_associationSuffix = namingConventions.AssociationSuffix;
        }

        private static string _tablePrefix = String.Empty;

        internal static MapCollection CsharpKeyWordEscapeMap { get; set; }
        internal static MapCollection SystemCSharpAliasMap { get; set; }

        private static List<Regex> _excludedColumns = null;
        internal static bool IsExcludedColumn(string s)
        {
            if (_excludedColumns == null)
                return false;

            var result = false;
            foreach (var exclude in _excludedColumns)
            {
                if (exclude.IsMatch(s))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        internal static TableNamingEnum TableNaming = TableNamingEnum.Mixed;
        internal static EntityNamingEnum EntityNaming = EntityNamingEnum.Singular;
        internal static AssociationNamingEnum AssociationNaming = AssociationNamingEnum.Table;
        internal static AssociationSuffixEnum AssociationSuffix = AssociationSuffixEnum.Plural;

        #endregion

        #region Variable & Class Name Methods
        
        public static string GetClassName(TableSchema table)
        {
            string className;

            if (table.ExtendedProperties.Contains(ExtendedPropertyName))
                className = table.ExtendedProperties[ExtendedPropertyName].Value.ToString();
            else
            {
                className = table.Name;

                if (!String.IsNullOrEmpty(_tablePrefix) && className.StartsWith(_tablePrefix))
                    className = className.Remove(0, _tablePrefix.Length);

                if (EntityNaming == EntityNamingEnum.Plural && TableNaming != TableNamingEnum.Plural)
                    className = StringUtil.ToPlural(className);
                else if (EntityNaming == EntityNamingEnum.Singular && TableNaming != TableNamingEnum.Singular)
                    className = StringUtil.ToSingular(className);

                className = StringUtil.ToPascalCase(className);
            }

            return ValidateName(className);
        }

        internal static string ValidateName(string memberName)
        {
            return (CsharpKeyWordEscapeMap.ContainsKey(memberName))
                ? CsharpKeyWordEscapeMap[memberName]
                : memberName;
        }
        internal static string ValidateName(ColumnSchema column, string memberName)
        {
            if (String.Compare(GetClassName(column.Table), memberName, true) == 0)
                memberName = String.Concat(memberName, SingularMemberSuffix);

            return ValidateName(memberName);
        }
        
        #endregion

        #region Many To Many Methods

        public static bool IsManyToMany(TableSchema table)
        {
            // Bypass logic if table contains Extended Property for ManyToMany
            if (table.ExtendedProperties.Contains(NHibernateHelper.ExtendedPropertyManyToMany))
                return true;

            // 1) Table must have Two ForeignKeys.
            //    a) Must be two unique tables.
            // 2) All columns must be either...
            //    a) Member of the Primary Key.
            //    b) Member of a Foreign Key.
            //    c) A DateTime stamp (CreateDate, EditDate, etc).
            //    d) Name matches Version Regex.

            if (table.ForeignKeys.Count != 2)
                return false;

            bool result = true;
            List<TableSchema> foreignTables = new List<TableSchema>();

            foreach (ColumnSchema column in table.Columns)
            {
                if (!(column.IsForeignKeyMember
                    || column.IsPrimaryKeyMember
                    || column.SystemType.Equals(typeof(DateTime))
                    || VersionRegex.IsMatch(column.Name)))
                {
                    result = false;
                    break;
                }
                else if (column.IsForeignKeyMember)
                {
                    foreach (TableKeySchema tks in column.Table.ForeignKeys)
                    {
                        if (tks.ForeignKeyMemberColumns.Contains(column))
                        {
                            if (!foreignTables.Contains(tks.PrimaryKeyTable))
                                foreignTables.Add(tks.PrimaryKeyTable);
                            break;
                        }
                    }
                }
            }

            if (foreignTables.Count != 2)
                result = false;

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
                    result = GetClassName(tks.PrimaryKeyTable);
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
        
        #endregion
    }
}
