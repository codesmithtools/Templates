using System;
using System.Collections.Generic;
using System.Linq;

using CodeSmith.Engine;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberBaseExtensions
    /// </summary>
    public static class MemberBaseExtensions
    {
        public static string SystemType(this MemberBase member, bool isNullable)
        {
            return isNullable ? member.SystemType : member.SystemType.TrimEnd(new char[] { '?' });
        }

        public static string BuildObjectInitializer(this MemberBase member)
        {
            return string.Format("{0} = {1}", member.PropertyName, member.VariableName);
        }

        public static string BuildCriteriaObjectInitializer(this MemberBase member, string className)
        {
            return string.Format("{0} = {1}", member.Entity.ResolveCriteriaPropertyName(member.ColumnName, className), member.Entity.ResolveCriteriaVariableName(member.ColumnName));
        }

        public static List<SearchCriteria> ListSearchCriteria(this MemberBase member)
        {
            return member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(member.Name)).ToList();
        }

        public static string BuildDataObjectField(this MemberBase member)
        {
            if (member.IsPrimaryKey)
            {
                return string.Format("\n\t\t[System.ComponentModel.DataObjectField(true, {0})]", member.IsIdentity.ToString().ToLower());
            }

            return string.Empty;
        }

        public static string GetReaderMethod(this MemberBase member)
        {
            return DbTypeToDataReaderMethod[member.Entity.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"];
        }

        public static bool HasByteArrayColumn(this MemberBase member)
        {
            return DbTypeToDataReaderMethod[member.Entity.Table.Columns[member.ColumnName].DataType.ToString(), "GetValue"] == "GetBytes";
        }

        public static string BuildParameterVariableName(this MemberBase member)
        {
            return string.Format("{0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
        }

        public static string BuildParametersVariable(this MemberBase member)
        {
            return member.BuildParametersVariable(true);
        }

        public static string BuildParametersVariable(this MemberBase member, bool isNullable)
        {
            string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });
            return string.Format("{0} {1}", systemType, member.VariableName);
        }

        public static string BuildParametersVariablesCriteria(this MemberBase member)
        {
            return member.BuildParametersVariablesCriteria(true);
        }

        public static string BuildParametersVariablesCriteria(this MemberBase member, bool isNullable)
        {
            string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });
            return string.Format("{0} {1}", systemType, member.Entity.ResolveCriteriaVariableName(member.ColumnName));
        }

        #region Internal Properties and MemberBases

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