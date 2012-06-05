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

            string dataType = null;
            if (property is ISchemaProperty)
                dataType = ((ISchemaProperty)property).DataType.ToString();
            else if (property.ExtendedProperties.ContainsKey("DataType"))
                dataType = property.ExtendedProperties["DataType"].ToString();

            if (!String.IsNullOrEmpty(dataType))
            {
                return DbTypeToDataReaderMethod[dataType, "GetValue"] == "GetBytes";
            }

            return false;
        }

        public static string BuildParameterVariableName(this IProperty property)
        {
            return String.Format("{0}{1}", Configuration.Instance.ParameterPrefix, property.KeyName);
        }

        public static string BuildDataBaseColumn(this IProperty property)
        {
            return String.Format("[{0}]", property.KeyName);
        }

        public static bool ExcludeBusinessSizeRule(this IProperty property)
        {
            string nativeType = null;
            if (property is ISchemaProperty)
                nativeType = ((ISchemaProperty)property).NativeType;
            else if (property.ExtendedProperties.ContainsKey("NativeType"))
                nativeType = property.ExtendedProperties["NativeType"].ToString();

            if (!String.IsNullOrEmpty(nativeType))
            {
                return nativeType.Equals("ntext", StringComparison.InvariantCultureIgnoreCase) ||
                       nativeType.Equals("text", StringComparison.InvariantCultureIgnoreCase) ||
                      (nativeType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase) && property.Size == -1) ||
                      (nativeType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase) && property.Size == -1);
            }

            return false;
        }

        public static bool IsBinarySqlDbType(this IProperty property)
        {
            string nativeType = null;
            if (property is ISchemaProperty)
                nativeType = ((ISchemaProperty)property).NativeType;
            else if (property.ExtendedProperties.ContainsKey("NativeType"))
                nativeType = property.ExtendedProperties["NativeType"].ToString();

            if (!String.IsNullOrEmpty(nativeType))
            {
                return nativeType.Equals("binary", StringComparison.InvariantCultureIgnoreCase) ||
                       nativeType.Equals("varbinary", StringComparison.InvariantCultureIgnoreCase) ||
                       nativeType.Equals("image", StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }


        public static string GetBinarySqlDbType(this IProperty property)
        {
            string nativeType = null;
            if (property is ISchemaProperty)
                nativeType = ((ISchemaProperty)property).NativeType;
            else if (property.ExtendedProperties.ContainsKey("NativeType"))
                nativeType = property.ExtendedProperties["NativeType"].ToString();

            if (!String.IsNullOrEmpty(nativeType))
            {
                if (nativeType.Equals("binary", StringComparison.InvariantCultureIgnoreCase) ||
                   nativeType.Equals("varbinary", StringComparison.InvariantCultureIgnoreCase))
                    return "SqlDbType.VarBinary";

                if (nativeType.Equals("image", StringComparison.InvariantCultureIgnoreCase))
                    return "SqlDbType.Binary";
            }

            return String.Empty;
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