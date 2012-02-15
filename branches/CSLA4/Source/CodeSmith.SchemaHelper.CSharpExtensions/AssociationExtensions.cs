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

            foreach (IProperty property in association.SearchCriteria.Properties)
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.ForeignProperty.KeyName) // && property.ForeignProperty == associationProperty.ForeignProperty.ForeignProperty)
                    {
                        if (property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") continue;

                        parameters += String.Format(", {0} = {1}", associationProperty.Property.Name,
                            usePropertyName ? property.Name : property.VariableName);
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

            foreach (IProperty property in association.SearchCriteria.Properties)
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.ForeignProperty.KeyName) // && property.Entity.EntityKeyName == associationProperty.ForeignProperty.ForeignProperty)
                    {
                        if ((property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") == false) continue;

                        parameters += String.Format("\r\n\t\t\t\tif({1}.HasValue) criteria.{0} = {1}.Value;", associationProperty.Property.Name,
                            usePropertyName ? property.Name : property.VariableName);
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

            foreach (IProperty property in association.SearchCriteria.Properties)
            {
                foreach (AssociationProperty associationProperty in association.Properties)
                {
                    if (property.KeyName == associationProperty.ForeignProperty.KeyName) // && property.ForeignProperty == associationProperty.ForeignProperty.ForeignProperty)
                    {
                        if ((property.IsNullable && property.SystemType != "System.String" && property.SystemType != "System.Byte[]") == false) continue;

                        lastParameter = parameters.Length == 0 ?
                            String.Format("({0}{1}.HasValue {2}", useNot ? "!" : string.Empty, usePropertyName ? property.Name : property.VariableName, exspression) :
                            String.Format(" {0}{1}.HasValue {2}", useNot ? "!" : string.Empty, usePropertyName ? property.Name : property.VariableName, exspression);

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

            foreach (AssociationProperty property in association.Properties)
            {
                var parameter = String.Format(", {0} {1}", property.Property.Name, Util.NamingConventions.VariableName(property.Property.Name));

                if (!parameters.Contains(parameter))
                    parameters += parameter;
            }

            if (includeConnectionParameter)
                parameters += ", var connection";

            return parameters.TrimStart(new[] { ',', ' ' });
        }

        #endregion

        public static string BuildUpdateStatementVariables(this Association association, List<Association> associations, int currentRecord, bool includeConnectionParameter)
        {
            string parameters = string.Empty;

            for (int index = 0; index < associations.Count; index++)
            {
                var parameter = String.Format(", {0}", Util.NamingConventions.VariableName(associations[index].Name));
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