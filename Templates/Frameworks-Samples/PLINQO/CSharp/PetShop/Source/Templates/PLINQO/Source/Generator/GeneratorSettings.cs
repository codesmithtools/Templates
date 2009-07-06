using System.Collections.Generic;
using System.Text.RegularExpressions;
using SchemaExplorer;

namespace LinqToSqlShared.Generator
{
    public class GeneratorSettings
    {
        private string mappingFile;
        public string MappingFile
        {
            get { return mappingFile; }
            set { mappingFile = value; }
        }

        private string _contextNamespace;
        public string ContextNamespace
        {
            get { return _contextNamespace; }
            set { _contextNamespace = value; }
        }

        private string _entityBase;
        public string EntityBase
        {
            get { return _entityBase; }
            set { _entityBase = value; }
        }

        private string _entityNamespace;
        public string EntityNamespace
        {
            get { return _entityNamespace; }
            set { _entityNamespace = value; }
        }

        private bool includeViews;
        public bool IncludeViews
        {
            get { return includeViews; }
            set { includeViews = value; }
        }

        private bool includeFunctions;
        public bool IncludeFunctions
        {
            get { return includeFunctions; }
            set { includeFunctions = value; }
        }

        private List<Regex> _ignoreExpressions = new List<Regex>();
        public List<Regex> IgnoreExpressions
        {
            get { return _ignoreExpressions; }
        }

        private List<Regex> _cleanExpressions = new List<Regex>();
        public List<Regex> CleanExpressions
        {
            get { return _cleanExpressions; }
        }

        private List<Regex> _enumExpressions = new List<Regex>();
        public List<Regex> EnumExpressions
        {
            get { return _enumExpressions; }
        }

        private List<Regex> _enumNameExpressions = new List<Regex>();
        public List<Regex> EnumNameExpressions
        {
            get { return _enumNameExpressions; }
        }

        private List<Regex> _enumDescriptionExpressions = new List<Regex>();
        public List<Regex> EnumDescriptionExpressions
        {
            get { return _enumDescriptionExpressions; }
        }

        private List<string> _userDefinedAssociations = new List<string>();
        public List<string> UserDefinedAssociations
        {
            get { return _userDefinedAssociations; }
            set { _userDefinedAssociations = value; }
        }

        private bool _disableRenaming = false;
        public bool DisableRenaming
        {
            get { return _disableRenaming; }
            set { _disableRenaming = value; }
        }

        public bool IsIgnored(string name)
        {
            return IsRegexMatch(name, IgnoreExpressions);
        }

        public bool IsEnum(TableSchema table)
        {
            return !IsIgnored(table.Name)                                       // 1) Is not ignored.
                && IsRegexMatch(table.Name, EnumExpressions)                    // 2) Matches the enum regex.
                && table.PrimaryKey != null                                     // 3) Has a Primary Key...
                && table.PrimaryKey.MemberColumns.Count == 1                    // 4) ...that is a single column...
                && IsEnumSystemType(table.PrimaryKey.MemberColumns[0])          // 5) ...of a number type.
                && !string.IsNullOrEmpty(GetEnumNameColumnName(table))          // 6) Contains a column for name.
                && table.GetTableData().Rows.Count > 0;                         // 7) Must have at least one row.
        }

        private bool IsEnumSystemType(MemberColumnSchema column)
        {
            return column.NativeType.Equals("int", System.StringComparison.OrdinalIgnoreCase)
                || column.NativeType.Equals("bigint", System.StringComparison.OrdinalIgnoreCase)
                || column.NativeType.Equals("tinyint", System.StringComparison.OrdinalIgnoreCase)
                || column.NativeType.Equals("byte", System.StringComparison.OrdinalIgnoreCase);
        }

        public string GetEnumNameColumnName(TableSchema table)
        {
            string result = GetEnumColumnName(table, EnumNameExpressions);

            // If no Regex match found, use first column of type string.
            if (string.IsNullOrEmpty(result))
                foreach (ColumnSchema column in table.Columns)
                    if (column.SystemType == typeof(string))
                    {
                        result = column.Name;
                        break;
                    }

            return result;
        }

        public string GetEnumDescriptionColumnName(TableSchema table)
        {
            return GetEnumColumnName(table, EnumDescriptionExpressions);
        }

        private string GetEnumColumnName(TableSchema table, List<Regex> regexList)
        {
            string result = string.Empty;

            foreach (ColumnSchema column in table.Columns)
                if (IsRegexMatch(column.Name, regexList))
                {
                    result = column.Name;
                    break;
                }

            return result;
        }

        private bool IsRegexMatch(string name, List<Regex> regexList)
        {
            bool isEnum = false;

            foreach (Regex regex in regexList)
                if (regex.IsMatch(name))
                {
                    isEnum = true;
                    break;
                }

            return isEnum;
        }

        public string CleanName(string name)
        {
            if (CleanExpressions.Count == 0)
                return name;

            foreach (Regex regex in CleanExpressions)
            {
                if (regex.IsMatch(name))
                {
                    return regex.Replace(name, "");
                }
            }

            return name;
        }

        private FrameworkEnum _framework = FrameworkEnum.v35_SP1;
        public FrameworkEnum Framework
        {
            get { return _framework; }
            set { _framework = value; }
        }

        private TableNamingEnum _tableNaming = TableNamingEnum.Singular;
        public TableNamingEnum TableNaming
        {
            get { return _tableNaming; }
            set { _tableNaming = value; }
        }

        private EntityNamingEnum _entityNaming = EntityNamingEnum.Singular;
        public EntityNamingEnum EntityNaming
        {
            get { return _entityNaming; }
            set { _entityNaming = value; }
        }

        private AssociationNamingEnum _associationNaming = AssociationNamingEnum.ListSuffix;
        public AssociationNamingEnum AssociationNaming
        {
            get { return _associationNaming; }
            set { _associationNaming = value; }
        }
    
        private bool _includeDeleteOnNull = true;
        public bool IncludeDeleteOnNull
        {
            get { return _includeDeleteOnNull; }
            set { _includeDeleteOnNull = value; }
        }

        private bool _includeDataContract = true;
        public bool IncludeDataContract
        {
            get { return _includeDataContract; }
            set { _includeDataContract = value; }
        }

        private bool _generateMetaData = true;
        public bool GenerateMetaData
        {
            get { return _generateMetaData; }
            set { _generateMetaData = value; }
        }

    }
}
