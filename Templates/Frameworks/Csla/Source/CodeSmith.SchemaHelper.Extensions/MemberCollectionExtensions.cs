using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for MemberCollectionExtensions
    /// </summary>
    public static class MemberCollectionExtensions
    {
        public static string BuildObjectInitializer(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0} = {1}{2}", member.PropertyName, member.VariableName, member.IsNullable && member.SystemType != "System.String" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildVariableArguments(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}{1}", member.VariableName, member.IsNullable && member.SystemType != "System.String" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariableArguments(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}{1}", member.PropertyName, member.IsNullable && member.SystemType != "System.String" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPrivateMemberVariableArguments(this List<Member> members)
        {
            string parameters = string.Empty;

            foreach (Member member in members)
            {
                parameters += string.Format(", {0}{1}", member.PrivateMemberVariableName, member.IsNullable && member.SystemType != "System.String" ? ".Value" : string.Empty);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseColumns(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format(", [{0}]", member.ColumnName);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseParameters(this List<Member> members)
        {
            return BuildDataBaseParameters(members, new List<AssociationMember>());
        }

        public static string BuildDataBaseParameters(this List<Member> members, List<AssociationMember> associationMembers)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
            }

            foreach (AssociationMember associationMember in associationMembers)
            {
                if (!associationMember.IsIdentity)
                {
                    string output = string.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, associationMember.ColumnName);

                    if (!columnNames.Contains(output))
                        columnNames += output;
                }
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildSetStatements(this List<Member> members)
        {
            string setStatements = "\t\t\t\t\t\t SET";

            foreach (Member member in members)
            {
                setStatements += string.Format(" [{0}] = {1},", member.ColumnName, member.BuildParameterVariableName());
            }

            return setStatements.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        }

        public static string BuildWhereStatements(this List<Member> members)
        {
            string columnNames = string.Empty;

            foreach (Member member in members)
            {
                columnNames += string.Format("[{0}] = {1} AND ", member.ColumnName, member.BuildParameterVariableName());
            }

            return string.Format("WHERE {0}", columnNames.Remove(columnNames.Length - 5, 5));
        }
    }
}
