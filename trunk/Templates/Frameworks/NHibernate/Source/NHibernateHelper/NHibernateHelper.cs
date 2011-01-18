using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const string SingularMemberSuffix = "Member";
        internal const string ListSuffix = "List";

        private static Regex NumPrefix = new Regex(@"^\d", RegexOptions.Compiled);

        #endregion

        #region HelperInit

        /// <summary>
        /// Should be called the first thing every time the master template executes.
        /// </summary>
        /// <param name="tablePrefix">TablePrefix Property</param>
        /// <param name="systemCSharpAliasMap"></param>
        /// <param name="csharpKeyWordEscapeMap"></param>
        /// <param name="excludedColumns"></param>
        /// <param name="versionRegex"></param>
        /// <param name="providerName"></param>
        public static void HelperInit(string tablePrefix, MapCollection systemCSharpAliasMap, MapCollection csharpKeyWordEscapeMap, string[] excludedColumns, string versionRegex, string providerName) //, NamingProperty namingConventions)
        {
            _tablePrefix = tablePrefix;

            SystemCSharpAliasMap = systemCSharpAliasMap;
            CsharpKeyWordEscapeMap = csharpKeyWordEscapeMap;
            VersionRegex = new Regex(versionRegex, RegexOptions.Compiled);

            _excludedColumns = new List<Regex>();
            foreach (var s in excludedColumns)
                if (!String.IsNullOrEmpty(s) && s.Trim().Length > 0)
                    _excludedColumns.Add(new Regex(s, RegexOptions.Compiled));

            if (!string.IsNullOrEmpty(providerName) && Enum.IsDefined(typeof (SchemaProvider), providerName.Trim()))
            {
                try
                {
                    SchemaProvider = (SchemaProvider) Enum.Parse(typeof (SchemaProvider), providerName.Trim());
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    SchemaProvider = SchemaProvider.ADOXSchemaProvider;
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
        internal static Regex VersionRegex { get; set; }

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

        internal static SchemaProvider SchemaProvider { get; set; }
        internal static bool IsSqlServer
        {
            get { return SchemaProvider == SchemaProvider.SqlSchemaProvider; }
        }

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

                className = StringUtil.ToPascalCase(className);

                if (EntityNaming == EntityNamingEnum.Plural && TableNaming != TableNamingEnum.Plural)
                    className = StringUtil.ToPlural(className);
                else if (EntityNaming == EntityNamingEnum.Singular && TableNaming != TableNamingEnum.Singular)
                    className = StringUtil.ToSingular(className);
            }

            return ValidateName(className);
        }

        internal static string ValidateName(string memberName)
        {
            // This is not the best place to put this, but I know that it is the first place
            // to blow up when the HelperInit has not been run by the master template.
            if (CsharpKeyWordEscapeMap == null)
                throw new Exception("Subtemplates may only be called via NHiberanteMaster.cst");

            if (NumPrefix.IsMatch(memberName))
                memberName = String.Concat("n", memberName);

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
            if (table.ExtendedProperties.Contains(ExtendedPropertyManyToMany))
            {
                bool manyToMany;
                if (Boolean.TryParse(table.ExtendedProperties[ExtendedPropertyManyToMany].Value.ToString(), out manyToMany))
                    return manyToMany;
            }

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

        public static string GetCascade(ColumnSchema column, AssociationTypeEnum association)
        {
            var ep = column.ExtendedProperties["cs_cascade"];
            if (ep != null && ep.Value != null)
            {
                var eps = ep.Value.ToString();
                if (!String.IsNullOrEmpty(eps))
                    return eps;
            }

            if (association == AssociationTypeEnum.OneToMany)
                return column.AllowDBNull ? "all" : "all-delete-orphan";

            if (association == AssociationTypeEnum.ManyToMany)
                return "save-update";

            return String.Empty;
        }

        public static string GetCriterionNamespace(NHibernateVersion version)
        {
            switch (version)
            {
                case NHibernateVersion.v1_2:
                    return "NHibernate.Expression";

                case NHibernateVersion.v2_1:
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

        public static string TableSafeName(TableSchema sourceTable)
        {
            var safeOwner = IsSqlServer && String.IsNullOrEmpty(sourceTable.Owner)
                               ? String.Empty
                               : String.Concat(SafeName(sourceTable.Owner), ".");

            return String.Concat(safeOwner, SafeName(sourceTable.Name));
        }

        public static string ColumnSafeName(ColumnSchema column)
        {
            return SafeName(column.Name);
        }

        public static string SafeName(string name)
        {
            return IsSqlServer
                      ? String.Concat("[", name, "]")
                      : name;
        }

        #endregion
    }
}
