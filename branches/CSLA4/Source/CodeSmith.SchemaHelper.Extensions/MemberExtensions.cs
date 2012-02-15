using System;
using System.IO;
using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberExtensions
    /// </summary>
    public static class MemberExtensions
    {
        public static string GetReaderMethod(this Member member)
        {
            return DbTypeToDataReaderMethod[member.Entity.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"];
        }

        public static bool HasByteArrayColumn(this Member member)
        {
            return DbTypeToDataReaderMethod[member.Entity.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes" || member.IsRowVersion;
        }

        public static string BuildParameterVariableName(this Member member)
        {
            return string.Format("{0}{1}", Configuration.Instance.ParameterPrefix, member.ColumnName);
        }

        public static string BuildDataBaseColumn(this Member member)
        {
            return string.Format("[{0}]", member.ColumnName);
        }

        public static bool ExcludeBusinessSizeRule(this Member member)
        {
            return member.NativeType.Equals("ntext", StringComparison.InvariantCultureIgnoreCase) ||
                   member.NativeType.Equals("text", StringComparison.InvariantCultureIgnoreCase) ||
                  (member.NativeType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase) && member.Size == -1) ||
                  (member.NativeType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase) && member.Size == -1);
        }

        #region Internal Properties and Members

        internal static MapCollection _dbTypeToDataReaderMethod;
        internal static MapCollection DbTypeToDataReaderMethod
        {
            get
            {
                if (_dbTypeToDataReaderMethod == null)
                {
                    string path;
                    if (!Map.TryResolvePath("DbType-DataReaderMethod", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\Common"));
                        Map.TryResolvePath("DbType-DataReaderMethod", baseDirectory, out path);
                    }

                    if (File.Exists(path))
                        _dbTypeToDataReaderMethod = Map.Load(path);
                }

                return _dbTypeToDataReaderMethod;
            }
        }

        #endregion
    }
}