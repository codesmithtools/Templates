using System;
using System.Collections.Generic;

namespace CodeSmith.SchemaHelper
{
    /// <summary>
    /// Extension Methods for AssociationExtensions
    /// </summary>
    public static class AssociationExtensions
    {
        #region BuildObjectInitializer

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
                        if (member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") continue;

                        parameters += string.Format(", {0} = {1}", Util.NamingConventions.PropertyName(associationMember.ColumnName),
                            usePropertyName ? member.PropertyName : member.VariableName);
                    }
                }
            }

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        #region BuildNullableObjectInitializer

        public static string BuildNullableObjectInitializer(this Association association)
        {
            return association.BuildNullableObjectInitializer(false);
        }

        public static string BuildNullableObjectInitializer(this Association association, bool usePropertyName)
        {
            string parameters = string.Empty;

            foreach (Member member in association.SearchCriteria.Members)
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        if ((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") == false) continue;

                        parameters += string.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1}.Value;", Util.NamingConventions.PropertyName(associationMember.ColumnName),
                            usePropertyName ? member.PropertyName : member.VariableName);
                    }
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n', '\t' });
        }

        #endregion

        #region BuildNullCheckStatement

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
        public static string BuildNullCheckStatement(this Association association, bool usePropertyName, bool useNot, bool useAndAlso, bool trimEnd, bool? nullExpression)
        {
            string exspression = useAndAlso ? "&& " : "|| ";
            string lastParameter = string.Empty;
            string parameters = string.Empty;

            foreach (Member member in association.SearchCriteria.Members)
            {
                foreach (AssociationMember associationMember in association)
                {
                    if (member.ColumnName == associationMember.AssociatedColumn.ColumnName && member.TableName == associationMember.AssociatedColumn.TableName)
                    {
                        if ((member.IsNullable && member.SystemType != "System.String" && member.SystemType != "System.Byte[]") == false) continue;

                        lastParameter = parameters.Length == 0 ?
                            string.Format("({0}{1}.HasValue {2}", useNot ? "!" : string.Empty, usePropertyName ? member.PropertyName : member.VariableName, exspression) :
                            string.Format(" {0}{1}.HasValue {2}", useNot ? "!" : string.Empty, usePropertyName ? member.PropertyName : member.VariableName, exspression);

                        parameters += lastParameter;
                    }
                }
            }

            // If there are no parameters then return.
            if (parameters.Length == 0)
            {
                if (!nullExpression.HasValue)
                    return string.Empty;

                return nullExpression.Value ? "(true)" : "(false)";
            }

            // Insert the last paren.
            parameters = parameters.Replace(lastParameter, lastParameter.Insert(lastParameter.IndexOf("HasValue") + 8, ")"));

            // Remove the last exspression if needed.
            if (trimEnd)
                parameters = parameters.Remove(parameters.Length - exspression.Length);

            // Remove leading characters.
            return parameters.TrimStart(new[] { ' ' });
        }

        #endregion

        #region BuildParametersVariables

        public static string BuildParametersVariables(this Association association)
        {
            return association.BuildParametersVariables(false);
        }

        public static string BuildParametersVariables(this Association association, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            foreach (AssociationMember member in association)
            {
                var parameter = string.Format(", {0} {1}", member.ClassName, Util.NamingConventions.VariableName(member.ClassName));

                if (!parameters.Contains(parameter))
                    parameters += parameter;
            }

            if (includeConnectionParameter)
                parameters += ", SqlConnection connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        public static string BuildUpdateStatementVariables(this Association association, List<Association> associations, int currentRecord, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            for (int index = 0; index < associations.Count; index++)
            {
                var parameter = string.Format(", {0}", Util.NamingConventions.VariableName(associations[index].ClassName));
                if (parameters.Contains(parameter)) continue;
                
                if(index == currentRecord)
                {
                    parameters += parameter;
                }
                else
                {
                    parameters += ", null";
                }
            }

            if (includeConnectionParameter)
                parameters += ", connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }
    }
}