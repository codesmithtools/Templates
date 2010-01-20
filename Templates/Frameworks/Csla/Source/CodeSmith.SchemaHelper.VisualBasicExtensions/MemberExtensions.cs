using System;
using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberExtensions
    /// </summary>
    public static class MemberExtensions
    {
        //public static List<SearchCriteria> ListSearchCriteria(this Member member)
        //{
        //    return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        //}

        //public static string BuildObjectInitializer(this Member member)
        //{
        //    return string.Format("{0} = {1}", member.PropertyName, member.VariableName);
        //}

        //public static string BuildDataObjectField(this Member member)
        //{
        //    if (member.IsPrimaryKey)
        //    {
        //        if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
        //            return string.Format("\n\t\t<System.ComponentModel.DataObjectField(true, {0})> _", member.IsIdentity.ToString().ToLower());
               
        //        return string.Format("\n\t\t[System.ComponentModel.DataObjectField(true, {0})]", member.IsIdentity.ToString().ToLower());
        //    }

        //    return string.Empty;
        //}

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
            return string.Format("{0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
        }

        //public static string BuildWhereStatement(this Member member)
        //{
        //    return string.Format("WHERE [{0}] = {1}", member.ColumnName, member.BuildParameterVariableName());
        //}

        //public static string BuildParametersVariable(this Member member)
        //{
        //    return member.BuildParametersVariable(true);
        //}

        //public static string BuildParametersVariable(this Member member, bool isNullable)
        //{
        //    string systemType = isNullable ? member.SystemType : member.BaseSystemType;

        //    if(Configuration.Instance.TargetLanguage == LanguageEnum.VB)
        //        return string.Format("ByVal {0} As {1}", member.VariableName, systemType);

        //    return string.Format("{0} {1}", systemType, member.VariableName);
        //}

        public static string BuildDataBaseColumn(this Member member)
        {
            return string.Format("[{0}]", member.ColumnName);
        }

        //public static string BuildCriteriaObjectInitializer(this Member member, string className)
        //{
        //    return BuildCriteriaObjectInitializer(member, className, false);
        //}

        //public static string BuildCriteriaObjectInitializer(this Member member, string className, bool isObjectFactory)
        //{
        //    string criteria = isObjectFactory ? string.Format("item.{0}", member.Entity.ResolveCriteriaPropertyName(member.ColumnName)) : member.Entity.ResolveCriteriaVariableName(member.ColumnName);
        //    return string.Format("{0} = {1}", member.Entity.ResolveCriteriaPropertyName(member.ColumnName, className), criteria);
        //}

        //public static string BuildParametersVariablesCriteria(this Member member)
        //{
        //    return member.BuildParametersVariablesCriteria(true);
        //}

        //public static string BuildParametersVariablesCriteria(this Member member, bool isNullable)
        //{
        //    string systemType = isNullable ? member.SystemType : member.BaseSystemType;

        //    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
        //        return string.Format("ByVal {0} As {1}", member.Entity.ResolveCriteriaVariableName(member.ColumnName), systemType);

        //    return string.Format("{0} {1}", systemType, member.Entity.ResolveCriteriaVariableName(member.ColumnName));
        //}

        #region Internal Properties and Members

        internal static MapCollection _dbTypeToDataReaderMethod;
        internal static MapCollection DbTypeToDataReaderMethod
        {
            get
            {
                if (_dbTypeToDataReaderMethod == null)
                {
                    string path;
                    if (Map.TryResolvePath("DbType-DataReaderMethod", "", out path))
                        _dbTypeToDataReaderMethod = Map.Load(path);
                }
                return _dbTypeToDataReaderMethod;
            }
        }

        #endregion
    }
}