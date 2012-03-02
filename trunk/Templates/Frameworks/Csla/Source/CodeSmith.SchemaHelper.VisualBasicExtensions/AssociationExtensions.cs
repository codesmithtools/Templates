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
            string parameters = String.Empty;

            foreach (IProperty property in association.SearchCriteria.Properties)
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.Property.KeyName && property == associationProperty.Property)
                    {
                        bool isNullable = property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte()";
                        if(isNullable)
                        {
                            parameters += string.Format("\r\n                If({1}.HasValue) Then criteria.{0} = {1}.Value",
                                associationProperty.Property.Name,
                                usePropertyName ? property.Name : property.VariableName);
                        }
                        else
                        {
                            parameters += String.Format("\r\n                criteria.{0} = {1}",
                                associationProperty.Property.Name,
                                usePropertyName ? property.Name : property.VariableName);
                        }
                    }
                }
            }

            return parameters.TrimStart(new[] { '\r', '\n', ' ', ',' });
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
        public static string BuildNullCheckStatement(this Association association, bool usePropertyName, bool useNot, bool useAndAlso, bool trimEnd, bool? nullExpression)
        {
            string exspression = useAndAlso ? "AndAlso " : "OrElse ";
            string lastParameter = String.Empty;
            string parameters = String.Empty;

            foreach (IProperty property in association.SearchCriteria.Properties)
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.Property.KeyName && property == associationProperty.Property)
                    {
                        if ((associationProperty.Property.IsNullable && associationProperty.Property.SystemType != "System.String" && associationProperty.Property.SystemType != "System.Byte[]") == false) 
                            continue;

                        lastParameter = parameters.Length == 0 ?
                            String.Format("({0}{1}.HasValue {2}", useNot ? "Not" : String.Empty, usePropertyName ? associationProperty.Property.Name : associationProperty.Property.VariableName, exspression) :
                            String.Format(" {0}{1}.HasValue {2}", useNot ? "Not" : String.Empty, usePropertyName ? associationProperty.Property.Name : associationProperty.Property.VariableName, exspression);

                        parameters += lastParameter;
                    }
                }
            }

            // If there are no parameters then return.
            if (parameters.Length == 0)
            {
                if (!nullExpression.HasValue)
                    return String.Empty;

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
            string parameters = String.Empty;

            foreach (AssociationProperty property in association.Properties)
            {
                var parameter = String.Format(", ByVal {1} As {0}", property.ForeignProperty.Entity.Name, property.ForeignProperty.Entity.VariableName);

                if (!parameters.Contains(parameter))
                    parameters += parameter;
            }

            if (includeConnectionParameter)
                parameters += ", ByVal connection As SqlConnection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        public static string BuildUpdateStatementVariables(this Association association, List<Association> associations, int currentRecord, bool includeConnectionParameter)
        {
            string parameters = String.Empty;

            for (int index = 0; index < associations.Count; index++)
            {
                var parameter = String.Format(", {0}", associations[index].ForeignEntity.VariableName);
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