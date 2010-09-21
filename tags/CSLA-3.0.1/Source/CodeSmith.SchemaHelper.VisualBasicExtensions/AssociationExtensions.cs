using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationExtensions
    {
        public static string BuildObjectInitializer(this Association association)
        {
            return association.BuildObjectInitializer(false);
        }

        public static string BuildObjectInitializer(this Association association, bool usePropertyName)
        {
            string parameters = string.Empty;

            foreach (Member member in association.SearchCriteria.Members)
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        bool isNullable = member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()";
                        if(isNullable)
                        {
                            parameters += string.Format("\r\n\t\t\t\tIf({1}.HasValue) Then criteria.{0} = {1}.Value",
                                associationMember.MemberPropertyName,
                                usePropertyName ? member.PropertyName : member.VariableName);
                        }
                        else
                        {
                            parameters += string.Format("\r\n\t\t\t\tcriteria.{0} = {1}",
                                associationMember.MemberPropertyName,
                                usePropertyName ? member.PropertyName : member.VariableName);
                        }
                    }
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t', ',', ' ' });
        }

        /// <summary>
        /// Builds a null check HasValue Statements for the Property Templates.
        /// </summary>
        /// <param name="association"></param>
        /// <returns></returns>
        public static string BuildNullCheckStatement(this Association association)
        {
            return association.BuildNullCheckStatement(false, true, false, false);
        }

        /// <summary>
        /// Builds a null check HasValue Statements for the Property Templates.
        /// </summary>
        /// <param name="association"></param>
        /// <param name="usePropertyName"></param>
        /// <param name="useNot"></param>
        /// <param name="useAndAlso"></param>
        /// <param name="trimEnd"></param>
        /// <returns></returns>
        public static string BuildNullCheckStatement(this Association association, bool usePropertyName, bool useNot, bool useAndAlso, bool trimEnd)
        {
            return association.BuildNullCheckStatement(false, true, false, false, null);
        }

        /// <summary>
        /// Builds a null check HasValue Statements for the Property Templates.
        /// </summary>
        /// <param name="association"></param>
        /// <param name="usePropertyName"></param>
        /// <param name="useNot"></param>
        /// <param name="useAndAlso"></param>
        /// <param name="trimEnd"></param>
        /// <param name="nullExpression">If this value is not set to null and the parameters is blank, then this exspression will be returned.</param>
        /// <returns></returns>
        public static string BuildNullCheckStatement(this Association association, bool usePropertyName, bool useNot, bool useAndAlso, bool trimEnd, bool? nullExpression )
        {
            string exspression = useAndAlso ? "AndAlso " : "OrElse ";
            string lastParameter = string.Empty;
            string parameters = string.Empty;

            foreach (Member member in association.SearchCriteria.Members)
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        if ((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte()") == false) continue;

                        lastParameter = parameters.Length == 0 ? 
                            string.Format("({0} {1}.HasValue {2}", useNot ? "Not" : string.Empty, usePropertyName ? member.PropertyName : member.VariableName, exspression) : 
                            string.Format(" {0} {1}.HasValue {2}", useNot ? "Not" : string.Empty, usePropertyName ? member.PropertyName : member.VariableName, exspression);

                        parameters += lastParameter; 
                    }
                }
            }

            // If there are no parameters then return.
            if (parameters.Length == 0)
            {
                if (!nullExpression.HasValue)
                    return string.Empty;

                return nullExpression.Value ? "(True)" : "(False)";
            }

            // Insert the last paren.
            parameters = parameters.Replace(lastParameter, lastParameter.Insert(lastParameter.IndexOf("HasValue") + 8, ")"));

            // Remove the last exspression if needed.
            if(trimEnd)
                parameters = parameters.Remove(parameters.Length - exspression.Length);

            // Remove leading characters.
            return parameters.TrimStart(new[] { ' ' });
        }

        public static string BuildParametersVariables(this Association association)
        {
            return association.BuildParametersVariables(false);
        }

        public static string BuildParametersVariables(this Association association, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                var parameter = string.Format(", ByVal {1} As {0}", member.ClassName, Util.NamingConventions.VariableName(member.ClassName));

                if (!parameters.Contains(parameter))
                    parameters += parameter;
            }

            if (includeConnectionParameter)
                parameters += ", ByVal connection As SqlConnection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildUpdateStatementVariables(this Association association, List<Association> associations, int currentRecord, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            for (int index = 0; index < associations.Count; index++)
            {
                var parameter = string.Format(", {0}", Util.NamingConventions.VariableName(associations[index].ClassName));
                if(index == currentRecord)
                {
                    parameters += parameter;
                }
                else
                {
                    parameters += ", Nothing";
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}