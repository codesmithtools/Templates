using System;
using System.Collections.Generic;
using System.Linq;
using CodeSmith.SchemaHelper.Util;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static string BuildParametersVariables(this List<Member> members)
        {
            return members.BuildParametersVariables(true);
        }

        public static string BuildParametersVariables(this List<Member> members, bool isNullable)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] {'?'});
                parameters += string.Format(", {0} {1}", systemType, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCommandParameters(this List<Member> members)
        {
            return BuildCommandParameters(members, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory)
        {
            return BuildCommandParameters(members, isObjectFactory, false);
        }

        public static string BuildCommandParameters(this List<Member> members, bool isObjectFactory, bool usePropertyName)
        {
            string commandParameters = string.Empty;
            string castPrefix = isObjectFactory ? "item." : string.Empty;

            foreach (Member member in members)
            {
                string propertyName = member.PropertyName;

                string cast;
                if (member.SystemType.Contains("SmartDate"))
                {
                    cast = member.IsNullable ? string.Format("(DateTime?){0}{1});", castPrefix, propertyName)
                                             : string.Format("(DateTime){0}{1});", castPrefix, propertyName);
                }
                else
                    cast = string.Format("{0}{1});", castPrefix, propertyName);

                commandParameters += string.Format("\n\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);
            }

            return commandParameters.TrimStart(new[] { '\t', '\n' });
        }

        //public static string BuildParametersVariablesCriteria(this List<Member> members)
        //{
        //    return members.BuildParametersVariablesCriteria(true);
        //}

        //public static string BuildParametersVariablesCriteria(this List<Member> members, bool isNullable)
        //{
        //    string parameters = string.Empty;

        //    foreach (Member member in members)
        //    {
        //        string systemType = isNullable ? member.SystemType : member.SystemType.TrimEnd(new[] { '?' });
        //        parameters += string.Format(", {0} {1}", systemType, member.VariableName);
        //    }

        //    return parameters.TrimStart(new[] { ',', ' ' });
        //}
    }
}
