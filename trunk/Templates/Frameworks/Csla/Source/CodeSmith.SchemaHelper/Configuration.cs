using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private MapCollection _systemTypeEscape;
        private MapCollection _keywordRenameAlias;

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
                                     TableNaming = TableNaming.Mixed,
                                     ColumnNaming = ColumnNaming.Preserve
                                 };

            SearchCriteriaProperty = new SearchCriteriaProperty {Prefix = "GetBy"};

            UseRowVersionRegex = false;
            RowVersionColumn = "^((R|r)ow)?(V|v)ersion$";
            VisualStudioVersion = VisualStudioVersion.VS_2008;
            SingularMemberSuffix = "Member";
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
                    Map.TryResolvePath(mapFileName, String.Empty, out path);
                    _systemTypeEscape = File.Exists(path) ? Map.Load(path) : new MapCollection();
                }

                return _systemTypeEscape;
            }
        }

        /// <summary>
        /// Returns the DBTypeToSystemTypeEscape MapCollection.
        /// </summary>
        /// <returns>Returns the correct SystemTypeEscape MapCollection.</returns>
        [Browsable(false)]
        public MapCollection KeywordRenameAlias
        {
            get
            {
                if (_keywordRenameAlias == null)
                {
                    string path;
                    if (!Map.TryResolvePath("KeywordRenameAlias", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\..\Common"));
                        if (!Map.TryResolvePath("KeywordRenameAlias", baseDirectory, out path))
                        {
                            baseDirectory = Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, "Common");
                            Map.TryResolvePath("KeywordRenameAlias", baseDirectory, out path);
                        }
                    }

                    // Prevents a NullReferenceException from occurring.
                    _keywordRenameAlias = File.Exists(path) ? Map.Load(path) : new MapCollection();
                }

                return _keywordRenameAlias;
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
            return ValidateName(column, memberName, false);
        }

        /// <summary>
        /// Returns a valid name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="memberName">The name to be validated.</param>
        /// <param name="isAssociation">If this is an association, then the table prefix on columns will not be removed.</param>
        /// <returns>A valid name.</returns>
        internal string ValidateName(ColumnSchema column, string memberName, bool isAssociation)
        {
            memberName = ValidateName(memberName);
            var tableName = column.Table.ClassName();

            // Check to see if the column name is prefixed with the table name
            if (Instance.NamingProperty.ColumnNaming == ColumnNaming.RemoveTablePrefix && !isAssociation)
            {
                // Does the updated column name start with the table name? The overrides file takes presidence.
                // Also make sure that the stripped name is atleast two characters (E.G CategoryID --> ID)
                if(memberName.StartsWith(tableName, StringComparison.CurrentCultureIgnoreCase) && memberName.Length > tableName.Length + 1)
                {
                    memberName = NamingConventions.PropertyName(memberName.Remove(0, tableName.Length));
                }
            }

            if (String.Compare(tableName, memberName, true) == 0)
                memberName = string.Format("{0}{1}", memberName, SingularMemberSuffix);

            return memberName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Table FullName or Column Name</param>
        /// <returns></returns>
        public bool ExcludeRegexIsMatch(string name)
        {
            foreach (Regex regex in IgnoreExpressions)
                if (regex.IsMatch(name))
                    return true;

            return false;
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

        public bool UseRowVersionRegex { get; set; }

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

        public bool IncludeSilverlightSupport { get; set; }

        public FrameworkVersion FrameworkVersion { get; set; }

        #endregion
    }
}