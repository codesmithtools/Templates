using System;
using System.Collections.Generic;
using System.Linq;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberExtensions
    /// </summary>
    public static class MemberExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this Member member)
        {
            return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        }

        public static string BuildObjectInitializer(this Member member)
        {
            return string.Format("{0} = {1}", member.PropertyName, member.VariableName);
        }

        public static string BuildDataObjectField(this Member member)
        {
            if (member.IsPrimaryKey)
            {
                return string.Format("\n\t\t[System.ComponentModel.DataObjectField(true, {0})]", member.IsIdentity.ToString().ToLower());
            }

            return string.Empty;
        }

        public static string GetReaderMethod(this Member member)
        {
            return DbTypeToDataReaderMethod[member.Entity.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"];
        }

        public static string BuildParameterVariableName(this Member member)
        {
            return string.Format("{0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
        }

        public static string BuildWhereStatement(this Member member)
        {
            return string.Format("WHERE [{0}] = {1}", member.ColumnName, member.BuildParameterVariableName());
        }

        public static string BuildParametersVariable(this Member member)
        {
            return member.BuildParametersVariable(true);
        }

        public static string BuildParametersVariable(this Member member, bool isNullable)
        {
            string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });
            return string.Format("{0} {1}", systemType, member.VariableName);
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
                    if (Map.TryResolvePath("DbType-DataReaderMethod", "", out path))
                        _dbTypeToDataReaderMethod = Map.Load(path);
                }
                return _dbTypeToDataReaderMethod;
            }
        }

        #endregion
    }
}