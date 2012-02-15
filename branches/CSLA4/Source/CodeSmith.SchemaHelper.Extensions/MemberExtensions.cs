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
        /// <summary>
        /// Returns true if the property is of type Identity / Concurrency / Computed
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsReadOnly(this IProperty property)
        {
            if (property.IsType(PropertyType.Identity))
                return true;

            if (property.IsType(PropertyType.Concurrency))
                return true;

            if (property.IsType(PropertyType.Computed))
                return true;

            return false;
        }

        public static string GetReaderMethod(this IProperty property)
        {
            var prop = property as ISchemaProperty;
            if (prop != null)
            {
                return DbTypeToDataReaderMethod[prop.DataType.ToString(), "GetValue"];
            }

            return "GetValue";
        }

        public static bool HasByteArrayColumn(this IProperty property)
        {
            if (property.IsType(PropertyType.Concurrency))
                return true;

            var prop = property as ISchemaProperty;
            if (prop != null)
            {
                return DbTypeToDataReaderMethod[prop.DataType.ToString(), "GetValue"] == "GetBytes";
            }

            return false;
        }

        public static string BuildParameterVariableName(this IProperty property)
        {
            return String.Format("{0}{1}", Configuration.Instance.ParameterPrefix, property.Name);
        }

        public static string BuildDataBaseColumn(this IProperty property)
        {
            return String.Format("[{0}]", property.Name);
        }

        public static bool ExcludeBusinessSizeRule(this IProperty property)
        {
            if (property is ISchemaProperty)
            {
                var prop = (ISchemaProperty) property;
                return prop.NativeType.Equals("ntext", StringComparison.InvariantCultureIgnoreCase) ||
                       prop.NativeType.Equals("text", StringComparison.InvariantCultureIgnoreCase) ||
                      (prop.NativeType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase) && property.Size == -1) ||
                      (prop.NativeType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase) && property.Size == -1);
            }

            return false;
        }

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

        internal static MapCollection _csharpToDbTypeMethod;
        internal static MapCollection CSharpToDbTypeMethod
        {
            get
            {
                if (_csharpToDbTypeMethod == null)
                {
                    string path;
                    if (!Map.TryResolvePath("DbType-CSharp", String.Empty, out path) && TemplateContext.Current != null)
                    {
                        // If the mapping file wasn't found in the maps folder than look it up in the common folder.
                        string baseDirectory = Path.GetFullPath(Path.Combine(TemplateContext.Current.RootCodeTemplate.CodeTemplateInfo.DirectoryName, @"..\Common"));
                        Map.TryResolvePath("DbType-CSharp", baseDirectory, out path);
                    }

                    if (File.Exists(path))
                        _csharpToDbTypeMethod = Map.Load(path);
                }

                return _csharpToDbTypeMethod;
            }
        }
    }
}