using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

using CodeSmith.Engine;
using CodeSmith.SchemaHelper.Util;

using SchemaExplorer;

namespace CodeSmith.SchemaHelper
{
    public class Configuration
    {
        #region Member(s)

        private string _rowVersionColumn;
        private MapCollection _dbTypeToSystemType;
        private MapCollection _systemTypeEscape;

        internal Regex RowVersionColumnRegex { get; private set; }

        #endregion

        #region Constructor(s)

        public Configuration()
        {
            AliasExtendedProperty = "CS_Alias";
            ManyToManyExtendedProperty = "CS_ManyToMany";
            TablePrefix = String.Empty;
            ParameterPrefix = "@p_";
            NamingProperty = new NamingProperty
                                 {
                                     EntityNaming = EntityNaming.Singular,
                                     TableNaming = TableNaming.Mixed
                                 };

            SearchCriteriaProperty = new SearchCriteriaProperty();
            SearchCriteriaProperty.Prefix = "GetBy";
            
            RowVersionColumn = "^((R|r)ow)?(V|v)ersion$";
            VisualStudioVersion = VisualStudioVersion.VS_2008;
            SingularMemberSuffix = "";
            IncludeManyToManyEntity = true;

            IgnoreExpressions = new List<Regex>();
            CleanExpressions = new List<Regex>();
        }

        #endregion

        #region Public Method(s)

        /// <summary>
        /// Returns the DBTypeToSystemTypeEscape MapCollection.
        /// </summary>
        /// <returns>Returns the correct SystemTypeEscape MapCollection.</returns>
        [Browsable(false)]
        public MapCollection SystemTypeEscape
        {

            get
            {
                string mapFileName = TargetLanguage == LanguageEnum.CSharp ? "CSharpKeywordEscape" : "VBKeywordEscape";

                if (_systemTypeEscape == null)
                {
                    string path;
                    Map.TryResolvePath(mapFileName, "", out path);
                    _systemTypeEscape = Map.Load(path);
                }

                return _systemTypeEscape;
            }
        }

        /// <summary>
        /// Returns the DBTypeToSystemLanguage MapCollection.
        /// </summary>
        /// <returns>Returns the correct language MapCollection.</returns>
        [Browsable(false)]
        public MapCollection ResolveSystemType
        {
            get
            {
                string mapFileName = TargetLanguage == LanguageEnum.CSharp ? "DbType-CSharp" : "DbType-VB";

                if (_dbTypeToSystemType == null)
                {
                    string path;
                    Map.TryResolvePath(mapFileName, "", out path);
                    _dbTypeToSystemType = Map.Load(path);
                }

                return _dbTypeToSystemType;
            }
        }

        #endregion

        #region Instance

        public static Configuration Instance
        {
            get { return Nested.Current; }
        }

        private class Nested
        {
            static Nested()
            {
                Current = new Configuration();
            }

            /// <summary>
            /// Current singleton instance.
            /// </summary>
            internal static readonly Configuration Current;
        }

        #endregion

        #region Internal Method(s)

        internal string ValidateName(string memberName)
        {
            memberName = (SystemTypeEscape.ContainsKey(memberName)) ? SystemTypeEscape[memberName] : memberName;

            return NamingConventions.PropertyName(memberName);
        }

        internal string ValidateName(ColumnSchema column, string memberName)
        {
            if (String.Compare(column.Table.ClassName(), memberName, true) == 0)
                memberName = string.Format("{0}{1}", memberName, SingularMemberSuffix);

            return ValidateName(memberName);
        }

        internal bool ExcludeTableRegexIsMatch(string tableName)
        {
            bool result = false;
            foreach (Regex regex in IgnoreExpressions)
                if (regex.IsMatch(tableName))
                {
                    result = true;
                    break;
                }
            return result;
        }

        #endregion

        #region Public Properties

        public LanguageEnum TargetLanguage { get; set; }

        public string AliasExtendedProperty { get; set; }

        public string ManyToManyExtendedProperty { get; set; }

        public string TablePrefix { get; set; }

        private string _parameterPrefix = "@p_";
        public string ParameterPrefix
        {
            get
            {
                return _parameterPrefix;
            }
            set
            {
                _parameterPrefix = value;
            }
        }

        public NamingProperty NamingProperty { get; set; }

        public SearchCriteriaProperty SearchCriteriaProperty { get; set; }

        public string RowVersionColumn
        {
            get { return _rowVersionColumn; }
            set
            {
                _rowVersionColumn = value;
                RowVersionColumnRegex = new Regex(value, RegexOptions.Compiled);
            }
        }

        public VisualStudioVersion VisualStudioVersion { get; set; }

        public string SingularMemberSuffix { get; set; }

        public string ListSuffix { get; set; }

        public List<Regex> CleanExpressions { get; set; }

        public List<Regex> IgnoreExpressions { get; set; }

        public bool IncludeManyToManyEntity { get; set; }

        #endregion
    }
}