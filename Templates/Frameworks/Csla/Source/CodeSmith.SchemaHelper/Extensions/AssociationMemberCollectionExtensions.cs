using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationMemberExtensions
    /// </summary>
    public static class AssociationMemberExtensions
    {
        public static List<SearchCriteria> ListSearchCriteria(this List<AssociationMember> members)
        {
            List<SearchCriteria> searchCriterias = new List<SearchCriteria>();

            foreach (var member in members)
            {
                string memberName = member.Name;
                searchCriterias.AddRange(member.SearchCriteria.Where(sc => !sc.IsUniqueResult && sc.MethodName.Contains(memberName)));
            }

            return searchCriterias;
        }

        public static string BuildParametersVariables(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    parameters += string.Format(", ByVal {0} As {1}", member.VariableName, member.SystemType);
                else
                    parameters += string.Format(", {0} {1}", member.SystemType, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildChildInsertParametersVariables(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                    parameters += string.Format(", ByVal {0} As {1}", member.VariableName, member.ClassName);
                else
                    parameters += string.Format(", {0} {1}", member.ClassName, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildObjectInitializer(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", {0} = {1}", member.PropertyName, member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildVariableArguments(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", {0}", member.VariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPropertyVariableArguments(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", {0}", member.PropertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCriteriaParametersVariables(this List<AssociationMember> members)
        {
            return BuildCriteriaParametersVariables(members, "criteria");
        }

        public static string BuildCriteriaParametersVariables(this List<AssociationMember> members, string criteria)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", {0}.{1}", criteria, member.PropertyName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildReadPropertyParametersVariables(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", ReadProperty({0}Property)", member.PrivateMemberVariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildPrivateAssociationMemberVariableParameters(this List<AssociationMember> members)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                parameters += string.Format(", {0}", member.PrivateMemberVariableName);
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseColumns(this List<AssociationMember> members)
        {
            string columnNames = string.Empty;

            foreach (AssociationMember member in members)
            {
                columnNames += string.Format(", [{0}]", member.ColumnName);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildDataBaseParameters(this List<AssociationMember> members)
        {
            string columnNames = string.Empty;

            foreach (AssociationMember member in members)
            {
                columnNames += string.Format(", {0}{1}", Configuration.Instance.ParameterPrefix, member.Name);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildCommandParameters(this List<AssociationMember> members)
        {
            string commandParameters = string.Empty;

            foreach (AssociationMember member in members)
            {
                string cast;
                if (member.SystemType.Contains("SmartDate"))
                {
                    if (Configuration.Instance.TargetLanguage == LanguageEnum.VB)
                        cast = member.IsNullable ? string.Format("IIf({0}.HasValue, DirectCast({0}.Value.Date, DateTime), System.DBNull.Value))", member.PropertyName)
                                                 : string.Format("DirectCast({0}.Date, DateTime))", member.PropertyName);
                    else
                        cast = member.IsNullable ? string.Format("(DateTime?){0});", member.PropertyName)
                                                 : string.Format("(DateTime){0});", member.PropertyName);
                }
                else
                    cast = Configuration.Instance.TargetLanguage == LanguageEnum.VB ? string.Format("{0})", member.PropertyName) : string.Format("{0});", member.PropertyName);

                commandParameters += string.Format("\n\t\t\t\t\t\tcommand.Parameters.AddWithValue(\"{0}{1}\", {2}", Configuration.Instance.ParameterPrefix, member.ColumnName, cast);
            }

            return commandParameters.TrimStart(new[] { '\t', '\n' });
        }

        public static string BuildSetStatements(this List<AssociationMember> members)
        {
            string setStatements = "\t\t\t\t\t\t SET";

            foreach (AssociationMember member in members)
            {
                setStatements += string.Format(" [{0}] = {1}{2},", member.Name, Configuration.Instance.ParameterPrefix, member.VariableName);
            }

            return setStatements.TrimStart(new[] { '\t', '\n' }).TrimEnd(new[] { ',' });
        }

        public static string BuildWhereStatements(this List<AssociationMember> members)
        {
            string columnNames = string.Empty;

            foreach (AssociationMember member in members)
            {
                columnNames += string.Format("[{0}] = {1} AND ", member.ColumnName, member.BuildParameterVariableName());
            }

            return string.Format("WHERE {0}", columnNames.Remove(columnNames.Length - 5, 5));
        }

        public static string BuildReaderStatements(this List<AssociationMember> members)
        {
            string columnNames = string.Empty;

            foreach (AssociationMember member in members)
            {
                columnNames += string.Format(", reader.{0}(\"{1}\")", member.GetReaderMethod(), member.Name);
            }

            return columnNames.TrimStart(new[] { ',', ' ' });
        }

        public static bool HasByteArrayColumn(this List<AssociationMember> members)
        {
            foreach (AssociationMember member in members)
            {
                if (member.HasByteArrayColumn())
                    return true;
            }

            return false;
        }
    }
}